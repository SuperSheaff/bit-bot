using UnityEngine;

// State representing the player being grounded
public class StartState : PlayerState
{
    // Constructor for the StartState
    public StartState(PlayerController player) : base(player) {}

    // Called when the state is entered
    public override void Enter() 
    {
        base.Enter();

        player.animator.SetBool("isStart", true);
    }

    // Called every frame to update the state
    public override void Update()
    {

        if (player.inputHandler.Jump)
        {
            player.stateMachine.ChangeState(player.standState);
            GameController.instance.stateMachine.ChangeState(GameController.instance.gameIntroState);
            // GameTimer.instance.StartTimer();
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

        player.animator.SetBool("isStart", false);
    }
}
