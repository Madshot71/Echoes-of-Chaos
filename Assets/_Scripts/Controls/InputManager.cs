using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Player player;
    private PlayerInputActions inputActions;
    private PlayerController.InputReceiver receiver = new();

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Enable();
        SetUp();

        if(player == null)
        {
            Debug.LogError($"{player} is null");
            enabled = false;
        }
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
        receiver.movementInput = inputActions.PlayerController.Movement.ReadValue<Vector2>();
        
        receiver.sprintInput = inputActions.PlayerController.Sprint.IsPressed();
        

        if(player != null)
        {
            //Passing all
            player.ControllerInputs(receiver);
        }

        Aim(inputActions.WeaponGun.Aim.IsPressed());
        Shoot(inputActions.WeaponGun.Shoot.IsPressed());
        
        player.playerCamera.input = inputActions.PlayerController.Camera.ReadValue<Vector2>();
        OnWeaponSwitch();
    }

    private void SetUp()
    {
        inputActions.PlayerController.View.started += View;
        inputActions.PlayerController.Interact.started += Interact;

        //Jump
        inputActions.PlayerController.Jump.started += i => receiver.jumpInput = true;
        inputActions.PlayerController.Jump.canceled += i => receiver.jumpInput = false;

        //Slide
        inputActions.PlayerController.Slide.started += i => receiver.slideInput = true;
        inputActions.PlayerController.Slide.canceled += i => receiver.slideInput = false;

        //Crouch
        inputActions.PlayerController.Crouch.started += i => receiver.crouchInput = true;
        inputActions.PlayerController.Crouch.canceled += i => receiver.crouchInput = false;


        inputActions.WeaponGun.Reload.performed += Reload;
        inputActions.WeaponGun.Switch.performed += ScrollSwitch;
        inputActions.WeaponGun.Holster.started += HolsterToggle;
        inputActions.WeaponGun.ToggleAimView.started += ToggleAimView;

    }

    private void View(InputAction.CallbackContext context)
    {
        player.playerCamera.TogglePov();
    }

    private void Pause(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            GameSceneManager.instance.Pause();
        }
    }

    private void PeakLeft()
    {
        if(player.controller.weaponSystem == null)
        {
            return;
        }
        player.controller.weaponSystem.peakDirection = -1;
    }

    private void PeakRight()
    {
        if(player.controller.weaponSystem == null)
        {
            return;
        }
        player.controller.weaponSystem.peakDirection = 1;
    }

    private void ToggleAimView(InputAction.CallbackContext context)
    {
        player.playerCamera.ToggleAimView();
    }


    #region Weapon Inputs
    private void Shoot(bool value)
    {
        player.controller.weaponSystem?.Attack(value);
    }

    private void Reload(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            player.controller.weaponSystem?.Reload();
        }
    }

    private void Aim(bool value)
    {
        player.controller.weaponSystem?.AimToggle(value);
        player.playerCamera?.AimToggle(value);
    }

    private void ScrollSwitch(InputAction.CallbackContext context)
    {
        player.controller.weaponSystem?.Switch(context.ReadValue<float>());
    }

    private void HolsterToggle(InputAction.CallbackContext context)
    {
        player.controller.weaponSystem?.HolsterToggle();
    }

    private void OnWeaponSwitch()
    {
        // Convert the key pressed to a number (1-9)
        if (Keyboard.current == null) return;

        for (int i = 1; i <= 3; i++)
        {
            if (Keyboard.current[Key.Digit1 + i - 1].wasPressedThisFrame)
            {
                Debug.Log($"Weapon {i} selected");
                Switch(i);
                break;
            }
        }
    }
    
    private void Interact(InputAction.CallbackContext context)
    {
        player.Interact();
    }

    private void Switch(float index)
    {
        // Call your weapon switching logic here
        player.controller.weaponSystem?.Switch((int)index - 1);
    }
    
    #endregion
}