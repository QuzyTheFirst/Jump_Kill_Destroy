using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPartsManager : MonoBehaviour
{
    [Header("Enemy Parts")]
    [SerializeField] private PartOfEnemy[] enemyParts;

    public PartOfEnemy GetPart(string name)
    {
        PartOfEnemy part = Array.Find(enemyParts, part => part.name == name);
        return part;
    }

    public PartOfEnemy[] GetEnemyParts()
    {
        return enemyParts;
    }

    public void DeactivateAllParts()
    {
        foreach(PartOfEnemy part in enemyParts)
        {
            part.DeactivateJoint();
        }
    }
}
