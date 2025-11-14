using UnityEngine;


public abstract class WeaponConfig : ScriptableObject 
{
    [Header("Weapon Settings")]
    public new string name;
    [RandomString(9)] public string ID;
    public Vector3 handOffset;
    public float reloadTime;
    public int AnimationIndex = 1;
}