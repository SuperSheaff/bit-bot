using UnityEngine;

// State representing the player being grounded
public class ControlState : PlayerState
{
    // Constructor for the ControlState
    public ControlState(PlayerController player) : base(player) {}

    // Called when the state is entered
    public override void Enter() 
    {
        base.Enter();
    }

    // Called every frame to update the state
    public override void Update()
    {
        base.Update();

        player.HandleRotation();
        player.HandleGravity();
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
