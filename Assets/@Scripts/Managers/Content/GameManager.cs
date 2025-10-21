using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float bpm = 60;  // BPM ���� ����
    float _beatInterval;  // �� ���� ���� (��)

    public float BeatInterval
    {
        get { return _beatInterval; }
    }
    Transform _firePos;
    public Transform FirePos
    {
        get { return _firePos; }
    }


    public void Awake()
    {
        _beatInterval = 60f / bpm;  // BPM�� �´� ���� ���
    }


    public void Init(Transform transform)
    {
        _firePos = transform;

    }


    public void SetBPM(float newBPM)
    {
        bpm = newBPM;  // BPM ����
        _beatInterval = 60f / bpm;  // BPM ���濡 ���� ������ �ٽ� ���
    }

    public void FireBlock()
    {
        Managers.Object.Spawn<BlockController>(_firePos.transform.position, "Block");
    }
}
