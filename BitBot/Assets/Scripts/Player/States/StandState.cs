using UnityEngine;

// State representing the player being grounded
public class StandState : PlayerState
{
    // Constructor for the StandState
    public StandState(PlayerController player) : base(player) {}

    // Called when the state is entered
    public override void Enter() 
    {
        base.Enter();

        player.animator.SetBool("isStand", true);
    }

    // Called every frame to update the state
    public override void Update()
    {
        if (player.IsIntroFinished)
        {
            player.animator.speed = 1;
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

        player.animator.SetBool("isStand", false);
    }

    // Triggers an animation event
    public override void OnAnimationEvent(string eventName)
    {
        switch (eventName)
        {
            case "Pause":
                player.animator.speed = 0;
                break;
        }
    }
}
