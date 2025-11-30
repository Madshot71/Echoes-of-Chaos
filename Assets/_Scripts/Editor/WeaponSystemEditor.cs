using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WeaponSystem))]
public class WeaponSystemEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var data = (WeaponSystem)target;


        if(GUILayout.Button("LockToAim"))
        {
            data.LockControlletToAim();
        }

        if(GUILayout.Button("LockToShoulder"))
        {
            data.LockControlletToShoulder();
        }

        if(GUILayout.Button("UpdateWeapon OffSets - Aim / Shoulder"))
        {
            data.UpdateIronSightAimOffset();
            data.UpdateShoulderAimOffset();
            data.UpdateWeapon();

        }
        
        if(GUILayout.Button("Unlock"))
        {
            data.Unlock();
        }

    }
}