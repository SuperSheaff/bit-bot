using UnityEngine;
using System.Collections;

public class DeathState : PlayerState
{
    private float dissolveTime = 0.4f; // Total time to dissolve
    private float deathTime = 1f; // Time to wait before respawning

    public DeathState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        player.StartCoroutine(DissolveEffect()); 
        SoundManager.instance?.PlaySound("BIT_DEATH", player.transform); // Play death sound
        player.animator.enabled = false; // Freeze the animator

        player.IsAlive = false;
    }

    private IEnumerator DissolveEffect()
    {
        float elapsedTime = 0f;

        while (elapsedTime < dissolveTime)
        {
            elapsedTime += Time.deltaTime;
            float dissolveAmount = Mathf.Lerp(1f, 0f, elapsedTime / dissolveTime);
            player.meshMaterial.SetFloat("_FadeIn", dissolveAmount);
            yield return null;
        }

        yield return new WaitForSeconds(deathTime);
        player.stateMachine.ChangeState(player.respawnState); // Transition to respawn state
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
        player.animator.enabled = true; // Re-enable the animator
        player.meshMaterial.SetFloat("_FadeIn", 1f); // Reset dissolve effect

        player.IsAlive = true;
    }
}
