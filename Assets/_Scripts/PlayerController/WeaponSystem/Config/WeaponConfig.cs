using UnityEngine;


public abstract class WeaponConfig : ScriptableObject 
{
    [Header("Weapon Settings")]
    public new string name;
    [RandomString(9)] public string ID;
    public Vector3 leftHandOffset;
    public Vector3 rightHandOffset;
    public Vector3 aimDownOffset;
    public Vector3 hipOffset;
    public float reloadTime;
    public float adsDistance = 2;
    public float hipDistance = 1;
    public int AnimationIndex = 1;
}