using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WeaponSystem))]
public class WeaponSystemEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var data = (WeaponSystem)target;

        if(!GUILayout.Button("Update Config"))
        {
            return;
        }

        if(data.config == null)
        {
            Debug.LogError($"Error : Cant set {data.config} is null");
            return;
        }

        data.config.aimPointPosition = data.aimPoint.localPosition;
        data.config.hipPointPosition = data.hipPoint.localPosition;

    }
}