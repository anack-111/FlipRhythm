using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Judge : MonoBehaviour
{
    [SerializeField] private Vector3 floatOffset;

    public TextMeshProUGUI _judgeText;
    private CanvasGroup canvasGroup;

    private float Speed = 0.5f;
    private float lifeTime = 0.3f;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }


    public void Show(string text, Vector3 worldPosition, Action onComplete = null)
    {
        transform.position = worldPosition + floatOffset;
        _judgeText.text = text.ToString();
        StartCoroutine(TextAnim(onComplete));
    }

    private IEnumerator TextAnim(Action onComplete = null)
    {
        float elapsed = 0f;

        while (elapsed < lifeTime)
        {
            float t = elapsed / lifeTime;


            transform.position += Vector3.up * Speed * Time.deltaTime;


            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0f;

        Destroy(gameObject);
        onComplete?.Invoke();
    }
}