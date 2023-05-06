using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerCameraController : ComponentController
{
    [Header("Sensitivity")]
    [SerializeField] private float sensitivity;

    [Header("FOV")]
    [SerializeField] private float FovTimeChangeMultiplier;

    [Header("Tilt Degrees")]
    [SerializeField] private float crouchCameraTiltDegree = 8f;

    [Header("Other")]
    [SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private Transform playerBody;

    private const float STANDART_FOV_VALUE = 60f;
    private const float MAX_FOV_VALUE = 80f;
    private float differenceBetweenFovs;

    private float cameraZAxisValue;
    private float cameraZAxisTargetValue;
    private float xRotation = 0f;

    private CinemachineBasicMultiChannelPerlin perlin;

    private Vector3 targetOffset = Vector3.zero;
    private Vector3 originalOffset = Vector3.zero;

    private void Start(){
        sensitivity = PlayerPrefs.GetFloat("Sensitivity", 3);
        Cursor.lockState = CursorLockMode.Locked;
        cameraZAxisValue = 0;
        differenceBetweenFovs = MAX_FOV_VALUE - STANDART_FOV_VALUE;
        perlin = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        originalOffset = cam.transform.localPosition;
    }
    private void LateUpdate()
    {
        CameraRotation();

        FindFov();

        if (cameraZAxisValue != cameraZAxisTargetValue)
            ChangeCameraZAxis();

        CameraOffset();

        
        FindCameraParams(playerController.state);
    }

    private void CameraOffset()
    {
        if (targetOffset != Vector3.zero)
        {
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, originalOffset + targetOffset, Time.deltaTime * 6f);

            targetOffset = Vector3.Lerp(targetOffset, Vector3.zero, Time.deltaTime * 6f);

            if (Vector3.Distance(targetOffset, Vector3.zero) < .001f)
            {
                targetOffset = Vector3.zero;
                cam.transform.localPosition = originalOffset;
            }
        }
    }

    public void SetTargetOffset(float offset)
    {
        targetOffset = new Vector3(0f, -offset, 0f);
    }

    private void CameraRotation()
    {
        float mouseX = PlayerInputController.mouseInputX * Time.deltaTime * sensitivity;
        float mouseY = PlayerInputController.mouseInputY * Time.deltaTime * sensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

        cam.m_Lens.Dutch = cameraZAxisValue;
    }

    private void FindFov()
    {
        float dot_LookInVelocityDir = Vector3.Dot(cam.transform.forward.normalized, rig.velocity.normalized);
        dot_LookInVelocityDir = Mathf.Clamp01(dot_LookInVelocityDir);

        float targetFov = STANDART_FOV_VALUE + differenceBetweenFovs * Mathf.InverseLerp(7, playerController.GetMaxSpeed() - 5, rig.velocity.magnitude) * dot_LookInVelocityDir;
        cam.m_Lens.FieldOfView = Mathf.Lerp(cam.m_Lens.FieldOfView, targetFov, Time.deltaTime * FovTimeChangeMultiplier);
    }

    private void ChangeCameraZAxis(){
        cameraZAxisValue = Mathf.Lerp(cameraZAxisValue, cameraZAxisTargetValue, Time.deltaTime * 8f);

        float difference = cameraZAxisTargetValue - cameraZAxisValue;
        if(difference <= .1f && difference >= -.1f){
            cameraZAxisValue = cameraZAxisTargetValue;
        }
    }

    public void UpdateSensitivity()
    {
        sensitivity = PlayerPrefs.GetFloat("Sensitivity", 3);
    }

    private void FindCameraParams(PlayerController.State state)
    {
        switch (state)
        {
            case PlayerController.State.Walk:
                if (playerController.GetIsGrounded())
                {
                    if (PlayerInputController.moveInputX == -1)
                    {
                        SetTargetValue(2f);
                    }
                    else if (PlayerInputController.moveInputX == 1)
                    {
                        SetTargetValue(-2f);
                    }
                    else
                    {
                        SetTargetValue(0f);
                    }
                }
                else
                {
                    SetTargetValue(0f);
                }
                break;
            case PlayerController.State.Wallrun:
                if (playerController.IsARightWall)
                {
                    SetTargetValue(12.5f);
                }
                else
                {
                    SetTargetValue(-12.5f);
                }
                break;
            case PlayerController.State.Crouch:
                if (PlayerInputController.moveInputX == -1)
                {
                    SetTargetValue(crouchCameraTiltDegree + 2);
                }
                else if (PlayerInputController.moveInputX == 1)
                {
                    SetTargetValue(crouchCameraTiltDegree - 2f);
                }
                else
                {
                    SetTargetValue(crouchCameraTiltDegree);
                }
                break;
            case PlayerController.State.Slide:
                /*if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 1f, groundLayerMask))
                {
                    SetTargetValue(crouchCameraTiltDegree);
                }
                else
                {
                    SetTargetValue(crouchCameraTiltDegree * .5f);
                }*/
                if (playerController.GetIsGrounded())
                {
                    SetTargetValue(crouchCameraTiltDegree);
                }
                else
                {
                    SetTargetValue(crouchCameraTiltDegree * .5f);
                }
                break;
        }
    }

        public void SetTargetValue(float value){
        cameraZAxisTargetValue = value;
    }

    public CinemachineVirtualCamera GetCamera(){
        return cam;
    }

    public void SetGains(float amplitudeGain = 0f, float frequencyGain = 0f)
    {
        perlin.m_AmplitudeGain = amplitudeGain;
        perlin.m_FrequencyGain = frequencyGain;
    }
}
