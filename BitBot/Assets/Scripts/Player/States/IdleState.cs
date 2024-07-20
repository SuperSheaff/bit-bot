using UnityEngine;

// State representing the player being idle
public class IdleState : GroundedState
{
    // Constructor for the IdleState
    public IdleState(PlayerController player) : base(player) {}

    // Called when the state is entered
    public override void Enter()
    {
        base.Enter();
        player.animator.SetBool("isIdle", true);
    }

    // Called every frame to update the state
    public override void Update()
    {
        base.Update();

        // Transition to running state if there is movement input
        if (player.inputHandler.Move != Vector2.zero)
        {
            player.stateMachine.ChangeState(player.runningState);
        }
    }

    // Called when the state is exited
    public override void Exit()
    {
        base.Exit();
        player.animator.SetBool("isIdle", false);
    }
}
