using System.Collections;
using UnityEngine;

public class BlockController : BaseController
{
    public float speed = 30f;

    [Header("Layers")]
    [SerializeField] private string projectileLayerName = "Projectile";
    [SerializeField] private string blockLayerName = "Block";
    [SerializeField] private Transform blockParent; // 부모 객체를 위한 Transform

    private int projectileLayer, blockLayer, hitLayers;
    private bool snapped;
    private Collider2D _col;
    private Rigidbody2D _rb;
    private Animator _anim;
    void Awake()
    {
        _col = GetComponent<Collider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();    
        projectileLayer = LayerMask.NameToLayer(projectileLayerName);
        blockLayer = LayerMask.NameToLayer(blockLayerName);

        // 발사 중엔 Projectile로 시작
        gameObject.layer = projectileLayer;

        // 맞춰야 하는 대상: 움직이는 투사체들 + 이미 놓인 블록들
        hitLayers = (1 << projectileLayer) | (1 << blockLayer);
    }

    void Update()
    {
        if (snapped)
            return;

        transform.Translate(Vector3.up * speed * Time.deltaTime);

        Vector2 origin = new Vector2(_col.bounds.center.x, _col.bounds.max.y + 0.05f);
        float checkDistance = 0.2f;

        Debug.DrawRay(origin, Vector2.up * checkDistance, Color.yellow);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, checkDistance, 1 << blockLayer);

        if (hit.collider != null && hit.collider != _col)
        {
            snapped = true;
            // 블록이 다른 블록 아래에 놓이도록 Y값을 계산
            float myHalfHeight = _col.bounds.extents.y;
            float targetY = hit.collider.bounds.min.y - myHalfHeight; // 블록 아래에 정확히 놓이도록
            Vector3 target = new Vector3(transform.position.x, targetY, transform.position.z);
            StartCoroutine(SnapToPosition(target, hit.collider.transform)); // 부모로 해당 블록의 Transform을 설정
        }
        else if (transform.position.y >= 1f)
        {
            snapped = true;
            Vector3 target = new Vector3(transform.position.x, 1f, transform.position.z);
            StartCoroutine(SnapToPosition(target, null)); // 부모가 없으면 그냥 놓기
        }
    }

    IEnumerator SnapToPosition(Vector3 target, Transform newParent)
    {
        BecomePlaced();

        // snapped 체크 제거 (중복 방지)
        Vector3 start = transform.position;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 6f;
            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }
        transform.position = target;

        // 부모 객체 설정
        if (newParent != null)
        {
            transform.SetParent(newParent); // 해당 블록의 부모로 설정
        }
        else
        {
            transform.SetParent(blockParent); // 기본 부모로 설정 (여기서 blockParent는 모든 블록의 부모)
        }
    }

    private void BecomePlaced()
    {
        _anim.SetTrigger("Stick");
        // 이제부터 '쌓인 블록'으로 취급
        gameObject.layer = blockLayer;

        if (_rb)
        {
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.gravityScale = 0f;
        }
    }
}
