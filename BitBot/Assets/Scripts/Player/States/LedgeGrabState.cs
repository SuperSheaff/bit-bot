using UnityEngine;

public class LedgeGrabState : PlayerState
{

    public LedgeGrabState(PlayerController player) : base(player) {}

    public override void Enter()
    {
        base.Enter();
        player.animator.SetBool("isLedgeGrabbing", true);
        SoundManager.instance?.PlaySound("Step", player.transform);
    }

    public override void Update()
    {
        base.Update();

        if (player.inputHandler.Jump)
        {
            player.stateMachine.ChangeState(player.jumpingState);
        }
        else if (player.inputHandler.Crouch) // Move down to let go of the ledge
        {
            player.stateMachine.ChangeState(player.inAirState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.animator.SetBool("isLedgeGrabbing", false);
    }
}
