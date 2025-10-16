using UnityEngine;
using System;

public class InputRouter : MonoBehaviour
{
    public Camera mainCam;
    public LayerMask zoneMask;
    public AudioSource music;

    public event Action<int, float> OnZoneVisited;

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
            OnZoneVisited?.Invoke(zone.index, music.time);
        }
    }
}
