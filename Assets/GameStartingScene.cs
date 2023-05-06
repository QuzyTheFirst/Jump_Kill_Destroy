using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartingScene : MonoBehaviour
{
    private Animator anim;
    private float timerToSecondTrack = 0f;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        SoundManager.Instance.Play("ComputerStartUp");
        timerToSecondTrack = 0f;
        StartCoroutine(PlayTrackAfterSeconds("ComputerWorking", 7f));
    }

    IEnumerator PlayTrackAfterSeconds(string trackName, float time) 
    {
        yield return new WaitForSeconds(time);
        SoundManager.Instance.Play(trackName);
    }

    private void Update()
    {
        timerToSecondTrack += Time.deltaTime;
        if(timerToSecondTrack > 2f)
        {
            anim.SetTrigger("ImageGoAway");
        }
    }
}
