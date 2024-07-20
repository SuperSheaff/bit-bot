using UnityEngine;

// State machine class to manage states of a generic entity
public class StateMachine<T>
{
    public State<T> CurrentState { get; private set; } // Current state of the state machine
    private readonly bool debugMode; // Flag to enable or disable debug mode

    // Constructor to initialize the state machine with optional debug mode
    public StateMachine(bool debugMode = false)
    {
        this.debugMode = debugMode;
    }

    // Initialize the state machine with the starting state
    public void Initialize(State<T> startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();

        if (debugMode)
        {
            Debug.Log("Initialized with state: " + startingState.GetType().Name);
        }
    }

    // Change the current state to a new state
    public void ChangeState(State<T> newState)
    {
        if (debugMode)
        {
            Debug.Log("Changing state from: " + CurrentState.GetType().Name + " to: " + newState.GetType().Name);
        }

        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    // Update the current state
    public void Update()
    {
        CurrentState.Update();
    }

    // Fixed update for the current state
    public void FixedUpdate()
    {
        CurrentState.FixedUpdate();
    }
}
