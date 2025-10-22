using UnityEngine;
using UnityEngine.UIElements;
using static Define;

public class FollowPlayer : MonoBehaviour
{
    public Vector2 cameraOffset;

    public float interpolationTime = 0.1f;

    public Transform _player;
    public Transform _groundCamera;
    public Transform _topGround;
    public Transform _backGround;
    public Movement _movement;

    Vector3 newVector;
    bool firstFrame = true; //첫 프레임에 순간적으로 이동할지, 부드럽게 이동할지 결정하는 플래그

    void FixedUpdate()
    {
        newVector = new Vector3(_player.position.x + cameraOffset.x, newVector.y, -10);

        if (Define.SCREEN_HEIGHT_VALUES[(int)_movement._currentGameMode] > 10)
            FreeCam(firstFrame);
        else
            StaticCam(firstFrame, _movement._yLastPortal, Define.SCREEN_HEIGHT_VALUES[(int)_movement._currentGameMode]);

        _backGround.localPosition = new Vector3((-_player.position.x * 0.5f) + Mathf.Floor(_player.transform.position.x / 96) * 48, 2.2f, 10);
        transform.position = newVector;
        firstFrame = false;
    }

    void FreeCam(bool doInstantly)
    {
        _groundCamera.position = InterpolateVec3(new Vector3(0, _groundCamera.position.y), Vector3.up * cameraOffset, 20) + Vector3.right * (Mathf.Floor(_player.transform.position.x / 5) * 5);

        if (Vector2.Distance(_topGround.localPosition, Vector3.up * 20f) < 0.3f)
            _topGround.gameObject.SetActive(false);
        if (_topGround)
            _topGround.localPosition = InterpolateVec3(_topGround.localPosition, Vector3.up * 20f, 100);

        if (!doInstantly)
            newVector += Vector3.up * (Mathf.Lerp(_player.transform.position.y + 1.7f - newVector.y, -0.6f - newVector.y, (_player.position.y <= 4.2f) ? 1 : 0)) * Time.deltaTime / interpolationTime;
        else
            newVector += Vector3.up * (_player.transform.position.y + 1.7f);
    }

    Vector3 InterpolateVec3(Vector3 current, Vector3 target, float speed)
    {
        return current + Vector3.Normalize(target - current) * Time.deltaTime * speed * Mathf.Clamp(Vector3.Distance(current, target), 0, 1);
    }

    void StaticCam(bool doInstantly, float yLastPortal, int screenHeight)
    {
        _topGround.gameObject.SetActive(true);

        _groundCamera.position = InterpolateVec3(new Vector3(0, _groundCamera.position.y), Vector3.up * Mathf.Clamp(yLastPortal - screenHeight * 0.5f, cameraOffset.y, float.MaxValue), 20) + Vector3.right * (Mathf.Floor(_player.transform.position.x / 5) * 5);
        _topGround.localPosition = InterpolateVec3(_topGround.localPosition, Vector3.up * (4.5f + screenHeight), 30);

        if (!doInstantly)
            newVector += Vector3.up * (5 + Mathf.Clamp(yLastPortal - screenHeight * 0.5f, cameraOffset.y, 2048) - newVector.y - ((11 - screenHeight) * 0.5f)) * Time.deltaTime / interpolationTime;
        else
            newVector += Vector3.up * (5 + Mathf.Clamp(yLastPortal - screenHeight * 0.5f, cameraOffset.y, 2048) - ((11 - screenHeight) * 0.5f));
    }
}