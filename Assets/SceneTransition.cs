using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private Animator anim_BlackScreen;
    [SerializeField] private Animator anim_LoadingScreen;

    public static SceneTransition Instance;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(WaitTwoSecondsAndMakeBlackScreenTransparent());
    }
    IEnumerator WaitTwoSecondsAndMakeBlackScreenTransparent()
    {
        yield return new WaitForSeconds(2f);
        anim_BlackScreen.SetTrigger("ImageGoAway");
    }

    public void PlayEndLevelTransition()
    {
        anim_LoadingScreen.SetTrigger("EndLevelTransition");
    }

    public void PlayStartLevelTransition()
    {
        anim_LoadingScreen.SetTrigger("StartLevelTransition");
    }

    public void BlackScreenDisappearance()
    {
        anim_BlackScreen.SetTrigger("ImageGoAway");
    }

    public void BlackScreenAppearance()
    {
        anim_BlackScreen.SetTrigger("ImageComeback");
    }
}
