using UnityEngine;

public class FollowObject : MonoBehaviour
{
    // The target object to follow
    public Transform target;

    // The offset at which to follow the target
    public Vector3 offset;

    // The smooth time for the damping effect
    public float smoothTime = 0.3f;

    // The velocity reference for the SmoothDamp function
    private Vector3 velocity = Vector3.zero;

    // Update is called once per frame
    void LateUpdate()
    {
        if (target != null)
        {
            // Define the target position with the given offset
            Vector3 targetPosition = target.position + offset;

            // Smoothly move to the target position
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
}
