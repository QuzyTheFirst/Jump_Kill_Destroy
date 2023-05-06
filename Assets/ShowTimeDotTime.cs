using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowTimeDotTime : MonoBehaviour
{
    private TextMeshProUGUI text;
    private float firstLevel, secondLevel, thirdLevel, fourthLevel, fifthLevel, sixthLevel, seventhLevel;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        firstLevel = PlayerPrefs.GetFloat("Level1", 0f);
        secondLevel = PlayerPrefs.GetFloat("Level2", 0f);
        thirdLevel = PlayerPrefs.GetFloat("Level3", 0f);
        fourthLevel = PlayerPrefs.GetFloat("Level4", 0f);
        fifthLevel = PlayerPrefs.GetFloat("Level5", 0f);
        sixthLevel = PlayerPrefs.GetFloat("Level6", 0f);
        seventhLevel = PlayerPrefs.GetFloat("Level7", 0f);

    }

    private void Update()
    {
        text.text = "1 level: " + firstLevel.ToString("00.00" + "s") + "\n"
                  + "2 level: " + secondLevel.ToString("00.00" + "s") + "\n"
                  + "3 level: " + thirdLevel.ToString("00.00" + "s") + "\n"
                  + "4 level: " + fourthLevel.ToString("00.00" + "s") + "\n"
                  + "5 level: " + fifthLevel.ToString("00.00" + "s") + "\n"
                  + "6 level: " + sixthLevel.ToString("00.00" + "s") + "\n"
                  + "7 level: " + seventhLevel.ToString("00.00" + "s") + "\n"
                  + "Total time: " + (firstLevel + secondLevel + thirdLevel + fourthLevel + fifthLevel + sixthLevel + seventhLevel).ToString("00.00" + "s");
    }
}
