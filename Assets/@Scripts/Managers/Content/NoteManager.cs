using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class NoteManager : MonoBehaviour
{
    [Header("����")]
    public AudioSource music;
    public InputRouter input;

    [Header("������")]
    public GameObject tapNotePrefab; // TapNote ������
    public float approachTime = 1.0f; // �����ʿ��� ���������� ���޽ð�

    private List<TapData> taps = new List<TapData>();
    private int spawnIndex = 0;

    public Transform _startPos;

    void Start()
    {
        LoadChart();
        music.Play();
    }

    void Update()
    {
        if (music == null || taps.Count == 0)
            return;

        float now = music.time;

        // spawnTime = hitTime - approachTime
        while (spawnIndex < taps.Count && now >= taps[spawnIndex].time - approachTime)
        {
            SpawnTap(taps[spawnIndex]);
            spawnIndex++;
        }
    }

    void LoadChart()
    {
        string path = Path.Combine(Application.persistentDataPath, "chart_final.json");

        if (!File.Exists(path))
        {
            Debug.LogError($"chart_final.json not found at: {path}");
            return;
        }

        var chart = JsonUtility.FromJson<Chart>(File.ReadAllText(path));
        taps = chart.taps;
        taps.Sort((a, b) => a.time.CompareTo(b.time));
        Debug.Log($" Loaded {taps.Count} notes from chart_final.json");
    }

    void SpawnTap(TapData d)
    {
        // �� �� ��ġ ã��
        var zone = GameObject.Find($"Zone_{d.zone}");
        if (zone == null)
        {
            Debug.LogWarning($"Zone_{d.zone} not found");
            return;
        }

        var go = Instantiate(tapNotePrefab, _startPos.position, Quaternion.identity);
        var note = go.GetComponent<TapNote>();
        note.Init(d.zone, d.time, music, input, -2.5f, approachTime);
    }

    [System.Serializable] public class TapData { public int zone; public float time; }
    [System.Serializable] public class Chart { public List<TapData> taps; public int offsetMs; }
}
