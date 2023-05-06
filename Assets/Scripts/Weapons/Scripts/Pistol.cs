using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : MonoBehaviour, IWeapon
{
    [SerializeField] Transform hollowBulletCreator;
    [SerializeField] Transform shootPos;
    [SerializeField] private float damage;
    [SerializeField] private float bulletEjectionPower;
    [SerializeField] private float bulletPushPower;
    [SerializeField] private float startAnimationTime;
    [SerializeField] private float shootAnimationTime;

    [SerializeField] LayerMask layerMask;

    [Header("Particles")]
    [SerializeField] private ParticleSystem vfx_MuzzleFlash;
    [SerializeField] private ParticleSystem vfx_Hit;

    private Animator anim;

    private int ammo;

    private bool isTimeAfterAnimationPassed;

    private Transform pf_HollowBullet, pf_BulletHole, pf_Bullet;

    private Camera cam;

    private void Start()
    {
        anim = GetComponent<Animator>();
        isTimeAfterAnimationPassed = false;
        StartCoroutine(WaitAnimationTime(startAnimationTime));
    }

    public void Shoot()
    {
        if (isTimeAfterAnimationPassed)
        {
            SoundManager.Instance.Play("Shot");
            anim.SetTrigger("Shoot");
            ammo--;

            SpawnHollowBullet();

            if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                IInteractable interactable = hit.transform.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact(this);
                    //SoundManager.Instance.Play("Hit");
                }

                PartOfEnemy part = hit.transform.GetComponent<PartOfEnemy>();
                if (part)
                {
                    part.Damage(damage);
                    SoundManager.Instance.Play("Hit");
                }

                Rigidbody rig = hit.transform.GetComponent<Rigidbody>();
                if (rig)
                {
                    rig.AddForceAtPosition(cam.transform.forward * bulletPushPower, hit.point, ForceMode.VelocityChange);
                    //SoundManager.Instance.Play("Hit");
                }
                else
                {
                    Transform bulletHole = Instantiate(pf_BulletHole, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(hit.normal));
                    bulletHole.SetParent(hit.transform);
                }

                vfx_Hit.transform.position = hit.point;
                vfx_Hit.transform.rotation = Quaternion.LookRotation(cam.transform.forward);
                vfx_Hit.Play();
            }
            else
            {
                SpawnBullet();
            }

            vfx_MuzzleFlash.Play();

            isTimeAfterAnimationPassed = false;
            StartCoroutine(WaitAnimationTime(shootAnimationTime));
        }
    }

    private void SpawnHollowBullet()
    {
        if (pf_HollowBullet && hollowBulletCreator)
        {
            Transform bullet = Instantiate(pf_HollowBullet, hollowBulletCreator.position, Quaternion.identity, null);
            Rigidbody bulletRig = bullet.GetComponent<Rigidbody>();
            bulletRig.velocity = (transform.up + transform.right).normalized * bulletEjectionPower;
        }
    }

    private void SpawnBullet()
    {
        if(pf_Bullet)
            Instantiate(pf_Bullet, shootPos.position, Quaternion.identity, null);
    }

    public bool isAmmoEmpty()
    {
        if (ammo <= 0) { return true; }
        else return false;
    }

    IEnumerator WaitAnimationTime(float time)
    {
        yield return new WaitForSeconds(time);
        isTimeAfterAnimationPassed = true;
    }

    public void SetAmmo(int ammo)
    {
        this.ammo = ammo;
    }

    public int GetAmmo()
    {
        return this.ammo;
    }

    public void SetBulletPrefab(Transform pf_Bullet)
    {
        this.pf_Bullet = pf_Bullet;
    }

    public void SetHollowBulletPrefab(Transform pf_HollowBullet)
    {
        this.pf_HollowBullet = pf_HollowBullet;
    }

    public void SetMainCamera(Camera cam)
    {
        this.cam = cam;
    }

    public void SetBulletHole(Transform pf_BulletHole)
    {
        this.pf_BulletHole = pf_BulletHole;
    }
}
