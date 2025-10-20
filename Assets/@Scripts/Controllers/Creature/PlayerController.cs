using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 1f;      // 점프 힘
    public float bpm = 120f;          // 음악 BPM
    public float groundCheckDistance = 0.1f;  // 바닥 체크 거리
    public LayerMask groundLayer;     // 땅 레이어
    public float fallSpeed = 3f;      // 하강 속도

    private Rigidbody2D rb;           // Rigidbody2D 컴포넌트
    private BoxCollider2D boxCollider; // BoxCollider2D 컴포넌트
    private float beatInterval;       // 1박자 간격 (초)
    private float timer;              // 타이머
    private bool isJumping = false;   // 점프 상태 체크

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();  // BoxCollider2D 컴포넌트 가져오기
        rb.gravityScale = 0;  // 중력 사용 안 함
    }

    void Start()
    {
        // BPM → 1박자당 시간 계산 (예: 120BPM = 0.5초)
        beatInterval = 60f / bpm;
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // BPM에 맞춰서 점프
        if (timer >= beatInterval)
        {
            timer -= beatInterval; // 타이머 초기화
            TryJump();
        }

        // 점프 중이 아니면, 점프 후 하강 처리
        if (!isJumping && !IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, -fallSpeed); // 하강
        }
    }

    void TryJump()
    {
        // 바닥에 닿았는지 확인하는 함수
        if (IsGrounded())
        {
            isJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); // 점프 수행
        }
    }

    // BoxCollider2D의 하단 부분을 기준으로 바닥에 닿았는지 확인하는 함수
    bool IsGrounded()
    {
        // BoxCollider2D의 하단 중앙을 기준으로 바닥 체크
        Vector2 colliderBottom = new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.min.y);
        RaycastHit2D hit = Physics2D.Raycast(colliderBottom, Vector2.down, groundCheckDistance, groundLayer);
        return hit.collider != null;
    }

    // 게임이 실행되지 않아도 Raycast를 시각적으로 볼 수 있게 그려주는 함수
    void OnDrawGizmos()
    {
        // BoxCollider2D의 하단 중앙에서 아래로 Ray를 그려줌 (빨간색으로 표시)
        if (boxCollider != null)
        {
            Gizmos.color = Color.red;
            Vector2 colliderBottom = new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.min.y);
            Gizmos.DrawLine(colliderBottom, colliderBottom + Vector2.down * groundCheckDistance);
        }
    }
}
