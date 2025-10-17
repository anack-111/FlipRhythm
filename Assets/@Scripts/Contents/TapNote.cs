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

    // DSP ��� �̵��� ���� �߰� �ʵ�
    double spawnDspTime;
    float startX;

    // �ܺο��� ������ �� �ֵ��� ������Ƽ �߰�
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

        // DOTween���� Linear ��� �̵� (�ε巴�� ��Ȯ)
        transform.DOMoveX(judgeX, approachTime)
            .SetEase(Ease.Linear)
            .SetUpdate(UpdateType.Normal, true);
    }

    void OnDestroy()
    {
    }



    void Update()
    {


        // ���� Ÿ�̹� üũ�� ������� music.time ��� (���� ��Ȯ�� ����)
        if (!judged && music != null && music.time > hitTime + goodWin)
        {
            Miss();
        }
    }

    // InputRouter�� ������ ��Ʈ�� ���� ������ �õ��ϵ��� public���� ����
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

        // ��������Ʈ ���̵�ƿ�
        sr.DOFade(0, 0.5f);

        // �ؽ�Ʈ�� ���� ���̵�ƿ�
        if (_zoneText != null)
            _zoneText.DOFade(0, 0.5f);

        Managers.Object.ShowJudgeFont(transform.position, "Miss");

        DOVirtual.DelayedCall(0.5f, () => Destroy(gameObject));
    }
}
