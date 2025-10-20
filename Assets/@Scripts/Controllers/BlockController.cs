using System.Collections;
using UnityEngine;

public class BlockController : BaseController
{
    public float speed = 20f;

    [Header("Layers")]
    [SerializeField] private string projectileLayerName = "Projectile";
    [SerializeField] private string blockLayerName = "Block";

    private int projectileLayer, blockLayer, hitLayers;
    private bool snapped;
    private Collider2D col;
    private Rigidbody2D rb;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        projectileLayer = LayerMask.NameToLayer(projectileLayerName);
        blockLayer = LayerMask.NameToLayer(blockLayerName);

        // �߻� �߿� Projectile�� ����
        gameObject.layer = projectileLayer;

        // ����� �ϴ� ���: �����̴� ����ü�� + �̹� ���� ��ϵ�
        hitLayers = (1 << projectileLayer) | (1 << blockLayer);
    }

    void Update()
    {
        if (snapped)
            return;

        transform.Translate(Vector3.up * speed * Time.deltaTime);

        Vector2 origin = new Vector2(col.bounds.center.x, col.bounds.max.y + 0.05f);
        float checkDistance = 2f;

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, checkDistance, 1 << blockLayer);

        if (hit.collider != null && hit.collider != col)
        {
            snapped = true;
            float myHalfHeight = col.bounds.extents.y;
            float targetY = hit.collider.bounds.min.y - myHalfHeight;
            Vector3 target = new Vector3(transform.position.x, targetY, transform.position.z);
            StartCoroutine(SnapToPosition(target));
        }
        else if (transform.position.y >= 1.5f)
        {
            snapped = true;
            Vector3 target = new Vector3(transform.position.x, 1.5f, transform.position.z);
            StartCoroutine(SnapToPosition(target));
        }
    }

    IEnumerator SnapToPosition(Vector3 target)
    {
        BecomePlaced();

        // snapped üũ ���� (�ߺ� ����)
        Vector3 start = transform.position;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 6f;
            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }
        transform.position = target;


    }

    private void BecomePlaced()
    {
        // �������� '���� ���'���� ���
        gameObject.layer = blockLayer;

        if (rb)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
        }
    }
}
