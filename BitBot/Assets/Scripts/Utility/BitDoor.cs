using UnityEngine;
using System.Collections;

// Script to handle door functionality, including optional sound effects, and linking to buttons
public class BitDoor : MonoBehaviour
{
    public Vector3 doorOpenOffset; // Offset from the initial position to the open position
    public float doorOpenSpeed = 2.0f; // Speed of the door opening
    public AnimationCurve doorOpenCurve; // Timing function for the door movement
    public string openSoundName; // Optional sound to play when the door opens
    public BitButton[] requiredButtons; // Buttons required to open the door

    private Vector3 doorClosedPosition; // Initial position of the door
    private Vector3 doorOpenPosition; // Final position of the door
    private bool isOpening = false;
    private int buttonsPressed = 0;

    // Initialize the door's positions
    void Start()
    {
        doorClosedPosition = transform.position;
        doorOpenPosition = doorClosedPosition + doorOpenOffset;
    }

    // Check if all required buttons are pressed to open the door
    void Update()
    {
        if (buttonsPressed == requiredButtons.Length && !isOpening)
        {
            OpenDoor();
        }
    }

    // Increment the count of pressed buttons
    public void OnButtonPressed()
    {
        buttonsPressed++;
    }

    // Open the door by starting the coroutine to move it
    private void OpenDoor()
    {
        isOpening = true;

        // Play the open sound if there is one
        if (!string.IsNullOrEmpty(openSoundName))
        {
            SoundManager.instance.PlaySound(openSoundName, transform);
        }

        StopAllCoroutines(); // Stop any ongoing door movements
        StartCoroutine(MoveDoor(doorOpenPosition));

        SoundManager.instance?.PlaySound("Door", this.transform);
    }

    // Coroutine to move the door to the target position
    private IEnumerator MoveDoor(Vector3 targetPosition)
    {
        float elapsedTime = 0;
        float duration = Vector3.Distance(doorClosedPosition, targetPosition) / doorOpenSpeed;
        Vector3 startingPosition = transform.position;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float curveT = doorOpenCurve.Evaluate(t);
            transform.position = Vector3.Lerp(startingPosition, targetPosition, curveT);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition; // Ensure the door reaches the exact target position
    }
}
