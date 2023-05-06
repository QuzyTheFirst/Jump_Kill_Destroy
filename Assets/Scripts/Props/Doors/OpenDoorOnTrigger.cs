using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorOnTrigger : MonoBehaviour
{
    [SerializeField] DoorController doorController;

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            doorController.OpenDoor();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            doorController.CloseDoor();
        }
    }
}
