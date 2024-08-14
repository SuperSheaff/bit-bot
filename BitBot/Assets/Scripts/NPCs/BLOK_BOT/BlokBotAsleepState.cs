using UnityEngine;

public class BlokBotAsleepState : BlokBotState
{

    public BlokBotAsleepState(BlokBotController blokBot) : base(blokBot) { }

    public override void Enter()
    {
        base.Enter();

        if (blokBot.animator != null)
        {
            blokBot.animator.SetBool("isSleeping", true);
        }

        if (blokBot.barrier != null)
        {
            blokBot.barrier.SetActive(false);
        }

        if (blokBot.sleepingParticles != null)
        {
            blokBot.sleepingParticles.Play();
        }

        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlaySound("blok_bot_snore", blokBot.transform);
        }
    }

    public override void Update()
    {
        base.Update();

        if (blokBot.isPlayerInDetectionZone && !blokBot.playerController.IsSneaking)
        {
            blokBot.stateMachine.ChangeState(blokBot.reactionState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        blokBot.animator.SetBool("isSleeping", false);
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlaySound("blok_bot_snore", blokBot.transform);
        }

        if (blokBot.sleepingParticles != null)
        {
            blokBot.sleepingParticles.Stop();
        }
    }
}
