using UnityEngine;

public class TileController : MonoBehaviour
{
    public float bpm = 120f;         // ���� BPM
    public float snapDistance = 1f;  // �� ���ڸ��� �̵��� �Ÿ�
    public bool moveOnBeat = true;

    private float beatInterval;      // �� ���� ����(��)
    private float timer;

    void Start()
    {
        beatInterval = 60f / bpm;    // 120BPM �� 0.5��
        timer = 0f;
    }

    void Update()
    {
        if (!moveOnBeat) return;

        timer += Time.deltaTime;
        if (timer >= beatInterval)
        {
            timer -= beatInterval;
            MoveLeftSnap();
        }
    }

    void MoveLeftSnap()
    {
        Vector3 target = transform.position + Vector3.left * snapDistance;
        transform.position = target;
    }
}
