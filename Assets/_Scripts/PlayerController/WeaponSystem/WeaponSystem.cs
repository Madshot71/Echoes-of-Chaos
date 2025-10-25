using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(PlayerController))]
public class WeaponSystem : MonoBehaviour
{

    [Header("Holsters")]
    private const int max = 3; // max number of holsters
    public Holster[] holsters = new Holster[max];


    [Header("Equipped Weapon")]
    [SerializeField] public Weapon current; // currrently equipped weapon

    private PlayerController controller;
    public Animator animator => controller.animator;

    private void OnValidate()
    {
        controller ??= GetComponent<PlayerController>();
    }







    public void Equip(Weapon weapon)
    {
        Holster holster = FindHolster(weapon.weaponType);
        if (holster == null)
        {
            Debug.LogWarning("Cannot equip weapon: " + weapon.config.name + ". No holster available.");
            return;
        }

        // If there's a weapon already in the holster, unequip it
        if (holster.weapon != null)
        {
            Unequip(holster.weapon);
        }

        // Equip the new weapon
        holster.weapon = weapon;
        weapon.transform.SetParent(holster.point);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;

        current ??= weapon; // Sets the current weapon if there is non
    }

    private void Unequip(Weapon weapon)
    {
        
    }
    
    private Holster FindHolster(Weapon.WeaponType type)
    {
        foreach (Holster holster in holsters)
        {
            if (holster.weapon != null && holster.weapon.weaponType == type)
            {
                return holster;
            }
        }

        Debug.LogWarning("No holster found for weapon type: " + type);
        return null;
    }





    //{ internal classes }

    [System.Serializable]
    public class Holster
    {
        public string name;
        public Transform point;
        public Weapon weapon;
    }
}
