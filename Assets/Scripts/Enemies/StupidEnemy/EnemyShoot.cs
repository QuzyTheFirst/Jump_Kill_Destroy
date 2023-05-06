using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    [Header("Transforms")]
    [SerializeField] private Transform tf_shootPos;
    [SerializeField] private Transform pf_bullet;

    [Header("Values")]
    [SerializeField] private float bulletsPerSecond;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletDamage;
    [SerializeField] private float recoilPower;
    private float shootNormalizedTime;
    private float lastTimeShoot;
    private float nextShotTime;
    [SerializeField] private float timeBeforeAttack = .2f;
    private float timeSeeingPlayer = 0f;
    [SerializeField] private float DistanceForSeeingPlayer;

    [Header("Components")]
    [SerializeField] private Rigidbody lowerArmRig;
    [SerializeField] private Rigidbody UpperArmRig;
    private AudioSource aud;

    [Header("Layers")]
    [SerializeField] private LayerMask whatIsShootable;



    private void OnValidate()
    {
        shootNormalizedTime = 1f / bulletsPerSecond;
    }

    private void Start()
    {
        aud = tf_shootPos.GetComponent<AudioSource>();
        shootNormalizedTime = 1f / bulletsPerSecond;
        nextShotTime = Time.time;
    }

    public void TryToShoot(Transform target)
    {
        if(Time.time > nextShotTime)
        {
            if(Physics.Raycast(tf_shootPos.position, target.position - tf_shootPos.position, out RaycastHit hit, DistanceForSeeingPlayer, whatIsShootable))
            {
                //Debug.Log(hit.transform);
                //Debug.Log(timeSeeingPlayer);
                if (hit.transform == target)
                {
                    timeSeeingPlayer += Time.deltaTime;
                    if (timeSeeingPlayer > timeBeforeAttack)
                    {
                        Shoot(target.position);
                        aud.Play();
                        nextShotTime = Time.time + shootNormalizedTime;
                    }
                }
                else
                {
                    timeSeeingPlayer = 0f;
                }
            }
        }
    }

    private void Shoot(Vector3 where)
    {
        BulletController bullet = Instantiate(pf_bullet, tf_shootPos.position, Quaternion.LookRotation(where - tf_shootPos.position), null).GetComponent<BulletController>();
        bullet.SetDamage(bulletDamage);

        Vector3 shootDir = (where - bullet.transform.position).normalized;
        Rigidbody rig = bullet.GetComponent<Rigidbody>();
        rig.velocity = shootDir * bulletSpeed;

        Vector3 recoilDir = (lowerArmRig.transform.position - where).normalized;
        lowerArmRig.AddForce(recoilDir * recoilPower, ForceMode.VelocityChange);
        UpperArmRig.AddForce(recoilDir * recoilPower, ForceMode.VelocityChange);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, DistanceForSeeingPlayer);
    }
}
