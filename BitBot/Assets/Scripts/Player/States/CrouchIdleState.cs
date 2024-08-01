using UnityEngine;

// State representing the player being idle
public class CrouchIdleState : GroundedState
{
    private bool isLeavingCrouch;

    // Constructor for the IdleState
    public CrouchIdleState(PlayerController player) : base(player) {}

    // Called when the state is entered
    public override void Enter()
    {
        base.Enter();
        player.animator.SetBool("isCrouchIdle", true);
        player.SetCrouchCollider(true);

        player.IsSneaking = true;
        
        isLeavingCrouch = true;
    }

    // Called every frame to update the state
    public override void Update()
    {
        base.Update();

        if (player.inputHandler.Move != Vector2.zero)
        {
            isLeavingCrouch = false;
            player.stateMachine.ChangeState(player.crouchMovingState);
        }
        else if (!player.inputHandler.Crouch && !player.IsBlockedAbove())
        {
            isLeavingCrouch = true;
            player.stateMachine.ChangeState(player.idleState);
            player.SetCrouchCollider(false);
        }
    }

    // Called when the state is exited
    public override void Exit()
    {
        base.Exit();
        player.animator.SetBool("isCrouchIdle", false);

        if (isLeavingCrouch)
        {
            player.IsSneaking = false;
        }
    }
}
