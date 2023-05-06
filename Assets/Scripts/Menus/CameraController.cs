using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    enum State
    {
        MainMenu,
        SettingsMenu,
        LevelsMenu
    }
    State state = State.MainMenu;

    [Header("Transitions")]
    [SerializeField] private Vector3 settingsMenuPos;
    [SerializeField] private Vector3 levelsMenuPos;
    private Vector3 mainMenuPos;
    private Vector3 targetPosition;
    private bool isMoving;
    public static CameraController Instance;

    [Header("Breathing")]
    [SerializeField] private float p_x_intensity;
    [SerializeField] private float p_y_intensity;
    [SerializeField] private float breathingPower = 4f;
    private Vector3 transformOrigin, targetPos;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        mainMenuPos = new Vector3(0f, 0f, -10f);
        targetPosition = mainMenuPos;
        transformOrigin = targetPosition;
        transform.position = mainMenuPos;
        state = State.MainMenu;
        isMoving = true;
    }

    private void Update()
    {
        if (isMoving)
        {
            if (Vector3.Distance(transform.position, targetPosition) > .001f)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 8f);
            }
            else
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
        else
        {
            HeadBob(p_x_intensity, p_y_intensity);
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * breathingPower);
        }
    }

    public void ToSettingsMenu()
    {
        state = State.SettingsMenu;
        isMoving = true;
        targetPosition = settingsMenuPos;
        transformOrigin = targetPosition;
    }

    public void ToLevelsMenu()
    {
        state = State.LevelsMenu;
        isMoving = true;
        targetPosition = levelsMenuPos;
        transformOrigin = targetPosition;
    }

    public void ToMainMenu()
    {
        state = State.MainMenu;
        isMoving = true;
        targetPosition = mainMenuPos;
        transformOrigin = targetPosition;
    }

    private void HeadBob(float p_x_intensity, float p_y_intensity)
    {
        targetPos = transformOrigin + new Vector3(Mathf.Cos(Time.time) * p_x_intensity, Mathf.Sin(Time.time * 2) * p_y_intensity, 0);
    }
}
