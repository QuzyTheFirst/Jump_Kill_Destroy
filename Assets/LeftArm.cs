using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftArm : MonoBehaviour
{

    public void TurnOnTheMusic()
    {
        SoundManager.Instance.Play("Select");
        SoundManager.Instance.ChangeSoundVolume("Music", .7f);
        SoundManager.Instance.Play("Music");
    }

    public void TurnOffTheMusic()
    {
        SoundManager.Instance.FadeAwayVolume("Music", .5f);
    }
}
