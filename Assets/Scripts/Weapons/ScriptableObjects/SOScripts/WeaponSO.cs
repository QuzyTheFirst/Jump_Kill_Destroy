using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New_Weapon_OS", menuName = "ScriptableObjects")]
public class WeaponSO : ScriptableObject
{
    public enum WeaponType
    {
        Pistol,
        Shotgun,
    }
    public WeaponType weaponType;

    public Transform pf_Bullet, pf_HollowBullet, pf_BulletHole;

    public int maxAmmo;

}
