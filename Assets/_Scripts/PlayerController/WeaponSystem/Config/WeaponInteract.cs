using UnityEngine;

public class WeaponInteract : Interactable
{
    private Weapon weapon;

    private void OnValidate()
    {
        if (weapon != null) 
            return;

        if (!TryGetComponent<Weapon>(out weapon))
        {
            // Remove from gameObject
            Debug.LogError("No weapon Script found : Script will Destory its self on Awake");
        }
    }
    
    private void Awake()
    {
        if(!weapon)
        {
            Destroy(this.gameObject);
        }
    }

    public override string InteractionPrompt()
    {
        return $"PICK UP {weapon.name}";
    }

    public override void Interact(CharacterBase character)
    {
        character.controller.weaponSystem.Equip(weapon);
        this.enabled = false;
    }

    public void Drop()
    {
        this.enabled = true;
    }
}