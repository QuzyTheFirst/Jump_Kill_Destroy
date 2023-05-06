using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionChecker : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsCeiling;
    private bool isColliding;

    public bool IsColliding { 
        get { return isColliding; }
    }

    private void FixedUpdate()
    {
        isColliding = false;
    }

    private void OnTriggerStay(Collider other)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + Vector3.up, .499f, whatIsCeiling);
        //Debug.Log(hitColliders.Length);
        if(hitColliders.GetLength(0) > 0)
            isColliding = true;
    }
}
