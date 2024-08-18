using UnityEngine;

public class GameStartState : GameState
{
    public GameStartState(GameController gameController) : base(gameController) { }

    public override void Enter()
    {
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
    }
}

public class GameIntroState : GameState
{
    public GameIntroState(GameController gameController) : base(gameController) { }

    public override void Enter()
    {
        Debug.Log("Game Intro State: Enter");
        CameraController.instance.StartHandleIntroSequence();
    }

    public override void Update()
    {
        // Handle game Intro state updates here
    }

    public override void Exit()
    {
        Debug.Log("Game Start State: Exit");
        gameController.player.stateMachine.ChangeState(gameController.player.idleState);
    }
}

public class GamePlayState : GameState
{
    public GamePlayState(GameController gameController) : base(gameController) { }

    public override void Enter()
    {
        Debug.Log("Game Play State: Enter");
        gameController.StartTimer();
        // Initialize game play logic here
    }

    public override void Update()
    {
        // Handle game play state updates here
    }

    public override void Exit()
    {
        Debug.Log("Game Play State: Exit");
        // Cleanup logic if any
    }
}

public class GamePauseState : GameState
{
    public GamePauseState(GameController gameController) : base(gameController) { }

    public override void Enter()
    {
        Debug.Log("Game Pause State: Enter");
        // Initialize game pause logic here
    }

    public override void Update()
    {
        // Handle game pause state updates here
    }

    public override void Exit()
    {
        Debug.Log("Game Pause State: Exit");
        // Cleanup logic if any
    }
}

public class GameEndState : GameState
{
    public GameEndState(GameController gameController) : base(gameController) { }

    public override void Enter()
    {
        Debug.Log("Game End State: Enter");
        // Initialize game end logic here
    }

    public override void Update()
    {
        // Handle game end state updates here
    }

    public override void Exit()
    {
        Debug.Log("Game End State: Exit");
        // Cleanup logic if any
    }
}
