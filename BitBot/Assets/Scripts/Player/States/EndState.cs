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
        player.animator.SetBool("isUpgrading", true);
        player.screenAnimator.SetActive(false);
    }

    // Called every frame to update the state
    public override void Update()
    {
        if (player.inputHandler.Pause)
        {
            Application.Quit();
        }
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
