using UnityEngine;

// State representing the player running
public class RunningState : GroundedState
{
    // Constructor for the RunningState
    public RunningState(PlayerController player) : base(player) {}

    // Called when the state is entered
    public override void Enter()
    {
        base.Enter();
        player.animator.SetBool("isRunning", true);
    }

    // Called every frame to update the state
    public override void Update()
    {
        base.Update();

        // Transition to idle state if there is no movement input
        if (player.inputHandler.Move == Vector2.zero)
        {
            player.stateMachine.ChangeState(player.idleState);
        }
    }

    // Called every fixed frame to update the state
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        
        HandleMovement();
    }

    // Handles movement while the player is running
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

            player.controller.Move(desiredDirection * player.settings.moveSpeed * Time.deltaTime);
        }
    }

    // Called when the state is exited
    public override void Exit()
    {
        base.Exit();
        player.animator.SetBool("isRunning", false);
    }
}
