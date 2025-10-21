using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float bpm = 60;  // BPM 값을 관리
    float _beatInterval;  // 한 박자 간격 (초)

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
        _beatInterval = 60f / bpm;  // BPM에 맞는 간격 계산
    }


    public void Init(Transform transform)
    {
        _firePos = transform;

    }


    public void SetBPM(float newBPM)
    {
        bpm = newBPM;  // BPM 변경
        _beatInterval = 60f / bpm;  // BPM 변경에 맞춰 간격을 다시 계산
    }

    public void FireBlock()
    {
        Managers.Object.Spawn<BlockController>(_firePos.transform.position, "Block");
    }
}
