using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

public class NoteManager : MonoBehaviour
{
    [Header("참조")]
    public AudioSource music;
    public InputRouter input;

    [Header("프리팹")]
    public GameObject tapNotePrefab; // TapNote 프리팹
    public float approachTime = 1.0f; // 오른쪽에서 판정선까지 도달시간

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

        // 1) persistentDataPath (사용자가 또는 외부에서 덮어쓴 경우)
        if (File.Exists(persistentPath))
        {
            json = File.ReadAllText(persistentPath);
            Debug.Log($"[LoadChart] loaded from persistentDataPath: {persistentPath}");
        }

        // 2) StreamingAssets (빌드에 포함된 기본 파일) - Android는 UnityWebRequest 필요
        if (string.IsNullOrEmpty(json))
        {
            string streamingPath = Path.Combine(Application.streamingAssetsPath, filename);
            Debug.Log($"[LoadChart] trying StreamingAssets: {streamingPath}");

            // Android: streamingPath는 "jar:file://..." 형태가 되므로 UnityWebRequest 사용
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

        // 3) Resources 폴더 (옵션) ? Resources에 넣었다면 파일명(확장자 제거)으로 로드
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

        // 파싱: Chart 구조 확인 및 배열 래핑 처리(혹시 파일이 배열 형태이면)
        Chart chart = null;
        try
        {
            chart = JsonUtility.FromJson<Chart>(json);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[LoadChart] JsonUtility.FromJson 예외: {ex.Message}");
        }

        if ((chart == null || chart.hits == null) && json.TrimStart().StartsWith("["))
        {
            // 배열 형태면 래핑해서 파싱
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
            Debug.LogError("[LoadChart] chart 파싱 실패.");
            taps = new List<TapData>();
            yield break;
        }

        taps = chart.hits ?? new List<TapData>();
        taps.Sort((a, b) => a.time.CompareTo(b.time));
        Debug.Log($" Loaded {taps.Count} notes from chart ({MusicName})");
    }

    void SpawnTap(TapData d)
    {
        // 각 존 위치 찾기
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
        public List<TapData> hits;  // ← taps → hits
        public int offsetMs;
    }
}
