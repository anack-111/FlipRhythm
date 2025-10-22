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
    public Transform player;        //  �÷��̾� Transform
    public Transform mapParent;     // ��Ʈ �θ� ������Ʈ

    [Header("Prefabs")]
    public GameObject notePrefab;

    [Header("Map Data")]
    public TextAsset csvFile;

    private List<MapObject> mapObjects = new();
    private bool isPlaying = true;
    private float playerSpeed = 10.4f;   //  FixedUpdate �̵� �ӵ��� �����ϰ� ����

    void Start()
    {
        startButton.onClick.AddListener(TogglePlay);
        timeSlider.onValueChanged.AddListener(OnSliderChanged);
        LoadCSV();

        //  ���� ���̿� ���� �� ���� ���
        float mapLength = audioSource.clip.length * playerSpeed;

        //  CSV���� �ҷ��� ��Ʈ ����
        foreach (var obj in mapObjects)
            CreateNoteMarker(obj);
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //    TogglePlay();

        if (isPlaying)
        {
            // �����̴� ���൵ ������Ʈ
            timeSlider.value = audioSource.time / audioSource.clip.length;

            // ī�޶� �÷��̾� ��ġ�� ���� �̵�
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
            // ����� �ð� = ���� �÷��̾� ��ġ �������� ����ȭ ����
            audioSource.time = player.position.x / playerSpeed;
            audioSource.Play();
        }

        isPlaying = !isPlaying;
    }

    void OnSliderChanged(float value)
    {
        // �����̴��� �����̸� ����� �� �÷��̾� ��ġ �̵�
        audioSource.time = value * audioSource.clip.length;
        player.position = new Vector3(audioSource.time * playerSpeed, 0, 0);
        UpdateCamera();
    }

    void UpdateCamera()
    {
        // ī�޶�� �÷��̾ ����
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
                time = timeVal // ms �� ��
            };
            mapObjects.Add(obj);
        }

        Debug.Log($"Loaded {mapObjects.Count} notes from CSV.");
    }

    void CreateNoteMarker(MapObject obj)
    {
        float xPos = obj.time * playerSpeed; //  �ð� ������� X ��ġ ���
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
