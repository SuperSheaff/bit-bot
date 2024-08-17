using System;
using UnityEngine;

public class InteractPromptHandler : MonoBehaviour
{

    public GameObject interactPrompt;

    private PlayerController playerController;

    private void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
    }

    public void DisplayPrompt(bool enable)
    {
        // Check if Player unlocked Pushing
        if (playerController.PushEnabled)
        {
            interactPrompt.SetActive(enable);
        }
    }
}
