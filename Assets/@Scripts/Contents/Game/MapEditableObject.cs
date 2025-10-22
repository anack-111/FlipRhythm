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
            return; // ��� �߿� ���� ����

        // UI �� Ŭ�� �� ����
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        isDragging = true;
    }

    void OnMouseUp()
    {
        if (!editorRef.IsEditingMode())
            return;

        isDragging = false;

        // ������Ʈ ��ġ ���� �� ������ ����
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

        // ȸ�� (R Ű)
        if (Input.GetKeyDown(KeyCode.R) && isDragging)
        {
            transform.Rotate(Vector3.forward * 45f);
            linkedData.rotation = transform.eulerAngles.z;
        }

        // ���� (Delete Ű)
        if (Input.GetKeyDown(KeyCode.Delete) && isDragging)
        {
            editorRef.RemoveObject(this);
            Destroy(gameObject);
        }
    }
}
