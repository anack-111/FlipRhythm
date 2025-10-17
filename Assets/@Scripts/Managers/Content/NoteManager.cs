using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

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

    public string MusicName;

    void Start()
    {
        StartCoroutine(LoadChartAndPlay());
    }

    IEnumerator LoadChartAndPlay()
    {
        yield return StartCoroutine(LoadChartCoroutine());
        if (music != null)
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

    IEnumerator LoadChartCoroutine()
    {
        string filename = MusicName + ".json";
        string persistentPath = Path.Combine(Application.persistentDataPath, filename);
        string json = null;

        // 1) persistentDataPath (����ڰ� �Ǵ� �ܺο��� ��� ���)
        if (File.Exists(persistentPath))
        {
            json = File.ReadAllText(persistentPath);
            Debug.Log($"[LoadChart] loaded from persistentDataPath: {persistentPath}");
        }

        // 2) StreamingAssets (���忡 ���Ե� �⺻ ����) - Android�� UnityWebRequest �ʿ�
        if (string.IsNullOrEmpty(json))
        {
            string streamingPath = Path.Combine(Application.streamingAssetsPath, filename);
            Debug.Log($"[LoadChart] trying StreamingAssets: {streamingPath}");

            // Android: streamingPath�� "jar:file://..." ���°� �ǹǷ� UnityWebRequest ���
            if (streamingPath.StartsWith("jar:") || streamingPath.StartsWith("file://") || streamingPath.Contains("://"))
            {
                using (UnityWebRequest www = UnityWebRequest.Get(streamingPath))
                {
                    yield return www.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
                    if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
#else
                    if (www.isNetworkError || www.isHttpError)
#endif
                    {
                        Debug.LogWarning($"[LoadChart] StreamingAssets load failed: {www.error}");
                    }
                    else
                    {
                        json = www.downloadHandler.text;
                        Debug.Log($"[LoadChart] loaded from StreamingAssets (UnityWebRequest).");
                    }
                }
            }
            else
            {
                if (File.Exists(streamingPath))
                {
                    json = File.ReadAllText(streamingPath);
                    Debug.Log($"[LoadChart] loaded from StreamingAssets file path.");
                }
            }
        }

        // 3) Resources ���� (�ɼ�) ? Resources�� �־��ٸ� ���ϸ�(Ȯ���� ����)���� �ε�
        if (string.IsNullOrEmpty(json))
        {
            string resourceKey = Path.GetFileNameWithoutExtension(filename);
            TextAsset ta = Resources.Load<TextAsset>(resourceKey);
            if (ta != null)
            {
                json = ta.text;
                Debug.Log($"[LoadChart] loaded from Resources: {resourceKey}");
            }
        }

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError($"[LoadChart] JSON not found for {MusicName}. Tried persistent, StreamingAssets, Resources.");
            taps = new List<TapData>();
            yield break;
        }

        // �Ľ�: Chart ���� Ȯ�� �� �迭 ���� ó��(Ȥ�� ������ �迭 �����̸�)
        Chart chart = null;
        try
        {
            chart = JsonUtility.FromJson<Chart>(json);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[LoadChart] JsonUtility.FromJson ����: {ex.Message}");
        }

        if ((chart == null || chart.hits == null) && json.TrimStart().StartsWith("["))
        {
            // �迭 ���¸� �����ؼ� �Ľ�
            string wrapped = "{\"hits\":" + json + "}";
            try
            {
                chart = JsonUtility.FromJson<Chart>(wrapped);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[LoadChart] wrapped parse failed: {ex.Message}");
            }
        }

        if (chart == null)
        {
            Debug.LogError("[LoadChart] chart �Ľ� ����.");
            taps = new List<TapData>();
            yield break;
        }

        taps = chart.hits ?? new List<TapData>();
        taps.Sort((a, b) => a.time.CompareTo(b.time));
        Debug.Log($" Loaded {taps.Count} notes from chart ({MusicName})");
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
    public class Chart
    {
        public List<TapData> hits;  // �� taps �� hits
        public int offsetMs;
    }
}
