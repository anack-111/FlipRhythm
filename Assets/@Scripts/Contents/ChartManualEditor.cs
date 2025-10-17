using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class TapData
{
    public int zone;    // 1~9
    public float time;  // RawRecorder에서 가져온 시간
}

public class ChartManualEditor : MonoBehaviour
{
    [Header("RawRecorder JSON 파일명 (persistentDataPath 기준)")]
    public string rawFileName = "raw_times.json";

    [Header("직접 편집할 타이밍/존 리스트")]
    public List<TapData> taps = new List<TapData>();

    void OnValidate()
    {
        // raw_times.json 불러와서 times를 taps에 자동 채우기 (한 번만)
        if (taps.Count == 0)
        {
            string path = Path.Combine(Application.persistentDataPath, rawFileName);
            if (!File.Exists(path)) return;

            var raw = JsonUtility.FromJson<RawTimes>(File.ReadAllText(path));
            foreach (var t in raw.times)
                taps.Add(new TapData { time = t, zone = 1 }); // 기본 zone = 1
        }
    }

    [ContextMenu("Save Chart Final")]
    void SaveChartFinal()
    {
        Chart chart = new Chart { taps = taps, offsetMs = 0 };
        string json = JsonUtility.ToJson(chart, true);
        string path = Path.Combine(Application.persistentDataPath, "chart_final.json");
        File.WriteAllText(path, json);
        Debug.Log($"chart_final.json saved: {path}");
    }

    [System.Serializable] public class RawTimes { public float[] times; }
    [System.Serializable] public class Chart { public List<TapData> taps; public int offsetMs; }
}
