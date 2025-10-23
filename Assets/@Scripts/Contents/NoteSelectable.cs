using UnityEngine;

public class NoteSelectable : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (Camera.main == null) return;

        // 클릭 위치값
        float selectedX = transform.position.x;

        // 메인 에디터 참조 찾아서 저장
        var editor = FindObjectOfType<MapEditorUIManager>();
        if (editor != null)
        {
            editor.selectedNoteX = selectedX;
            Debug.Log($"?? Note selected at X = {selectedX:F2}");
        }
    }
}
