using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 1f;  // ���� ��
    private Rigidbody2D rb;        // Rigidbody2D ������Ʈ
    private float jumpInterval = 1f;  // ���� ���� (1��)
    private float timer = 0f;      // Ÿ�̸�

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();  // Rigidbody2D ������Ʈ �ʱ�ȭ
    }

    void Update()
    {
        timer += Time.deltaTime;  // Ÿ�̸� ����

        if (timer >= jumpInterval)  // 1�ʸ��� ����
        {
            Jump();
            timer = 0f;  // Ÿ�̸� �ʱ�ȭ
        }
    }

    void Jump()
    {
        rb.velocity = Vector2.up * jumpForce;  // ���� ����
    }
}
