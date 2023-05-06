using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField] private Transform tf_ToWhere;

    private void OnTriggerEnter(Collider other)
    {
        other.transform.position = tf_ToWhere.position;
    }
}
