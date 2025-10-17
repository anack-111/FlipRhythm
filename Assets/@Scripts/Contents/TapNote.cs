using UnityEngine;
using DG.Tweening;
using TMPro;

public class TapNote : MonoBehaviour
{
    public enum HitResult { None, Perfect, Good, Miss }

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

        // 더 이상 이벤트로 구독하지 않음 (InputRouter가 직접 적절한 노트를 찾아 호출)
        _zoneText.text = zoneIndex.ToString();
    }

    void OnDestroy()
    {
        // 이벤트 구독을 하지 않으므로 해제 불필요
    }

    void Update()
    {
        float t = music.time;
        float moveStart = hitTime - approachTime;
        float moveProgress = Mathf.InverseLerp(moveStart, hitTime, t);
        float x = Mathf.Lerp(8f, judgeX, moveProgress);
        transform.position = new Vector3(x, transform.position.y, 0);

        if (!judged && t > hitTime + goodWin)
        {
            Miss();
        }
    }

    // InputRouter가 선택한 노트에 대해 판정을 시도하도록 public으로 노출
    // 결과를 반환해서 호출자(InputRouter)가 후속 연출을 할 수 있게 함
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
