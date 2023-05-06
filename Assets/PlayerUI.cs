using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject EscMenu;
    [SerializeField] private GameObject SettingsMenu;
    [SerializeField] private TextMeshProUGUI TMP_Sensitivity;
    [SerializeField] private Slider sensitivitySlider;

    private PlayerCameraController playerCamController;

    // Player Input
    private PlayerInputActions playerInputActions;
    private bool inMenu;
    private float sensitivity;

    public static PlayerUI Instance;
    private void Awake()
    {
        sensitivity = PlayerPrefs.GetFloat("Sensitivity", 3);
        sensitivitySlider.value = sensitivity;
        TMP_Sensitivity.text = sensitivity.ToString("0.00");

        playerCamController = FindObjectOfType<PlayerCameraController>();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Time.timeScale = 1;

        playerInputActions = new PlayerInputActions();

        playerInputActions.UI.OpenCloseMenu.performed += OpenCloseMenu_performed;
        playerInputActions.UI.RestartLevel.performed += RestartLevel_performed;
    }

    private void RestartLevel_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (inMenu)
        {
            SoundManager.Instance.Play("Select");
            Restart();
        }
    }

    private void OpenCloseMenu_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        bool isMenuActivated = !EscMenu.activeInHierarchy;
        EscMenu.SetActive(isMenuActivated);
        if (isMenuActivated == true)
        {
            Cursor.lockState = CursorLockMode.None;
            SoundManager.Instance.FadeInVolume("ComputerWorking", .4f, .5f);
            Time.timeScale = 0;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            SoundManager.Instance.FadeAwayVolume("ComputerWorking", .5f);
            Time.timeScale = 1;
        }
        inMenu = isMenuActivated;
    }

    public void Continue()
    {
        EscMenu.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        SoundManager.Instance.Play("Select");
        SoundManager.Instance.FadeAwayVolume("ComputerWorking", .5f);
    }
    public void Restart()
    {
        SoundManager.Instance.Play("Select");
        StartCoroutine(GoToLevelIn(SceneManager.GetActiveScene().buildIndex, .5f));
    }
    public void Settings()
    {
        SoundManager.Instance.Play("Select");
        SettingsMenu.SetActive(true);
    }
    public void ToMainMenu()
    {
        SoundManager.Instance.Play("Select");
        StartCoroutine(GoToLevelIn(0, .5f));
    }
    public void Exit()
    {
        SoundManager.Instance.Stop("ComputerWorking");
        SoundManager.Instance.Stop("ComputerStartUp");
        SoundManager.Instance.Stop("Shot");
        SoundManager.Instance.Play("ComputerShutDown");
        SceneTransition.Instance.BlackScreenAppearance();
        StartCoroutine(ExitGame());
    }
    IEnumerator ExitGame()
    {
        yield return new WaitForSecondsRealtime(1f);
        Debug.Log("Exit");
        Application.Quit();
    }

    IEnumerator GoToLevelIn(int level, float time)
    {
        SceneTransition.Instance.PlayEndLevelTransition();
        yield return new WaitForSecondsRealtime(time);
        SceneManager.LoadScene(level);
    }

    public void ChangeSensitivity(float value)
    {
        sensitivity = value;
        TMP_Sensitivity.text = value.ToString("0.00");
    }

    public void SaveValues()
    {
        SoundManager.Instance.Play("Select");
        PlayerPrefs.SetFloat("Sensitivity", sensitivity);
        playerCamController.UpdateSensitivity();
    }

    private void UpdateValues()
    {
        // Sensitivity
        sensitivity = PlayerPrefs.GetFloat("Sensitivity", 3);
        sensitivitySlider.value = sensitivity;
        TMP_Sensitivity.text = sensitivity.ToString("0.00");
    }

    public void GoBack()
    {
        SoundManager.Instance.Play("Select");
        SettingsMenu.SetActive(false);
        UpdateValues();
    }

    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
    }
}
