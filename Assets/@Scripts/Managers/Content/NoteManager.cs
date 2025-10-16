using UnityEngine;
using System.Collections.Generic;

public class NoteManager : MonoBehaviour
{
    [System.Serializable]
    public class TapData 
    {
        public int zone;
        public float time; 
    }

    [Header("참조")]
    public AudioSource music;
    public InputRouter input;

    [Header("프리팹")]
    public GameObject tapNotePrefab;

    float _startPosY = 0.6f;

    [Header("존 (Zone_1~9)")]
    public Transform[] zones;

    [Header("설정")]
    public float spawnX = 8f;          // 노트 생성 위치 (오른쪽)
    public float judgeX = -4f;         // 판정선 위치 (왼쪽)
    public float approachTime = 2.0f;  // 노트가 이동하는 시간 (1초간 접근)

    [Header("테스트 패턴")]
    public List<TapData> taps;
    int spawnIndex = 0;

    void Start()
    {
        music.Play();
    }

    void Update()
    {
        float now = music.time;

        // Spawn Notes 미리 등장
        while (spawnIndex < taps.Count && now >= taps[spawnIndex].time - approachTime)
        {
            SpawnTap(taps[spawnIndex]);
            spawnIndex++;
        }
    }

    void SpawnTap(TapData data)
    {
        Vector3 startPos = new(spawnX, _startPosY, 0);
        var go = Instantiate(tapNotePrefab, startPos, Quaternion.identity);

        var note = go.GetComponent<TapNote>();
        note.Init(data.zone, data.time, music, input, judgeX, approachTime);
    }

    public void GetMusicTime()
    {
        float currentTime = music.time;          // 현재 재생된 시간(초)
        float totalTime = music.clip.length;   // 음악 전체 길이(초)
    }
}
