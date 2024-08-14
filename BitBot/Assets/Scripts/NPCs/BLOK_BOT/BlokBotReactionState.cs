using System.Collections;
using UnityEngine;

public class BlokBotReactionState : BlokBotState
{
    private const float reactionDuration = 2.0f; // Set a duration for the reaction state failsafe

    private Coroutine reactionTimeoutCoroutine;

    public BlokBotReactionState(BlokBotController blokBot) : base(blokBot) { }

    public override void Enter()
    {
        base.Enter();
        blokBot.animator.SetBool("isReacting", true);
        SoundManager.instance.PlaySound("blok_bot_alert", blokBot.transform);

        if (blokBot.alertParticles != null)
        {
            blokBot.StartCoroutine(PlayParticlesWithDelay(0.1f));
        }

        // Start the failsafe timer
        reactionTimeoutCoroutine = blokBot.StartCoroutine(FailsafeTimer(reactionDuration));
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
        blokBot.animator.SetBool("isReacting", false);

        if (blokBot.alertParticles != null)
        {
            blokBot.alertParticles.Stop();
        }

        // Stop the failsafe timer if the state exits early
        if (reactionTimeoutCoroutine != null)
        {
            blokBot.StopCoroutine(reactionTimeoutCoroutine);
        }
    }

    // Triggers an animation event
    public override void OnAnimationEvent(string eventName)
    {
        switch (eventName)
        {
            case "ReactFinish":
                blokBot.stateMachine.ChangeState(blokBot.blockingState);
                break;
        }
    }

    private IEnumerator PlayParticlesWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        blokBot.alertParticles.Play();
    }

    private IEnumerator FailsafeTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (blokBot.stateMachine.CurrentState == this)
        {
            blokBot.stateMachine.ChangeState(blokBot.blockingState);
            Debug.LogWarning("Failsafe triggered: Reaction state timed out.");
        }
    }
}