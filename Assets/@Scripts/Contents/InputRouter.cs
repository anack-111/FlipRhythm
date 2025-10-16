using UnityEngine;
using System;

public class InputRouter : MonoBehaviour
{
    [Header("참조")]
    public Camera mainCam;
    public LayerMask zoneMask;    // "Zones" 레이어
    public AudioSource music;

    [Header("옵션")]
    public bool invokeOnBeginTick = true;   // 터치 시작 프레임에 즉시 존 체크

    // 이벤트
    public event Action<int, float> OnZoneVisited;  // (zoneIndex, musicTime) - 새 존에 진입할 때만 1회 발행
    public event Action OnTouchBegin;
    public event Action OnTouchEnd;

    // 상태
    public bool IsTouching { get; private set; }     // 현재 터치 중인가
    public int CurrentZone { get; private set; } = -1; // 현재 손가락이 올려져 있는 존(-1이면 없음)
    public Vector2 TouchWorldPos { get; private set; } // 현재 터치 월드 좌표

    int _lastEmittedZone = -1;

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0)) Begin(Input.mousePosition);
        if (Input.GetMouseButton(0)) Tick(Input.mousePosition);
        if (Input.GetMouseButtonUp(0)) End();
#else
        if (Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began) Begin(t.position);
            if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary) Tick(t.position);
            if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled) End();
        }
#endif
    }

    // --- 외부에서 쓰기 좋은 헬퍼 ---
    /// <summary>지정한 존 위를 현재 터치로 누르고/드래그 중인가?</summary>
    public bool IsTouchingZone(int zoneIndex) => IsTouching && CurrentZone == zoneIndex;

    /// <summary>현재 어떤 존 위에 있는지 얻기 (없으면 false)</summary>
    public bool TryGetCurrentZone(out int zoneIndex)
    {
        zoneIndex = CurrentZone;
        return IsTouching && CurrentZone != -1;
    }
    // --------------------------------

    void Begin(Vector2 screenPos)
    {
        IsTouching = true;
        _lastEmittedZone = -1;
        CurrentZone = -1;
        OnTouchBegin?.Invoke();

        if (invokeOnBeginTick) Tick(screenPos); // 시작 프레임에 바로 한 번 판정
    }

    void End()
    {
        IsTouching = false;
        CurrentZone = -1;
        _lastEmittedZone = -1;
        OnTouchEnd?.Invoke();
    }

    void Tick(Vector2 screenPos)
    {
        if (!IsTouching) return;

        TouchWorldPos = mainCam.ScreenToWorldPoint(screenPos);

        // 점 오버랩으로 안정적으로 존 충돌 확인
        var hit = Physics2D.OverlapPoint(TouchWorldPos, zoneMask);
        int zoneNow = -1;

        if (hit && hit.TryGetComponent<ZoneObject>(out var zone))
            zoneNow = zone.index;

        // 상태 갱신
        if (zoneNow != CurrentZone)
        {
            // 시각 효과(선택)
            if (zoneNow != -1 && hit.TryGetComponent<ZoneObject>(out var z1))
                z1.Flash(new Color(0.7f, 1f, 1f));

            CurrentZone = zoneNow;
        }

        // 새 존에 들어섰을 때만 이벤트 발행 (드래그 경로 판정용)
        if (CurrentZone != -1 && CurrentZone != _lastEmittedZone)
        {
            _lastEmittedZone = CurrentZone;
            OnZoneVisited?.Invoke(CurrentZone, music ? music.time : 0f);
        }
    }
}
