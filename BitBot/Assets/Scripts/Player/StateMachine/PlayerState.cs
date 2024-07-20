// Abstract class for player states, inheriting from the base State class
public abstract class PlayerState : State<PlayerController>
{
    protected PlayerController player; // Reference to the player controller
    
    // Constructor for the PlayerState
    public PlayerState(PlayerController player) : base(player)
    {
        this.player = player;
    }

    // Method to be called from the animator
    public override void OnAnimationEvent(string eventName) {}
}
