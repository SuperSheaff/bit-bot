using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StarSlot : MonoBehaviour
{
    public Light[] slotLights; // Lights for each slot
    public Transform[] slotTransforms; // The exact positions of the slots where the stars should go
    public float arcHeight = 5f; // Height of the arc when moving to the slot
    public float moveSpeed = 1f; // Speed of the star moving to the slot
    public FinalDoorController doorController; // Reference to the door controller
    public Material starLitMaterial; // Reference to the "StarLit" material

    private int currentSlotIndex = 0; // The index of the current slot to fill
    private Queue<Transform> collectedStarsQueue = new Queue<Transform>(); // Queue to manage collected stars
    private bool isPlacingStars = false; // Flag to prevent overlapping coroutines

    private void OnEnable()
    {
        // Reset flags and slot index if necessary
        if (currentSlotIndex >= slotTransforms.Length)
        {
            currentSlotIndex = 0;
        }
        isPlacingStars = false;

        // Ensure lights are off at start
        foreach (Light light in slotLights)
        {
            if (light != null)
            {
                light.enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Star"))
        {
            StarCollectible star = other.GetComponent<StarCollectible>();
            if (star != null && star.isCollected)
            {
                collectedStarsQueue.Enqueue(other.transform);
                star.enabled = false; // Disable the star's movement script

                // Start placing the stars in slots if it's the first star being placed
                if (!isPlacingStars && currentSlotIndex < slotTransforms.Length)
                {
                    StartCoroutine(PlaceStarsInSlots());
                }
            }
        }
    }

    private IEnumerator PlaceStarsInSlots()
    {
        isPlacingStars = true;

        while (collectedStarsQueue.Count > 0 && currentSlotIndex < slotTransforms.Length)
        {
            Transform star = collectedStarsQueue.Dequeue();
            Transform targetSlot = slotTransforms[currentSlotIndex];

            if (star == null || targetSlot == null)
            {
                Debug.LogWarning("Star or target slot is null, skipping this star.");
                continue;
            }

            Vector3 startPosition = star.position;
            Vector3 arcPeak = (startPosition + targetSlot.position) / 2 + Vector3.up * arcHeight;

            // Play sound when star begins moving
            if (SoundManager.instance != null)
            {
                SoundManager.instance.PlaySound("STAR_ENTER_SLOT");
            }

            // Move the star to the slot with an arc
            float elapsedTime = 0;
            float duration = 1f / moveSpeed; // Adjust duration by speed
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                Vector3 currentPosition = Vector3.Lerp(Vector3.Lerp(startPosition, arcPeak, t), Vector3.Lerp(arcPeak, targetSlot.position, t), t);
                star.position = currentPosition;
                yield return null;
            }

            // Snap to final position and activate the slot
            star.position = targetSlot.position;

            // Change the starTransform's material to "StarLit"
            Renderer starRenderer = targetSlot.GetComponentInChildren<Renderer>();
            if (starRenderer != null && starLitMaterial != null)
            {
                starRenderer.material = starLitMaterial;
            }
            else
            {
                Debug.LogWarning("Star renderer or star lit material is null, skipping material change.");
            }

            if (slotLights[currentSlotIndex] != null)
            {
                slotLights[currentSlotIndex].enabled = true;
            }
            else
            {
                Debug.LogWarning("Slot light is null, skipping light activation.");
            }

            // Play sound when star lands in the slot
            if (SoundManager.instance != null)
            {
                SoundManager.instance.PlaySound("STAR_ACTIVATE");
            }

            currentSlotIndex++;

            // Destroy the star object after placing it in the slot
            Destroy(star.gameObject);

            // Wait a moment before processing the next star
            yield return new WaitForSeconds(0.5f);
        }

        isPlacingStars = false;

        // If all slots are filled, trigger the final event (like opening a door)
        if (currentSlotIndex >= slotTransforms.Length)
        {
            Debug.Log("All slots filled. Triggering event...");
            if (doorController != null)
            {
                doorController.OpenDoor();
            }
            else
            {
                Debug.LogWarning("Door controller is null, cannot trigger event.");
            }
        }
    }
}
