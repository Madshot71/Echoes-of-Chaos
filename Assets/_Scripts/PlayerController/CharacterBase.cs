using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    public PlayerController controller;
    public ControllerCamera playerCamera;
    public Vehicle currentVehicle;
    public Vector3 position => currentVehicle == null?
        controller.transform.position :
        currentVehicle.transform.position;

    private void Awake()
    {
        controller?.Init(this);
        playerCamera = controller.camController;
    }

    
}