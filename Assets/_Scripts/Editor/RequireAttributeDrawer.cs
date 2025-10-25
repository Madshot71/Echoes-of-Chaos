using System;
using UnityEngine;
using UnityEditor;
[CustomPropertyDrawer(typeof(RequireAttribute))]
public class RequireAttributeDrawer: PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        // Ensure this drawer is used only on object reference properties
        if (property.propertyType != SerializedPropertyType.ObjectReference)
        {
            EditorGUI.LabelField(position, label.text, "Use [Require] with Object reference.");
            return;
        }

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float iconWidth = 18f;

        // Field rect (single line)
        var fieldRect = new Rect(position.x, position.y, position.width - iconWidth - 4f, lineHeight);

        // Determine the concrete type expected by the underlying field (fall back to UnityEngine.Object)
        Type requiredType = fieldInfo.FieldType;
        if (!typeof(UnityEngine.Object).IsAssignableFrom(requiredType))
            requiredType = typeof(UnityEngine.Object);

        EditorGUI.BeginProperty(position, label, property);
        // Use the concrete requiredType so the picker only allows that type (GameObject, Collider, etc.)
        property.objectReferenceValue = EditorGUI.ObjectField(fieldRect, label, property.objectReferenceValue, requiredType, true);
        EditorGUI.EndProperty();

        // Determine if we need to show a help box
        bool showHelp = false;
        string helpMessage = null;
        MessageType helpType = MessageType.Warning;

        if (property.objectReferenceValue == null)
        {
            showHelp = true;
            helpMessage = "This property cannot be null.";
            helpType = MessageType.Error;
        }
        else if (!IsValidType(property.objectReferenceValue, requiredType))
        {
            showHelp = true;
            helpMessage = $"Assigned object must be of type {requiredType.Name}.";
            helpType = MessageType.Error;
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

        // Only object reference properties can show the help box
        if (property.propertyType == SerializedPropertyType.ObjectReference)
        {
            Type requiredType = fieldInfo.FieldType;
            if (!typeof(UnityEngine.Object).IsAssignableFrom(requiredType))
                requiredType = typeof(UnityEngine.Object);

            if (property.objectReferenceValue == null || !IsValidType(property.objectReferenceValue, requiredType))
            {
                return baseHeight + (EditorGUIUtility.singleLineHeight + 6f);
            }
        }

        return baseHeight;
    }

    private bool IsValidType(UnityEngine.Object obj, Type requiredType)
    {
        if (obj == null) return false;
        // requiredType is guaranteed to be a UnityEngine.Object-derived type
        return requiredType.IsAssignableFrom(obj.GetType());
    }
}