using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorUnlocker : MonoBehaviour
{
    public UnityEvent OnEnter;

    public UnityEvent OnExit;

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("DoorUnlocker"))
        {
            OnEnter?.Invoke();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("DoorUnlocker"))
        {
            OnExit?.Invoke();
        }
    }
}
