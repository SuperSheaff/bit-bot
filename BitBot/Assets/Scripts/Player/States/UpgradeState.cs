using UnityEngine;

// State representing the player being idle
public class UpgradeState : PlayerState
{
    private Quaternion targetRotation;
    private float rotationSpeed = 1.0f; // Adjust this to control the rotation speed

    // Constructor for the UpgradeState
    public UpgradeState(PlayerController player) : base(player) {}

    // Called when the state is entered
    public override void Enter()
    {
        base.Enter();
        player.animator.SetBool("isUpgrading", true);

        // Calculate the target rotation to face the camera
        Vector3 directionToCamera = player.cameraTransform.position - player.transform.position;
        directionToCamera.y = 0; // Keep only the horizontal direction
        targetRotation = Quaternion.LookRotation(directionToCamera);
        player.screenAnimator.SetActive(false);
    }

    // Called every frame to update the state
    public override void Update()
    {
        base.Update();

        // Smoothly rotate the player to face the camera
        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // Called when the state is exited
    public override void Exit()
    {
        base.Exit();
        player.animator.SetBool("isUpgrading", false);
        player.screenAnimator.SetActive(true);
    }
}
