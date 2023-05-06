using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float forceMultiplier;
    [SerializeField] private bool makeOnlyYZero = true;
    [SerializeField] private bool makeEverythingZero = false;

    [Header("Effects")]
    [SerializeField] private ParticleSystem particles;

    private AudioSource aud;

    private void Start()
    {
        aud = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rig = other.GetComponent<Rigidbody>();

        if (rig)
        {
            if (makeOnlyYZero)
            {
                ZeroY(rig);
            }
            if(makeEverythingZero){
                VelocityIsZero(rig);
            }
            aud.Play();
            particles.Play();
        }
    }

    private void ZeroY(Rigidbody rig)
    {
        Vector3 zeroY = new Vector3(rig.velocity.x, 0f, rig.velocity.z);
        rig.velocity = zeroY;
        rig.AddForce(transform.up * forceMultiplier, ForceMode.VelocityChange);
    }

    private void VelocityIsZero(Rigidbody rig)
    {
        rig.velocity = Vector3.zero;
        rig.AddForce(transform.up * forceMultiplier, ForceMode.VelocityChange);
    }
}
