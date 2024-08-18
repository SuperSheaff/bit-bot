using UnityEngine;

public class BitSecurityCamera : MonoBehaviour
{
    public Transform player; // Assign the player's transform in the Inspector
    public float rotationSpeed = 5.0f; // Adjust the speed of rotation
    public Vector3 rotationOffset; // Use this to add an offset to the camera's rotation

    void Update()
    {
        if (player != null)
        {
            // Calculate the direction to the player
            Vector3 direction = player.position - transform.position;

            // Apply the rotation offset
            Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(rotationOffset);

            // Smoothly rotate towards the player
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
