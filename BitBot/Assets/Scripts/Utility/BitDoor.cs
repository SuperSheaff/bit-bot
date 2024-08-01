using UnityEngine;
using System.Collections;

public class BitDoor : MonoBehaviour
{
    public Vector3 doorOpenOffset; // Offset from the initial position to the open position
    public float doorOpenSpeed = 2.0f; // Speed of the door opening
    public AnimationCurve doorOpenCurve; // Timing function for the door movement
    public string openSoundName; // Optional sound to play when the door opens
    public string closeSoundName; // Optional sound to play when the door closes
    public BitButton[] requiredButtons; // Buttons required to open the door

    private Vector3 doorClosedPosition; // Initial position of the door
    private Vector3 doorOpenPosition; // Final position of the door
    private bool isOpening = false;
    private int buttonsPressed = 0;

    public bool IsOpen { get { return buttonsPressed == requiredButtons.Length; } }

    void Start()
    {
        doorClosedPosition = transform.position;
        doorOpenPosition = doorClosedPosition + doorOpenOffset;
    }

    void Update()
    {
        if (buttonsPressed == requiredButtons.Length && !isOpening)
        {
            OpenDoor();
        }
        else if (buttonsPressed < requiredButtons.Length && isOpening)
        {
            CloseDoor();
        }
    }

    public void OnButtonPressed()
    {
        buttonsPressed++;
    }

    public void OnButtonReleased()
    {
        buttonsPressed--;
    }

    private void OpenDoor()
    {
        isOpening = true;

        if (!string.IsNullOrEmpty(openSoundName))
        {
            SoundManager.instance.PlaySound(openSoundName, transform);
        }

        StopAllCoroutines();
        StartCoroutine(MoveDoor(doorOpenPosition));
    }

    private void CloseDoor()
    {
        isOpening = false;

        if (!string.IsNullOrEmpty(closeSoundName))
        {
            SoundManager.instance.PlaySound(closeSoundName, transform);
        }

        StopAllCoroutines();
        StartCoroutine(MoveDoor(doorClosedPosition));
    }

    private IEnumerator MoveDoor(Vector3 targetPosition)
    {
        float elapsedTime = 0;
        float duration = Vector3.Distance(transform.position, targetPosition) / doorOpenSpeed;
        Vector3 startingPosition = transform.position;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float curveT = doorOpenCurve.Evaluate(t);
            transform.position = Vector3.Lerp(startingPosition, targetPosition, curveT);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }
}
