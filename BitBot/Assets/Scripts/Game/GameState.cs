using UnityEngine;

public abstract class GameState : State<GameController>
{
    protected GameController gameController;

    public GameState(GameController gameController) : base(gameController)
    {
        this.gameController = gameController;
    }
}
