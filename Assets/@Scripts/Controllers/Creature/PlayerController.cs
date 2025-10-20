using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 1f;  // ���� ��
    public LayerMask groundLayer; // �ٴ� ���̾�
    private Rigidbody2D rb;       // Rigidbody2D ������Ʈ
    private float jumpInterval = 1f;  // ���� ���� (1��)
    private float timer = 0f;     // Ÿ�̸�
    public bool isGrounded;      // �ٴڿ� ��Ҵ��� Ȯ��

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();  // Rigidbody2D ������Ʈ �ʱ�ȭ
    }

    void Update()
    {
        timer += Time.deltaTime;  // Ÿ�̸� ����

        // �ٴڿ� ��Ұ� 1�ʸ��� ����
        if (isGrounded && timer >= jumpInterval)
        {
            Jump();
            timer = 0f;  // Ÿ�̸� �ʱ�ȭ
        }
    }

    // �ٴڰ� �浹���� �� isGrounded�� true�� ����
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = true;  // �ٴڿ� ����� ��
        }
    }

    // �ٴڿ��� ����� isGrounded�� false�� ����
    void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = false;  // �ٴڿ��� �������� ��
        }
    }

    void Jump()
    {
        rb.velocity = Vector2.up * jumpForce;  // ���� ����
    }
}
