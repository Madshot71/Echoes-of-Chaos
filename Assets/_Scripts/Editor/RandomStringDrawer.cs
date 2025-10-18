using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(RandomStringAttribute))]
public class RandomStringDrawer: PropertyDrawer 
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        if (property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.LabelField(position, label.text, "Use [RandomString] with string.");
            return;
        }

        EditorGUI.BeginProperty(position, label, property);

        // Calculate rects for the string field and button
        float buttonWidth = 70f;
        Rect textRect = new Rect(position.x, position.y, position.width - buttonWidth - 4, position.height);
        Rect buttonRect = new Rect(position.x + position.width - buttonWidth, position.y, buttonWidth, position.height);

        property.stringValue = EditorGUI.TextField(textRect, label, property.stringValue);

        if (GUI.Button(buttonRect, "Generate"))
        {
            property.stringValue = string.Empty.RandomString(9);
        }

        EditorGUI.EndProperty();
    }
}