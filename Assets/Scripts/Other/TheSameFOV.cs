using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheSameFOV : MonoBehaviour
{
    [SerializeField] Camera otherCam;

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        cam.fieldOfView = otherCam.fieldOfView; 
    }
}
