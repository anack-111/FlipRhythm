using System.Globalization;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    [Header("Map Data")]
    public TextAsset mapCsvFile;

    [Header("References")]
    public Transform mapParent;
    public float playerSpeed = 10.4f;

    [Header("Managers")]
    public PrefabManager prefabManager; // ScriptableObject 버전

    void Start()
    {
        if (mapCsvFile == null)
        {
            Debug.LogError(" Map CSV 파일이 지정되지 않았습니다!");
            return;
        }

        if (prefabManager == null)
        {
            Debug.LogError(" PrefabManager(ScriptableObject)가 지정되지 않았습니다!");
            return;
        }

        LoadMapCSV(mapCsvFile);
    }

    void LoadMapCSV(TextAsset csv)
    {
        string[] lines = csv.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            string[] data = lines[i].Trim().Split(',');
            if (data.Length < 5) continue;

            string type = data[0];
            float x = float.Parse(data[1], CultureInfo.InvariantCulture);
            float y = float.Parse(data[2], CultureInfo.InvariantCulture);
            float z = float.Parse(data[3], CultureInfo.InvariantCulture);
            float rot = float.Parse(data[4], CultureInfo.InvariantCulture);

            Vector3 pos = new Vector3(x, y, z);
            Quaternion rotQ = Quaternion.Euler(0, 0, rot);

            // ScriptableObject PrefabManager에서 prefab 찾기
            GameObject prefab = prefabManager.GetPrefabByType(type);
            if (prefab == null)
            {
                Debug.LogWarning($" '{type}' 프리팹을 찾을 수 없습니다. ({pos})");
                continue;
            }

            GameObject obj = Instantiate(prefab, pos, rotQ, mapParent);
            obj.name = type;
        }

        Debug.Log($" 맵 로드 완료 ({lines.Length - 1}개 오브젝트)");
    }
}
