using UnityEngine;
using MidiPlayerTK;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class MidiToJsonExporter : MonoBehaviour
{
    public MidiFilePlayer midiPlayer;

    [Header("필터 옵션")]
    public int targetTrack = 0;     // 특정 트랙만 추출
    public int targetChannel = 0;   // 특정 채널만 추출
    public float minNoteInterval = 0.05f; // 노트 최소 간격 (초)
    public bool logNotes = true;    // 콘솔 출력 여부

    [System.Serializable]
    public class Hit
    {
        public int zone;
        public float time;
    }

    [System.Serializable]
    public class HitList
    {
        public List<Hit> hits = new List<Hit>();
    }

    private HitList result = new HitList();
    private float lastNoteTime = -999f;

    void Start()
    {
        if (midiPlayer == null)
        {
            Debug.LogError(" MidiFilePlayer reference is missing!");
            return;
        }

        // List<MPTKEvent> 이벤트 수신
        midiPlayer.OnEventNotesMidi.AddListener(OnNoteEvents);

        midiPlayer.MPTK_Load();
        midiPlayer.MPTK_Play();
    }

    void OnNoteEvents(List<MPTKEvent> events)
    {
        if (events == null) return;

        foreach (var e in events)
        {
            //  NoteOn만 추출
            if (e.Command == MPTKCommand.NoteOn && e.Velocity > 0)
            {
                //  트랙/채널 필터링
                if (e.Track != targetTrack || e.Channel != targetChannel)
                    continue;

                float timeSec = (float)(e.RealTime / 1000.0);

                // 너무 촘촘한 노트는 스킵 (게임 플레이성을 위해)
                if (timeSec - lastNoteTime < minNoteInterval)
                    continue;

                lastNoteTime = timeSec;

                // 음 높이를 1~9로 매핑
                int zone = (e.Value % 9) + 1;

                result.hits.Add(new Hit
                {
                    zone = zone,
                    time = timeSec
                });

                if (logNotes)
                    Debug.Log($"🎶 Note -> Track {e.Track} | Ch {e.Channel} | Zone {zone} | Time {timeSec:F3}s | Pitch {e.Value}");
            }
        }
    }

    void OnDestroy()
    {
        if (midiPlayer != null)
            midiPlayer.OnEventNotesMidi.RemoveListener(OnNoteEvents);
    }

    [ContextMenu("Export JSON")]
    void ExportJson()
    {
        // 시간 순 정렬
        result.hits = result.hits.OrderBy(h => h.time).ToList();

        string path = Application.dataPath + "/midi_output.json";
        string json = JsonUtility.ToJson(result, true);
        File.WriteAllText(path, json);

        Debug.Log($" 리듬게임용 JSON 추출 완료: {path}");
        Debug.Log($" 총 노트 수: {result.hits.Count}");
    }
}
