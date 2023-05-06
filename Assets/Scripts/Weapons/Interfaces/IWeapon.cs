using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    public void Shoot();

    public void SetAmmo(int ammo);
    public int GetAmmo();
    public bool isAmmoEmpty();

    public void SetBulletPrefab(Transform pf);

    public void SetHollowBulletPrefab(Transform pf);

    public void SetMainCamera(Camera camera);

    public void SetBulletHole(Transform pf);
    
}
