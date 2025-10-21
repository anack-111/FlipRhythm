using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : TileController
{
    private bool isLaunched = false;

    void Start()
    {
        isLaunched = false;
    }

    // ��ֹ��� ������ ���� �޼���
    public void DestoryObstacle()
    {
        Destroy(gameObject);
    }



    protected override void ScaleTile()
    {
        if (isLaunched)
            return;

        // Ÿ���� �� ���ڸ�ŭ �������� �̵�
        Vector3 target = transform.position + Vector3.left * snapDistance;
        transform.DOMove(target, Managers.Game.BeatInterval).SetEase(Ease.InOutSine);
    }
}
