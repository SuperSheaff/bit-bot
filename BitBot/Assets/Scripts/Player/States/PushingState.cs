using System.Collections;
using UnityEngine;

public class PushingState : PlayerState
{
    private float pushCooldown = 0.2f; // Cooldown time in seconds
    private float lastPushTime; // The time when the player last entered the push state
    private PushableObject currentPushableObject;
    private bool isSoundPlaying = false;

    public PushingState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        player.animator.SetBool("isPushing", true);
        lastPushTime = Time.time; // Record the time when the player enters the push state

        if (player.currentPushHandle != null)
        {
            currentPushableObject = player.currentPushHandle.pushableObject;
            currentPushableObject.StartPushing();

            Vector3 lockPosition = player.currentPushHandle.playerLockPosition.position;
            player.transform.position = lockPosition;
            player.transform.rotation = Quaternion.LookRotation(player.currentPushHandle.transform.forward);
        }
    }

    public override void Update()
    {
        base.Update();

        HandlePushMovement();
        UpdatePushAnimation();

        // Exit the pushing state only if the push button is pressed again and the cooldown has passed
        if (player.inputHandler.Push && Time.time - lastPushTime > pushCooldown)
        {
            player.stateMachine.ChangeState(player.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.animator.SetBool("isPushing", false);
        player.SetLastPushTime();
        if (currentPushableObject != null)
        {
            currentPushableObject.StopPushing();
            currentPushableObject = null;
        }

        player.animator.speed = 1f; // Set animation speed to normal
        StopSlidingSound(); // Ensure the sound is stopped when exiting the state
    }

    private void HandlePushMovement()
    {
        if (player.currentPushHandle == null) return;

        Vector2 moveInput = player.inputHandler.Move;
        if (moveInput != Vector2.zero)
        {
            Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);

            // Calculate movement direction based on camera's perspective
            Vector3 forward = player.cameraTransform.forward;
            Vector3 right = player.cameraTransform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            Vector3 desiredDirection = forward * moveDirection.z + right * moveDirection.x;
            desiredDirection = Vector3.Project(desiredDirection, player.currentPushHandle.transform.forward); // Constrain movement to the push direction

            bool canMoveForward = true;
            bool canMoveBackward = true;

            // Raycast to check for obstacles in the forward direction
            if (moveDirection.z > 0) // Only check when moving forward
            {
                canMoveForward = !CheckForObstacle(Vector3.forward);
                canMoveForward &= CheckForGround(Vector3.forward);
            }
            else if (moveDirection.z < 0) // Check when moving backward
            {
                canMoveBackward = CheckForGround(Vector3.back, player.settings.pushDistanceCheck * 1.5f); // Closer distance for backward check
            }

            if (canMoveForward && moveDirection.z > 0)
            {
                player.controller.Move(desiredDirection * player.settings.pushingMoveSpeed * Time.deltaTime);
                currentPushableObject.transform.position += desiredDirection * player.settings.pushingMoveSpeed * Time.deltaTime;

                if (!isSoundPlaying)
                {
                    PlaySlidingSound();
                }
            }
            else if (canMoveBackward && moveDirection.z < 0)
            {
                player.controller.Move(desiredDirection * player.settings.pushingMoveSpeed * Time.deltaTime);
                currentPushableObject.transform.position += desiredDirection * player.settings.pushingMoveSpeed * Time.deltaTime;

                if (!isSoundPlaying)
                {
                    PlaySlidingSound();
                }
            }
            else
            {
                // Stop animation if can't move
                player.animator.SetFloat("pushingDirection", 0.0f);
                player.animator.speed = 0f; // Set animation speed to very low
                StopSlidingSound();
            }
        }
        else
        {
            // Stop sliding sound if player stops moving
            StopSlidingSound();
        }
    }

    private void UpdatePushAnimation()
    {
        Vector2 moveInput = player.inputHandler.Move;
        if (moveInput != Vector2.zero)
        {
            Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;

            // Calculate desired movement direction based on camera's perspective
            Vector3 forward = player.cameraTransform.forward;
            Vector3 right = player.cameraTransform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            Vector3 desiredDirection = forward * moveDirection.z + right * moveDirection.x;

            // Update animation only if allowed to move
            bool canMoveForward = moveDirection.z > 0 ? !CheckForObstacle(Vector3.forward) && CheckForGround(Vector3.forward) : true;
            bool canMoveBackward = moveDirection.z < 0 ? CheckForGround(Vector3.back, player.settings.pushDistanceCheck * 1.5f) : true;

            if (canMoveForward && moveDirection.z > 0)
            {
                player.animator.SetFloat("pushingDirection", 1f);
                player.animator.speed = 1f; // Set animation speed to normal
            }
            else if (canMoveBackward && moveDirection.z < 0)
            {
                player.animator.SetFloat("pushingDirection", -1f);
                player.animator.speed = 1f; // Set animation speed to normal
            }
            else
            {
                player.animator.SetFloat("pushingDirection", 0.0f);
                player.animator.speed = 0f; // Set animation speed to very low
            }
        }
        else
        {
            player.animator.SetFloat("pushingDirection", 0.6f);
            player.animator.speed = 0.1f; // Set animation speed to very low
        }
    }

    private bool CheckForObstacle(Vector3 direction)
    {
        RaycastHit hit;
        Vector3 rayOrigin = player.currentPushHandle.transform.position;
        Vector3 rayDirection = player.currentPushHandle.transform.TransformDirection(direction);
        return Physics.Raycast(rayOrigin, rayDirection, out hit, player.settings.pushDistanceCheck, player.groundLayer);
    }

    private bool CheckForGround(Vector3 direction, float distance = -1)
    {
        RaycastHit hit;
        Vector3 rayOrigin = player.currentPushHandle.transform.position + direction * (distance > 0 ? distance : player.settings.pushDistanceCheck);
        Vector3 rayDirection = Vector3.down;
        return Physics.Raycast(rayOrigin, rayDirection, out hit, player.settings.pushDistanceCheck, player.groundLayer);
    }

    private void PlaySlidingSound()
    {
        SoundManager.instance?.PlaySound("BIT_CRATE", player.transform);
        isSoundPlaying = true;
    }

    private void StopSlidingSound()
    {
        if (isSoundPlaying)
        {
            SoundManager.instance?.StopSound("BIT_CRATE");
            isSoundPlaying = false;
        }
    }
}
