// Abstract base class representing a state in a state machine
public abstract class State<T>
{
    protected T entity; // The entity associated with this state

    // Constructor to initialize the state with the associated entity
    public State(T entity)
    {
        this.entity = entity;
    }

    // Called when the state is entered
    public virtual void Enter() {}

    // Called every frame to update the state
    public virtual void Update() {}

    public virtual void LateUpdate() {}

    // Called every fixed frame to update the state
    public virtual void FixedUpdate() {}

    // Called when the state is exited
    public virtual void Exit() {}

    // Virtual method to handle animation events
    public virtual void OnAnimationEvent(string eventName) {}
}