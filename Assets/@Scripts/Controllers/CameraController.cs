using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.1f;

    void LateUpdate()
    {
        Vector3 targetPos = new Vector3(target.position.x, target.position.y, -10);
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed);
    }
}
