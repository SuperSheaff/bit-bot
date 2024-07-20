using UnityEngine;

// State representing the player being grounded
public class GroundedState : ControlState
{
    // Constructor for the GroundedState
    public GroundedState(PlayerController player) : base(player) {}

    private float notGroundedTime;

    // Called when the state is entered
    public override void Enter() 
    {
        base.Enter();
        player.ResetJumpCount();

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
        else if (!player.IsGrounded())
        {
            notGroundedTime += Time.deltaTime;
            if (notGroundedTime >= 0.08f)
            {
                player.stateMachine.ChangeState(player.inAirState);
            }
        }
        else
        {
            notGroundedTime = 0f;
        }
    }

    // Called every fixed frame to update the state
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    // Called when the state is exited
    public override void Exit() 
    {
        base.Exit();
    }
}
