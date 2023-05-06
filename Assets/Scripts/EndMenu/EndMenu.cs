using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class EndMenu : MonoBehaviour
{
    [SerializeField] private PlayableDirector playableDirector;
    [SerializeField] private GameObject[] objectsToDeactivate;
    private void Awake()
    {
        Time.timeScale = 1f;
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void ToMainMenu()
    {
        playableDirector.Play();
        foreach(GameObject obj in objectsToDeactivate)
        {
            obj.SetActive(false);
        }
    }

    public void TheEnd()
    {
        Application.Quit();
    }

    public void GoodEnd()
    {
        SceneManager.LoadScene(0);
    }
}
