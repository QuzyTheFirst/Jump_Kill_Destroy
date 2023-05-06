using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLegController : MonoBehaviour
{
    public event EventHandler OnLegStateChange;

    public string legName;

    [Header("Leg")]
    [SerializeField] private Transform whereLegShouldBe;
    [SerializeField] private Transform legTarget;
    [SerializeField] private Transform legEnd;
    public EnemyLegController oppositeLeg;
    private FastIKFabric legIK;
    private AudioSource aud;

    [Header("Ground")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float legToGroundPower;
    [SerializeField] private LayerMask groundMask;

    [Header("Step")]
    [SerializeField] private float stepHeight;
    [SerializeField] private float maxStepDistance;
    [SerializeField] private float overstepDistance;
    [SerializeField] private float moveDuration;

    private Rigidbody legRig;

    private float legLerp;

    public bool CanBeGrounded { get; private set; }
    private bool wasGroundedLastFrame;
    public bool IsMoving { get; private set; }

    private Vector3 oldLegPos;
    private Vector3 currentLegPos;
    private Vector3 newPossibleLegPos;

    private void Start()
    {
        aud = legEnd.GetComponent<AudioSource>();

        wasGroundedLastFrame = CanBeGrounded;
        legIK = legEnd.GetComponent<FastIKFabric>();
        legRig = legEnd.parent.GetComponent<Rigidbody>();
        if (Physics.Raycast(whereLegShouldBe.position, Vector3.down, out RaycastHit hit, groundCheckDistance, groundMask))
        {
            newPossibleLegPos = hit.point;
        }
        TryToMove();
        CheckForStates();
    }

    private void FixedUpdate()
    {
        legTarget.position = currentLegPos;

        if (Physics.Raycast(whereLegShouldBe.position, Vector3.down, out RaycastHit hit, groundCheckDistance, groundMask))
        {
            CanBeGrounded = true;
            newPossibleLegPos = hit.point;
        }
        else
        {
            CanBeGrounded = false;
        }
        CheckForStates();

        legRig.AddForce(Vector3.down * legToGroundPower, ForceMode.Acceleration);
    }

    public void TurnOffIK()
    {
        legIK.enabled = false;
    }
    public void TurnOnIK()
    {
        legIK.enabled = true;
    }

    public void TryToMove()
    {
        float distFromNewPos = Vector3.Distance(currentLegPos, newPossibleLegPos);
        if (distFromNewPos > maxStepDistance)
        {
            oldLegPos = legEnd.position;
            StartCoroutine(MoveToNewPosition(currentLegPos, newPossibleLegPos));
        }
    }

    IEnumerator MoveToNewPosition(Vector3 startPoint, Vector3 newPossiblePos)
    {
        IsMoving = true;

        Vector3 toNewPos = (newPossiblePos - startPoint).normalized;

        Vector3 overshootVector = toNewPos * overstepDistance;
        overshootVector = Vector3.ProjectOnPlane(overshootVector, Vector3.up);

        Vector3 endPoint = newPossiblePos + overshootVector;

        if (Physics.Raycast(new Vector3(endPoint.x, whereLegShouldBe.position.y, endPoint.z), Vector3.down, out RaycastHit hit, groundCheckDistance, groundMask))
        {
            endPoint = hit.point;
        }
        Vector3 centerPoint = (startPoint + endPoint) * .5f;

        float timeElapsed = 0;

        do
        {
            float normalizedTime = timeElapsed / moveDuration;

            Vector3 footPosition = Vector3.Lerp(startPoint, endPoint, normalizedTime);
            footPosition.y += Mathf.Sin(normalizedTime * Mathf.PI) * stepHeight;

            currentLegPos = footPosition;

            timeElapsed += Time.deltaTime;

            yield return null;
        }
        while (timeElapsed < moveDuration);
        aud.Play();
        IsMoving = false;
    }

    private void CheckForStates()
    {
        if(wasGroundedLastFrame && !CanBeGrounded)
        {
            OnLegStateChange?.Invoke(this, EventArgs.Empty);
        }
        if(!wasGroundedLastFrame && CanBeGrounded)
        {
            OnLegStateChange?.Invoke(this, EventArgs.Empty);
        }
        wasGroundedLastFrame = CanBeGrounded;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(whereLegShouldBe.position, whereLegShouldBe.position + Vector3.down * groundCheckDistance);
    }
}
