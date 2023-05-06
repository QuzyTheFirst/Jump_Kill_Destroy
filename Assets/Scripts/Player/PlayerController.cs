using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : ComponentController, IDamagable
{
    public enum State
    {
        Walk,
        Wallrun,
        Crouch,
        Slide,
    }
    public State state = State.Walk;

    [Header("Health")]
    [SerializeField] private bool canDie;
    [SerializeField] private float health;
    [SerializeField] private float maxHealth;

    public float Health => health;
    public float MaxHealth => maxHealth;

    public event EventHandler OnPlayerDamage;


    [Header("Walking and Sprinting")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float accelerationPower;
    [SerializeField] private float maxGroundAngle;
    [SerializeField] private LayerMask groundLayerMask;

    [Header("Steps")]
    [SerializeField] private float timeBetweenStepsOnGround;
    [SerializeField] private float timeBetweenStepsOnWall;
    private Vector3 lastStepOnGroundPos;
    private Vector3 lastStepOnWallPos;
    private float timerStepsOnGround;
    private float timerStepsOnWall;

    [Header("Acceleration")]
    [SerializeField] private float standardAccelerationFactor = .18f;
    [SerializeField] private float maxAccelerationFactor = .14f;
    [SerializeField] private float timeToFullAcceleration;
    private float accelerationFactor;
    private float accelerationTimer;

    [Header("Crouch and Slide")]
    private const float SITTINGHEIGHT = 1f;
    private const float STAYINGHEIGHT = 2f;
    [SerializeField] private float sizeChangeSpeed;
    [SerializeField] private CollisionChecker ceilingChecker;
    private CapsuleCollider playerCollider;
    private float playerTargetHeight;
    private bool tryToGetUp;

    [Header("Slide")]
    [SerializeField] private float slideCrouchThreshold;
    [SerializeField] private float slidingAdditionalSpeedMultiplier;
    private bool isSliding = false;
    private float slideSlowdown = .2f;

    [Header("Crouch")]
    private bool isCrouchingButtonPressed = false;

    [Header("Jump")]
    [SerializeField] private float jumpHeight;
    private bool doJump;
    
    private float minGroundDotProduct;
    private bool isGrounded;
    private CinemachineVirtualCamera cam;

    // ---WallRun---
    private Vector3 wallrunDirection;
    private Vector3 whereIsWall;
    private Vector3 wallrunNormal;
    private Vector3 lastNormal;

    private float distanceFromWall;
    private float maxWallrunSpeed;
    private float wallrunTimer;
    private float upNumb;
    private float timeToNextWallrun;

    private bool isARightWall;
    public bool IsARightWall
    {
        get { return isARightWall; }
    }

    private int changeDirection;

    [Header("Wallrun")]
    [SerializeField] private LayerMask wallrunLayerMask;
    [SerializeField] private float timeToNextWallrunMax;
    [SerializeField] private float timeToResetLastNormal;
    [SerializeField] private float wallrunAcceleration;
    [SerializeField] private float wallrunUpwardForce;
    [SerializeField] private float wallrunJumpPower;
    [SerializeField] private float wallrunJumpPowerY;
    [Header("Wallrun Angles")]
    [SerializeField] private float maxWallAngle;
    [SerializeField] private float minWallAngle;
    [SerializeField] private float maxAngleFromWall = -5f;
    [SerializeField] private float maxAngleToWall = 40f;
    private float minWallDotProduct;
    private float maxWallDotProduct;
    // -------------

    [Header("Other")]
    [SerializeField] private ParticleSystem landingParticle;
    private float relY;

    // Last Frame
    private bool isLastFrameGrounded;

    private void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        minWallDotProduct = Mathf.Sin(minWallAngle * Mathf.Deg2Rad);
        maxWallDotProduct = Mathf.Sin(maxWallAngle * Mathf.Deg2Rad);
    }

    private new void Awake()
    {
        base.Awake();
        health = maxHealth;
    }

    private void Start()
    {
        accelerationFactor = standardAccelerationFactor;
        OnValidate();
        cam = playerLookController.GetCamera();
        playerTargetHeight = STAYINGHEIGHT;
        playerCollider = GetComponent<CapsuleCollider>();
        lastNormal = Vector3.zero;
    }

    private void Update()
    {
        StepSounds();
        ChangePlayerSize();
        if (tryToGetUp)
        {
            TryToGetUp();
        }

        if (state == State.Slide && isGrounded)
        {
            if (SoundManager.Instance)
            {
                SoundManager.Instance.Play("Slide");
                float pitchMultiplier = Mathf.InverseLerp(5, 40, rig.velocity.magnitude);
                SoundManager.Instance.ChangeSoundPitch("Slide", 3 * pitchMultiplier);
            }
        }
        else
        {
            if (SoundManager.Instance)
                SoundManager.Instance.Stop("Slide");
        }
        //Debug.Log(state);
    }

    #region Crouch and Slide
    public void CheckCrouch(InputAction.CallbackContext obj)
    {
        if (state != State.Wallrun)
        {
            if (rig.velocity.magnitude <= slideCrouchThreshold && isGrounded)
            {
                ChangeState(State.Crouch);
            }

            if (rig.velocity.magnitude > slideCrouchThreshold)
            {
                if (isGrounded)
                    rig.AddForce(rig.velocity.normalized * slidingAdditionalSpeedMultiplier, ForceMode.VelocityChange);

                ChangeState(State.Slide);
            }
        }
        else
        {
            StopWallrunning();
            ChangeState(State.Slide);
        }

        tryToGetUp = false;
        isCrouchingButtonPressed = true;
    }
    public void StopCrouch(InputAction.CallbackContext obj)
    {
        if (!ceilingChecker.IsColliding)
        {
            ChangeState(State.Walk);
        }
        else
        {
            ChangeState(State.Crouch);
            tryToGetUp = true;
        }

        isCrouchingButtonPressed = false;
    }

    private void ChangePlayerSize()
    {
        if (FindDifference(playerTargetHeight, playerCollider.height) > .01f)
        {
            if (playerTargetHeight == STAYINGHEIGHT)
            {
                if (!ceilingChecker.IsColliding)
                    playerCollider.height = Mathf.Lerp(playerCollider.height, playerTargetHeight, Time.deltaTime * sizeChangeSpeed);
            }
            else
            {
                playerCollider.height = Mathf.Lerp(playerCollider.height, playerTargetHeight, Time.deltaTime * sizeChangeSpeed);
            }
        }
        else
        {
            playerCollider.height = playerTargetHeight;
        }
    }

    private void TryToGetUp()
    {
        if (!ceilingChecker.IsColliding)
        {
            ChangeState(State.Walk);
            tryToGetUp = false;
        }

    }

    private void Crouching()
    {
        CheckForSlideTransition();
    }

    private void Sliding()
    {
        if (!tryToGetUp)
        {
            if (isGrounded)
            {
                if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 1f))
                {
                    Vector3 planeDir = FindPlaneDir(hit.normal);
                    if (planeDir.y > 0)
                    {
                        rig.AddForce(-planeDir * 10f, ForceMode.Acceleration);
                    }
                }

                CheckForCrouchTransition();
            }

            if (doJump)
            {
                if (isGrounded)
                {
                    float initialVel = Mathf.Sqrt(0 - 2 * Physics.gravity.y * jumpHeight);
                    rig.velocity = new Vector3(rig.velocity.x, 0f, rig.velocity.z);
                    rig.AddForce(Vector3.up * initialVel, ForceMode.VelocityChange);
                    landingParticle.Play();
                }

                doJump = false;
            }
        }
        else
        {
            ChangeState(State.Crouch);
        }
    }

    private void CheckForCrouchTransition()
    {
        if (rig.velocity.magnitude < slideCrouchThreshold && isGrounded)
        {
            ChangeState(State.Crouch);
        }
    }

    private void CheckForSlideTransition()
    {
        if (!tryToGetUp)
        {
            if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 1f, groundLayerMask))
            {
                if (hit.normal.y != 0)
                {
                    ChangeState(State.Slide);
                }
            }

            if (rig.velocity.magnitude > slideCrouchThreshold || !isGrounded)
            {
                ChangeState(State.Slide);
            }
        }
    }
    #endregion

    #region Jump
    public void CheckJump(InputAction.CallbackContext obj)
    {
        if ((state == State.Walk && isGrounded) || (state == State.Wallrun) || (state == State.Slide && isGrounded))
        {
            if (SoundManager.Instance)
                SoundManager.Instance.Play("Jump");
            doJump = true;
        }
    }

    private void Jump()
    {
        if (doJump && isGrounded)
        {
            float initialVel = Mathf.Sqrt(0 - 2 * Physics.gravity.y * jumpHeight);
            rig.AddRelativeForce(transform.up * initialVel, ForceMode.VelocityChange);
            landingParticle.Play();
            doJump = false;
        }
    }
    #endregion

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Walk:
                Move();
                Jump();
                WallsCheck();
                break;
            case State.Wallrun:
                Wallrunning();
                break;
            case State.Crouch:
                Move();
                Crouching();
                break;
            case State.Slide:
                Sliding();
                Move();
                break;
        }

        isLastFrameGrounded = isGrounded;
        isGrounded = false;
    }

    private void ChangeState(State state)
    {
        switch (state)
        {
            case State.Walk:
                playerTargetHeight = STAYINGHEIGHT;
                isSliding = false;
                this.state = state;
                landingParticle.transform.localPosition = new Vector3(0f, -1f, 0f);
                break;
            case State.Wallrun:
                playerTargetHeight = STAYINGHEIGHT;
                isSliding = false;
                this.state = state;
                landingParticle.transform.localPosition = new Vector3(0f, -1f, 0f);
                break;
            case State.Crouch:
                playerTargetHeight = SITTINGHEIGHT;
                isSliding = false;
                this.state = state;
                landingParticle.transform.localPosition = new Vector3(0f, -.5f, 0f);
                break;
            case State.Slide:
                playerTargetHeight = SITTINGHEIGHT;
                isSliding = true;
                this.state = state;
                landingParticle.transform.localPosition = new Vector3(0f, -.5f, 0f);
                break;
        }
    }
    #region Movement
    private void Move()
    {
        rig.AddForce(Vector3.down * Time.deltaTime * 10);

        Vector3 movement = new Vector3(PlayerInputController.moveInputX, 0, PlayerInputController.moveInputY).normalized;

        Vector2 velRelToLook = new Vector2(Vector3.Dot(transform.right, rig.velocity), Vector3.Dot(transform.forward, rig.velocity));

        if (velRelToLook.x > maxSpeed)
        {
            movement.x = 0f;
        }
        else if (velRelToLook.x < -maxSpeed)
        {
            movement.x = 0f;
        }
        if (velRelToLook.y > maxSpeed)
        {
            movement.z = 0f;
        }
        else if (velRelToLook.y < -maxSpeed)
        {
            movement.z = 0f;
        }


        //Debug.Log("Movement: " + movement + " || velRelToLook: " + velRelToLook);

        float xModifier = 1f, zModifier = 1f;

        if (!isGrounded)
        {
            zModifier = .05f;
            xModifier = .1f;
        }
        // Sliding
        if (state == State.Slide && isGrounded)
        {
            zModifier = 0f;
            xModifier = .1f;
        }
        if (state == State.Slide && !isGrounded)
        {
            zModifier = 0f;
            xModifier = .1f;
        }
        //Crouching
        if (state == State.Crouch && isGrounded)
        {
            zModifier = .5f;
            xModifier = .5f;
        }
        if (state == State.Crouch && !isGrounded)
        {
            zModifier = 0f;
            xModifier = 0f;
        }

        if (movement != Vector3.zero)
        {
            accelerationTimer += Time.deltaTime;
            float normalizedTime = accelerationTimer / timeToFullAcceleration;
            float accelerationValue = Mathf.Lerp(standardAccelerationFactor, maxAccelerationFactor, normalizedTime);
            accelerationFactor = accelerationValue;
        }
        else
        {
            accelerationTimer = 0f;
            accelerationFactor = standardAccelerationFactor;
        }

        if (movement != Vector3.zero)
        {
            rig.AddForce(transform.forward * movement.z * accelerationPower * Time.deltaTime * zModifier);
            rig.AddForce(transform.right * movement.x * accelerationPower * Time.deltaTime * xModifier);
        }

        CounterMovement(movement.x, movement.y, velRelToLook);
    }

    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!isGrounded) { return; }

        if (state == State.Slide)
        {
            rig.AddForce(maxSpeed * Time.deltaTime * -rig.velocity * slideSlowdown, ForceMode.Acceleration);
            return;
        }

        rig.AddForce(accelerationPower * transform.right * Time.deltaTime * -mag.x * accelerationFactor);

        rig.AddForce(accelerationPower * transform.forward * Time.deltaTime * -mag.y * accelerationFactor);

        float rigXZMag = new Vector3(rig.velocity.x, 0f, rig.velocity.z).magnitude;
        //Debug.Log("RigXZMAg: " + rigXZMag + " || mag: " + mag);
        if (rigXZMag > maxSpeed)
        {
            float rigVelY = rig.velocity.y;
            Vector3 vector = rig.velocity.normalized * maxSpeed;
            rig.velocity = new Vector3(vector.x, rigVelY, vector.z);
        }
    }
    #endregion

    #region Wallrun
    private void WallsCheck()
    {
        if (timeToNextWallrun > 0f)
        {
            timeToNextWallrun -= Time.deltaTime;
            return;
        }

        RaycastHit hitRight;
        RaycastHit hitLeft;
        RaycastHit hitRightForward;
        RaycastHit hitLeftForward;

        wallrunDirection = Vector3.zero;
        whereIsWall = Vector3.zero;
        wallrunNormal = Vector3.zero;

        // Right
        CheckWall(transform.right, out hitRight, 1f, ref wallrunDirection, true, -1);

        // Right Forward
        CheckWall(transform.right + transform.forward, out hitRightForward, 1f, ref wallrunDirection, true, -1);

        // Left
        CheckWall(-transform.right, out hitLeft, 1f, ref wallrunDirection, false);

        // Left Forward
        CheckWall(-transform.right + transform.forward, out hitLeftForward, 1f, ref wallrunDirection, false);

        if (whereIsWall != Vector3.zero)
        {
            float wallDot = Vector3.Dot(whereIsWall.normalized, transform.forward.normalized) * Mathf.Rad2Deg;
            //Debug.Log(wallDot);
            if (wallDot < maxAngleFromWall || wallDot > maxAngleToWall) { return; }
        }

        Vector3 rigVelXZ = new Vector3(rig.velocity.x, 0f, rig.velocity.z);

        if (wallrunDirection != Vector3.zero && whereIsWall != Vector3.zero && !isGrounded && rigVelXZ.magnitude > 7f && rig.velocity.y > -10f && lastNormal != -whereIsWall)
        {
            StartWallrun(rigVelXZ);
        }
    }

    private void StartWallrun(Vector3 relVelXZ)
    {
        ChangeState(State.Wallrun);
        maxWallrunSpeed = relVelXZ.magnitude;
        upNumb = rig.velocity.y;
        wallrunTimer = 0;
        rig.useGravity = false;
        rig.velocity = wallrunDirection * maxWallrunSpeed;

    }

    private void Wallrunning()
    {
        rig.AddForce(-Vector3.up * wallrunTimer * -Physics.gravity.y);
        RaycastHit hit;

        wallrunTimer += Time.deltaTime;
        Mathf.Clamp(wallrunTimer, 0f, 1f);

        if (!CheckWall(whereIsWall, out hit, distanceFromWall, ref wallrunDirection, isARightWall, changeDirection))
        {
            StopWallrunning();
        }
        else
        {
            if (wallrunNormal != -whereIsWall)
            {
                float up = rig.velocity.y;
                rig.velocity = wallrunDirection * maxWallrunSpeed + transform.up * up;
                wallrunNormal = -whereIsWall;
            }
        }

        if (rig.velocity.magnitude < maxSpeed)
        {
            rig.AddForce(wallrunDirection * wallrunAcceleration);
        }

        if (upNumb > 0f)
        {
            rig.AddForce(transform.up * upNumb * wallrunUpwardForce, ForceMode.Acceleration);
        }

        if (rig.velocity.magnitude < 5f || isGrounded || rig.velocity.y <= -9.81f)
        {
            StopWallrunning();
        }

        if (doJump)
        {
            StopWallrunning();

            rig.velocity = new Vector3(rig.velocity.x, 0f, rig.velocity.z);
            float magnitude = rig.velocity.magnitude;

            Vector3 camForwardDirWithoutY = new Vector3(cam.transform.forward.x, 0f, cam.transform.forward.z).normalized;
            Vector3 upVector = transform.up * wallrunJumpPowerY;
            Vector3 jumpDir = transform.forward * wallrunJumpPower;
            rig.velocity = jumpDir * magnitude;
            rig.AddForce(upVector, ForceMode.VelocityChange);

            doJump = false;
        }
    }

    private bool CheckWall(Vector3 to, out RaycastHit hit, float distance, ref Vector3 runDirection, bool isARightWall = false, int changeDirection = 1)
    {
        if (Physics.Raycast(transform.position + whereIsWall * .2f, to, out hit, 1, wallrunLayerMask))
        {
            if (Vector3.Dot(hit.normal, Vector3.up) >= minWallDotProduct && Vector3.Dot(hit.normal, Vector3.up) <= maxWallDotProduct)
            {
                runDirection = Vector3.Cross(hit.normal, Vector3.up) * changeDirection;
                whereIsWall = -hit.normal;
                this.isARightWall = isARightWall;
                distanceFromWall = distance;
                this.changeDirection = changeDirection;
                return true;
            }
        }
        return false;
    }

    private void StopWallrunning()
    {
        state = State.Walk;
        lastNormal = -whereIsWall;
        timeToNextWallrun = timeToNextWallrunMax;
        rig.useGravity = true;
        StartCoroutine(ResetLastNormal(timeToResetLastNormal));
    }

    IEnumerator ResetLastNormal(float time)
    {
        yield return new WaitForSeconds(time);
        lastNormal = Vector3.zero;
    }
#endregion

    private void StepSounds()
    {
        if(isGrounded && state == State.Walk && rig.velocity.magnitude > 2f)
        {
            timerStepsOnGround += Time.deltaTime;
            if(Vector3.Distance(transform.position, lastStepOnGroundPos) > 2.5f)
            {
                MakeGroundStep();
            }
        }
        if(!isGrounded && state == State.Wallrun)
        {
            timerStepsOnWall += Time.deltaTime;
            if (Vector3.Distance(transform.position, lastStepOnWallPos) > 2.5f)
            {
                MakeWallrunStep();
            }
        }

        if(timerStepsOnGround >= timeBetweenStepsOnGround)
        {
            MakeGroundStep();
        }
        if(timerStepsOnWall >= timeBetweenStepsOnWall)
        {
            MakeWallrunStep();
        }
    }

    private void MakeGroundStep()
    {
        SoundManager.Instance.Play("StepOnGround");
        lastStepOnGroundPos = transform.position;
        timerStepsOnGround = 0f;
    }
    private void MakeWallrunStep()
    {
        SoundManager.Instance.Play("StepOnWall");
        lastStepOnWallPos = transform.position;
        timerStepsOnWall = 0f;
    }

    private void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }

    private void OnCollisionEnter(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;

            if (normal.y >= minGroundDotProduct)
            {
                CameraGoDown(collision, i);
            }

        }
    }

    private void CameraGoDown(Collision collision, int i)
    {
        relY = collision.relativeVelocity.y;

        //Debug.Log(relY);
        if (relY > 0)
        {
            float targetOffset = relY * .1f;
            targetOffset = Mathf.Clamp(targetOffset, 0f, 3.5f);
            //Debug.Log(targetOffset);
            playerLookController.SetTargetOffset(targetOffset);
        }

        if (relY >= 10f)
        {
            landingParticle.Play();

            if (SoundManager.Instance)
                SoundManager.Instance.Play("Fall");
        }
    }

    public void Damage(float damage)
    {
        health -= damage;
        OnPlayerDamage?.Invoke(this, EventArgs.Empty);
        if (!AmIStillAlive())
        {
            Debug.Log("You Are Dead!!!");
            SoundManager.Instance.Play("Death");
            if (canDie)
            {
                PlayerUI.Instance.Restart();
            }
        }
        else
        {
            SoundManager.Instance.Play("PlayerDamage");
        }
    }

    private bool AmIStillAlive()
    {
        return health <= 0f ? false : true;
    }

    private void OnCollisionExit()
    {
        isGrounded = false;
    }

    private void EvaluateCollision(Collision collision)
    {
        int goodPoint = 0;

        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;

            if (normal.y >= minGroundDotProduct)
            {
                isGrounded = true;

                goodPoint = i;

                if (state == State.Walk)
                {
                    Vector3 planeDir = FindPlaneDir(normal);
                    float playerMovementDir = Vector3.Dot(planeDir.normalized, rig.velocity.normalized);
                    if (playerMovementDir < -.99f)
                    {
                        if (PlayerInputController.moveInputX == 0 && PlayerInputController.moveInputY == 0)
                        {
                            rig.velocity = Vector3.zero;
                            rig.AddForce(planeDir * Time.deltaTime);
                        }
                    }
                }
            }

        }

        if (isLastFrameGrounded == false && isGrounded == true)
        {
            CameraGoDown(collision, goodPoint);
            StartCoroutine(ResetLastNormal(0f));
        }
    }

    private Vector3 FindPlaneDir(Vector3 normal)
    {
        Vector3 right = Vector3.Cross(transform.up, normal);
        Vector3 planeDir = Vector3.Cross(normal, right);
        return planeDir;
    }

    public float GetMaxSpeed()
    {
        return maxSpeed;
    }

    public bool GetIsGrounded()
    {
        return isGrounded;
    }

    public float FindDifference(float nr1, float nr2)
    {
        return Math.Abs(nr1 - nr2);
    }
}
