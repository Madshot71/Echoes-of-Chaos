using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TimedAnimation))]
public class TimedAnimationEditor : Editor
{
    private float animationTime = 0f;

    // A static field to hold the reference object, so it persists between selections.
    private static GameObject referenceObject;

    public override void OnInspectorGUI()
    {
        // Update the serialized object
        serializedObject.Update();

        // 1. Draw the default fields for the TimedAnimation component first.
        base.OnInspectorGUI();

        TimedAnimation data = (TimedAnimation)target;
        AnimationClip clip = data.clip;

        // 2. Add an ObjectField for the reference object, managed by the editor.
        referenceObject = (GameObject)EditorGUILayout.ObjectField("Reference Object", referenceObject, typeof(GameObject), true);
        
        if (clip == null || referenceObject == null)
        {
            EditorGUILayout.HelpBox("Assign a Clip and a Reference Object to enable the preview.", MessageType.Info);
            return;
        }

        EditorGUILayout.Space(10);

        // 3. Draw the custom previewer UI inside a box at the bottom.
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("Animation Preview", EditorStyles.boldLabel);
        
        float newTime = EditorGUILayout.Slider("Time", animationTime, 0, clip.length);
        if (newTime != animationTime)
        {
            animationTime = newTime;
            SampleAnimation(referenceObject, clip, animationTime);
        }

        // --- Timestamp Label and "Set" Button ---
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Timestamp:", $"{animationTime:F2}s / {clip.length:F2}s");

        if (GUILayout.Button("Set Current Time"))
        {
            SerializedProperty timeProp = serializedObject.FindProperty("time");
            if (timeProp != null)
            {
                timeProp.floatValue = animationTime;
                serializedObject.ApplyModifiedProperties(); // Save the change
                Debug.Log($"Set TimedAnimation 'time' to {animationTime}");
            }
            else
            {
                Debug.LogWarning("Could not find a 'public float time;' variable on the TimedAnimation script to set.");
            }
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();

        // Apply any changes made in the inspector
        if (GUI.changed)
        {
            EditorUtility.SetDirty(data);
        }

        
    }

    private void SampleAnimation(GameObject targetObject, AnimationClip clip, float time)
    {
        clip.SampleAnimation(targetObject, clip.length * time);
    }

    private void OnDisable()
    {
        // Don't stop preview on deselection if the reference is static
        // StopPreview();
    }
}