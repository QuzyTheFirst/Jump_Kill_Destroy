using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : ComponentController
{
    // Player Input
    private PlayerInputActions playerInputActions;

    public static float moveInputX, moveInputY;
    public static float mouseInputX, mouseInputY;

    public static Vector3 playerPosition;

    private void Update()
    {
        playerPosition = transform.position;
    }

    private new void Awake()
    {
        base.Awake();

        PlayerInputStart();
    }
    private void PlayerInputStart()
    {
        playerInputActions = new PlayerInputActions();

        playerInputActions.Player.Jump.performed += playerController.CheckJump;
        playerInputActions.Player.Crouch.performed += playerController.CheckCrouch;
        playerInputActions.Player.LegAttack.performed += legController.LegAttack;
        playerInputActions.Player.WeaponTake.performed += weaponTakeController.TryTakeWeapon;
        //playerInputActions.Player.WeaponDrop.performed += weaponTakeController.TryDropWeapon;
        playerInputActions.Player.Shoot.performed += weaponTakeController.TryShootWeapon;

        playerInputActions.Player.Movement.performed += NewMoveInput;
        playerInputActions.Player.Movement.canceled += StopMoveInput;
        playerInputActions.Player.Camera.performed += NewMouseInput;
        playerInputActions.Player.Camera.canceled += StopMouseInput;

        //playerInputActions.Player.Sprint.started += playerController.StartSprint;
        //playerInputActions.Player.Sprint.canceled += playerController.StopSprint;
        playerInputActions.Player.Crouch.canceled += playerController.StopCrouch;
    }

    private void StopMoveInput(InputAction.CallbackContext obj)
    {
        moveInputX = 0f;
        moveInputY = 0f;
    }

    private void StopMouseInput(InputAction.CallbackContext obj)
    {
        mouseInputX = 0f;
        mouseInputY = 0f;
    }

    private void NewMouseInput(InputAction.CallbackContext obj)
    {
        Vector2 input = obj.ReadValue<Vector2>();
        mouseInputX = input.x;
        mouseInputY = input.y;
    }

    private void NewMoveInput(InputAction.CallbackContext obj)
    {
        Vector2 input = obj.ReadValue<Vector2>();
        moveInputX = input.x;
        moveInputY = input.y;
    }

    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
    }
}
