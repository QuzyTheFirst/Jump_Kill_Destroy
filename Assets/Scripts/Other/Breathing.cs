using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breathing : MonoBehaviour
{
    [SerializeField] private Transform targetObj;
    [SerializeField] private float p_x_intensity, p_y_intensity, breathingPower = 4f;

    private Vector3 targetObjectOrigin, targetPos;

    private void Start()
    {
        targetObjectOrigin = targetObj.transform.localPosition;
    }

    private void Update()
    {
        HeadBob(p_x_intensity, p_y_intensity);
        targetObj.localPosition = Vector3.Lerp(targetObj.localPosition, targetPos, Time.deltaTime * breathingPower);
    }

    private void HeadBob(float p_x_intensity, float p_y_intensity)
    {
        targetPos = targetObjectOrigin + new Vector3(Mathf.Cos(Time.time) * p_x_intensity, Mathf.Sin(Time.time * 2) * p_y_intensity, 0);
    }
}
