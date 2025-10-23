using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapEditorUIManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public Slider timeSlider;
    public Button startButton;
    public Button saveButton;

    [Header("References")]
    public Transform player;
    public Transform mapParent;

    [Header("Data Files")]
    public TextAsset midiCsvFile; // MIDI -> CSV�� ��ȯ�� ���� Ÿ�ֿ̹�
    public TextAsset mapCsvFile;  // ����� �� ������ �ҷ������

    public string musicMapName = "DefaultMap";
    private string currentType = "Spike";

    private List<NoteMarker> noteMarkers = new();
    private List<MapObject> mapObjects = new();

    private bool isPlaying = true;
    public float playerSpeed = 10.4f;

    [HideInInspector] public float selectedNoteX = 0f;
    [SerializeField] private PrefabManager prefabManager;
    [SerializeField] public GameObject notePrefab;
    public TextMeshProUGUI timeText;


    private void Awake()
    {
#if UNITY_EDITOR
        // ������ ȯ�濡�� SO ���� ���� �ε�
        prefabManager = AssetDatabase.LoadAssetAtPath<PrefabManager>("Assets/Editor/PrefabManager.asset");
        if (prefabManager == null)
        {
            Debug.LogError(" PrefabManager.asset�� ã�� �� �����ϴ�! ��θ� Ȯ���ϼ���.");
        }
        else
        {
            prefabManager.LoadAllPrefabs(); // ���� �� ������ �ڵ� �ε�
        }
#else
    Debug.LogWarning("PrefabManager�� ������ ȯ�濡���� �ε�˴ϴ�.");
#endif
    }
    public bool IsEditingMode()
    {
        return !isPlaying;

    }
    void Start()
    {
        startButton.onClick.AddListener(TogglePlay);
        saveButton.onClick.AddListener(SaveMapCSV);

        timeSlider.onValueChanged.AddListener(OnSliderChanged);

        //  MIDI ��� CSV �ε� (���� Ÿ�ֿ̹�)
        if (midiCsvFile != null)
            LoadMusicCSV();

        //  ����� �� ������ �ε�
        if (mapCsvFile != null)
            LoadMapCSV(mapCsvFile);
    }

    void Update()
    {
        if (audioSource != null && audioSource.clip != null && timeText != null)
        {
            float currentTime = audioSource.time;
            float totalTime = audioSource.clip.length;
            timeText.text = $"{Extension.FormatTime(currentTime)} / {Extension.FormatTime(totalTime)}";
        }

        if (isPlaying)
        {
            timeSlider.value = audioSource.time / audioSource.clip.length;
            UpdateCamera();
        }

        ////  Ŭ�� �� ������Ʈ ��ġ (UI �� Ŭ�� ����)
        //if (Input.GetMouseButtonDown(0) && !isPlaying && !EventSystem.current.IsPointerOverGameObject())
        //{
        //    Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    pos.z = 0;
        //    CreateCustomObject(pos);
        //}

        //  Ctrl + S ����
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S))
        {
            SaveMapCSV();
        }
    }

    void FixedUpdate()
    {
        if (isPlaying)
            player.position = Vector3.right * (audioSource.time * playerSpeed);
    }

    //  ���/����
    void TogglePlay()
    {
        if (isPlaying)
            audioSource.Pause();
        else
        {
            audioSource.time = player.position.x / playerSpeed;
            audioSource.Play();
        }

        isPlaying = !isPlaying;
    }

    //  �����̴��� �ð� �̵�
    void OnSliderChanged(float value)
    {
        audioSource.time = value * audioSource.clip.length;
        player.position = new Vector3(audioSource.time * playerSpeed, 0, 0);
        UpdateCamera();
    }

    void UpdateCamera()
    {
        Camera.main.transform.position = new Vector3(player.position.x, 0, -10);
    }

    // ---------------------------
    //  ���ǿ� MIDI CSV �ε�
    // ---------------------------
    void LoadMusicCSV()
    {
        noteMarkers.Clear();
        string[] lines = midiCsvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            string[] data = lines[i].Trim().Split(',');

            float timeVal;
            if (!float.TryParse(data[6], NumberStyles.Float, CultureInfo.InvariantCulture, out timeVal))
                continue;

            NoteMarker note = new NoteMarker { time = timeVal };
            noteMarkers.Add(note);

            CreateNoteMarker(note);
        }

        Debug.Log($" Loaded {noteMarkers.Count} MIDI note markers.");
    }


    public void RegisterGridGroup(GameObject gridGroup)
    {
        foreach (Transform child in gridGroup.transform)
        {
            TileController tile = child.GetComponent<TileController>();
            if (tile == null) continue;

            MapObject obj = new MapObject
            {
                type = tile.TileType.ToString(),
                position = child.position,
                rotation = child.eulerAngles.z
            };
            mapObjects.Add(obj);
        }
    }

    void CreateNoteMarker(NoteMarker note)
    {
        // --- ��Ʈ ���� �θ� �׷� Ȯ�� ---
        Transform notesParent = mapParent.Find("NoteMarkers");
        if (notesParent == null)
        {
            GameObject noteGroup = new GameObject("NoteMarkers");
            noteGroup.transform.SetParent(mapParent);
            noteGroup.transform.localPosition = Vector3.zero;
            notesParent = noteGroup.transform;
        }

        // --- ��Ʈ ���� ---
        float xPos = note.time * playerSpeed;
        Vector3 spawnPos = new Vector3(xPos, 0f, 0f);

        GameObject marker = Instantiate(notePrefab, spawnPos, Quaternion.identity);
        marker.transform.localScale = Vector3.one * 0.2f;
        marker.name = $"Note_{note.time:F2}";
        marker.transform.SetParent(notesParent);

        // --- Ŭ�� �����ϵ��� Collider + Script �߰� ---
        if (marker.GetComponent<Collider2D>() == null)
            marker.AddComponent<BoxCollider2D>();

        if (marker.GetComponent<NoteSelectable>() == null)
            marker.AddComponent<NoteSelectable>();
    }

    // ---------------------------
    //  ������ ����
    // ---------------------------
    public void SetObjectType(string type)
    {
        currentType = type;
        Debug.Log($"[Editor] Selected object type: {type}");
    }

    GameObject GetPrefabByType(string type)
    {
        return prefabManager.GetPrefabByType(type);
    }

    // ---------------------------
    //  �� ���� (type, pos, rot)
    // ---------------------------
    public void SaveMapCSV()
    {
        string folderPath = Application.dataPath + "/Data/MapData";
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string path = folderPath + "/" + musicMapName + "_Map.csv";
        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteLine("type,x,y,z,rotation");

            foreach (Transform group in mapParent)
            {
                // NoteMarkers ����
                if (group.name == "NoteMarkers")
                    continue;

                // �׷� ������� �ڽı��� ����
                if (group.childCount > 0)
                {
                    foreach (Transform child in group)
                    {
                        TileController tile = child.GetComponent<TileController>();
                        if (tile == null) continue;

                        writer.WriteLine(
                            $"{tile.TileType}," +
                            $"{child.position.x.ToString(CultureInfo.InvariantCulture)}," +
                            $"{child.position.y.ToString(CultureInfo.InvariantCulture)}," +
                            $"{child.position.z.ToString(CultureInfo.InvariantCulture)}," +
                            $"{child.eulerAngles.z.ToString(CultureInfo.InvariantCulture)}"
                        );
                    }
                }
                //  ���� ������Ʈ��� ���ε� �˻��ؼ� ����
                else
                {
                    TileController tile = group.GetComponent<TileController>();
                    if (tile == null) continue;

                    writer.WriteLine(
                        $"{tile.TileType}," +
                        $"{group.position.x.ToString(CultureInfo.InvariantCulture)}," +
                        $"{group.position.y.ToString(CultureInfo.InvariantCulture)}," +
                        $"{group.position.z.ToString(CultureInfo.InvariantCulture)}," +
                        $"{group.eulerAngles.z.ToString(CultureInfo.InvariantCulture)}"
                    );
                }
            }
        }

        Debug.Log($" �� ���� �Ϸ�: {path}");
    }



    // ---------------------------
    //  �� �ε�
    // ---------------------------
    public void LoadMapCSV(TextAsset csv)
    {
        if (csv == null) return;
        mapObjects.Clear();

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

            MapObject obj = new MapObject
            {
                type = type,
                position = new Vector3(x, y, z),
                rotation = rot
            };
            mapObjects.Add(obj);

            GameObject prefab = GetPrefabByType(type);
            if (prefab != null)
                Instantiate(prefab, obj.position, Quaternion.Euler(0, 0, rot), mapParent);
        }

        Debug.Log($" �� �ҷ����� �Ϸ� ({mapObjects.Count}�� ������Ʈ)");
    }

    public void RemoveObject(MapEditableObject obj)
    {
        if (mapObjects.Contains(obj.linkedData))
            mapObjects.Remove(obj.linkedData);

        Debug.Log($"Removed object: {obj.linkedData.type}");
    }



    #region �ӽ����
    // ---------------------------
    //  �� ������Ʈ ����
    // ---------------------------
    void CreateCustomObject(Vector3 pos)
    {
        GameObject prefab = GetPrefabByType(currentType);
        if (prefab == null) return;

        GameObject obj = Instantiate(prefab, pos, Quaternion.identity, mapParent);
        obj.tag = "Editable";
        obj.name = $"{currentType}_{pos.x:F2}";

        MapObject newObj = new MapObject
        {
            type = currentType,
            position = pos,
            rotation = obj.transform.eulerAngles.z
        };
        mapObjects.Add(newObj);

        Debug.Log($" Placed {currentType} at {pos}");
    }
    #endregion
}

//  MIDI ��� ��Ʈ ��Ŀ ������
[System.Serializable]
public class NoteMarker
{
    public float time;
}

// �� ������Ʈ ������
[System.Serializable]
public class MapObject
{
    public string type;
    public Vector3 position;
    public float rotation;
}
