using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    public new string name;
    public Sprite characterSprite;
    internal PlayerController controller;
    internal ControllerCamera playerCamera;
    public Vehicle currentVehicle;
    public Vector3 position => currentVehicle == null?
        controller.transform.position :
        currentVehicle.transform.position;

    private void OnValidate()
    {
        controller?.Init(this);
    }

    
}