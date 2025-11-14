using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Weapon) , true )]
public class WeaponScriptEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var data = (Weapon)target;

        if (data._config == null)
        {
            return;
        }

        if(GUILayout.Button("Update Offset"))
        {
            data._config.handOffset = data.transform.localPosition;
        }
    }
}