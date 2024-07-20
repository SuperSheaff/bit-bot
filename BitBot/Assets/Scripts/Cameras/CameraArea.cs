using UnityEngine;
using Unity.Cinemachine;

public class CameraArea : MonoBehaviour
{
    public CinemachineCamera cameraToActivate; // The camera to activate when the player enters this area
    public LayerMask excludeLayers; // The layer to exclude from the culling mask
    public GameObject[] objectsToActivate; // Groups of objects to activate
    public GameObject[] objectsToDeactivate; // Groups of objects to deactivate

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Activate and deactivate the specified objects
            SetActiveObjects(objectsToDeactivate, false);
            SetActiveObjects(objectsToActivate, true);

            // Activate the specified camera
            CameraController cameraController = FindFirstObjectByType<CameraController>();
            if (cameraController != null && cameraToActivate != null)
            {
                cameraController.SetActiveCamera(cameraToActivate, excludeLayers);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Reset the culling mask to default
            CameraController cameraController = FindFirstObjectByType<CameraController>();
            if (cameraController != null)
            {
                cameraController.ResetCullingMask();
            }
        }
    }

    private void SetActiveObjects(GameObject[] objects, bool state)
    {
        foreach (GameObject obj in objects)
        {
            if (obj != null)
            {
                obj.SetActive(state);
            }
        }
    }
}
