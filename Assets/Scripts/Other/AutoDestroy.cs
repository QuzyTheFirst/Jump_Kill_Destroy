using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] float timeToDestroy;

    private void Start()
    {
        if(timeToDestroy > 0)
        {
            Destroy(gameObject, timeToDestroy);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
