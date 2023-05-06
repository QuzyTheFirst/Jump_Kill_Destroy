using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepellantController : MonoBehaviour, IInteractable
{
    [SerializeField] private ObjectPusher objectPusher;
    [SerializeField] private float pushPower = 25f;
    [SerializeField] private int objectsToPushPerFrame = 5;
    [SerializeField] private bool nullifyVelocity = false;
    [SerializeField] private ParticleSystem particles;

    private AudioSource aud;
    private void Start()
    {
        aud = GetComponent<AudioSource>();
    }

    public void Interact(object sender)
    {
        objectPusher.Push(pushPower, objectsToPushPerFrame, nullifyVelocity);
        particles.Play();
        //aud.Play();
        SoundManager.Instance.Play("Repellant");
    }
}
