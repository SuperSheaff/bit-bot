using UnityEngine;

// Script to handle respawn functionality when the player enters a respawn zone
public class BitRespawnZone : MonoBehaviour
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
                // Debug log for entering the respawn zone
                if (playerController.settings.debugMode)
                {
                    Debug.Log("Player entered respawn zone at position: " + transform.position);
                }

                playerController.Respawn();
            }
            else
            {
                Debug.LogWarning("PlayerController component not found on the player object.");
            }
        }
    }
}
