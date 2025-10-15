using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RhythmZigZag/Beatmap", fileName = "Beatmap")]
public class BeatmapSO : ScriptableObject
{
    [Serializable]
    public struct Segment
    {
        public float time;       // 이 세그먼트가 시작되는 음악 초(time)
        public float length;     // 세그먼트 길이(월드 단위, Y로 전개)
        public float angle;      // +각도면 우측으로, -각도면 좌측으로 꺾임(도)
        public float speed;      // 이 구간 스크롤 속도(선택)
        public bool reverse;     // 역재생/되감기 연출 구간 표시(선택)
    }

    public AudioClip music;
    public float bpm;                // 검출/설정된 BPM
    public List<Segment> segments = new List<Segment>();
}
