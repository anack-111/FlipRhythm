using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using UnityEngine.SceneManagement;
public class TileGenerator2D : EditorWindow
{
    GameObject tilePrefab;  // 생성할 타일 프리팹
    int rows = 5;           // 행 수
    int cols = 5;           // 열 수
    float spacing = 1f;     // 타일 간격
    [MenuItem("Tools/2D Tile Grid Generator")]
    static void Init()
    {
        GetWindow<TileGenerator2D>("2D Tile Grid Generator");
    }
    private void OnGUI()
    {
        // 타일 프리팹 선택
        tilePrefab = (GameObject)EditorGUILayout.ObjectField("Tile Prefab", tilePrefab, typeof(GameObject), false);
        // 행, 열, 간격 설정
        rows = EditorGUILayout.IntField("Rows", rows);
        cols = EditorGUILayout.IntField("Cols", cols);
        spacing = EditorGUILayout.FloatField("Spacing", spacing);
        // 맵 생성 버튼
        if (GUILayout.Button("Generate"))
        {
            GenerateGrid();
        }
    }
    private void GenerateGrid()
    {
        // 타일 프리팹이 할당되지 않은 경우 경고 메시지 출력
        if (tilePrefab == null)
        {
            Debug.LogWarning("타일 프리팹을 할당해주세요.");
            return;
        }
        // 부모 GameObject 생성
        GameObject parent = new GameObject("TileGrid");
        // 2D 격자 생성
        for (int y = 0; y < rows; y++)  // 행
        {
            for (int x = 0; x < cols; x++)  // 열
            {
                // 위치 계산 (2D 평면에서만 X, Y 좌표 사용)
                Vector3 pos = new Vector3(x * spacing, y * spacing, 0);
                // 타일 프리팹을 씬에 생성하고 위치 설정
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(tilePrefab, SceneManager.GetActiveScene());
                instance.transform.position = pos;
                instance.transform.SetParent(parent.transform);  // 부모 오브젝트 아래에 배치
            }
        }
    }
}