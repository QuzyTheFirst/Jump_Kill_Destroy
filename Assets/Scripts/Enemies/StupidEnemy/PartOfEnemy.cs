using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartOfEnemy : MonoBehaviour, IDamagable
{
    public string partName;
    public bool isBreakable;
    public ConfigurableJoint connectedJoint;

    public event EventHandler<DamageClass> OnPartDamaged;
    public event EventHandler<DamageClass> OnLegPunch;

    private ConfigurableJoint joint;

    private void Start()
    {
        joint = GetComponent<ConfigurableJoint>();
    }

    public void DeactivateJoint()
    {
        if (connectedJoint)
        {
            connectedJoint.breakForce = 0f;
        }
        else
        {
            if (joint && isBreakable)
                joint.breakForce = 0f;
        }
    }

    public void Damage(float damageValue = 0)
    {
        OnPartDamaged?.Invoke(this, new DamageClass { damage = damageValue });
    }

    public void LegPunch(int damageValue)
    {
        OnLegPunch?.Invoke(this, new DamageClass { damage = damageValue });

    }
}

public class DamageClass
{
    public float damage;
    public ConfigurableJoint joint;
}
