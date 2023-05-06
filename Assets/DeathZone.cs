using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        IDamagable iDamagable = other.GetComponent<IDamagable>();
        if (iDamagable != null) 
        {
            if (other.CompareTag("Player"))
            {
                iDamagable.Damage(25f);
                CheckpointManager.Instance.SpawnAtLastCheckpoint();
            }
            else
            {
                iDamagable.Damage(9999f); 
            }
        }
    }
}
