using UnityEditor;

[CustomEditor(typeof(Portal)), CanEditMultipleObjects]
public class PortalEditor : Editor
{
    public enum DisplayCategory
    {
        Gamemode, Speed, Gravity
    }
    public DisplayCategory categoryToDisplay;

    bool FirstTime = true;

    public override void OnInspectorGUI()
    {
        if (FirstTime)
        {
            switch (serializedObject.FindProperty("_state").intValue)
            {
                case 0:
                    categoryToDisplay = DisplayCategory.Speed;
                    break;
                case 1:
                    categoryToDisplay = DisplayCategory.Gamemode;
                    break;
                case 2:
                    categoryToDisplay = DisplayCategory.Gravity;
                    break;
            }
        }
        else
            categoryToDisplay = (DisplayCategory)EditorGUILayout.EnumPopup("Display", categoryToDisplay);

        EditorGUILayout.Space();

        switch (categoryToDisplay)
        {
            case DisplayCategory.Gamemode:
                DisplayProperty("_gameMode", 1);
                break;

            case DisplayCategory.Gravity:
                DisplayProperty("_gravity", 2);
                break;

            case DisplayCategory.Speed:
                DisplayProperty("_speed", 0);
                break;

        }

        FirstTime = false;

        serializedObject.ApplyModifiedProperties();
    }

    void DisplayProperty(string property, int PropNumb)
    {
        try
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(property));
        }
        catch
        { }
        serializedObject.FindProperty("_state").intValue = PropNumb;
    }
}