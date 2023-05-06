using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfTheLevelZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(GoToNextLevel(.5f));
        }
    }

    IEnumerator GoToNextLevel(float time)
    {
        SceneTransition.Instance.PlayEndLevelTransition();
        PlayerPrefs.SetFloat("Level" + SceneManager.GetActiveScene().buildIndex, Timer.Instance.GetTimeFromLevelBegining());
        yield return new WaitForSecondsRealtime(time);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
