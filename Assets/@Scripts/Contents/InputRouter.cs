using UnityEngine;
using System;

public class InputRouter : MonoBehaviour
{
    public Camera mainCam;
    public LayerMask zoneMask; // "Zones" ·¹ÀÌ¾î
    public AudioSource music;

    public event Action<int, float> OnZoneVisited; // (zoneIndex, musicTime)
    public event Action OnTouchBegin;
    public event Action OnTouchEnd;

    bool touching;
    int lastZone = -1;

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0)) Begin();
        if (Input.GetMouseButton(0)) Tick(Input.mousePosition);
        if (Input.GetMouseButtonUp(0)) End();
#else
        if (Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began) Begin();
            if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary) Tick(t.position);
            if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled) End();
        }
#endif
    }

    void Begin() { touching = true; lastZone = -1; OnTouchBegin?.Invoke(); }
    void End() { touching = false; lastZone = -1; OnTouchEnd?.Invoke(); }

    void Tick(Vector2 screenPos)
    {
        if (!touching) return;

        Vector2 world = mainCam.ScreenToWorldPoint(screenPos);
        RaycastHit2D hit = Physics2D.Raycast(world, Vector2.zero, 0.01f, zoneMask);
        if (hit.collider != null && hit.collider.TryGetComponent<ZoneObject>(out var zone))
        {
            if (zone.index != lastZone)
            {
                lastZone = zone.index;
                zone.Flash(new Color(0.7f, 1f, 1f));
                OnZoneVisited?.Invoke(zone.index, music.time);
            }
        }
    }
}
