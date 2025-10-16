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
        public int[] route;      // ��: {1,2,5,8,9}
        public float startTime;  // ���� ���� Ÿ�̹�(���� �ð�)
        public float moveTime = 2.5f; // ��ü �̵� �ð�(���� ���� �� �׽�Ʈ��)
    }

    [Header("����")]
    public AudioSource music;
    public InputRouter input;
    [Tooltip("Zone_1 ~ Zone_9 Transform. ����θ� �̸����� �ڵ� Ž���մϴ�.")]
    public Transform[] zoneAnchors; // ���� (SlidePath�� ���)

    [Header("������")]
    public GameObject tapNotePrefab;     // TapNote ���� ������
    public GameObject slideBundlePrefab; // SlidePath + SlideArrow ���� ������

    [Header("���� �ɼ�")]
    public float spawnAhead = 1.0f; // ���� �ð����� �̸�ŭ �ռ� ����

    [Header("�̹� �׽�Ʈ ���� - Tap")]
    public List<TapData> taps = new List<TapData>()
    {
        new TapData{ zone=3, time=1.00f },
        new TapData{ zone=7, time=1.50f },
        new TapData{ zone=2, time=2.00f },
        new TapData{ zone=8, time=2.50f },
        new TapData{ zone=5, time=3.00f },
    };

    [Header("�̹� �׽�Ʈ ���� - Slide")]
    public List<SlideData> slides = new List<SlideData>()
    {
        new SlideData { route=new[]{1,2,5,8,9,5}, startTime=3.50f, moveTime=2.5f },
        // �ʿ��ϸ� ���� �� �߰�
    };

    int tapSpawnIdx = 0;
    int slideSpawnIdx = 0;

    void Start()
    {
        // zoneAnchors ��������� �ڵ����� Zone_1 ~ Zone_9 ã��
        if (zoneAnchors == null || zoneAnchors.Length < 9)
            zoneAnchors = FindZoneAnchors();

        music.Play();
    }

    void Update()
    {
        float t = music.time;

        // Tap ����
        while (tapSpawnIdx < taps.Count && t >= taps[tapSpawnIdx].time - spawnAhead)
        {
            SpawnTap(taps[tapSpawnIdx]);
            tapSpawnIdx++;
        }

        // Slide ����
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
        if (pos == Vector3.positiveInfinity) { Debug.LogWarning($"Zone_{d.zone} ����"); return; }

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
        if (slideBundlePrefab == null) { Debug.LogError("slideBundlePrefab ���Ҵ�"); return; }

        var go = Instantiate(slideBundlePrefab, Vector3.zero, Quaternion.identity);

        var path = go.GetComponentInChildren<SlidePath>();
        var arrow = go.GetComponentInChildren<SlideArrow>();

        if (path == null || arrow == null)
        {
            Debug.LogError("SlideBundle �����տ� SlidePath/SlideArrow�� �����ϴ�.");
            Destroy(go);
            return;
        }

        // ���/��Ŀ ���� (SlidePath = ���� �ҽ�)
        path.route = d.route;
        path.zoneAnchors = zoneAnchors;
        path.DrawPath();
        
        // Arrow �ʱ�ȭ (���� ���� ��: moveTime���θ� �̵�)
        // SlideArrow�� Init(path, input, moveTime)�� �����Ѵٰ� ����
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
