using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Define;

public class RunTimeMapPlacer : EditorWindow
{
    private PrefabManager prefabManagerAsset;   // Prefab 자동 로드 매니저
    private string[] prefabNames;          // 드롭다운용 이름 목록
    private int selectedPrefabIndex = 0;   // 현재 선택된 인덱스

    private int rows = 5;
    private int cols = 5;
    private float spacing = 1f;

    [MenuItem("Tools/RunTimeMapPlacer")]
    static void Init()
    {
        GetWindow<RunTimeMapPlacer>("RunTimeMapPlacer");
    }

    void OnEnable()
    {
        if (prefabManagerAsset == null)
            prefabManagerAsset = AssetDatabase.LoadAssetAtPath<PrefabManager>("Assets/Editor/PrefabManager.asset");

        prefabManagerAsset.LoadAllPrefabs();
        prefabNames = prefabManagerAsset.GetPrefabNames();
    }

    void OnGUI()
    {
        if (prefabManagerAsset == null)
        {
            EditorGUILayout.HelpBox("PrefabManager가 씬에 존재해야 합니다!", MessageType.Warning);
            return;
        }

        EditorGUILayout.LabelField("자동 프리팹 배치기", EditorStyles.boldLabel);

        //  폴더에서 자동으로 로드된 프리팹 드롭다운
        selectedPrefabIndex = EditorGUILayout.Popup("Prefab Type", selectedPrefabIndex, prefabNames);

        rows = EditorGUILayout.IntField("Rows", rows);
        cols = EditorGUILayout.IntField("Cols", cols);
        spacing = EditorGUILayout.FloatField("Spacing", spacing);

        GUILayout.Space(10);
        if (GUILayout.Button("Generate Grid (Selected Note X)"))
            GenerateGrid();

        if (GUILayout.Button("Clear Map Objects"))
            ClearGenerated();
    }

    void GenerateGrid()
    {
        var editor = FindObjectOfType<MapEditorUIManager>();
        if (editor == null || editor.mapParent == null)
        {
            Debug.LogError(" MapEditorUIManager 또는 mapParent가 없습니다!");
            return;
        }

        string prefabKey = prefabNames[selectedPrefabIndex];
        GameObject prefab = prefabManagerAsset.GetPrefabByType(prefabKey);
        if (prefab == null)
        {
            Debug.LogError($" '{prefabKey}' 프리팹을 찾을 수 없습니다!");
            return;
        }

        // 부모 그룹 생성
        float baseX = editor.selectedNoteX;
        GameObject gridParent = new GameObject($"{prefabKey}");
        gridParent.transform.SetParent(editor.mapParent);
        gridParent.transform.position = new Vector3(baseX, 0, 0);

        // 격자 생성
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Vector3 pos = new Vector3(x * spacing, y * spacing, 0);
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, SceneManager.GetActiveScene());
                instance.transform.SetParent(gridParent.transform);
                instance.transform.localPosition = pos;
                instance.tag = "Editable";

                // TileController 자동 추가
                TileController tile = instance.GetComponent<TileController>();
                if (tile == null) tile = instance.AddComponent<TileController>();
                tile.TileType = (ETileType)System.Enum.Parse(typeof(ETileType), prefabKey);
            }
        }

        Debug.Log($" {prefabKey} {rows * cols}개 생성 완료 (기준 X={baseX:F2})");
    }

    void ClearGenerated()
    {
        var editor = FindObjectOfType<MapEditorUIManager>();
        if (editor == null || editor.mapParent == null)
        {
            Debug.LogWarning(" mapParent를 찾을 수 없습니다.");
            return;
        }

        for (int i = editor.mapParent.childCount - 1; i >= 0; i--)
        {
            Transform child = editor.mapParent.GetChild(i);
            if (child.CompareTag("Editable"))
                DestroyImmediate(child.gameObject);
        }

        Debug.Log(" 기존 타일 제거 완료.");
    }
}
