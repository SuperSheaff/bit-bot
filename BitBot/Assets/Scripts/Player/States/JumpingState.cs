using UnityEngine;

// State representing the player initiating a jump
public class JumpingState : ControlState
{
    // Constructor for the JumpingState
    public JumpingState(PlayerController player) : base(player) {}

    // Called when the state is entered
    public override void Enter()
    {
        base.Enter();
        player.velocity.y = player.settings.jumpForce;
        player.StartJump();
        player.stateMachine.ChangeState(player.inAirState);
        SoundManager.instance?.PlaySound("Jump", player.transform);
    }

    // Called every frame to update the state
    public override void Update()
    {
        base.Update();
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
