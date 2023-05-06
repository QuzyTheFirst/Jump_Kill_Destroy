using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableButton : MonoBehaviour, IInteractable
{
    public UnityEvent events;
    public Vector3 usedButtonPosition;
    public GameObject go_Light;
    public ParticleSystem particles;

    private bool isUsed = false;
    private AudioSource aud;

    private void Awake()
    {
        aud = GetComponent<AudioSource>();
    }

    public void Interact(object sender)
    {
        if (!isUsed)
        {
            transform.localPosition = usedButtonPosition;
            events?.Invoke();
            go_Light.SetActive(false);
            particles.Play();
            //aud.Play();
            SoundManager.Instance.Play("ButtonUse");
            isUsed = true;
        }
    }
}
