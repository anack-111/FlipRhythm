using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class SlidePath : MonoBehaviour
{
    [Header("경로 설정")]
    public Transform[] zoneAnchors;   // NoteManager에서 주입됨
    public int[] route = { 1, 2, 5, 8, 9 };

    [Header("연출 설정")]
    public float growDuration = 1.5f; // 전체 라인 자라나는 시간

    private LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 0;
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = new Color(1f, 1f, 1f, 0.2f);
        line.endColor = new Color(1f, 1f, 1f, 0.2f);
    }

    public void DrawPath()
    {
        if (zoneAnchors == null || zoneAnchors.Length == 0)
        {
            Debug.LogError("[SlidePath] zoneAnchors가 비어 있음.");
            return;
        }

        line.positionCount = route.Length;

        for (int i = 0; i < route.Length; i++)
        {
            int index = route[i] - 1;
            if (index >= 0 && index < zoneAnchors.Length && zoneAnchors[index] != null)
                line.SetPosition(i, zoneAnchors[index].position);
        }

        // 처음엔 안 보이게 초기화
        line.material.SetTextureScale("_MainTex", new Vector2(1, 1));
        StartCoroutine(GrowLine(growDuration));
    }

    // 라인이 자라나는 코루틴
    IEnumerator GrowLine(float duration)
    {
        Vector3[] points = GetWorldPath();
        if (points.Length < 2) yield break;

        // 전체 거리 계산
        float totalDist = 0f;
        for (int i = 1; i < points.Length; i++)
            totalDist += Vector3.Distance(points[i - 1], points[i]);

        // 라인 초기화
        line.positionCount = 2;
        line.SetPosition(0, points[0]);
        line.SetPosition(1, points[0]);

        float elapsed = 0f;
        float coveredDist = 0f;
        int currentIndex = 0;

        while (elapsed < duration && currentIndex < points.Length - 1)
        {
            elapsed += Time.deltaTime;
            float targetDist = Mathf.Lerp(0, totalDist, elapsed / duration);

            // 현재 세그먼트 찾기 (완료된 세그먼트는 currentIndex 증가)
            while (currentIndex < points.Length - 1)
            {
                float segmentDist = Vector3.Distance(points[currentIndex], points[currentIndex + 1]);
                if (coveredDist + segmentDist > targetDist)
                    break;

                coveredDist += segmentDist;
                currentIndex++;
                line.positionCount = currentIndex + 2;

                // 증가한 currentIndex 위치는 확정된 점으로 설정
                line.SetPosition(currentIndex, points[currentIndex]);

                // 다음 슬롯(보간이 쓸 위치)은 현재 점으로 초기화해 안전하게 만듦
                if (currentIndex + 1 < line.positionCount)
                    line.SetPosition(currentIndex + 1, points[currentIndex]);
            }

            // 만약 마지막 지점에 도달했다면 보정 루프로 빠져나감
            if (currentIndex >= points.Length - 1)
                break;

            // 보간 위치 계산 (안전하게 currentIndex+1 사용)
            float remain = targetDist - coveredDist;
            Vector3 dir = (points[currentIndex + 1] - points[currentIndex]).normalized;
            Vector3 newPos = points[currentIndex] + dir * remain;
            line.SetPosition(currentIndex + 1, newPos);

            yield return null;
        }

        // 보정: 마지막 점까지 도달
        for (int i = 0; i < points.Length; i++)
            line.SetPosition(i, points[i]);
    }

    public Vector3[] GetWorldPath()
    {
        Vector3[] path = new Vector3[route.Length];
        for (int i = 0; i < route.Length; i++)
        {
            int index = route[i] - 1;
            if (index >= 0 && index < zoneAnchors.Length && zoneAnchors[index] != null)
                path[i] = zoneAnchors[index].position;
        }
        return path;
    }
}
