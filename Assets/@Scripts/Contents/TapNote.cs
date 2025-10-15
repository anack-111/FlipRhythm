using UnityEngine;
using DG.Tweening;

public class TapNote : MonoBehaviour
{
    [Header("데이터")]
    public int zoneIndex;    // 예: 3
    public float time;       // 예: 1.50초
    public float perfectWin = 0.06f;
    public float goodWin = 0.12f;

    [Header("참조")]
    public AudioSource music;
    public InputRouter input;
    public SpriteRenderer visual; // 원 스프라이트

    bool active, finished;

    void Start()
    {
        input.OnZoneVisited += OnZoneVisited;
       // gameObject.SetActive(false);
        //  대신 처음엔 보이지 않게만
        transform.localScale = Vector3.zero;
        if (visual != null) visual.color = new Color(1f, 1f, 1f, 0f);
    }
    void OnDestroy()
    {
        input.OnZoneVisited -= OnZoneVisited;
    }

    void Update()
    {
        // 등장 시점
        if (!active && music.time >= time - 0.8f)
        {
            active = true;

            // 보이기 연출만
            transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack);
            if (visual != null) visual.DOFade(1f, 0.2f);
        }

        // 타임아웃 미스
        if (!finished && active && music.time > time + goodWin + 0.2f)
            Miss();
    }

    void OnZoneVisited(int z, float t)
    {
        if (!active || finished) return;
        if (z != zoneIndex) return;

        float diff = Mathf.Abs(t - time);
        if (diff <= perfectWin) Perfect();
        else if (diff <= goodWin) Good();
        else Miss();
    }

    void Appear()
    {
        transform.localScale = Vector3.zero;
        visual.color = new Color(1f, 1f, 1f, 0.2f);
        transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack);
        visual.DOFade(1f, 0.2f);
    }

    void Perfect() { finished = true; HitColor(Color.cyan); }
    void Good() { finished = true; HitColor(Color.yellow); }
    void Miss() { finished = true; HitColor(Color.gray); }

    void HitColor(Color c)
    {
        visual.color = c;
        transform.DOScale(1.35f, 0.12f).SetLoops(2, LoopType.Yoyo)
            .OnComplete(() => Destroy(gameObject));
    }
}
