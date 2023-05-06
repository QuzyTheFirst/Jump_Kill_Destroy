using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentController : MonoBehaviour
{
    // Components
    protected Rigidbody rig;
    protected PlayerCameraController playerLookController;
    protected PlayerController playerController;
    protected ArmsController armsController;
    protected LegController legController;
    protected WeaponTakeController weaponTakeController;
    protected PlayerInputController playerInputController;

    protected void Awake()
    {
        GetComponents();
    }

    private void GetComponents()
    {
        rig = GetComponent<Rigidbody>();
        playerLookController = GetComponent<PlayerCameraController>();
        playerController = GetComponent<PlayerController>();
        armsController = GetComponent<ArmsController>();
        legController = GetComponentInChildren<LegController>();
        weaponTakeController = GetComponent<WeaponTakeController>();
        playerInputController = GetComponent<PlayerInputController>();
    }
}
