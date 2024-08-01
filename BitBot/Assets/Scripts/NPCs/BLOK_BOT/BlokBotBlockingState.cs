using System.Collections;
using UnityEngine;

public class BlokBotBlockingState : BlokBotState
{
    private Quaternion targetRotation;
    private bool returningToOriginalRotation = false;

    public BlokBotBlockingState(BlokBotController blokBot) : base(blokBot) { }

    public override void Enter()
    {
        base.Enter();
        blokBot.barrier.SetActive(true);
        blokBot.animator.SetBool("isBlocking", true);
        targetRotation = blokBot.headTransform.localRotation; // Save the original rotation

        if (blokBot.blockingParticles != null)
        {
            blokBot.blockingParticles.Play();
        }
        SoundManager.instance.PlaySound("blok_bot_activate", blokBot.transform);
        SoundManager.instance.PlaySound("blok_bot_laser", blokBot.transform);
    }

    public override void Update()
    {
        base.Update();

        if (!blokBot.isPlayerInDetectionZone)
        {
            blokBot.timeSinceLastSeenPlayer += Time.deltaTime;
            if (blokBot.timeSinceLastSeenPlayer >= 10f)
            {
                blokBot.stateMachine.ChangeState(blokBot.asleepState);
            }

            if (!returningToOriginalRotation)
            {
                returningToOriginalRotation = true;
            }
        }
        else
        {
            blokBot.timeSinceLastSeenPlayer = 0;
            returningToOriginalRotation = false;
        }

    }

    public override void LateUpdate()
    {
        base.LateUpdate();

        if (blokBot.isPlayerInDetectionZone && blokBot.playerTransform != null)
        {
            blokBot.headTransform.LookAt(blokBot.playerTransform.position);
        }
        else if (returningToOriginalRotation)
        {
            blokBot.headTransform.localRotation = Quaternion.Slerp(blokBot.headTransform.localRotation, targetRotation, Time.deltaTime);
        }
    }

    public override void Exit()
    {
        base.Exit();
        blokBot.barrier.SetActive(false);
        blokBot.animator.SetBool("isBlocking", false);

        if (blokBot.blockingParticles != null)
        {
            blokBot.blockingParticles.Stop();
        }

        returningToOriginalRotation = false; // Reset the flag when exiting the state
        SoundManager.instance.StopSound("blok_bot_laser");
    }

}
