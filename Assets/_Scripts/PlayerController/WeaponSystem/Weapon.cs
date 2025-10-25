using UnityEngine;

[RequireComponent(typeof(AudioSource) , typeof(WeaponInteract))]
public abstract class Weapon : MonoBehaviour
{
    public WeaponConfig config;
    public WeaponType weaponType;

    internal abstract void Attack();
    internal abstract void Reload();

    internal void HIT()
    {

    }

    public enum WeaponType
    {
        melee,
        ranged
    }
}