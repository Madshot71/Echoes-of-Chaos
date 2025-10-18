using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SceneAttribute))]
public class SceneAttributeDrawer: PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.LabelField(position, label.text, "Use [Scene] with string.");
            return;
        }

        EditorGUI.BeginProperty(position, label, property);

        // Find the SceneAsset by name
        SceneAsset sceneAsset = null;
        if (!string.IsNullOrEmpty(property.stringValue))
        {
            string scenePath = AssetDatabase.GetAllAssetPaths()
                .FirstOrDefault(p => p.EndsWith(property.stringValue + ".unity"));
            if (!string.IsNullOrEmpty(scenePath))
            {
                sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            }
        }

        // Draw the object field
        var newSceneAsset = EditorGUI.ObjectField(position, label, sceneAsset, typeof(SceneAsset), false) as SceneAsset;
        if (newSceneAsset != null)
        {
            property.stringValue = newSceneAsset.name;
        }
        else if (sceneAsset != null && newSceneAsset == null)
        {
            property.stringValue = string.Empty;
        }

        EditorGUI.EndProperty();
    }
}