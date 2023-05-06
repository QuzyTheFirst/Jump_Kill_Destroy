using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHole : MonoBehaviour
{
    [Header("Clips")]
    [SerializeField] private AudioClip clip_BulletRicochet;
    [SerializeField] private AudioClip clip_BulletHit;
    private AudioSource aud;

    private void Awake()
    {
        aud = GetComponent<AudioSource>();

        int chance = Random.Range(1, 11);

        if(chance > 8)
        {
            aud.clip = clip_BulletRicochet;
        }
        else
        {
            aud.clip = clip_BulletHit;
        }
        aud.Play();
    }
}
