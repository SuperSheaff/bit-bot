using UnityEngine;

public abstract class BlokBotState : State<BlokBotController>
{
    protected BlokBotController blokBot;

    public BlokBotState(BlokBotController blokBot) : base(blokBot)
    {
        this.blokBot = blokBot;
    }

    // Method to be called from the animator
    public override void OnAnimationEvent(string eventName) {}
}
