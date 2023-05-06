using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponTakeController : ComponentController
{
    [Header("Weapon From Start")]
    [SerializeField] private Weapon weaponToTake;

    [Header("Attributes")]
    [SerializeField] private float dropWeaponForwardForce;
    [SerializeField] private LayerMask whatIsWeapon;

    [Header("Other")]
    [SerializeField] private Camera mainCam;
    [SerializeField] private Transform armsPos;

    private Transform currentArmsGO, currentBullet, currentHollowBullet, currentBulletHole;
    private IWeapon currentIWeapon;
    private Arm currentArm;

    private bool isWeaponTaken = false;

    private void Start()
    {
        if(isWeaponTaken == false)
        {
            if(weaponToTake != null)
            {
                TryTakeWeapon(weaponToTake);
            }
        }
    }

    public void TryShootWeapon(InputAction.CallbackContext obj)
    {
        if (isWeaponTaken)
        {
            // Check Ammo
            if (currentIWeapon.isAmmoEmpty()) 
                return;
            else 
                currentIWeapon.Shoot();// If ammo is not empty shoot
        }
    }

    public void TryTakeWeapon(InputAction.CallbackContext obj)
    {
        if(isWeaponTaken) { return; }

        if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out RaycastHit hit, 3f, whatIsWeapon))
        {
            if (!hit.transform.CompareTag("Weapon")) { return; }


            Weapon weapon = hit.transform.GetComponent<Weapon>();
            WeaponStats weaponStats = weapon.GetWeaponStats();
            WeaponSO.WeaponType weaponType = weaponStats.weaponSO.weaponType;
            this.currentHollowBullet = weaponStats.weaponSO.pf_HollowBullet;
            this.currentBullet = weaponStats.weaponSO.pf_Bullet;
            this.currentBulletHole = weaponStats.weaponSO.pf_BulletHole;

            switch (weaponType)
            {
                case WeaponSO.WeaponType.Pistol:
                    this.currentArm = armsController.GetArm("PistolArms");
                    this.currentArmsGO = Instantiate(currentArm.pf_Arm, armsPos.position, Quaternion.identity, armsPos);
                    this.currentIWeapon = currentArmsGO.GetComponent<IWeapon>();
                    this.currentIWeapon.SetAmmo(weaponStats.ammo);
                    this.currentIWeapon.SetHollowBulletPrefab(currentHollowBullet);
                    this.currentIWeapon.SetMainCamera(mainCam);
                    this.currentIWeapon.SetBulletHole(currentBulletHole);
                    this.currentIWeapon.SetBulletPrefab(currentBullet);
                    this.isWeaponTaken = true;
                    Destroy(hit.transform.gameObject);
                    break;
                case WeaponSO.WeaponType.Shotgun:

                    break;
            }

        }
    }

    private void TryTakeWeapon(Weapon weapon)
    {
        WeaponStats weaponStats = weapon.GetWeaponStats();
        WeaponSO.WeaponType weaponType = weaponStats.weaponSO.weaponType;
        this.currentHollowBullet = weaponStats.weaponSO.pf_HollowBullet;
        this.currentBullet = weaponStats.weaponSO.pf_Bullet;
        this.currentBulletHole = weaponStats.weaponSO.pf_BulletHole;

        switch (weaponType)
        {
            case WeaponSO.WeaponType.Pistol:
                this.currentArm = armsController.GetArm("PistolArms");
                this.currentArmsGO = Instantiate(currentArm.pf_Arm, armsPos.position, Quaternion.identity, armsPos);
                this.currentIWeapon = currentArmsGO.GetComponent<IWeapon>();
                this.currentIWeapon.SetAmmo(weaponStats.weaponSO.maxAmmo);
                this.currentIWeapon.SetHollowBulletPrefab(currentHollowBullet);
                this.currentIWeapon.SetMainCamera(mainCam);
                this.currentIWeapon.SetBulletHole(currentBulletHole);
                this.currentIWeapon.SetBulletPrefab(currentBullet);
                this.isWeaponTaken = true;
                break;
            case WeaponSO.WeaponType.Shotgun:

                break;
        }
    }

    /*private void DropWeapon()
    {
        if (isWeaponTaken)
        {
            //Debug.Log("Dropping Weapon...");

            Transform gun = Instantiate(currentArm.pf_Weapon, mainCam.transform.position + mainCam.transform.forward, Quaternion.identity, null);

            Weapon weapon = gun.GetComponent<Weapon>();
            weapon.SetWeaponAmmo(currentIWeapon.GetAmmo());

            Rigidbody gunRig = gun.GetComponent<Rigidbody>();
            gunRig.AddForce(rig.velocity + (mainCam.transform.forward + transform.up).normalized * dropWeaponForwardForce, ForceMode.VelocityChange);

            currentArm = null;
            currentIWeapon = null;
            currentBullet = null;
            currentBulletHole = null;
            currentHollowBullet = null;
            this.isWeaponTaken = false;
            Destroy(currentArmsGO.gameObject);
        }
        else { Debug.LogError("You don't have a weapon"); }
    }*/
}
