using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform tf_BrokenParts;
    [SerializeField] private bool isCollider;
    [SerializeField] private bool breakOnTouch;
    [SerializeField] private bool breakOnShot;
    [SerializeField] private bool breakOnLegPunch;

    private bool done;

    private BoxCollider boxCollider;

    private void Awake()
    {
        done = false;
        boxCollider = GetComponent<BoxCollider>();
        if (isCollider)
        {
            boxCollider.isTrigger = false;
        }
        else
        {
            boxCollider.isTrigger = true;
        }
    }

    public void Interact(object sender)
    {
        if(sender is LegController && breakOnLegPunch)
        {
            BreakObject();
        }

        if(sender is Pistol && breakOnShot)
        {
            BreakObject();
        }
    }

    private void BreakObject()
    {
        if (done == false)
        {
            Transform obj = Instantiate(tf_BrokenParts, transform.parent.position, transform.parent.rotation);
            obj.localScale = transform.parent.localScale;
            Destroy(gameObject);
            done = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (breakOnTouch)
        {
            BreakObject();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (breakOnTouch)
        {
            BreakObject();
        }
    }
}
