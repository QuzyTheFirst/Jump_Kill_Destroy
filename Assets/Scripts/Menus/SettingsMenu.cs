using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] TextMeshProUGUI TMP_Sensitivity;
    private float sensitivity;

    private void Start()
    {
        sensitivity = PlayerPrefs.GetFloat("Sensitivity", 3);
        sensitivitySlider.value = sensitivity;
        TMP_Sensitivity.text = sensitivity.ToString("0.00");
    }

    public void ChangeSensitivity(float value)
    {
        sensitivity = value;
        TMP_Sensitivity.text = value.ToString("0.00");
    }

    public void SaveValues()
    {
        PlayerPrefs.SetFloat("Sensitivity", sensitivity);
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
        CameraController.Instance.ToMainMenu();
        UpdateValues();
        SoundManager.Instance.Play("Swish");
    }
}
