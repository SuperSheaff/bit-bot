using UnityEngine;

public class BitTriggerDestroy : MonoBehaviour
{
    [SerializeField] private GameObject objectToDelete; // The object to delete
    [SerializeField] private LayerMask playerLayer; // Layer to detect the player

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other))
        {
            if (objectToDelete != null)
            {
                Destroy(objectToDelete);
            }
        }
    }

    private bool IsPlayer(Collider other)
    {
        return ((1 << other.gameObject.layer) & playerLayer) != 0;
    }
}
