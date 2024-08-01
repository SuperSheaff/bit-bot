using UnityEngine;
using System.Collections;

public class RespawnState : PlayerState
{
    public RespawnState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        player.animator.SetBool("isRespawning", true);
        player.Respawn(); // Custom respawn logic

    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
        player.animator.SetBool("isRespawning", false);
    }

    // Triggers an animation event
    public override void OnAnimationEvent(string eventName)
    {
        switch (eventName)
        {
            case "Idle":
                player.stateMachine.ChangeState(player.idleState); // Transition to idle state after respawn
                break;
        }
    }
}
