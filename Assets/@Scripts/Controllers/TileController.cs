using UnityEngine;

public class TileController : MonoBehaviour
{
    public float bpm = 120f;         // 음악 BPM
    public float snapDistance = 1f;  // 한 박자마다 이동할 거리
    public bool moveOnBeat = true;

    private float beatInterval;      // 한 박자 간격(초)
    private float timer;

    void Start()
    {
        beatInterval = 60f / bpm;    // 120BPM → 0.5초
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
