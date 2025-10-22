using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using static Define;

public class CSVMapLoader : MonoBehaviour
{
    public TextAsset csvFile; // ����Ƽ �ν����Ϳ��� csv ����
    public List<MapObject> mapObjects = new List<MapObject>();

    void Start()
    {
        LoadCSV();
    }

    void LoadCSV()
    {
        string[] lines = csvFile.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            string[] data = lines[i].Split(',');
            if (data.Length < 4) continue;

            MapObject obj = new MapObject();
            obj.time = float.Parse(data[0], CultureInfo.InvariantCulture) / 1000f; // ms �� �� ��ȯ


            mapObjects.Add(obj);
        }
    }
}
