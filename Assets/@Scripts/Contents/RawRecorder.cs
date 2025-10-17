using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class RawRecorder : MonoBehaviour
{
    public AudioSource music;
    List<HitData> hits = new();
    public string _fileName = "raw.json";

    void Start()
    {
        if (music == null)
            music = GetComponent<AudioSource>();

        if (music != null && !music.isPlaying)
            music.Play();
        else
            Debug.LogWarning("AudioSource 미연결 또는 이미 재생 중입니다.");
    }

    void Update()
    {
        if (music == null) return;

        // 숫자패드 1~9 입력 체크
        for (int i = 1; i <= 9; i++)
        {
            KeyCode key = KeyCode.Keypad0 + i;
            if (Input.GetKeyDown(key))
            {
                float t = music.time;
                hits.Add(new HitData { zone = i, time = t });
                Debug.Log($"[REC] zone {i} hit @ {t:F3}s");
            }
        }

        // 되돌리기 (U)
        if (Input.GetKeyDown(KeyCode.U) && hits.Count > 0)
        {
            hits.RemoveAt(hits.Count - 1);
            Debug.Log("[REC] undo last");
        }

        // 저장 (S)
        if (Input.GetKeyDown(KeyCode.S))
        {
            hits.Sort((a, b) => a.time.CompareTo(b.time));
            var json = JsonUtility.ToJson(new RawData { hits = hits.ToArray() }, true);
            string path = Path.Combine(Application.persistentDataPath, _fileName + ".json");
            File.WriteAllText(path, json);
            Debug.Log($"Saved raw: {path}");
        }
    }

    [System.Serializable]
    public class HitData
    {
        public int zone;
        public float time;
    }

    [System.Serializable]
    public class RawData
    {
        public HitData[] hits;
    }
}
