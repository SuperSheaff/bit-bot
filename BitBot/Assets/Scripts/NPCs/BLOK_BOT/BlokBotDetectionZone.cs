using UnityEngine;

public class BlokBotDetectZone : MonoBehaviour
{
    private BlokBotController blokBotController;

    void Start()
    {
        blokBotController = GetComponentInParent<BlokBotController>();
        if (blokBotController == null)
        {
            Debug.LogError("BlokBotDetectZone is not a child of BlokBotController");
        }
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
}
