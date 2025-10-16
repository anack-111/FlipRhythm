using UnityEngine;
using DG.Tweening;
using System.Collections;
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
    SpriteRenderer visual; // 원 스프라이트
    bool active, finished;
    private void Awake()
    {
        visual = gameObject.GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        input.OnZoneVisited += OnZoneVisited;
        // 처음 크기를 0.2로 설정
        transform.localScale = new Vector3(0.2f, 0.2f, 1f);
        // 1초 동안 크기가 1로 증가
        transform.DOScale(1f, 1f);//.SetEase(Ease.OutBack).SetDelay(0f); // outback 하면 커졋다가 작아짐 효과 추가
        // 노트가 활성화되는 시점
        active = false;
        finished = false;
    }
    void OnDestroy()
    {
        input.OnZoneVisited -= OnZoneVisited;
    }
    void Update()
    {
        // 타이밍 미스 처리 (타이밍이 지나면 자동으로 미스를 처리)
        if (!finished && !active && music.time >= time - 0.8f)
        {
            // 노트가 등장하고 0.8초 이후, 활성화된 상태로 타이밍 처리 시작
            active = true;
            //Appear();  // 노트 애니메이션
        }
        // 타임아웃 미스 처리
        if (!finished && active && music.time > time + goodWin)
        {
            Miss(); // 타이밍을 놓친 경우 미스 처리
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
        HitColor(Color.cyan);  // 퍼팩트 타이밍
    }
    void Miss()
    {
        finished = true;
        // 미스 판정 시 0.5초 동안 투명하게 만들어주고, 끝나면 삭제
        StartCoroutine(FadeOutAndDestroy());
    }
    private IEnumerator FadeOutAndDestroy()
    {
        
        float fadeDuration = 0.2f;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            // 투명도 조절 (시간에 따라 점점 투명해짐)
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            visual.color = new Color(1f, 1f, 1f, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // 완전히 투명해졌을 때, 오브젝트를 삭제
        Destroy(gameObject);
    }
    void HitColor(Color c)
    {
        visual.color = c;
        transform.DOScale(1.35f, 0.12f).SetLoops(2, LoopType.Yoyo)
            .OnComplete(() => Destroy(gameObject));
    }
}