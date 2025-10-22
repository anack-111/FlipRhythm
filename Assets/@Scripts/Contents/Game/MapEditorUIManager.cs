using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapEditorUIManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public Slider timeSlider;
    public Button startButton;

    [Header("References")]
    public Transform player;
    public Transform mapParent;

    [Header("Prefabs")]
    public GameObject notePrefab;
    public GameObject spikePrefab;
    public GameObject blockPrefab;
    public GameObject ballPrefab;
    public GameObject shipPrefab;
    public GameObject cubePrefab;
    public GameObject spiderPrefab;
    public GameObject G_OffPrefab;
    public GameObject G_OnPrefab;
    public GameObject UFOPrefab;
    public GameObject WavePrefab;

    [Header("Data Files")]
    public TextAsset midiCsvFile; // MIDI -> CSV로 변환한 음악 타이밍용
    public TextAsset mapCsvFile;  // 저장된 맵 데이터 불러오기용

    public string musicMapName = "DefaultMap";
    private string currentType = "Spike";

    private List<NoteMarker> noteMarkers = new();
    private List<MapObject> mapObjects = new();

    private bool isPlaying = true;
    public float playerSpeed = 10.4f;

    public bool IsEditingMode()
    {
        return !isPlaying;

    }
    void Start()
    {
        startButton.onClick.AddListener(TogglePlay);
        timeSlider.onValueChanged.AddListener(OnSliderChanged);

        //  MIDI 기반 CSV 로드 (음악 타이밍용)
        if (midiCsvFile != null)
            LoadMusicCSV();

        //  저장된 맵 데이터 로드
        if (mapCsvFile != null)
            LoadMapCSV(mapCsvFile);
    }

    void Update()
    {
        if (isPlaying)
        {
            timeSlider.value = audioSource.time / audioSource.clip.length;
            UpdateCamera();
        }

        //  클릭 시 오브젝트 배치 (UI 위 클릭 제외)
        if (Input.GetMouseButtonDown(0) && !isPlaying && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            CreateCustomObject(pos);
        }

        //  Ctrl + S 저장
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

    //  재생/정지
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

    //  슬라이더로 시간 이동
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
    //  음악용 MIDI CSV 로드
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

    void CreateNoteMarker(NoteMarker note)
    {
        float xPos = note.time * playerSpeed;
        Vector3 spawnPos = new Vector3(xPos, 0f, 0f);
        GameObject marker = Instantiate(notePrefab, spawnPos, Quaternion.identity);
        marker.transform.localScale = Vector3.one * 0.2f;
        marker.name = $"Note_{note.time:F2}";
        marker.transform.SetParent(mapParent);
    }

    // ---------------------------
    //  맵 오브젝트 생성
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

    // ---------------------------
    //  프리팹 선택
    // ---------------------------
    public void SetObjectType(string type)
    {
        currentType = type;
        Debug.Log($"[Editor] Selected object type: {type}");
    }

    GameObject GetPrefabByType(string type)
    {
        return type switch
        {
            "Spike" => spikePrefab,
            "Block" => blockPrefab,
            "ball" => ballPrefab,
            "Spider" => spiderPrefab,
            "Ship" => shipPrefab,
            "Cube" => cubePrefab,
            "Gravity_Off" => G_OffPrefab,
            "Gravity_On" => G_OnPrefab,
            "Wave" => WavePrefab,
            "UFO" => UFOPrefab,
            _ => null
        };
    }

    // ---------------------------
    //  맵 저장 (type, pos, rot)
    // ---------------------------
    void SaveMapCSV()
    {
        string folderPath = Application.dataPath + "/Data/MapData";
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string path = folderPath + "/" + musicMapName + "_Map.csv";

        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteLine("type,x,y,z,rotation");

            foreach (Transform child in mapParent)
            {
                // 편집 중 생성된 오브젝트만 저장
                if (!child.CompareTag("Editable")) continue;

                writer.WriteLine(
                    $"{child.name.Split('_')[0]}," +
                    $"{child.position.x.ToString(CultureInfo.InvariantCulture)}," +
                    $"{child.position.y.ToString(CultureInfo.InvariantCulture)}," +
                    $"{child.position.z.ToString(CultureInfo.InvariantCulture)}," +
                    $"{child.eulerAngles.z.ToString(CultureInfo.InvariantCulture)}"
                );
            }
        }

        Debug.Log($" Saved map objects from current Scene transforms to: {path}");
    }


    // ---------------------------
    //  맵 로드
    // ---------------------------
    void LoadMapCSV(TextAsset mapFile)
    {
        mapObjects.Clear();
        string[] lines = mapFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            string[] data = lines[i].Trim().Split(',');
            if (data.Length < 5) continue;

            string typeVal = data[0];
            float x = float.Parse(data[1], CultureInfo.InvariantCulture);
            float y = float.Parse(data[2], CultureInfo.InvariantCulture);
            float z = float.Parse(data[3], CultureInfo.InvariantCulture);
            float rot = float.Parse(data[4], CultureInfo.InvariantCulture);

            MapObject obj = new MapObject
            {
                type = typeVal,
                position = new Vector3(x, y, z),
                rotation = rot
            };
            mapObjects.Add(obj);

            GameObject prefab = GetPrefabByType(typeVal);
            if (prefab != null)
                Instantiate(prefab, obj.position, Quaternion.Euler(0, 0, rot), mapParent);
        }

        Debug.Log($" Loaded {mapObjects.Count} map objects from {mapFile.name}");
    }

    public void RemoveObject(MapEditableObject obj)
    {
        if (mapObjects.Contains(obj.linkedData))
            mapObjects.Remove(obj.linkedData);

        Debug.Log($"Removed object: {obj.linkedData.type}");
    }


}

//  MIDI 기반 노트 마커 데이터
[System.Serializable]
public class NoteMarker
{
    public float time;
}

// 맵 오브젝트 데이터
[System.Serializable]
public class MapObject
{
    public string type;
    public Vector3 position;
    public float rotation;
}
