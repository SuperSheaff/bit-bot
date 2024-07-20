using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public GameSettings gameSettings;

    public StateMachine<GameController> stateMachine;
    public GameStartState gameStartState;
    public GameIntroState gameIntroState;
    public GamePlayState gamePlayState;
    public GamePauseState gamePauseState;
    public GameEndState gameEndState;

    public PlayerController player;

    private void Awake()
    {
        // Singleton pattern implementation
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeStateMachine();
    }

    private void InitializeStateMachine()
    {
        stateMachine = new StateMachine<GameController>(true);

        gameStartState  = new GameStartState(this);
        gameIntroState  = new GameIntroState(this);
        gamePlayState   = new GamePlayState(this);
        gamePauseState  = new GamePauseState(this);
        gameEndState    = new GameEndState(this);

        if (gameSettings.skipIntro)
        {
            stateMachine.Initialize(gamePlayState);
        }
        else
        {
            stateMachine.Initialize(gameStartState);
        }
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }
}
