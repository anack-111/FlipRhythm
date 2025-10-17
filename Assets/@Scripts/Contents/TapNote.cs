using UnityEngine;
using DG.Tweening;
using TMPro;

public class TapNote : MonoBehaviour
{
    public TextMeshProUGUI _zoneText;
    int zoneIndex;
    float hitTime;
    float judgeX;
    float approachTime;
    AudioSource music;
    InputRouter input;
    SpriteRenderer sr;

    bool judged = false;

    float perfectWin = 0.2f;
    float goodWin = 0.5f;

    // DSP 기반 이동을 위해 추가 필드
    double spawnDspTime;
    float startX;

    // 외부에서 참조할 수 있도록 프로퍼티 추가
    public int ZoneIndex => zoneIndex;
    public float HitTime => hitTime;
    public bool IsJudged => judged;

    public void Init(int zone, float time, AudioSource src, InputRouter inp, float jx, float approach)
    {
        zoneIndex = zone;
        hitTime = time;
        music = src;
        input = inp;
        judgeX = jx;
        approachTime = approach;
        sr = GetComponent<SpriteRenderer>();
        startX = transform.position.x;

        _zoneText.text = zoneIndex.ToString();

        // DOTween으로 Linear 등속 이동 (부드럽고 정확)
        transform.DOMoveX(judgeX, approachTime)
            .SetEase(Ease.Linear)
            .SetUpdate(UpdateType.Normal, true);
    }

    void OnDestroy()
    {
    }



    void Update()
    {


        // 판정 타이밍 체크는 기존대로 music.time 사용 (판정 정확성 유지)
        if (!judged && music != null && music.time > hitTime + goodWin)
        {
            Miss();
        }
    }

    // InputRouter가 선택한 노트에 대해 판정을 시도하도록 public으로 노출
    public enum HitResult { None, Perfect, Good, Miss }

    public HitResult TryHandleHit(float t)
    {
        if (judged) return HitResult.None;

        float diff = Mathf.Abs(t - hitTime);

        if (diff <= perfectWin)
        {
            Perfect();
            return HitResult.Perfect;
        }
        else if (diff <= goodWin)
        {
            Good();
            return HitResult.Good;
        }
        else
        {
            Miss();
            return HitResult.Miss;
        }
    }

    void Perfect()
    {
        Debug.Log("Perfect");
        judged = true;
        sr.color = Color.cyan;
        transform.DOScale(0.7f, 0.1f).SetLoops(2, LoopType.Yoyo)
            .OnComplete(() => Destroy(gameObject));

        Managers.Object.ShowJudgeFont(transform.position, "Perfect");
    }

    void Good()
    {
        Debug.Log("Good");
        judged = true;
        sr.color = Color.yellow;
        transform.DOScale(0.7f, 0.1f).SetLoops(2, LoopType.Yoyo)
            .OnComplete(() => Destroy(gameObject));

        Managers.Object.ShowJudgeFont(transform.position, "Good");
    }

    void Miss()
    {
        Debug.Log("Miss");
        judged = true;
        sr.color = Color.gray;

        // 스프라이트 페이드아웃
        sr.DOFade(0, 0.5f);

        // 텍스트도 같이 페이드아웃
        if (_zoneText != null)
            _zoneText.DOFade(0, 0.5f);

        Managers.Object.ShowJudgeFont(transform.position, "Miss");

        DOVirtual.DelayedCall(0.5f, () => Destroy(gameObject));
    }
}
