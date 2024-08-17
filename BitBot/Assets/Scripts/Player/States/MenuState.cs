using UnityEngine;

public class MenuState : PlayerState
{
    private Quaternion targetRotation;
    private float rotationSpeed = 10f; // Adjust this to control the rotation speed
    private float exitTimer;
    private float cameraTimer;
    private bool cameraFlag;

    public MenuState(PlayerController player) : base(player) {}

    public override void Enter()
    {
        base.Enter();
        player.animator.SetBool("isUpgrading", true);
        cameraFlag = true;
        // Reset the timers every time the state is entered
        exitTimer = Time.time + 0.5f;
        cameraTimer = Time.time + 1f;

        // Calculate the target rotation to face the camera
        Vector3 directionToCamera = player.cameraTransform.position - player.transform.position;
        directionToCamera.y = 0; // Keep only the horizontal direction
        targetRotation = Quaternion.LookRotation(directionToCamera);


    }

    public override void Update()
    {
        base.Update();

        // Smoothly rotate the player to face the camera
        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        HandleChangeCamera();
        HandleExitMenuState();
    }

    public override void Exit()
    {
        base.Exit();
        player.animator.SetBool("isUpgrading", false);

        // Switch back to the previous camera
        player.pauseMenu.SetActive(false);
        player.screenAnimator.SetActive(true);
        CameraController.instance.ReturnToPreviousCamera();
    }

    private void HandleExitMenuState()
    {
        if (Time.time > exitTimer)
        {
            if (player.inputHandler.Pause)
            {
                player.stateMachine.ChangeState(player.idleState);
            }
        }
    }

    private void HandleChangeCamera()
    {
        if (Time.time > cameraTimer && cameraFlag)
        {
            CameraController.instance.SetActiveCamera(CameraController.instance.menuCamera, LayerMask.NameToLayer("UI"));
            cameraFlag = false;
            player.pauseMenu.SetActive(true);
            player.screenAnimator.SetActive(false);
        }
    }
}
