using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class idleController : ComponentController
{
    // Player Input
    private PlayerInputActions playerInputActions;

    [SerializeField] private Animator leftArmAnim;

    private float timeFromLastMove;
    private bool fourthActivated;
    private bool isDisabled;

    private new void Awake()
    {
        base.Awake();

        PlayerInputStart();
    }

    private void PlayerInputStart()
    {
        playerInputActions = new PlayerInputActions();

        playerInputActions.Player.Jump.performed += UpdateTimer;
        playerInputActions.Player.Crouch.performed += UpdateTimer;
        playerInputActions.Player.LegAttack.performed += UpdateTimer;
        playerInputActions.Player.WeaponTake.performed += UpdateTimer;
        playerInputActions.Player.Shoot.performed += UpdateTimer;

        playerInputActions.Player.Movement.performed += UpdateTimer;
        playerInputActions.Player.Movement.canceled += UpdateTimer;
        playerInputActions.Player.Camera.performed += UpdateTimer;
        playerInputActions.Player.Camera.canceled += UpdateTimer;

        playerInputActions.Player.Crouch.canceled += UpdateTimer;

        playerInputActions.UI.OpenCloseMenu.performed += UpdateTimer;
        playerInputActions.UI.RestartLevel.performed += UpdateTimer;
    }

    private void UpdateTimer(InputAction.CallbackContext obj)
    {
        timeFromLastMove = 0f;
        if (!isDisabled)
        {
            if (fourthActivated)
            {
                leftArmAnim.SetTrigger("turnTheMusicOff");
            }

            fourthActivated = false;
            isDisabled = true;
        }
    }

    private void Update()
    {
        timeFromLastMove += Time.unscaledDeltaTime;
        if(rig.velocity.magnitude > 1f)
        {
            UpdateTimer(new InputAction.CallbackContext());
        }
        if (timeFromLastMove > 30f)
        {
            if (!fourthActivated)
            {
                Debug.Log("Turn On Music");
                leftArmAnim.SetTrigger("turnTheMusicOn");
                fourthActivated = true;
                isDisabled = false;
            }
        }
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
