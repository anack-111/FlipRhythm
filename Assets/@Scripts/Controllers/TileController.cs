using UnityEngine;
using DG.Tweening;  // DOTween 네임스페이스 추가

public class TileController : BaseController
{
    public float bpm = 120f;         // 음악 BPM
    public float snapDistance = 1f;  // 한 박자마다 이동할 거리
    public bool moveOnBeat = true;
    public float scaleAmount = 0.8f; // 크기 변경 비율 (작아지는 비율)
    public float scaleDuration = 0.1f; // 크기 변화 시간 (한 박자에 맞추기)

    //private float beatInterval;      // 한 박자 간격(초)
    private float timer;
    private Vector3 originalScale;  // 원래 크기 저장

    void Start()
    {
        originalScale = transform.localScale; // 타일의 원래 크기 저장

        timer = 0f;
    }

    void Update()
    {
        if (!moveOnBeat) 
            return;

        timer += Time.deltaTime;
        if (timer >= Managers.Game.BeatInterval)
        {
            timer -= Managers.Game.BeatInterval;
            ScaleTile();

        }

    }

    virtual protected void ScaleTile()
    {
        ////타일을 작게 만들고 원래 크기로 돌아오는 연출
        //transform.DOScale(originalScale * scaleAmount, scaleDuration)  // 작게 만들기
        //         .SetEase(Ease.OutBack)  // 자연스럽게 축소
        //         .OnComplete(() =>
        //         {
        //             // 원래 크기로 돌아오기
        //             transform.DOScale(originalScale, scaleDuration)
        //                      .SetEase(Ease.InOutBack);  // 자연스럽게 확대
        //         });

        // 타일을 한 박자만큼 왼쪽으로 이동
        Vector3 target = transform.position + Vector3.left * snapDistance;
        transform.DOMove(target, Managers.Game.BeatInterval).SetEase(Ease.InOutSine);
    }
}
