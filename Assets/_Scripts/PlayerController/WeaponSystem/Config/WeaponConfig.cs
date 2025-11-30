using UnityEngine;


public abstract class WeaponConfig : ScriptableObject 
{
    [Header("Weapon Settings")]
    public new string name;
    [RandomString(9)] public string ID;
    public Sprite Icon;
    public float reloadTime;
    public float adsDistance = 2;
    public float hipDistance = 1;
    public int AnimationIndex = 1;


    [Header("Offsets")]
    public Vector3 leftHandOffset;
    public Quaternion leftHandRotation;
    public Vector3 rightHandOffset;
    public Quaternion rightHandRotation;
    public Vector3 aimDownOffset;
    public Vector3 hipOffset;
    
}