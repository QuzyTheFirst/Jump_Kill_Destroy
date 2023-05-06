using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ArmsController : MonoBehaviour
{
    public Arm[] arms;

    public Arm GetArm(string name)
    {
        Arm arm = Array.Find(arms, arm => arm.name == name);

        if(arm != null)
        {
            return arm;
        }
        else
        {
            Debug.Log("This arm doesn't exist: " + name);
            return null;
        }
    }
}
