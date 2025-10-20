using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 1f;  // 점프 힘
    private Rigidbody2D rb;        // Rigidbody2D 컴포넌트
    private float jumpInterval = 1f;  // 점프 간격 (1초)
    private float timer = 0f;      // 타이머

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();  // Rigidbody2D 컴포넌트 초기화
    }

    void Update()
    {
        timer += Time.deltaTime;  // 타이머 갱신

        if (timer >= jumpInterval)  // 1초마다 점프
        {
            Jump();
            timer = 0f;  // 타이머 초기화
        }
    }

    void Jump()
    {
        rb.velocity = Vector2.up * jumpForce;  // 위로 점프
    }
}
