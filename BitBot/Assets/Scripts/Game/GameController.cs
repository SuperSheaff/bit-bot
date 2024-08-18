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

    public float playerTime { get; private set; } // The player's time
    public int secretScore { get; private set; } // The number of secrets found
    private bool isTimerRunning = false;

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

        if (isTimerRunning)
        {
            playerTime += Time.deltaTime; // Increment the timer by the time elapsed since the last frame
        }
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    public void StartTimer()
    {
        playerTime = 0f; // Reset the timer
        isTimerRunning = true; // Start the timer
    }

    public void StopTimer()
    {
        isTimerRunning = false; // Stop the timer
    }

    public void ResetTimer()
    {
        playerTime = 0f; // Reset the timer to 0
    }

    public void AddSecret()
    {
        secretScore++; // Increment the secret score
    }
}
