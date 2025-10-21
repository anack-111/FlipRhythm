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

    // 장애물을 날리기 위한 메서드
    public void DestoryObstacle()
    {
        Destroy(gameObject);
    }



    protected override void ScaleTile()
    {
        if (isLaunched)
            return;

        // 타일을 한 박자만큼 왼쪽으로 이동
        Vector3 target = transform.position + Vector3.left * snapDistance;
        transform.DOMove(target, Managers.Game.BeatInterval).SetEase(Ease.InOutSine);
    }
}
