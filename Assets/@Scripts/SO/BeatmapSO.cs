using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RhythmZigZag/Beatmap", fileName = "Beatmap")]
public class BeatmapSO : ScriptableObject
{
    [Serializable]
    public struct Segment
    {
        public float time;       // �� ���׸�Ʈ�� ���۵Ǵ� ���� ��(time)
        public float length;     // ���׸�Ʈ ����(���� ����, Y�� ����)
        public float angle;      // +������ ��������, -������ �������� ����(��)
        public float speed;      // �� ���� ��ũ�� �ӵ�(����)
        public bool reverse;     // �����/�ǰ��� ���� ���� ǥ��(����)
    }

    public AudioClip music;
    public float bpm;                // ����/������ BPM
    public List<Segment> segments = new List<Segment>();
}
