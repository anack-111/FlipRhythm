using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 1f;  // 점프 힘
    public LayerMask groundLayer; // 바닥 레이어
    private Rigidbody2D rb;       // Rigidbody2D 컴포넌트
    private float jumpInterval = 1f;  // 점프 간격 (1초)
    private float timer = 0f;     // 타이머
    public bool isGrounded;      // 바닥에 닿았는지 확인

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();  // Rigidbody2D 컴포넌트 초기화
    }

    void Update()
    {
        timer += Time.deltaTime;  // 타이머 갱신

        // 바닥에 닿았고 1초마다 점프
        if (isGrounded && timer >= jumpInterval)
        {
            Jump();
            timer = 0f;  // 타이머 초기화
        }
    }

    // 바닥과 충돌했을 때 isGrounded를 true로 설정
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = true;  // 바닥에 닿았을 때
        }
    }

    // 바닥에서 벗어나면 isGrounded를 false로 설정
    void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = false;  // 바닥에서 떨어졌을 때
        }
    }

    void Jump()
    {
        rb.velocity = Vector2.up * jumpForce;  // 위로 점프
    }
}
