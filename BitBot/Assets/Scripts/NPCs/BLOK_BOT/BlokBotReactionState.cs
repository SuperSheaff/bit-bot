using System.Collections;
using UnityEngine;

public class BlokBotReactionState : BlokBotState
{
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
}
