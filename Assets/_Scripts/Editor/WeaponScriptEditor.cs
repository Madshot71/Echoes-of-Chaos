using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Weapon) , true )]
public class WeaponScriptEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var data = (Weapon)target;

        if(!GUILayout.Button("Update Offset"))
        {
            return;
        }

        if (data._config == null)
        {
            Debug.LogError($"Error : cant set {data._config} is null");
            return;
        }

        data.UpdateConfig();
    }
}
