using UnityEngine;
using UnityEngine.EventSystems;

public class MapEditableObject : MonoBehaviour
{
    private bool isDragging = false;
    private Camera mainCam;

    public MapEditorUIManager editorRef;
    public MapObject linkedData;

    void Start()
    {
        mainCam = Camera.main;
    }

    void OnMouseDown()
    {
        if (!editorRef.IsEditingMode())
            return; // 재생 중엔 편집 막기

        // UI 위 클릭 시 무시
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        isDragging = true;
    }

    void OnMouseUp()
    {
        if (!editorRef.IsEditingMode())
            return;

        isDragging = false;

        // 오브젝트 위치 수정 후 데이터 갱신
        linkedData.position = transform.position;
        linkedData.rotation = transform.eulerAngles.z;

        Debug.Log($"[Edit] {linkedData.type} moved to {transform.position}");
    }

    void Update()
    {
        if (!editorRef.IsEditingMode())
            return;

        if (isDragging)
        {
            Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            transform.position = mousePos;
        }

        // 회전 (R 키)
        if (Input.GetKeyDown(KeyCode.R) && isDragging)
        {
            transform.Rotate(Vector3.forward * 45f);
            linkedData.rotation = transform.eulerAngles.z;
        }

        // 삭제 (Delete 키)
        if (Input.GetKeyDown(KeyCode.Delete) && isDragging)
        {
            editorRef.RemoveObject(this);
            Destroy(gameObject);
        }
    }
}
