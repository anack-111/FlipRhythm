using UnityEngine;

public class BackgroundMovement : MonoBehaviour
{
    public float speed = 5f;  //  이동 속도
    public float resetPositionX = -15f;  //  벗어난 후 리셋될 X 좌표
    public float startPositionX = 15f;  //  처음 시작할 X 좌표

    private Vector3 startPosition;
    private bool isRewinding = false;  // 되감기 여부

    void Start()
    {
        startPosition = new Vector3(startPositionX, transform.position.y, transform.position.z);
        transform.position = startPosition;  // 시작 위치에 배치
    }

    void Update()
    {
        if (!isRewinding)
        {
            MoveObject();  // 이동 중일 때
        }
        else
        {
            ReverseObject();  // 되감기 중일 때
        }
    }

    // 구름을 오른쪽에서 왼쪽으로 이동시키는 함수
    void MoveObject()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        // 구름이 화면을 벗어나면 다시 리셋
        if (transform.position.x <= resetPositionX)
        {
            transform.position = startPosition;
        }
    }

    // 구름이 되감기되면서 반대로 이동하는 함수
    void ReverseObject()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        // 되감기 후 구름이 다시 시작 위치로 돌아가면 되감기 종료
        if (transform.position.x >= startPositionX)
        {
            isRewinding = false;
        }
    }

    // 되감기 시작
    public void StartRewind()
    {
        isRewinding = true;
    }

    // 되감기 종료
    public void StopRewind()
    {
        isRewinding = false;
    }
}
