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

        // �� �̻� �̺�Ʈ�� �������� ���� (InputRouter�� ���� ������ ��Ʈ�� ã�� ȣ��)
        _zoneText.text = zoneIndex.ToString();
    }

    void OnDestroy()
    {
        // �̺�Ʈ ������ ���� �����Ƿ� ���� ���ʿ�
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

    // InputRouter�� ������ ��Ʈ�� ���� ������ �õ��ϵ��� public���� ����
    // ����� ��ȯ�ؼ� ȣ����(InputRouter)�� �ļ� ������ �� �� �ְ� ��
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
