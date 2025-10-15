using UnityEngine;
using System.Linq;
using DG.Tweening;

public class SlideNote : MonoBehaviour
{
    [Header("데이터")]
    public int[] route;         // 예: {1,5,4}
    public float startTime;     // 첫 스텝 시간
    public float stepGap = 0.35f;
    public float perfectWin = 0.06f;
    public float goodWin = 0.12f;

    [Header("참조")]
    public AudioSource music;
    public InputRouter input;
    public SpriteRenderer[] nodes; // 각 스텝 시각(원 3개)

    public Color normalColor = Color.white;
    public Color passedColor = new Color(0.6f, 1f, 0.6f);

    int curStep = 0;
    bool active, finished;
    float[] targets;

    void Start()
    {
        targets = new float[route.Length];
        for (int i = 0; i < route.Length; i++) targets[i] = startTime + stepGap * i;
        foreach (var n in nodes) if (n) { n.color = normalColor; n.transform.localScale = Vector3.zero; }

        input.OnZoneVisited += OnZoneVisited;
        input.OnTouchEnd += OnTouchEnd;
        //gameObject.SetActive(false);
    }
    void OnDestroy()
    {
        input.OnZoneVisited -= OnZoneVisited;
        input.OnTouchEnd -= OnTouchEnd;
    }

    void Update()
    {
        if (!active && music.time >= startTime - 0.9f)
        {
            active = true;
            // 등장 연출
            foreach (var n in nodes)
                if (n)
                {
                    n.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack);
                    var c = n.color; n.color = new Color(c.r, c.g, c.b, 1f);
                }
        }

        if (!finished && active && music.time > targets[targets.Length - 1] + goodWin + 0.25f)
            Miss();
    }

    void OnZoneVisited(int zone, float t)
    {
        if (!active || finished) return;
        if (curStep >= route.Length) return;

        int expect = route[curStep];
        if (zone != expect) return;

        float diff = Mathf.Abs(t - targets[curStep]);

        // 스텝 통과
        if (nodes[curStep]) nodes[curStep].color = passedColor;
        curStep++;

        if (curStep >= route.Length)
        {
            if (diff <= perfectWin) Perfect();
            else if (diff <= goodWin) Good();
            else Miss();
        }
    }

    void OnTouchEnd()
    {
        if (!finished && active && curStep < route.Length && music.time > targets[curStep] + goodWin)
            Miss();
    }

    void Perfect() { finished = true; EndFX(Color.cyan); }
    void Good() { finished = true; EndFX(Color.yellow); }
    void Miss() { finished = true; EndFX(Color.gray); }

    void EndFX(Color c)
    {
        foreach (var n in nodes) if (n) n.color = c;
        transform.DOScale(0f, 0.15f).SetEase(Ease.InBack).OnComplete(() => Destroy(gameObject));
    }
}
