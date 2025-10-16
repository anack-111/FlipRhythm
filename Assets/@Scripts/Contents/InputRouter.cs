using UnityEngine;
using System;

public class InputRouter : MonoBehaviour
{
    [Header("����")]
    public Camera mainCam;
    public LayerMask zoneMask;    // "Zones" ���̾�
    public AudioSource music;

    [Header("�ɼ�")]
    public bool invokeOnBeginTick = true;   // ��ġ ���� �����ӿ� ��� �� üũ

    // �̺�Ʈ
    public event Action<int, float> OnZoneVisited;  // (zoneIndex, musicTime) - �� ���� ������ ���� 1ȸ ����
    public event Action OnTouchBegin;
    public event Action OnTouchEnd;

    // ����
    public bool IsTouching { get; private set; }     // ���� ��ġ ���ΰ�
    public int CurrentZone { get; private set; } = -1; // ���� �հ����� �÷��� �ִ� ��(-1�̸� ����)
    public Vector2 TouchWorldPos { get; private set; } // ���� ��ġ ���� ��ǥ

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

    // --- �ܺο��� ���� ���� ���� ---
    /// <summary>������ �� ���� ���� ��ġ�� ������/�巡�� ���ΰ�?</summary>
    public bool IsTouchingZone(int zoneIndex) => IsTouching && CurrentZone == zoneIndex;

    /// <summary>���� � �� ���� �ִ��� ��� (������ false)</summary>
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

        if (invokeOnBeginTick) Tick(screenPos); // ���� �����ӿ� �ٷ� �� �� ����
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

        // �� ���������� ���������� �� �浹 Ȯ��
        var hit = Physics2D.OverlapPoint(TouchWorldPos, zoneMask);
        int zoneNow = -1;

        if (hit && hit.TryGetComponent<ZoneObject>(out var zone))
            zoneNow = zone.index;

        // ���� ����
        if (zoneNow != CurrentZone)
        {
            // �ð� ȿ��(����)
            if (zoneNow != -1 && hit.TryGetComponent<ZoneObject>(out var z1))
                z1.Flash(new Color(0.7f, 1f, 1f));

            CurrentZone = zoneNow;
        }

        // �� ���� ���� ���� �̺�Ʈ ���� (�巡�� ��� ������)
        if (CurrentZone != -1 && CurrentZone != _lastEmittedZone)
        {
            _lastEmittedZone = CurrentZone;
            OnZoneVisited?.Invoke(CurrentZone, music ? music.time : 0f);
        }
    }
}
