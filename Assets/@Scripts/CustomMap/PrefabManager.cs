using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabManager", menuName = "MapEditor/PrefabManager")]
public class PrefabManager : ScriptableObject
{
    public string prefabFolder = "Assets/@Resources/Prefabs/Object";
    private Dictionary<string, GameObject> prefabDict = new();

#if UNITY_EDITOR
    public void LoadAllPrefabs()
    {
        prefabDict.Clear();
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Prefab", new[] { prefabFolder });
        foreach (string guid in guids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
                prefabDict[prefab.name] = prefab;
        }
    }
#endif

    public string[] GetPrefabNames() => new List<string>(prefabDict.Keys).ToArray();
    public GameObject GetPrefabByType(string name) =>
        prefabDict.TryGetValue(name, out var prefab) ? prefab : null;
}
