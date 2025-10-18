using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Player player;
    private PlayerInputActions inputActions;


    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Enable();
        SetUp();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    } 

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        Move(inputActions.PlayerController.Movement.ReadValue<Vector2>());
        Jump(inputActions.PlayerController.Jump.IsPressed());
        CrouchToggle(inputActions.PlayerController.Crouch.IsPressed());
        Sprint(inputActions.PlayerController.Sprint.IsPressed());
        Slide(inputActions.PlayerController.Slide.IsPressed());

    }

    private void SetUp()
    {
        inputActions.PlayerController.View.started += View;
    }

    private void Jump(bool value)
    {
        player.controller.receiver.jumpInput = value;
    }

    private void Move(Vector2 input)
    {
        player.controller.receiver.movementInput = input;
    }

    private void CrouchToggle(bool value)
    {
        player.controller.receiver.crouchInput = value;
    }

    private void Sprint(bool value)
    {
        player.controller.receiver.sprintInput = value;
    }

    private void Slide(bool value)
    {
        player.controller.receiver.slideInput = value;
    }

    private void View(InputAction.CallbackContext context)
    {
        player.playerCamera.TogglePov();
    }
    
}
