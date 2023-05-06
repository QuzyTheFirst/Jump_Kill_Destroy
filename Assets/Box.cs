using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField] private AudioSource aud;

    private void Awake()
    {
        aud = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.relativeVelocity.magnitude > .5f)
        {
            aud.volume = 1f * Mathf.InverseLerp(.5f, 15f, collision.relativeVelocity.magnitude);
            if (!aud.isPlaying)
            {
                aud.Play();
            }
        }
    }
}
