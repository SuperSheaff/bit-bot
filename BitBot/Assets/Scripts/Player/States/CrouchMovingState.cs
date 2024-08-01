using UnityEngine;

public class CrouchMovingState : GroundedState
{

    private bool isLeavingCrouch;

    public CrouchMovingState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        player.animator.SetBool("isCrouchMoving", true);
        player.SetCrouchCollider(true);

        player.IsSneaking = true;
        
        isLeavingCrouch = true;
    }

    public override void Update()
    {
        base.Update();

        if (player.inputHandler.Move == Vector2.zero)
        {
            isLeavingCrouch = false;
            player.stateMachine.ChangeState(player.crouchIdleState);
        }
        else if (!player.inputHandler.Crouch && !player.IsBlockedAbove())
        {
            isLeavingCrouch = true;
            player.stateMachine.ChangeState(player.runningState);
            player.SetCrouchCollider(false);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        HandleCrouchMovement();
    }

    private void HandleCrouchMovement()
    {
        Vector2 moveInput = player.inputHandler.Move;
        if (moveInput != Vector2.zero)
        {
            Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);

            Vector3 forward = player.cameraTransform.forward;
            Vector3 right = player.cameraTransform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            Vector3 desiredDirection = forward * moveDirection.z + right * moveDirection.x;
            desiredDirection.Normalize();

            player.controller.Move(desiredDirection * player.settings.crouchMoveSpeed * Time.deltaTime);
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.animator.SetBool("isCrouchMoving", false);

        if (isLeavingCrouch)
        {
            player.IsSneaking = false;
        }
    }
}
