using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class StupidEnemyController : MonoBehaviour
{
    private enum State
    {
        Dead,
        InAir,
        OnGround,
        TurnedOff,
    }
    State state = State.OnGround;
    [Header("AI")]
    [SerializeField] private float timeToGetUp;
    [SerializeField] private bool canShoot;
    [SerializeField] private bool canFindPath;
    private Transform followObj;
    private bool isOneLegCanBeGrounded;
    private bool canGetUp;
    private float getUpTimer; 
    private NavMeshAgent agent;
    private Vector3 nextPosToGo;
    private float elapsed;

    [Header("HP")]
    [SerializeField] private float maxHealthPoints;
    [SerializeField] private AudioSource aud;
    private float healthPoints;
    private bool wasPunchedWithLeg;
    private bool wasDamaged;

    [Header("Walking")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float accelerationPower;
    [SerializeField] private float accelerationFactor;
    [SerializeField] private float rotationForce;
    [SerializeField] private float rotationBalanceForce;

    [Header("Hip")]
    [SerializeField] private Transform tf_Hip;
    [SerializeField] private float hip_UpPower;
    [SerializeField] private float hip_DistanceToGround;
    private Rigidbody rig_Hip;

    [Header("Head")]
    [SerializeField] private Transform tf_Head;
    private Rigidbody rig_Head;

    [Header("Spine")]
    [SerializeField] private Transform tf_Spine;
    private Rigidbody rig_Spine;

    [Header("Arms")]
    [SerializeField] private FastIKFabric ik_RightArm;

    [Header("Legs")]
    [SerializeField] private EnemyLegController[] Legs;
    private EnemyPartsManager enemyPartsManager;

    [Header("Other")]
    [SerializeField] private LayerMask whereIsGround;
    [SerializeField] private float balanceForce;
    private EnemyShoot enemyShoot;

    private void Start()
    {
        followObj = FindObjectOfType<PlayerController>().transform;
        ik_RightArm.Target = followObj;

        state = State.OnGround;

        rig_Hip = tf_Hip.GetComponent<Rigidbody>();
        rig_Head = tf_Head.GetComponent<Rigidbody>();
        rig_Spine = tf_Spine.GetComponent<Rigidbody>();

        nextPosToGo = rig_Hip.transform.position;

        enemyPartsManager = GetComponent<EnemyPartsManager>();

        enemyShoot = GetComponent<EnemyShoot>();

        healthPoints = maxHealthPoints;

        agent = GetComponent<NavMeshAgent>();

        elapsed = 0f;

        Legs = GetComponents<EnemyLegController>();

        foreach(PartOfEnemy part in enemyPartsManager.GetEnemyParts())
        {
            part.OnPartDamaged += OnDamage;
            part.OnLegPunch += OnLegPunch;
        }
        foreach(EnemyLegController leg in Legs)
        {
            leg.OnLegStateChange += OnLegStateChange;
        }
    }

    private void OnLegStateChange(object sender, EventArgs e)
    {
        isOneLegCanBeGrounded = false;
        foreach(EnemyLegController leg in Legs)
        {
            isOneLegCanBeGrounded = leg.CanBeGrounded ? true : isOneLegCanBeGrounded;
        }
    }

    private void OnLegPunch(object sender, DamageClass e)
    {
        Damage(e.damage, (PartOfEnemy)sender);
        wasPunchedWithLeg = true;
    }

    private void OnDamage(object sender, DamageClass e)
    {
        Damage(e.damage, (PartOfEnemy)sender);
        wasDamaged = true;
    }

    private void Damage(float damage, PartOfEnemy part = null)
    {
        healthPoints -= damage;
        if (!IsEnoughHealth())
        {
            if(part != null)
                part.DeactivateJoint();
            ChangeState(State.Dead);
        }
    }

    private void FixedUpdate()
    {
        if (state != State.Dead)
        {
            switch (state)
            {
                case State.OnGround:
                    if (canShoot)
                    {
                        enemyShoot.TryToShoot(followObj);
                    }
                    TryToMoveLegs();
                    StabilizeBody();
                    FindNextPosToGo();
                    UppForces();
                    Move();
                    break;
            }

            StateChanger();
        }
    }

    private void TryToMoveLegs()
    {
        foreach (EnemyLegController leg in Legs)
        {
            if (leg.oppositeLeg != null)
            {
                if (leg.CanBeGrounded && !leg.oppositeLeg.IsMoving)
                {
                    leg.TryToMove();
                }
            }
            else
            {
                if (leg.CanBeGrounded)
                {
                    leg.TryToMove();
                }
            }
        }
    }

    private void UppForces()
    {
        // Hip
        if (Physics.Raycast(tf_Hip.position, Vector3.down, out RaycastHit hit, hip_DistanceToGround, whereIsGround))
        {
            Vector3 targetPos = hit.normal * hip_DistanceToGround;
            rig_Hip.AddForce(Vector3.up * hip_UpPower, ForceMode.Acceleration);
        }
    }

    private void StabilizeBody()
    {
        rig_Head.AddForce(Vector3.up * balanceForce);
        rig_Hip.AddForce(Vector3.down * balanceForce);

        rig_Hip.AddTorque(-rig_Hip.angularVelocity * rotationBalanceForce, ForceMode.Acceleration);
        var rot = Quaternion.FromToRotation(-transform.right, Vector3.right);
        rig_Hip.AddTorque(new Vector3(rot.x, rot.y, rot.z) * rotationBalanceForce, ForceMode.Acceleration);
    }

    private bool IsEnoughHealth()
    {
        if(healthPoints <= 0f)
        {
            return false;
        }
        return true;
    }

    private void Move()
    {
        if (Vector3.Distance(tf_Hip.position, followObj.position) > 2f)
        {
            if (nextPosToGo != Vector3.zero)
            {
                Vector3 moveDir = (nextPosToGo - tf_Hip.position).normalized;

                moveDir = new Vector3(moveDir.x, 0f, moveDir.z);
                rig_Hip.AddForce(moveDir * moveSpeed * accelerationPower * Time.deltaTime);
                rig_Hip.AddForce(-moveDir * moveSpeed * accelerationPower * Time.deltaTime * accelerationFactor);
            }

            Vector3 rigXZ = new Vector3(rig_Hip.velocity.x, 0f, rig_Hip.velocity.z);
            if (rigXZ.magnitude > maxSpeed)
            {
                ChangeState(State.InAir);
                Debug.Log("Too fast");
            }

            float rootAngle = tf_Hip.eulerAngles.y;
            float desiredAngle = Quaternion.LookRotation(followObj.position - tf_Hip.position).eulerAngles.y;
            float deltaAngle = Mathf.DeltaAngle(rootAngle, desiredAngle + 90);
            rig_Hip.AddTorque(Vector3.up * deltaAngle * rotationForce, ForceMode.Acceleration);
        }
        else
        {
            rig_Hip.velocity = new Vector3(0, rig_Hip.velocity.y, 0);

            float rootAngle = tf_Hip.eulerAngles.y;
            float desiredAngle = Quaternion.LookRotation(followObj.position - tf_Hip.position).eulerAngles.y;
            float deltaAngle = Mathf.DeltaAngle(rootAngle, desiredAngle + 90);
            rig_Hip.AddTorque(Vector3.up * deltaAngle * rotationForce, ForceMode.Acceleration);
        }
    }

    private void FindNextPosToGo()
    {
        if (canFindPath)
        {
            elapsed += Time.deltaTime;
            if (elapsed > 1.0f)
            {
                agent.enabled = true;

                if (agent.isOnNavMesh)
                {
                    NavMeshPath path = new NavMeshPath();
                    if (NavMesh.CalculatePath(tf_Hip.position, followObj.position, NavMesh.AllAreas, path))
                    {
                        agent.path = path;
                    }
                }

                nextPosToGo = agent.steeringTarget;

                agent.enabled = false;

                elapsed -= 1.0f;
            }
        }
    }

    private void StateChanger()
    {
        if (canGetUp)
        {
            ChangeState(State.OnGround);
        }
        else
        {
            TryToGetUp();
        }

        if (!isOneLegCanBeGrounded)
        {
            ChangeState(State.InAir);
        }
    }

    private void ChangeState(State state)
    {
        switch (state)
        {
            case State.OnGround:
                ChangeState_OnGround();
                break;
            case State.InAir:
                ChangeState_InAir();
                break;
            case State.TurnedOff:
                ChangeState_TurnOff();
                break;
            case State.Dead:
                ChangeState_Die();
                break;
        }
    }

    private void ChangeState_OnGround()
    {
        if (state != State.OnGround)
        {
            SetIK(true);
            state = State.OnGround;
        }
    }

    private void ChangeState_TurnOff()
    {
        if (state != State.TurnedOff)
        {
            SetIK(false);
            state = State.TurnedOff;
        }
    }

    private void ChangeState_InAir()
    {
        if (state != State.InAir)
        {
            SetIK(false);
            getUpTimer = timeToGetUp;
            canGetUp = false;
            state = State.InAir;
        }
    }

    public void ChangeState_Die()
    {
        if (state != State.Dead)
        {
            SetIK(false);
            aud.Play();
            state = State.Dead;
        }
    }

    private void TryToGetUp()
    {
        if (isOneLegCanBeGrounded && state == State.InAir)
        {
            getUpTimer -= Time.deltaTime;
            if (getUpTimer <= 0f)
            {
                canGetUp = true;
            }
        }
        else
        {
            getUpTimer = timeToGetUp;
        }
    }

    private void SetIK(bool value)
    {
        if (value == true)
        {
            foreach (EnemyLegController leg in Legs)
            {
                leg.TurnOnIK();
            }
            ik_RightArm.enabled = true;
        }
        else
        {
            foreach (EnemyLegController leg in Legs)
            {
                leg.TurnOffIK();
            }
            ik_RightArm.enabled = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(tf_Hip.transform.position, tf_Hip.transform.position + Vector3.down * hip_DistanceToGround);
    }

}
