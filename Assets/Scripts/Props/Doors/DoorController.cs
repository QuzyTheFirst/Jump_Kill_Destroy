using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DoorController : MonoBehaviour
{
    enum State
    {
        Opened,
        Closed
    }
    [SerializeField] State state = State.Closed;
    [SerializeField] private float doorMovingSpeed = 8f;
    [SerializeField] private Transform tf_Door;
    [SerializeField] private Vector3 openedPosition = new Vector3(0f, 6f, 0f);
    private Vector3 closedPosition;
    private Vector3 targetPosition;

    [Header("Keys")]
    [SerializeField] private bool openWithSeveralKeys;
    [SerializeField] private int howManyKeysToOpen;
    private int keysCollected = 0;


    private AudioSource aud;

    private void Awake()
    {
        closedPosition = tf_Door.localPosition;
        openedPosition += closedPosition;
    }

    private void Start()
    {
        if(state == State.Closed)
        {
            targetPosition = closedPosition;
        }
        else
        {
            targetPosition = openedPosition;
        }
        aud = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(Vector3.Distance(tf_Door.localPosition, targetPosition) > .01f)
        {
            tf_Door.localPosition = Vector3.Lerp(tf_Door.localPosition, targetPosition, Time.deltaTime * doorMovingSpeed);
        }
        else
        {
            tf_Door.localPosition = targetPosition;
        }
    }

    public void OpenDoor()
    {
        if (openWithSeveralKeys)
        {
            keysCollected++;
            if(keysCollected == howManyKeysToOpen)
            {
                if (targetPosition != openedPosition)
                {
                    aud.Play();
                    targetPosition = openedPosition;
                }
            }
        }
        else
        {
            if (targetPosition != openedPosition)
            {
                aud.Play();
                targetPosition = openedPosition;
            }
        }
    }

    public void CloseDoor()
    {
        if (targetPosition != closedPosition)
        {
            aud.Play();
            targetPosition = closedPosition;
        }
    }
}
