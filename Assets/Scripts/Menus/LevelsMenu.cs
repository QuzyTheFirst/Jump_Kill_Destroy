using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsMenu : MonoBehaviour
{
    public void GoBack()
    {
        SoundManager.Instance.Play("Swish");
        CameraController.Instance.ToMainMenu();
    }

    public void GoToLevel(int levelNum)
    {
        PlayerPrefs.SetFloat("Level1", 0f);
        PlayerPrefs.SetFloat("Level2", 0f);
        PlayerPrefs.SetFloat("Level3", 0f);
        PlayerPrefs.SetFloat("Level4", 0f);
        PlayerPrefs.SetFloat("Level5", 0f);
        PlayerPrefs.SetFloat("Level6", 0f);
        PlayerPrefs.SetFloat("Level7", 0f);
        SceneManager.LoadScene(levelNum);
    }
}
