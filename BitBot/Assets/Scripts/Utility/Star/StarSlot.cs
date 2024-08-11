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
            light.enabled = false;
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

            Vector3 startPosition = star.position;
            Vector3 arcPeak = (startPosition + targetSlot.position) / 2 + Vector3.up * arcHeight;

            // Play sound when star begins moving
            SoundManager.instance.PlaySound("STAR_ENTER_SLOT");

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
            slotLights[currentSlotIndex].enabled = true;

            // Play sound when star lands in the slot
            SoundManager.instance.PlaySound("STAR_ACTIVATE");

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
        }
    }
}
