using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController
{
    public float moveSpeed = 10f;
    private int dir = 1; // 1 = ������, -1 = ����
    private bool isDead = false;

    void Update()
    {
        if (isDead)
            return;

        // ��ġ�� Ŭ������ ���� ��ȯ
        if (Input.GetMouseButtonDown(0))
            dir *= -1;


    }

}
