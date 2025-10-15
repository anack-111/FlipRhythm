using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LPController : MonoBehaviour
{
    [Header("ȸ�� ����")]
    public float rotationSpeed = 5f; 

    [Header("������ ���� ����")]
    public float scaleAmount = 0.1f; 
    public float scaleDuration = 0.5f;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;

        // ȸ�� Ʈ��
        transform.DORotate
        (
            new Vector3(0, 0, -360),
            rotationSpeed,
            RotateMode.FastBeyond360
        )
        .SetLoops(-1, LoopType.Restart)
        .SetEase(Ease.Linear);

        // ����Ŀ ����
        transform.DOScale(originalScale * (1 + scaleAmount), scaleDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}
