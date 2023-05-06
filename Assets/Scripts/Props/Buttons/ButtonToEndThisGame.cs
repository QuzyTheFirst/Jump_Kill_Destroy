using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ButtonToEndThisGame : MonoBehaviour, IInteractable
{
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Interact(object sender)
    {
        anim.SetTrigger("Press");
        StartCoroutine(WaitForAndLoadLevel(1f));
    }
    IEnumerator WaitForAndLoadLevel(float time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(0);
    }
}
