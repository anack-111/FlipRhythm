using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LPController : MonoBehaviour
{
    [Header("회전 설정")]
    public float rotationSpeed = 5f; 

    [Header("스케일 진동 설정")]
    public float scaleAmount = 0.1f; 
    public float scaleDuration = 0.5f;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;

        // 회전 트윈
        transform.DORotate
        (
            new Vector3(0, 0, -360),
            rotationSpeed,
            RotateMode.FastBeyond360
        )
        .SetLoops(-1, LoopType.Restart)
        .SetEase(Ease.Linear);

        // 스피커 진동
        transform.DOScale(originalScale * (1 + scaleAmount), scaleDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}
