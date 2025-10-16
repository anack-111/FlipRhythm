using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class SlidePath : MonoBehaviour
{
    [Header("��� ����")]
    public Transform[] zoneAnchors;   // NoteManager���� ���Ե�
    public int[] route = { 1, 2, 5, 8, 9 };

    [Header("���� ����")]
    public float growDuration = 1.5f; // ��ü ���� �ڶ󳪴� �ð�

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
            Debug.LogError("[SlidePath] zoneAnchors�� ��� ����.");
            return;
        }

        line.positionCount = route.Length;

        for (int i = 0; i < route.Length; i++)
        {
            int index = route[i] - 1;
            if (index >= 0 && index < zoneAnchors.Length && zoneAnchors[index] != null)
                line.SetPosition(i, zoneAnchors[index].position);
        }

        // ó���� �� ���̰� �ʱ�ȭ
        line.material.SetTextureScale("_MainTex", new Vector2(1, 1));
        StartCoroutine(GrowLine(growDuration));
    }

    // ������ �ڶ󳪴� �ڷ�ƾ
    IEnumerator GrowLine(float duration)
    {
        Vector3[] points = GetWorldPath();
        if (points.Length < 2) yield break;

        // ��ü �Ÿ� ���
        float totalDist = 0f;
        for (int i = 1; i < points.Length; i++)
            totalDist += Vector3.Distance(points[i - 1], points[i]);

        // ���� �ʱ�ȭ
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

            // ���� ���׸�Ʈ ã�� (�Ϸ�� ���׸�Ʈ�� currentIndex ����)
            while (currentIndex < points.Length - 1)
            {
                float segmentDist = Vector3.Distance(points[currentIndex], points[currentIndex + 1]);
                if (coveredDist + segmentDist > targetDist)
                    break;

                coveredDist += segmentDist;
                currentIndex++;
                line.positionCount = currentIndex + 2;

                // ������ currentIndex ��ġ�� Ȯ���� ������ ����
                line.SetPosition(currentIndex, points[currentIndex]);

                // ���� ����(������ �� ��ġ)�� ���� ������ �ʱ�ȭ�� �����ϰ� ����
                if (currentIndex + 1 < line.positionCount)
                    line.SetPosition(currentIndex + 1, points[currentIndex]);
            }

            // ���� ������ ������ �����ߴٸ� ���� ������ ��������
            if (currentIndex >= points.Length - 1)
                break;

            // ���� ��ġ ��� (�����ϰ� currentIndex+1 ���)
            float remain = targetDist - coveredDist;
            Vector3 dir = (points[currentIndex + 1] - points[currentIndex]).normalized;
            Vector3 newPos = points[currentIndex] + dir * remain;
            line.SetPosition(currentIndex + 1, newPos);

            yield return null;
        }

        // ����: ������ ������ ����
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
