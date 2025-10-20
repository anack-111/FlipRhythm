using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 1f;      // ���� ��
    public float bpm = 120f;          // ���� BPM
    public float groundCheckDistance = 0.1f;  // �ٴ� üũ �Ÿ�
    public LayerMask groundLayer;     // �� ���̾�
    public float fallSpeed = 3f;      // �ϰ� �ӵ�

    private Rigidbody2D rb;           // Rigidbody2D ������Ʈ
    private BoxCollider2D boxCollider; // BoxCollider2D ������Ʈ
    private float beatInterval;       // 1���� ���� (��)
    private float timer;              // Ÿ�̸�
    private bool isJumping = false;   // ���� ���� üũ

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();  // BoxCollider2D ������Ʈ ��������
        rb.gravityScale = 0;  // �߷� ��� �� ��
    }

    void Start()
    {
        // BPM �� 1���ڴ� �ð� ��� (��: 120BPM = 0.5��)
        beatInterval = 60f / bpm;
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // BPM�� ���缭 ����
        if (timer >= beatInterval)
        {
            timer -= beatInterval; // Ÿ�̸� �ʱ�ȭ
            TryJump();
        }

        // ���� ���� �ƴϸ�, ���� �� �ϰ� ó��
        if (!isJumping && !IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, -fallSpeed); // �ϰ�
        }
    }

    void TryJump()
    {
        // �ٴڿ� ��Ҵ��� Ȯ���ϴ� �Լ�
        if (IsGrounded())
        {
            isJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); // ���� ����
        }
    }

    // BoxCollider2D�� �ϴ� �κ��� �������� �ٴڿ� ��Ҵ��� Ȯ���ϴ� �Լ�
    bool IsGrounded()
    {
        // BoxCollider2D�� �ϴ� �߾��� �������� �ٴ� üũ
        Vector2 colliderBottom = new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.min.y);
        RaycastHit2D hit = Physics2D.Raycast(colliderBottom, Vector2.down, groundCheckDistance, groundLayer);
        return hit.collider != null;
    }

    // ������ ������� �ʾƵ� Raycast�� �ð������� �� �� �ְ� �׷��ִ� �Լ�
    void OnDrawGizmos()
    {
        // BoxCollider2D�� �ϴ� �߾ӿ��� �Ʒ��� Ray�� �׷��� (���������� ǥ��)
        if (boxCollider != null)
        {
            Gizmos.color = Color.red;
            Vector2 colliderBottom = new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.min.y);
            Gizmos.DrawLine(colliderBottom, colliderBottom + Vector2.down * groundCheckDistance);
        }
    }
}
