using UnityEngine;

public class PushHandle : MonoBehaviour
{
    public PushableObject pushableObject; // Reference to the pushable object
    public Vector3 pushDirection = Vector3.forward; // Direction to push in local space
    public Transform playerLockPosition; // Position where the player should lock to

    private void Awake()
    {
        // Find the PushableObject component in the parent hierarchy
        if (pushableObject == null)
        {
            pushableObject = GetComponentInParent<PushableObject>();
            if (pushableObject == null)
            {
                Debug.LogError("PushableObject not found in parent hierarchy.");
            }
        }

        // Automatically set the player lock position
        if (playerLockPosition == null)
        {
            GameObject lockPositionObject = new GameObject("PlayerLockPosition");
            lockPositionObject.transform.SetParent(transform);
            lockPositionObject.transform.localPosition = new Vector3(0, -0.5f, 0f); // Adjust as necessary
            playerLockPosition = lockPositionObject.transform;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(pushDirection) * 2);

        if (playerLockPosition != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(playerLockPosition.position, 0.1f);
        }
    }

}
