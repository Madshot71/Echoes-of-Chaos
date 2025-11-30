using System.Collections.Generic;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using GhostBoy;

public class CharacterBase : MonoBehaviour
{

    [Header("Basic Data")]
    public new string name;
    [RandomString(8)] public string ID;

    public PlayerController controller;
    public ControllerCamera playerCamera;
    public Vehicle currentVehicle;

    internal List<CharacterBase> Allies {get; private set;} = new();
    internal Data data;

    public Interactable interactable { get; private set; } // the current Interactable 

    // Storing the players current location 
    public Vector3 position => currentVehicle == null ?
        controller.transform.position :
        currentVehicle.transform.position;

    private void Awake()
    {
        controller?.Init(this);
        playerCamera = controller.camController;
        data = new Data (this);
    }

    private void Update() 
    {
        data.UpdateData(this);

        SetIronSight();
    }

    public void FixedUpdate()
    {
        if (controller.interact == null)
            return;

        interactable = controller.interact.GetInteractable();
    }


    public void Interact()
    {
        if (interactable == null)
        {
            return;
        }

        interactable.Interact(this);
    }


    public void ControllerInputs(PlayerController.InputReceiver input)
    {
        if(controller == null)
        {
            return;
        }   
        controller.receiver = input;
    }

    #region  WeaponInputs

    public void SetIronSight()
    {
        if(controller == null || playerCamera == null)
        {
            return;    
        }

        controller.weaponSystem.useIronSight = playerCamera.isFirstPerson && playerCamera.useFirstAim;
    }

    #endregion

    // Data Handling


    [System.Serializable]
    public class Data
    {
        public string name;
        public string ID;

        public float health;
        public float maxHealth;
        public float stamina;
        public float maxStamina;

        public bool isPlayer;
        public float3 Position;
        public float3 Rotation;

        //Weapons 
        public int currentammo;
        public int maxAmmo;
        public float reloadProgress;


        public Data(CharacterBase character )
        {
            UpdateData(character);
        }

        public void UpdateData(CharacterBase character)
        {
            this.isPlayer = character is Player;
            this.Position = character.position;
            this.Rotation = character.transform.eulerAngles;

            this.name = character.name;
            this.ID = character.ID;

            var controller = character.controller;

            if(controller == null)
            {
                return;
            }

            SetHealth(controller);

        }

        private void SetHealth(PlayerController controller)
        {
            health = controller.hitBox.currentHealth;
            maxHealth = controller.hitBox.maxHealth;

            stamina = controller.staminaHandler? controller.staminaHandler.current : 1;
            maxStamina = controller.staminaHandler? controller.staminaHandler.MaxStamina : 1;  
        }

        private void SetWeaponSystem(PlayerController controller)
        {
            if(controller.weaponSystem == null || controller.weaponSystem.current == null)
            {
                return;
            }
            var currentWeapon = controller.weaponSystem.current;

            currentammo = currentWeapon.currentAmmo;
            reloadProgress = currentWeapon.ReloadProgress();
        }
    }
}