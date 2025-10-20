using UnityEngine;

public class BackgroundMovement : MonoBehaviour
{
    public float speed = 5f;  //  �̵� �ӵ�
    public float resetPositionX = -15f;  //  ��� �� ���µ� X ��ǥ
    public float startPositionX = 15f;  //  ó�� ������ X ��ǥ

    private Vector3 startPosition;
    private bool isRewinding = false;  // �ǰ��� ����

    void Start()
    {
        startPosition = new Vector3(startPositionX, transform.position.y, transform.position.z);
        transform.position = startPosition;  // ���� ��ġ�� ��ġ
    }

    void Update()
    {
        if (!isRewinding)
        {
            MoveObject();  // �̵� ���� ��
        }
        else
        {
            ReverseObject();  // �ǰ��� ���� ��
        }
    }

    // ������ �����ʿ��� �������� �̵���Ű�� �Լ�
    void MoveObject()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        // ������ ȭ���� ����� �ٽ� ����
        if (transform.position.x <= resetPositionX)
        {
            transform.position = startPosition;
        }
    }

    // ������ �ǰ���Ǹ鼭 �ݴ�� �̵��ϴ� �Լ�
    void ReverseObject()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        // �ǰ��� �� ������ �ٽ� ���� ��ġ�� ���ư��� �ǰ��� ����
        if (transform.position.x >= startPositionX)
        {
            isRewinding = false;
        }
    }

    // �ǰ��� ����
    public void StartRewind()
    {
        isRewinding = true;
    }

    // �ǰ��� ����
    public void StopRewind()
    {
        isRewinding = false;
    }
}
