using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class NoteUI : MonoBehaviour
{
    //private NoteManager manager;
    //private float targetTime;
    //private bool judged = false;
    //private Image img;

    //public void Init(NoteManager mgr, float time)
    //{
    //    manager = mgr;
    //    targetTime = time;
    //    img = GetComponent<Image>();
    //    AppearAnimation();
    //}

    //void Update()
    //{
    //    // 일정 시간 지나면 자동 Miss
    //    if (!judged && manager.GetMusicTime() - targetTime > 0.2f)
    //    {
    //        judged = true;
    //        Miss();
    //    }
    //}

    //void AppearAnimation()
    //{
    //    transform.localScale = Vector3.zero;
    //    transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
    //    img.DOFade(1f, 0.3f);
    //}

    //public void OnJudge()
    //{
    //    if (judged) return;

    //    float diff = Mathf.Abs(manager.GetMusicTime() - targetTime);
    //    judged = true;

    //    if (diff < 0.05f) Perfect();
    //    else if (diff < 0.1f) Good();
    //    else Miss();
    //}

    //void Perfect()
    //{
    //    img.color = Color.cyan;
    //    Debug.Log("Perfect");
    //    FX();
    //}

    //void Good()
    //{
    //    img.color = Color.yellow;
    //    Debug.Log("Good");
    //    FX();
    //}

    //void Miss()
    //{
    //    img.color = Color.gray;
    //    Debug.Log("Miss");
    //    FX();
    //}

    //void FX()
    //{
    //    transform.DOScale(1.4f, 0.15f).SetLoops(2, LoopType.Yoyo)
    //        .OnComplete(() => Destroy(gameObject));
    //}
}
