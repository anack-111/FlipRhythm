using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController
{
    public float moveSpeed = 10f;
    private int dir = 1; // 1 = 오른쪽, -1 = 왼쪽
    private bool isDead = false;

    void Update()
    {
        if (isDead)
            return;

        // 터치나 클릭으로 방향 전환
        if (Input.GetMouseButtonDown(0))
            dir *= -1;


    }

}
