using UnityEngine;

[CreateAssetMenu(fileName = "WeaponConfig", menuName = "WeaponConfig", order = 0)]
public class WeaponConfig : ScriptableObject 
{
    [Header("Weapon Settings")]
    public new string name;
    [RandomString(9)] public string ID; 
}