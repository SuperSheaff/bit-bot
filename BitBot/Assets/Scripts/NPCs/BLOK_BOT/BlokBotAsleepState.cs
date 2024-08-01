using UnityEngine;

public class BlokBotAsleepState : BlokBotState
{

    public BlokBotAsleepState(BlokBotController blokBot) : base(blokBot) { }

    public override void Enter()
    {
        base.Enter();
        blokBot.animator.SetBool("isSleeping", true);
        blokBot.barrier.SetActive(false);

        if (blokBot.sleepingParticles != null)
        {
            blokBot.sleepingParticles.Play();
        }
        SoundManager.instance.PlaySound("blok_bot_snore", blokBot.transform);
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
        SoundManager.instance.StopSound("blok_bot_snore"); 

        if (blokBot.sleepingParticles != null)
        {
            blokBot.sleepingParticles.Stop();
        }
    }
}
