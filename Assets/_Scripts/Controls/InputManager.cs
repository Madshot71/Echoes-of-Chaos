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
        Move(inputActions.PlayerController.Movement.ReadValue<Vector2>());
        Jump(inputActions.PlayerController.Jump.IsPressed());
        CrouchToggle(inputActions.PlayerController.Crouch.IsPressed());
        Sprint(inputActions.PlayerController.Sprint.IsPressed());
        Slide(inputActions.PlayerController.Slide.IsPressed());
        Aim(inputActions.WeaponGun.Aim.IsPressed());

        player.playerCamera.input = inputActions.PlayerController.Camera.ReadValue<Vector2>();
        OnWeaponSwitch();
    }

    private void SetUp()
    {
        inputActions.PlayerController.View.started += View;
        inputActions.PlayerController.Interact.started += Interact;

        inputActions.WeaponGun.Shoot.performed += Shoot;
        inputActions.WeaponGun.Reload.performed += Reload;
        inputActions.WeaponGun.Switch.performed += ScrollSwitch;
        inputActions.WeaponGun.Holster.started += HolsterToggle;
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

    private void Pause(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            GameSceneManager.instance.Pause();
        }
    }


    #region Weapon Inputs
    private void Shoot(InputAction.CallbackContext context)
    {
        player.controller.weaponSystem?.Attack();
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