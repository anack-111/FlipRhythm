using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SlidePath))]
public class SlideArrow : MonoBehaviour
{
    [Header("참조")]
    public SlidePath pathDrawer;   // SlidePath 연결 (NoteManager에서 Init으로 주입)
    public InputRouter input;      // 드래그 입력 체크용

    [Header("설정")]
    public float moveTime = 2.5f;  // 전체 이동 시간
    public float checkInterval = 0.1f; // 터치 체크 주기

    private Tween moveTween;
    private bool finished;

    void Awake()
    {

       
    }

    public void Init(SlidePath path, InputRouter input, float moveTime)
    {
        this.pathDrawer = path;
        this.input = input;
        this.moveTime = moveTime;
    }

    void Start()
    {
        if (!pathDrawer)
        {
            Debug.LogError("SlideArrow: pathDrawer가 연결되지 않았습니다!");
            return;
        }

        Vector3[] pathPoints = pathDrawer.GetWorldPath();
        if (pathPoints.Length == 0)
        {
            Debug.LogError("SlideArrow: 경로 데이터가 없습니다!");
            return;
        }

        transform.position = pathPoints[0];

        // DOTween으로 경로 이동
        moveTween = transform.DOPath(pathPoints, moveTime, PathType.Linear)
            .SetEase(Ease.Linear)
            .OnWaypointChange(OnReachNode)
            .OnComplete(() =>
            {
                if (!finished)
                    Success();
            });

        // 터치 유지 여부 확인
        InvokeRepeating(nameof(CheckTouch), 0f, checkInterval);
    }

    void CheckTouch()
    {
        if (finished || input == null) return;

        //int currentZone = GetClosestZone();
        //if (!input.IsTouchingZone(currentZone))
        //{
        //    Miss();
        //}
    }

    int GetClosestZone()
    {
        Vector3 pos = transform.position;
        float minDist = float.MaxValue;
        int zone = -1;

        for (int i = 0; i < pathDrawer.zoneAnchors.Length; i++)
        {
            if (pathDrawer.zoneAnchors[i] == null) continue;

            float d = Vector3.Distance(pos, pathDrawer.zoneAnchors[i].position);
            if (d < minDist)
            {
                minDist = d;
                zone = i + 1;
            }
        }
        return zone;
    }

    void OnReachNode(int stepIndex)
    {
        Debug.Log($"도달: {stepIndex + 1}번째 노드 (Zone_{pathDrawer.route[stepIndex]})");
    }

    void Success()
    {
        finished = true;
        moveTween.Kill();
        Debug.Log("<color=cyan>[SLIDE SUCCESS]</color>");
    }

    void Miss()
    {
        finished = true;
        moveTween.Kill();
        Debug.Log("<color=red>[SLIDE MISS]</color>");
    }
}
