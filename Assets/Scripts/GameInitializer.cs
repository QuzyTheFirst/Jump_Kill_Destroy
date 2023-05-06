using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private Transform pf_SoundManager;
    [SerializeField] private Transform pf_playerUI;
    [SerializeField] private Transform pf_SceneTransition;
    private void Awake()
    {
        if(SoundManager.Instance == null)
        {
            Instantiate(pf_SoundManager);
        }
        if(SceneTransition.Instance == null)
        {
            Instantiate(pf_SceneTransition);
        }
    }
    private void Start()
    {
        SceneTransition.Instance.PlayStartLevelTransition();
        SoundManager.Instance.FadeAwayVolume("ComputerWorking", 1f);
        SoundManager.Instance.FadeAwayVolume("ComputerStartUp", 1f);
        SoundManager.Instance.FadeAwayVolume("StrongWindBlow", .5f);
        SoundManager.Instance.FadeAwayVolume("LightWindBlow", .5f);
    }
}
