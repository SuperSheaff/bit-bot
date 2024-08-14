using UnityEngine;

public class BlokBotDetectZone : MonoBehaviour
{
    public BlokBotController blokBotController;

    void Start()
    {
        blokBotController = GetComponentInParent<BlokBotController>();
        if (blokBotController == null)
        {
            Debug.LogError("BlokBotDetectZone is not a child of BlokBotController");
        }
    }

    private void OnEnable()
    {
        blokBotController = GetComponentInParent<BlokBotController>();
        if (blokBotController == null)
        {
            Debug.LogError("BlokBotDetectZone is not a child of BlokBotController");
        }

        blokBotController.SetPlayerInDetectionZone(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            blokBotController.SetPlayerInDetectionZone(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            blokBotController.SetPlayerInDetectionZone(false);
        }
    }

    void OnTriggerStay(Collider other)
    {
        // Continuously check if the player is still within the detection zone
        if (other.CompareTag("Player"))
        {
            blokBotController.SetPlayerInDetectionZone(true);
        }
    }
}
