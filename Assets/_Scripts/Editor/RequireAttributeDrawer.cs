using UnityEngine;
using UnityEditor;
[CustomPropertyDrawer(typeof(RequireAttribute))]
public class RequireAttributeDrawer: PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float iconWidth = 18f;

        // Field rect (single line)
        var fieldRect = new Rect(position.x, position.y, position.width - iconWidth - 4f, lineHeight);

        EditorGUI.BeginProperty(fieldRect, label, property);
        property.objectReferenceValue = EditorGUI.ObjectField(fieldRect, label, property.objectReferenceValue, typeof(Object), true);
        EditorGUI.EndProperty();

        // Determine if we need to show a help box
        bool showHelp = false;
        string helpMessage = null;
        MessageType helpType = MessageType.Warning;

        if (property.objectReferenceValue == null)
        {
            showHelp = true;
            helpMessage = "This property cannot be null.";
            helpType = MessageType.Warning;
        }

        if (showHelp)
        {
            var helpRect = new Rect(position.x, position.y + lineHeight + 2f, position.width, lineHeight + 4f);
            EditorGUI.HelpBox(helpRect, helpMessage, helpType);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float baseHeight = EditorGUIUtility.singleLineHeight;
        // If the property is an object reference and currently null or invalid type, add space for the help box
        if (property.propertyType == SerializedPropertyType.ObjectReference && (property.objectReferenceValue == null || !(property.objectReferenceValue is MonoBehaviour)))
        {
            return baseHeight + (EditorGUIUtility.singleLineHeight + 6f);
        }

        return baseHeight;
    }
}