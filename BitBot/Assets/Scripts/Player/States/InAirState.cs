using UnityEngine;

// State representing the player being in the air (jumping or falling)
public class InAirState : ControlState
{

    private Vector3 ledgePosition;
    private Vector3 ledgeNormal;

    // Constructor for the InAirState
    public InAirState(PlayerController player) : base(player) {}

    // Called when the state is entered
    public override void Enter() 
    {
        base.Enter();
        player.animator.SetBool("isJumping", true);
    }

    // Called every frame to update the state
    public override void Update()
    {
        base.Update();

        if (player.CanJump() && player.JumpEnabled)
        {
            player.ResetJumpBuffer();
            player.stateMachine.ChangeState(player.jumpingState);
        }
        // Transition to idle state if player is grounded
        else if (player.velocity.y <= 0 && player.IsGrounded())
        {
            player.stateMachine.ChangeState(player.idleState);
        }
        else if (player.velocity.y < 0)
        {
            if (player.HandleLedgeGrab() && player.LedgeGrabEnabled)
            {
                player.stateMachine.ChangeState(player.ledgeGrabState);
            }
        }

    }

    // Called every fixed frame to update the state
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        HandleMovement();
    }

    // Handles movement while the player is in the air
    private void HandleMovement()
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

            player.controller.Move(desiredDirection * player.settings.jumpMoveSpeed * Time.deltaTime);
        }

        if (player.inputHandler.JumpReleased)
        {
            player.EndJump();
        }
    }

    // Called when the state is exited
    public override void Exit()
    {
        base.Exit();
        player.EndJump();
        player.animator.SetBool("isJumping", false);
    }
}
