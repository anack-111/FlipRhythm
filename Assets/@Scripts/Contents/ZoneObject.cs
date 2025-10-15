using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class ZoneObject : MonoBehaviour
{
    public int index; // 1~9
    SpriteRenderer sr;
    Color baseColor;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        baseColor = sr.color;
        gameObject.layer = LayerMask.NameToLayer("Zones");
    }

    public void Flash(Color c, float t = 0.08f)
    {
        sr.color = c;
        CancelInvoke();
        Invoke(nameof(ResetColor), t);
    }
    void ResetColor() => sr.color = baseColor;
}
