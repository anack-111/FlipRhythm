using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class RawRecorder : MonoBehaviour
{
    public AudioSource music;
    List<float> hits = new();

    void Start()
    {
        if (music == null)
            music = GetComponent<AudioSource>();

        if (music != null && !music.isPlaying)
            music.Play();
        else
            Debug.LogWarning("AudioSource �̿��� �Ǵ� �̹� ��� ���Դϴ�.");
    }

    void Update()
    {
        if (music == null) return;

        // �����̽��� �ð� ���
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float t = music.time;
            hits.Add(t);
            Debug.Log($"[REC] hit @ {t:F3}s");
        }

        // U�� �ǵ�����
        if (Input.GetKeyDown(KeyCode.U) && hits.Count > 0)
        {
            hits.RemoveAt(hits.Count - 1);
            Debug.Log("[REC] undo last");
        }

        // S�� ����
        if (Input.GetKeyDown(KeyCode.S))
        {
            hits.Sort();
            var json = JsonUtility.ToJson(new RawTimes { times = hits.ToArray() }, true);
            string path = Path.Combine(Application.persistentDataPath, "raw_times.json");
            File.WriteAllText(path, json);
            Debug.Log($" Saved raw: {path}");
        }
    }

    [System.Serializable]
    public class RawTimes { public float[] times; }
}
