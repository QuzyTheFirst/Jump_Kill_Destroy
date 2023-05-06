using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public static Timer Instance;
    private TextMeshProUGUI timer;
    private float timeFromLevelBegining;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        timer = GetComponent<TextMeshProUGUI>();
        timeFromLevelBegining = 0f;
    }

    private void Update()
    {
        timeFromLevelBegining += Time.deltaTime;
        timer.text = timeFromLevelBegining.ToString("00.00");
    }

    public float GetTimeFromLevelBegining() => timeFromLevelBegining;
}
