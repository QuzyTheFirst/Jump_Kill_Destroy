using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private static bool firstTimeHere = true;
    private void Awake()
    {
        Time.timeScale = 1;
    }
    private void Start()
    {
        SceneTransition.Instance.PlayStartLevelTransition();
        Cursor.lockState = CursorLockMode.None;
        if (firstTimeHere)
        {
            SoundManager.Instance.ChangeSoundVolume("ComputerWorking", .4f);
            SoundManager.Instance.Play("ComputerStartUp");
            StartCoroutine(PlayTrackAfterSeconds("ComputerWorking", 7f));
            firstTimeHere = false;
        }
        else
        {
            StopAllCoroutines();
            SoundManager.Instance.ChangeSoundVolume("ComputerWorking", .4f);
            SoundManager.Instance.Play("ComputerWorking");
        }
    }

    IEnumerator PlayTrackAfterSeconds(string trackName, float time)
    {
        yield return new WaitForSeconds(time);
        SoundManager.Instance.Play(trackName);
    }
    public void PlayButton()
    {
        SoundManager.Instance.Play("Play");
        PlayerPrefs.SetFloat("Level1", 0f);
        PlayerPrefs.SetFloat("Level2", 0f);
        PlayerPrefs.SetFloat("Level3", 0f);
        PlayerPrefs.SetFloat("Level4", 0f);
        PlayerPrefs.SetFloat("Level5", 0f);
        PlayerPrefs.SetFloat("Level6", 0f);
        PlayerPrefs.SetFloat("Level7", 0f);
        StartCoroutine(GoToFirstLevel(.5f));
    }
    IEnumerator GoToFirstLevel(float time)
    {
        SceneTransition.Instance.PlayEndLevelTransition();
        yield return new WaitForSecondsRealtime(time);
        SceneManager.LoadScene(1);
    }

    public void ChooseLevelButton()
    {
        SoundManager.Instance.Play("Swish");
        CameraController.Instance.ToLevelsMenu();
    }

    public void SettingsButton()
    {
        SoundManager.Instance.Play("Swish");
        CameraController.Instance.ToSettingsMenu();
    }

    public void ExitButton()
    {
        SoundManager.Instance.Stop("ComputerWorking");
        SoundManager.Instance.Stop("ComputerStartUp");
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
}
