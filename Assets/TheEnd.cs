using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheEnd : MonoBehaviour,IInteractable
{
    public AudioSource aud;
    public void Interact(object sender)
    {
        SoundManager.Instance.Stop("ComputerWorking");
        SoundManager.Instance.Stop("ComputerStartUp");
        SoundManager.Instance.Stop("Shot");
        aud.Stop();
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
