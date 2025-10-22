using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorUIManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public Slider timeSlider;
    public Button startButton;

    [Header("References")]
    public Transform player;        //  플레이어 Transform
    public Transform mapParent;     // 노트 부모 오브젝트

    [Header("Prefabs")]
    public GameObject notePrefab;

    [Header("Map Data")]
    public TextAsset csvFile;

    private List<MapObject> mapObjects = new();
    private bool isPlaying = true;
    private float playerSpeed = 10.4f;   //  FixedUpdate 이동 속도와 동일하게 고정

    void Start()
    {
        startButton.onClick.AddListener(TogglePlay);
        timeSlider.onValueChanged.AddListener(OnSliderChanged);
        LoadCSV();

        //  음악 길이에 맞춰 맵 길이 계산
        float mapLength = audioSource.clip.length * playerSpeed;

        //  CSV에서 불러온 노트 생성
        foreach (var obj in mapObjects)
            CreateNoteMarker(obj);
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //    TogglePlay();

        if (isPlaying)
        {
            // 슬라이더 진행도 업데이트
            timeSlider.value = audioSource.time / audioSource.clip.length;

            // 카메라를 플레이어 위치에 맞춰 이동
            UpdateCamera();
        }
    }

    void FixedUpdate()
    {
        if (isPlaying)
        {
            player.position = Vector3.right * (audioSource.time * playerSpeed);


        }
    }

    void TogglePlay()
    {
        if (isPlaying)
        {
            audioSource.Pause();
        }
        else
        {
            // 오디오 시간 = 현재 플레이어 위치 기준으로 동기화 시작
            audioSource.time = player.position.x / playerSpeed;
            audioSource.Play();
        }

        isPlaying = !isPlaying;
    }

    void OnSliderChanged(float value)
    {
        // 슬라이더를 움직이면 오디오 및 플레이어 위치 이동
        audioSource.time = value * audioSource.clip.length;
        player.position = new Vector3(audioSource.time * playerSpeed, 0, 0);
        UpdateCamera();
    }

    void UpdateCamera()
    {
        // 카메라는 플레이어를 따라감
        Camera.main.transform.position = new Vector3(player.position.x, 0, -10);
    }

    void LoadCSV()
    {
        mapObjects.Clear();
        string[] lines = csvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            string[] data = lines[i].Trim().Split(',');

            float timeVal;
            if (!float.TryParse(data[6], NumberStyles.Float, CultureInfo.InvariantCulture, out timeVal))
                continue;

            MapObject obj = new MapObject
            {
                time = timeVal // ms → 초
            };
            mapObjects.Add(obj);
        }

        Debug.Log($"Loaded {mapObjects.Count} notes from CSV.");
    }

    void CreateNoteMarker(MapObject obj)
    {
        float xPos = obj.time * playerSpeed; //  시간 기반으로 X 위치 계산
        Vector3 spawnPos = new Vector3(xPos, 0f, 0f);

        GameObject marker = Instantiate(notePrefab, spawnPos, Quaternion.identity);
        marker.transform.localScale = Vector3.one * 0.2f;
        marker.name = $"Note_{obj.time:F2}";
    }
}

[System.Serializable]
public class MapObject
{
    public float time;
}
