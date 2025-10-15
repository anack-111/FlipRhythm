using UnityEngine;
using System.Collections.Generic;

public class NoteManager : MonoBehaviour
{
    [System.Serializable]
    public class TapData { public int zone; public float time; }

    [Header("참조")]
    public AudioSource music;
    public InputRouter input;

    [Header("프리팹")]
    public GameObject tapNotePrefab;   // TapNote + SpriteRenderer 포함
    public GameObject slideNotePrefab; // SlideNote + nodes(3개 SpriteRenderer) 포함

    [Header("이번 테스트 패턴")]
    public List<TapData> taps = new List<TapData>()
    {
        new TapData{ zone=3, time=1.00f },
        new TapData{ zone=7, time=1.50f },
        new TapData{ zone=2, time=2.00f },
        new TapData{ zone=8, time=2.50f },
        new TapData{ zone=5, time=3.00f },
    };
    public int[] slideRoute = { 1, 5, 4 };
    public float slideStartTime = 4.00f;
    public float slideStepGap = 0.35f;

    int tapSpawnIdx = 0;
    bool slideSpawned = false;

    void Start()
    {
        music.Play();
    }

    void Update()
    {
        float t = music.time;

        // Tap 스폰 (미리 0.5초 전에 등장)
        while (tapSpawnIdx < taps.Count && t >= taps[tapSpawnIdx].time - 1f)
        {
            SpawnTap(taps[tapSpawnIdx]);
            tapSpawnIdx++;
        }

        //// Slide 스폰 (미리 1초 전에 등장)
        //if (!slideSpawned && t >= slideStartTime -1f)
        //{
        //    SpawnSlide(slideRoute, slideStartTime, slideStepGap);
        //    slideSpawned = true;
        //}
    }

    void SpawnTap(TapData d)
    {
        var zone = GameObject.Find($"Zone_{d.zone}");
        if (zone == null) { Debug.LogWarning($"Zone_{d.zone} 없음"); return; }

        var go = Instantiate(tapNotePrefab, zone.transform.position, Quaternion.identity);
        var tn = go.GetComponent<TapNote>();
        tn.zoneIndex = d.zone;
        tn.time = d.time;
        tn.music = music;
        tn.input = input;
        Debug.Log($"[Spawn Tap] Z{d.zone} @ {d.time:F2}");
    }

    void SpawnSlide(int[] route, float start, float gap)
    {
        var go = Instantiate(slideNotePrefab, Vector3.zero, Quaternion.identity);
        var sn = go.GetComponent<SlideNote>();
        sn.music = music;
        sn.input = input;
        sn.route = (int[])route.Clone();
        sn.startTime = start;
        sn.stepGap = gap;

        // 노드 3개를 각 Zone 위치로 스냅
        for (int i = 0; i < sn.route.Length && i < sn.nodes.Length; i++)
        {
            var zone = GameObject.Find($"Zone_{sn.route[i]}");
            if (zone) sn.nodes[i].transform.position = zone.transform.position;
        }
      
    }
}


    