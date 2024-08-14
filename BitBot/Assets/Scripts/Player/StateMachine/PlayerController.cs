using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region State Variables

    public StateMachine<PlayerController> stateMachine;
    public StartState startState;
    public StandState standState;
    public IdleState idleState;
    public PauseState pauseState;
    public RunningState runningState;
    public JumpingState jumpingState;
    public InAirState inAirState;
    public EndState endState;
    public CrouchIdleState crouchIdleState;
    public CrouchMovingState crouchMovingState;
    public PushingState pushingState;
    public LedgeGrabState ledgeGrabState;
    public DeathState deathState;
    public RespawnState respawnState;
    public UpgradeState upgradeState;

    #endregion

    #region Component Variables

    public Animator animator;
    public CharacterController controller;
    public GameSettings settings;
    public PlayerInputHandler inputHandler;
    public Transform cameraTransform;
    public Transform headTransform;
    public Material meshMaterial; // Assign this in the inspector
    public Renderer playerRenderer; // Assign this in the inspector


    #endregion

    #region Movement Variables

    public bool IsAlive             = true;

    public bool JumpEnabled         = false;
    public bool SneakEnabled        = false;
    public bool PushEnabled         = false;
    public bool LedgeGrabEnabled    = false;

    private int currentSneakSoundIndex = 0;

    public Vector3 velocity;
    private SphereCollider groundCheckCollider; // Collider for ground check
    public float groundCheckRadius = 10f; // Radius of the sphere used to check for the ground
    public LayerMask groundLayer; // Layer mask to define what is considered ground

    public Vector3 ledgePosition;
    public Vector3 ledgeDetectedPosition;
    private bool isLedgeDetected;
    private Vector3 respawnPosition;
    private bool isGrounded;
    private float jumpBufferCounter;
    private float coyoteTimeCounter;
    private float jumpTimeCounter;
    private bool isJumping;
    private int jumpCount;
    private bool isCrouching;
    private Vector3 originalCenter;
    private float originalHeight;
    private float crouchHeight = 1f;
    private float headCheckDistance = 0.5f;
    public float pushCooldown = 0.5f; // Cooldown time in seconds
    public float lastPushTime; // Time of the last push state exit
    private float pushCheckRadius = 1f;
    public LayerMask pushableLayer;
    public PushHandle currentPushHandle;
    public Vector3 ledgeOffset = new Vector3(0f, -1f, 0.5f); // Adjustable offset for the ledge grab position

    public bool IsSneaking = false;

    public GameObject pushableObject;

    #endregion

    #region State Flags

    public bool IsIntroFinished = false;
    
    #endregion


    #region Unity Callbacks

    private void Start()
    {
        InitializeStateMachine();
        InitializeCursor();
        InitializeSound();
        InitializeChecks();

        // Assuming the player has a MeshRenderer component
        if (playerRenderer != null)
        {
            meshMaterial = playerRenderer.material;
        }

        if (settings.debugMode)
        {
            Debug.Log("PlayerController initialized.");
        }
    }

    void Update()
    {
        HandleJumpBuffer();
        HandleCoyoteTime();
        stateMachine.Update();
    }

    void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    void LateUpdate()
    {
        stateMachine.LateUpdate();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, groundCheckRadius);

        // Draw ground check sphere
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + Vector3.down * 0.5f, 0.1f);

        // Draw ledge check ray
        Gizmos.color = Color.blue;
        Vector3 origin = transform.position + Vector3.up * 2.0f;
        Vector3 direction = transform.forward * 1.5f;
        Gizmos.DrawRay(origin, direction);

        if (isLedgeDetected)
        {
            // Draw detected ledge position
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(ledgeDetectedPosition, 0.1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PushHandle handle = other.GetComponent<PushHandle>();
        if (handle != null && handle.pushableObject != null)
        {
            currentPushHandle = handle;
        }
        if (other.CompareTag("Deadly"))
        {
            stateMachine.ChangeState(deathState);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PushHandle handle = other.GetComponent<PushHandle>();
        if (handle != null && handle.pushableObject != null)
        {
            currentPushHandle = null;
        }
    }

    #endregion

    #region Initialization Functions

    private void InitializeStateMachine()
    {
        CalculateJumpParameters();
        stateMachine        = new StateMachine<PlayerController>(settings.debugMode);

        startState          = new StartState(this);
        standState          = new StandState(this);
        idleState           = new IdleState(this);
        pauseState          = new PauseState(this);
        runningState        = new RunningState(this);
        jumpingState        = new JumpingState(this);
        inAirState          = new InAirState(this);
        endState            = new EndState(this);
        crouchIdleState     = new CrouchIdleState(this);
        crouchMovingState   = new CrouchMovingState(this);
        pushingState        = new PushingState(this);
        ledgeGrabState      = new LedgeGrabState(this);
        deathState          = new DeathState(this);
        respawnState        = new RespawnState(this);
        upgradeState        = new UpgradeState(this);

        if(settings.skipIntro)
        {
            stateMachine.Initialize(idleState);
        }
        else
        {
            stateMachine.Initialize(startState);
        }
    }

    private void InitializeCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void InitializeSound()
    {
        SoundManager.instance?.PlaySound("Music");
    }

    private void InitializeChecks()
    {
        groundCheckCollider = gameObject.AddComponent<SphereCollider>();
        groundCheckCollider.isTrigger = true;
        groundCheckCollider.radius = groundCheckRadius;
        groundCheckCollider.center = new Vector3(0, 0, 0);

        originalCenter = controller.center;
        originalHeight = controller.height;
    }

    // Calculates the jump parameters based on settings
    private void CalculateJumpParameters()
    {
        settings.gravity = -2 * settings.jumpHeight / Mathf.Pow(settings.jumpDuration, 2);
        settings.jumpForce = 2 * settings.jumpHeight / settings.jumpDuration;
    }

    #endregion

    #region Movement Functions

    // Checks if the player is grounded
    public bool IsGrounded()
    {
        // Use a sphere cast to check for ground
        Collider[] colliders = Physics.OverlapSphere(transform.position + groundCheckCollider.center, groundCheckCollider.radius, groundLayer);
        return controller.isGrounded || colliders.Length > 0;
    }

    // Handles gravity for the player
    public void HandleGravity()
    {
        isGrounded = IsGrounded();

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small negative value to keep the player grounded
        }

        if (velocity.y < 0)
        {
            velocity.y += settings.extraFallGravity * Time.deltaTime;
        }
        else if (isJumping && inputHandler.Jump && jumpTimeCounter > 0)
        {
            jumpTimeCounter -= Time.deltaTime;
            velocity.y += settings.jumpHoldGravity * Time.deltaTime;
        }
        else
        {
            velocity.y += settings.gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    // Handles rotation based on input
    public void HandleRotation()
    {
        Vector2 moveInput = inputHandler.Move;
        if (moveInput != Vector2.zero)
        {
            Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);

            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            Vector3 desiredDirection = forward * moveDirection.z + right * moveDirection.x;

            if (desiredDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(desiredDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * settings.rotationSpeed);
            }
        }
    }

    #endregion

    #region Jump Functions

    // Handles jump buffer logic
    private void HandleJumpBuffer()
    {
        if (inputHandler.Jump)
        {
            jumpBufferCounter = settings.jumpBufferTime;
        }
        else if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    // Handles coyote time logic
    private void HandleCoyoteTime()
    {
        if (isGrounded)
        {
            coyoteTimeCounter = settings.coyoteTime;
        }
        else if (coyoteTimeCounter > 0)
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    // Checks if the player can jump
    public bool CanJump()
    {
        if (settings.debugMode)
        {
            Debug.Log("Grounded:" + isGrounded);
            Debug.Log("Coyote:" + (coyoteTimeCounter > 0));
            Debug.Log("Jump Buffer:" + (jumpBufferCounter > 0));
        }
       
        return jumpCount > 0 && jumpBufferCounter > 0 && (isGrounded || coyoteTimeCounter > 0);
    }

    // Starts the jump process
    public void StartJump()
    {
        isJumping = true;
        jumpTimeCounter = settings.jumpHoldTime;
        jumpCount--;
    }

    // Ends the jump process
    public void EndJump()
    {
        isJumping = false;
    }

    // Resets the jump buffer
    public void ResetJumpBuffer()
    {
        jumpBufferCounter = 0f;
    }

    // Resets the jump count (base 1)
    public void ResetJumpCount()
    {
        jumpCount = 1;
    }

    // Enables jump functionality
    public void EnableJump()
    {
        JumpEnabled = true;
    }

    // Enables sneak functionality
    public void EnableSneak()
    {
        SneakEnabled = true;
    }

    // Enables push functionality
    public void EnablePush()
    {
        PushEnabled = true;
    }

    // Enables LedgeGrab functionality
    public void EnableLedgeGrab()
    {
        LedgeGrabEnabled = true;
    }

    #endregion

    #region Crouch Functions

    public void SetCrouchCollider(bool isCrouching)
    {
        if (isCrouching)
        {
            controller.height = crouchHeight;
            controller.center = new Vector3(controller.center.x, crouchHeight / 2, controller.center.z);
        }
        else
        {
            controller.height = originalHeight;
            controller.center = originalCenter;
        }
    }

    public bool IsBlockedAbove()
    {
        RaycastHit hit;
        Vector3 start = transform.position + Vector3.up * controller.height;
        return Physics.SphereCast(start, controller.radius, Vector3.up, out hit, headCheckDistance, groundLayer);
    }

    #endregion

    #region Push Functions

    // Checks if the player can push
    public bool CanPush()
    {
        return inputHandler.Push && currentPushHandle != null && Time.time - lastPushTime > pushCooldown && PushEnabled;
    }

    // Sets the pushable object
    public void SetPushableObject(GameObject obj)
    {
        pushableObject = obj;
    }

    // Gets the pushable object
    public GameObject GetPushableObject()
    {
        return pushableObject;
    }

    public void SetPushHandle(PushHandle handle)
    {
        currentPushHandle = handle;
    }

    public void SetLastPushTime()
    {
        lastPushTime = Time.time; // Update the last push time
    }

    // Checks if there is a pushable object nearby
    // private bool IsNearPushableObject()
    // {
    //     Collider[] colliders = Physics.OverlapSphere(transform.position, pushCheckRadius, pushableLayer);
    //     if (colliders.Length > 0)
    //     {
    //         SetPushableObject(colliders[0].gameObject);
    //         return true;
    //     }
    //     else
    //     {
    //         SetPushableObject(null);
    //         return false;
    //     }
    // }

    #endregion

    #region Ledge Grab Functions

    // public bool IsLedgeDetected(out Vector3 ledgePosition, out Vector3 ledgeNormal)
    // {
    //     float ledgeCheckDistance = 1.5f;
    //     float ledgeHeight = 2.0f;
    //     float ledgeDepth = 0.3f;

    //     Vector3 origin = transform.position + Vector3.up * ledgeHeight;
    //     Vector3 direction = transform.forward;

    //     RaycastHit hit;
    //     if (Physics.Raycast(origin, direction, out hit, ledgeCheckDistance))
    //     {
    //         Vector3 ledgeOrigin = hit.point + Vector3.up * ledgeDepth;
    //         if (Physics.Raycast(ledgeOrigin, -Vector3.up, out hit, ledgeDepth))
    //         {
    //             if (IsValidLedgePoint(hit.point, out ledgePosition))
    //             {
    //                 ledgeNormal = hit.normal;
    //                 return true;
    //             }
    //         }
    //     }

    //     ledgePosition = Vector3.zero;
    //     ledgeNormal = Vector3.zero;
    //     return false;
    // }

    // private bool IsValidLedgePoint(Vector3 point, out Vector3 validLedgePosition)
    // {
    //     float minDistanceFromSideVertices = 0.25f;

    //     if (Vector3.Distance(point, transform.position) > minDistanceFromSideVertices)
    //     {
    //         validLedgePosition = point;
    //         return true;
    //     }

    //     validLedgePosition = Vector3.zero;
    //     return false;
    // }

    // public void SetLedgePosition(Vector3 ledgePosition, Vector3 ledgeNormal)
    // {
    //     if (ledgePosition != Vector3.zero)
    //     {
    //         Vector3 adjustedLedgePosition = ledgePosition + ledgeOffset;
    //         transform.position = adjustedLedgePosition;
    //         // Ensure the player is facing directly into the wall (only adjusting y-rotation)
    //         // Vector3 forward = new Vector3(-ledgeNormal.x, 0, -ledgeNormal.z).normalized;
    //         // Quaternion targetRotation = Quaternion.LookRotation(forward, Vector3.up);
    //         // transform.rotation = targetRotation;
    //     }
    // }

    public bool HandleLedgeGrab()
    {
        RaycastHit downHit;
        Vector3 lineDownStart = (transform.position + Vector3.up * 1.5f) + transform.forward;
        Vector3 lineDownEnd  = (transform.position + Vector3.up * 0.7f) + transform.forward;
        Physics.Linecast(lineDownStart, lineDownEnd, out downHit, groundLayer);
        Debug.DrawLine(lineDownStart, lineDownEnd);

        if (LedgeGrabEnabled)
        {
            if (downHit.collider != null)
            {
                RaycastHit forwardHit;
                Vector3 lineForwardStart = new Vector3(transform.position.x, downHit.point.y - 0.1f, transform.position.z);
                Vector3 lineForwardEnd  = new Vector3(transform.position.x, downHit.point.y - 0.1f, transform.position.z) + transform.forward;
                Physics.Linecast(lineForwardStart, lineForwardEnd, out forwardHit, groundLayer);
                Debug.DrawLine(lineForwardStart, lineForwardEnd);

                if (forwardHit.collider != null) 
                {
                    Vector3 hangPosition = new Vector3(forwardHit.point.x, downHit.point.y, forwardHit.point.z);
                    Vector3 offset = transform.forward * -0.3f + transform.up * -2.3f;
                    hangPosition += offset;
                    transform.position = hangPosition;
                    transform.forward = -forwardHit.normal;

                    return true;
                }

                else
                {
                    return false;
                }
            }
            else 
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }


    #endregion

    #region Respawn Functions

    // Sets the respawn checkpoint
    public void SetCheckpoint(Vector3 checkpointPosition)
    {
        if (settings.debugMode)
        {
            Debug.Log("Checkpoint set to: " + checkpointPosition);
        }

        respawnPosition = checkpointPosition;
    }

    // Respawns the player
    public void Respawn()
    {
        StartCoroutine(RespawnCoroutine());
    }

    // Respawn coroutine
    private IEnumerator RespawnCoroutine()
    {
        controller.enabled = false; 
        transform.position = respawnPosition; 
        transform.rotation = Quaternion.Euler(0, -135, 0); // Set rotation to (0, -135, 0)
        yield return null;
        controller.enabled = true;
    }

    #endregion

    #region Miscellaneous Functions

    // Moves the player based on input
    public void Move()
    {
        Vector2 moveInput = inputHandler.Move;
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        controller.Move(move * Time.deltaTime * settings.moveSpeed);
    }

    // Triggers an animation event
    public void AnimationEvent(string eventName)
    {
        stateMachine.CurrentState.OnAnimationEvent(eventName);
    }

    public void StepSound()
    {
        SoundManager.instance?.PlaySound("Step", this.transform);
    }

    public void SneakStepSound()
    {
        string[] soundNames = { "SNEAK_01", "SNEAK_02", "SNEAK_03", "SNEAK_04" };

        // Play the current sound
        SoundManager.instance?.PlaySound(soundNames[currentSneakSoundIndex], this.transform);

        // Update the index to the next sound, looping back to the start if necessary
        currentSneakSoundIndex = (currentSneakSoundIndex + 1) % soundNames.Length;
    }

    #endregion
}
