using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPusher : MonoBehaviour
{
    private bool doPush = false;
    private bool nullifyVelocity = false;
    private int objectsPerFrame;
    private float pushPower = 0f;
    private BoxCollider boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }
    public void Push(float pushPower, int objectsPerFrame, bool nullifyVelocity)
    {
        doPush = true;
        StartCoroutine(DontPushNextFrame());
        this.pushPower = pushPower;
        this.nullifyVelocity = nullifyVelocity;
        this.objectsPerFrame = objectsPerFrame;
    }

    private void OnTriggerStay(Collider coll)
    {
        if (doPush)
        {
            Vector3 worldCenter = transform.TransformPoint(boxCollider.center);
            Collider[] colls =  Physics.OverlapBox(worldCenter, boxCollider.size, transform.rotation);
            StartCoroutine(PushObjects(colls));
            doPush = false;
        }
    }

    IEnumerator PushObjects(Collider[] colls)
    {
        int objectsPushed = 0;
        foreach(Collider coll in colls)
        {
            Rigidbody rig = coll.GetComponent<Rigidbody>();
            if (rig)
            {
                if (nullifyVelocity)
                {
                    rig.velocity = Vector3.zero;
                }
                rig.AddForce(-transform.forward * pushPower, ForceMode.VelocityChange);
                Debug.Log("Pushed: " + coll.transform.name);
                objectsPushed++;
                if(objectsPushed == objectsPerFrame)
                {
                    yield return new WaitForEndOfFrame();
                }
            }
        }
    }

    IEnumerator DontPushNextFrame()
    {
        yield return new WaitForFixedUpdate();
        doPush = false;
    }
}
