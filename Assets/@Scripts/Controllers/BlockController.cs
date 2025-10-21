using System.Collections;
using UnityEngine;

public class BlockController : BaseController
{
    public float speed = 30f;

    [Header("Layers")]
    [SerializeField] private string projectileLayerName = "Projectile";
    [SerializeField] private string blockLayerName = "Block";
    [SerializeField] private Transform blockParent; // �θ� ��ü�� ���� Transform

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
        float checkDistance = 0.1f;

        Debug.DrawRay(origin, Vector2.up * checkDistance, Color.yellow);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, checkDistance, 1 << blockLayer);

        if (hit.collider != null && hit.collider != col)
        {
            snapped = true;
            // ����� �ٸ� ��� �Ʒ��� ���̵��� Y���� ���
            float myHalfHeight = col.bounds.extents.y;
            float targetY = hit.collider.bounds.min.y - myHalfHeight; // ��� �Ʒ��� ��Ȯ�� ���̵���
            Vector3 target = new Vector3(transform.position.x, targetY, transform.position.z);
            StartCoroutine(SnapToPosition(target, hit.collider.transform)); // �θ�� �ش� ����� Transform�� ����
        }
        else if (transform.position.y >= 1f)
        {
            snapped = true;
            Vector3 target = new Vector3(transform.position.x, 1f, transform.position.z);
            StartCoroutine(SnapToPosition(target, null)); // �θ� ������ �׳� ����
        }
    }

    IEnumerator SnapToPosition(Vector3 target, Transform newParent)
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

        // �θ� ��ü ����
        if (newParent != null)
        {
            transform.SetParent(newParent); // �ش� ����� �θ�� ����
        }
        else
        {
            transform.SetParent(blockParent); // �⺻ �θ�� ���� (���⼭ blockParent�� ��� ����� �θ�)
        }
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
