using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class NoteManager : MonoBehaviour
{
    [System.Serializable]
    public class TapData { public int zone; public float time; }

    [System.Serializable]
    public class SlideData
    {
        public int[] route;      // 예: {1,2,5,8,9}
        public float startTime;  // 시작 스폰 타이밍(음악 시간)
        public float moveTime = 2.5f; // 전체 이동 시간(음악 결합 전 테스트용)
    }

    [Header("참조")]
    public AudioSource music;
    public InputRouter input;
    [Tooltip("Zone_1 ~ Zone_9 Transform. 비워두면 이름으로 자동 탐색합니다.")]
    public Transform[] zoneAnchors; // 선택 (SlidePath가 사용)

    [Header("프리팹")]
    public GameObject tapNotePrefab;     // TapNote 포함 프리팹
    public GameObject slideBundlePrefab; // SlidePath + SlideArrow 묶음 프리팹

    [Header("스폰 옵션")]
    public float spawnAhead = 1.0f; // 음악 시간보다 이만큼 앞서 스폰

    [Header("이번 테스트 패턴 - Tap")]
    public List<TapData> taps = new List<TapData>()
    {
        new TapData{ zone=3, time=1.00f },
        new TapData{ zone=7, time=1.50f },
        new TapData{ zone=2, time=2.00f },
        new TapData{ zone=8, time=2.50f },
        new TapData{ zone=5, time=3.00f },
    };

    [Header("이번 테스트 패턴 - Slide")]
    public List<SlideData> slides = new List<SlideData>()
    {
        new SlideData { route=new[]{1,2,5,8,9,5}, startTime=3.50f, moveTime=2.5f },
        // 필요하면 여러 개 추가
    };

    int tapSpawnIdx = 0;
    int slideSpawnIdx = 0;

    void Start()
    {
        // zoneAnchors 비어있으면 자동으로 Zone_1 ~ Zone_9 찾기
        if (zoneAnchors == null || zoneAnchors.Length < 9)
            zoneAnchors = FindZoneAnchors();

        music.Play();
    }

    void Update()
    {
        float t = music.time;

        // Tap 스폰
        while (tapSpawnIdx < taps.Count && t >= taps[tapSpawnIdx].time - spawnAhead)
        {
            SpawnTap(taps[tapSpawnIdx]);
            tapSpawnIdx++;
        }

        // Slide 스폰
        while (slideSpawnIdx < slides.Count && t >= slides[slideSpawnIdx].startTime - spawnAhead)
        {
            SpawnSlide(slides[slideSpawnIdx]);
            slideSpawnIdx++;
        }
    }

    // ---------------- Spawners ----------------

    void SpawnTap(TapData d)
    {
        Vector3 pos = GetZonePos(d.zone);
        if (pos == Vector3.positiveInfinity) { Debug.LogWarning($"Zone_{d.zone} 없음"); return; }

        var go = Instantiate(tapNotePrefab, pos, Quaternion.identity);
        var tn = go.GetComponent<TapNote>();
        tn.zoneIndex = d.zone;
        tn.time = d.time;
        tn.music = music;
        tn.input = input;
        Debug.Log($"[Spawn Tap] Z{d.zone} @ {d.time:F2}");
    }

    void SpawnSlide(SlideData d)
    {
        if (slideBundlePrefab == null) { Debug.LogError("slideBundlePrefab 미할당"); return; }

        var go = Instantiate(slideBundlePrefab, Vector3.zero, Quaternion.identity);

        var path = go.GetComponentInChildren<SlidePath>();
        var arrow = go.GetComponentInChildren<SlideArrow>();

        if (path == null || arrow == null)
        {
            Debug.LogError("SlideBundle 프리팹에 SlidePath/SlideArrow가 없습니다.");
            Destroy(go);
            return;
        }

        // 경로/앵커 세팅 (SlidePath = 진실 소스)
        path.route = d.route;
        path.zoneAnchors = zoneAnchors;
        path.DrawPath();
        
        // Arrow 초기화 (음악 결합 전: moveTime으로만 이동)
        // SlideArrow가 Init(path, input, moveTime)을 지원한다고 가정
        arrow.Init(path, input, d.moveTime);

   
    }

    // ---------------- Utils ----------------

    Vector3 GetZonePos(int zone)
    {
        if (zoneAnchors != null && zoneAnchors.Length >= zone && zone > 0 && zoneAnchors[zone - 1] != null)
            return zoneAnchors[zone - 1].position;

        var z = GameObject.Find($"Zone_{zone}");
        if (z != null) return z.transform.position;

        return Vector3.positiveInfinity;
    }

    Transform[] FindZoneAnchors()
    {
        var arr = new Transform[9];
        for (int i = 1; i <= 9; i++)
        {
            var z = GameObject.Find($"Zone_{i}");
            if (z != null) arr[i - 1] = z.transform;
        }
        return arr;
    }
}
