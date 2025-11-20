using System.Collections.Generic;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

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
    // Data Handling


    [System.Serializable]
    public struct Data
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


        public Data(CharacterBase character , PlayerController controller)
        {
            this.isPlayer = character is Player;
            this.Position = character.position;
            this.Rotation = character.transform.eulerAngles;

            health = controller.hitBox.currentHealth;
            maxHealth = controller.hitBox.maxHealth;

            stamina = controller.staminaHandler? controller.staminaHandler.current : 1;
            maxStamina = controller.staminaHandler? controller.staminaHandler.MaxStamina : 1;

            this.name = character.name;
            this.ID = character.ID;


            // Controller Data
        }

    }

    
}