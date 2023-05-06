using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private float damage;

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }
    private void OnCollisionEnter(Collision collision)
    {
        IDamagable somethingToDamage = collision.transform.GetComponent<IDamagable>();
        if (somethingToDamage != null)
        {
            somethingToDamage.Damage(damage);
        }

        Destroy(gameObject);
    }
}
