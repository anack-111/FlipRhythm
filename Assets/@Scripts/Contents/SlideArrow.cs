using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SlidePath))]
public class SlideArrow : MonoBehaviour
{
    [Header("����")]
    public SlidePath pathDrawer;   // SlidePath ���� (NoteManager���� Init���� ����)
    public InputRouter input;      // �巡�� �Է� üũ��

    [Header("����")]
    public float moveTime = 2.5f;  // ��ü �̵� �ð�
    public float checkInterval = 0.1f; // ��ġ üũ �ֱ�

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
            Debug.LogError("SlideArrow: pathDrawer�� ������� �ʾҽ��ϴ�!");
            return;
        }

        Vector3[] pathPoints = pathDrawer.GetWorldPath();
        if (pathPoints.Length == 0)
        {
            Debug.LogError("SlideArrow: ��� �����Ͱ� �����ϴ�!");
            return;
        }

        transform.position = pathPoints[0];

        // DOTween���� ��� �̵�
        moveTween = transform.DOPath(pathPoints, moveTime, PathType.Linear)
            .SetEase(Ease.Linear)
            .OnWaypointChange(OnReachNode)
            .OnComplete(() =>
            {
                if (!finished)
                    Success();
            });

        // ��ġ ���� ���� Ȯ��
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
        Debug.Log($"����: {stepIndex + 1}��° ��� (Zone_{pathDrawer.route[stepIndex]})");
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
