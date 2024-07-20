using UnityEngine;

// Script to handle checkpoint functionality
public class BitCheckpoint : MonoBehaviour
{
    // Triggered when another collider enters the trigger collider attached to this object
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();

            // Check if PlayerController component is present
            if (playerController != null)
            {
                playerController.SetCheckpoint(transform.position);

                // Debug log for checkpoint set
                if (playerController.settings.debugMode)
                {
                    Debug.Log("Checkpoint set at position: " + transform.position);
                }
            }
            else
            {
                Debug.LogWarning("PlayerController component not found on the player object.");
            }
        }
    }
}
