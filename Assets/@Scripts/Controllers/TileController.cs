using UnityEngine;
using DG.Tweening;  // DOTween ���ӽ����̽� �߰�

public class TileController : BaseController
{
    public float bpm = 120f;         // ���� BPM
    public float snapDistance = 1f;  // �� ���ڸ��� �̵��� �Ÿ�
    public bool moveOnBeat = true;
    public float scaleAmount = 0.8f; // ũ�� ���� ���� (�۾����� ����)
    public float scaleDuration = 0.1f; // ũ�� ��ȭ �ð� (�� ���ڿ� ���߱�)

    //private float beatInterval;      // �� ���� ����(��)
    private float timer;
    private Vector3 originalScale;  // ���� ũ�� ����

    void Start()
    {
        originalScale = transform.localScale; // Ÿ���� ���� ũ�� ����

        timer = 0f;
    }

    void Update()
    {
        if (!moveOnBeat) 
            return;

        timer += Time.deltaTime;
        if (timer >= Managers.Game.BeatInterval)
        {
            timer -= Managers.Game.BeatInterval;
            ScaleTile();

        }

    }

    virtual protected void ScaleTile()
    {
        ////Ÿ���� �۰� ����� ���� ũ��� ���ƿ��� ����
        //transform.DOScale(originalScale * scaleAmount, scaleDuration)  // �۰� �����
        //         .SetEase(Ease.OutBack)  // �ڿ������� ���
        //         .OnComplete(() =>
        //         {
        //             // ���� ũ��� ���ƿ���
        //             transform.DOScale(originalScale, scaleDuration)
        //                      .SetEase(Ease.InOutBack);  // �ڿ������� Ȯ��
        //         });

        // Ÿ���� �� ���ڸ�ŭ �������� �̵�
        Vector3 target = transform.position + Vector3.left * snapDistance;
        transform.DOMove(target, Managers.Game.BeatInterval).SetEase(Ease.InOutSine);
    }
}
