using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponStats 
{
    public WeaponStats(int ammo, WeaponSO weaponSO)
    {
        this.ammo = ammo;
        this.weaponSO = weaponSO;
    }

    public int ammo;
    public WeaponSO weaponSO;
}


public class Weapon : MonoBehaviour
{
    public UnityEvent OnWeaponTake;

    [SerializeField] private WeaponSO weaponSO;
    private int ammo;
    private void Awake()
    {
        this.ammo = weaponSO.maxAmmo;
    }

    public WeaponStats GetWeaponStats()
    {
        OnWeaponTake?.Invoke();
        return new WeaponStats(ammo, weaponSO);
    }

    public void SetWeaponAmmo(int ammo)
    {
        this.ammo = ammo;
    }
}
