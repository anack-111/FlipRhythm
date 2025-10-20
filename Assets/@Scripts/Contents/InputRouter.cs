using UnityEngine;
using System;

public class InputRouter : MonoBehaviour
{
    public Camera mainCam;
    public LayerMask zoneMask;
    public AudioSource music;

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
            Detect(Input.mousePosition);
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            Detect(Input.GetTouch(0).position);
#endif
    }

    void Detect(Vector2 screenPos)
    {
        Vector2 world = mainCam.ScreenToWorldPoint(screenPos);
        RaycastHit2D hit = Physics2D.Raycast(world, Vector2.zero, 0.1f, zoneMask);
        if (hit.collider != null && hit.collider.TryGetComponent<ZoneObject>(out var zone))
        {
            float currentTime = music != null ? music.time : 0f;
            int tappedZoneIndex = zone.index + 1; // TapNote는 1-based zoneIndex

            // 즉시 터치 피드백
            zone.Flash(Color.white, 0.06f);

            // 같은 존의 비판정 TapNote 중에서 hitTime과 가장 가까운(=적절한) 하나만 찾는다
            TapNote[] notes = FindObjectsOfType<TapNote>();
            TapNote best = null;
            float bestDiff = float.MaxValue;
            foreach (var n in notes)
            {
                if (n.IsJudged) continue;
                if (n.ZoneIndex != tappedZoneIndex) continue;
                float diff = Mathf.Abs(n.HitTime - currentTime);
                if (diff < bestDiff)
                {
                    bestDiff = diff;
                    best = n;
                }
            }

            if (best != null)
            {
                var result = best.TryHandleHit(currentTime);

                // 판정 결과에 따른 존 플래시 색상
                Color c = Color.white;
                switch (result)
                {
                    case TapNote.HitResult.Perfect: c = Color.cyan;
                        break;
                    case TapNote.HitResult.Good: c = Color.yellow;
                        break;
                    case TapNote.HitResult.Miss: c = Color.gray;
                        break;
                    default: c = Color.white; break;
                }
                zone.Flash(c, 0.12f);
            }
        }
    }
}
