using UnityEngine;
using DG.Tweening;
using System.Collections;
public class TapNote : MonoBehaviour
{
    [Header("������")]
    public int zoneIndex;    // ��: 3
    public float time;       // ��: 1.50��
    public float perfectWin = 0.06f;
    public float goodWin = 0.12f;
    [Header("����")]
    public AudioSource music;
    public InputRouter input;
    SpriteRenderer visual; // �� ��������Ʈ
    bool active, finished;
    private void Awake()
    {
        visual = gameObject.GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        input.OnZoneVisited += OnZoneVisited;
        // ó�� ũ�⸦ 0.2�� ����
        transform.localScale = new Vector3(0.2f, 0.2f, 1f);
        // 1�� ���� ũ�Ⱑ 1�� ����
        transform.DOScale(1f, 1f);//.SetEase(Ease.OutBack).SetDelay(0f); // outback �ϸ� Ŀ���ٰ� �۾��� ȿ�� �߰�
        // ��Ʈ�� Ȱ��ȭ�Ǵ� ����
        active = false;
        finished = false;
    }
    void OnDestroy()
    {
        input.OnZoneVisited -= OnZoneVisited;
    }
    void Update()
    {
        // Ÿ�̹� �̽� ó�� (Ÿ�̹��� ������ �ڵ����� �̽��� ó��)
        if (!finished && !active && music.time >= time - 0.8f)
        {
            // ��Ʈ�� �����ϰ� 0.8�� ����, Ȱ��ȭ�� ���·� Ÿ�̹� ó�� ����
            active = true;
            //Appear();  // ��Ʈ �ִϸ��̼�
        }
        // Ÿ�Ӿƿ� �̽� ó��
        if (!finished && active && music.time > time + goodWin)
        {
            Miss(); // Ÿ�̹��� ��ģ ��� �̽� ó��
        }
    }
    void OnZoneVisited(int z, float t)
    {
        if (!active || finished) return;
        if (z != zoneIndex) return;
        float diff = Mathf.Abs(t - time);
        if (diff <= perfectWin)
            Perfect();
        else
            Miss();
    }
    void Appear()
    {
        transform.localScale = Vector3.zero;
        visual.color = new Color(1f, 1f, 1f, 0.5f);
        visual.DOFade(0f, 0.2f);
    }
    void Perfect()
    {
        finished = true;
        HitColor(Color.cyan);  // ����Ʈ Ÿ�̹�
    }
    void Miss()
    {
        finished = true;
        // �̽� ���� �� 0.5�� ���� �����ϰ� ������ְ�, ������ ����
        StartCoroutine(FadeOutAndDestroy());
    }
    private IEnumerator FadeOutAndDestroy()
    {
        
        float fadeDuration = 0.2f;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            // ���� ���� (�ð��� ���� ���� ��������)
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            visual.color = new Color(1f, 1f, 1f, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // ������ ���������� ��, ������Ʈ�� ����
        Destroy(gameObject);
    }
    void HitColor(Color c)
    {
        visual.color = c;
        transform.DOScale(1.35f, 0.12f).SetLoops(2, LoopType.Yoyo)
            .OnComplete(() => Destroy(gameObject));
    }
}