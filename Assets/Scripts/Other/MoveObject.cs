using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    [SerializeField] private Vector3 where;
    [SerializeField] private float speed;

    private void Update()
    {
        transform.position += where * Time.deltaTime * speed;
    }
}
