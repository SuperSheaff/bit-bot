using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public Vector3 rotationOffset; // Optional rotation offset if adjustments are needed

    private Transform cameraTransform;

    private void Start()
    {
        // Find the main camera in the scene
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (cameraTransform != null)
        {
            // Make the text face the camera
            transform.LookAt(transform.position + cameraTransform.rotation * Vector3.forward,
                             cameraTransform.rotation * Vector3.up);

            // Apply the rotation offset
            transform.Rotate(rotationOffset);
        }
    }
}
