using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LegController : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int legDamage;

    [Header("Anim")]
    [SerializeField] private Animator legAnim;

    [Header("Forces")]
    [SerializeField] private float kickForce;
    [SerializeField] private float bulletPunchForce;

    [Header("Other")]
    [SerializeField] private Camera cam;
    [SerializeField] private PlayerController player;

    [Header("VFX")]
    [SerializeField] private ParticleSystem vfx_ShockWave;

    private bool attackedThisFrame;
    private Vector3 where;

    private void Update()
    {
        if (attackedThisFrame)
        {
            //vfx_ShockWave.transform.position = where;
            //vfx_ShockWave.transform.rotation = Quaternion.LookRotation(where - transform.position);
            vfx_ShockWave.Play(); 
            SoundManager.Instance.Play("LegPunch");
            attackedThisFrame = false; 
        }
    }

    public void PlaySound()
    {
        SoundManager.Instance.Play("LegMove");
    }

    public void LegAttack(InputAction.CallbackContext obj)
    {
        legAnim.SetTrigger("Attack");

    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
            return;

        IInteractable interactable = col.transform.GetComponent<IInteractable>();
        if(interactable != null)
        {
            interactable.Interact(this);
            attackedThisFrame = true;
            return;
        }

        Rigidbody rig = col.transform.GetComponent<Rigidbody>();
        if (!rig) { return; }

        BulletController bullet = col.transform.GetComponent<BulletController>();
        if (bullet)
        {
            Vector3 dir = cam.transform.forward;
            rig.velocity = dir * bulletPunchForce;
            attackedThisFrame = true;
            return;
        }

        PartOfEnemy partOfEnemy = col.transform.GetComponent<PartOfEnemy>();
        if (partOfEnemy)
        {
            partOfEnemy.LegPunch(legDamage);
        }

        Vector3 newVector = col.transform.position - cam.transform.position;
        newVector = new Vector3(newVector.x, 0, newVector.z).normalized;
        Vector3 kickDir = (cam.transform.forward + newVector).normalized;
        rig.AddForce(kickDir * kickForce, ForceMode.VelocityChange);

        attackedThisFrame = true;
        where = col.ClosestPointOnBounds(transform.position);
    }
}
