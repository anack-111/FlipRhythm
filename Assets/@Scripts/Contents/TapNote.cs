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

    public void Init(int zone, float time, AudioSource src, InputRouter inp, float jx, float approach)
    {
        zoneIndex = zone;
        hitTime = time;
        music = src;
        input = inp;
        judgeX = jx;
        approachTime = approach;
        sr = GetComponent<SpriteRenderer>();

        input.OnZoneVisited += OnZoneVisited;

        _zoneText.text = zoneIndex.ToString();
    }

    void OnDestroy()
    {
        if (input != null)
            input.OnZoneVisited -= OnZoneVisited;
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

    void OnZoneVisited(int z, float t)
    {
        z += 1;
        if (judged)
            return;

        if (z != zoneIndex)
            return;

        float diff = Mathf.Abs(t - hitTime);

        if (diff <= perfectWin)
            Perfect();
        else if (diff <= goodWin)
            Good();
        else
            Miss();
    }

    void Perfect()
    {
        Debug.Log("Perfect");
        judged = true;
        sr.color = Color.cyan;
        transform.DOScale(1.3f, 0.1f).SetLoops(2, LoopType.Yoyo)
            .OnComplete(() => Destroy(gameObject));
    }

    void Good()
    {
        Debug.Log("Good");
        judged = true;
        sr.color = Color.yellow;
        transform.DOScale(1.3f, 0.1f).SetLoops(2, LoopType.Yoyo)
            .OnComplete(() => Destroy(gameObject));
    }

    void Miss()
    {
        Debug.Log("Miss");
        judged = true;
        sr.color = Color.gray;
        sr.DOFade(0, 0.5f).OnComplete(() => Destroy(gameObject));
    }
}
