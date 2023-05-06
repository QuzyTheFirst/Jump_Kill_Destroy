using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyZone : MonoBehaviour
{
    [SerializeField] private float upForce;

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rig = other.GetComponent<Rigidbody>();
        if(rig)
            rig.AddForce(transform.up * upForce * Time.deltaTime, ForceMode.VelocityChange);
    }
}
