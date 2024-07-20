using UnityEngine;

// State representing the player being grounded
public class EndState : PlayerState
{
    // Constructor for the EndState
    public EndState(PlayerController player) : base(player) {}

    // Called when the state is entered
    public override void Enter() 
    {
        base.Enter();
        player.animator.SetBool("isIdle", true);
    }

    // Called every frame to update the state
    public override void Update()
    {
    }

    // Called every fixed frame to update the state
    public override void FixedUpdate()
    {
    }

    // Called when the state is exited
    public override void Exit() 
    {
    }
}
