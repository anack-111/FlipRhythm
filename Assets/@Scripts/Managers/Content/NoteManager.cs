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

    [Header("����")]
    public AudioSource music;
    public InputRouter input;

    [Header("������")]
    public GameObject tapNotePrefab;

    float _startPosY = 0.6f;

    [Header("�� (Zone_1~9)")]
    public Transform[] zones;

    [Header("����")]
    public float spawnX = 8f;          // ��Ʈ ���� ��ġ (������)
    public float judgeX = -4f;         // ������ ��ġ (����)
    public float approachTime = 2.0f;  // ��Ʈ�� �̵��ϴ� �ð� (1�ʰ� ����)

    [Header("�׽�Ʈ ����")]
    public List<TapData> taps;
    int spawnIndex = 0;

    void Start()
    {
        music.Play();
    }

    void Update()
    {
        float now = music.time;

        // Spawn Notes �̸� ����
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
        float currentTime = music.time;          // ���� ����� �ð�(��)
        float totalTime = music.clip.length;   // ���� ��ü ����(��)
    }
}
