using UnityEngine;
using System.Collections;

public class BitDoor : MonoBehaviour
{
    public Vector3 doorOpenOffset;
    public float doorOpenSpeed = 2.0f;
    public AnimationCurve doorOpenCurve;
    public string openSoundName;
    public string closeSoundName;
    public BitButton[] requiredButtons;

    private Vector3 doorClosedPosition;
    private Vector3 doorOpenPosition;
    private bool isOpening = false;
    private bool[] buttonStates;

    public bool IsOpen 
    { 
        get 
        { 
            foreach (bool state in buttonStates)
            {
                if (!state) return false;
            }
            return true;
        } 
    }

    void Start()
    {
        doorClosedPosition = transform.position;
        doorOpenPosition = doorClosedPosition + doorOpenOffset;
        buttonStates = new bool[requiredButtons.Length];

        // Initialize button states based on current button states (useful when re-enabling)
        for (int i = 0; i < requiredButtons.Length; i++)
        {
            buttonStates[i] = requiredButtons[i].IsPressed;
        }
    }

    void Update()
    {
        if (IsOpen && !isOpening)
        {
            OpenDoor();
        }
        else if (!IsOpen && isOpening)
        {
            CloseDoor();
        }
    }

    public void OnButtonPressed(BitButton button)
    {
        for (int i = 0; i < requiredButtons.Length; i++)
        {
            if (requiredButtons[i] == button)
            {
                buttonStates[i] = true;
                break;
            }
        }
    }

    public void OnButtonReleased(BitButton button)
    {
        for (int i = 0; i < requiredButtons.Length; i++)
        {
            if (requiredButtons[i] == button)
            {
                buttonStates[i] = false;
                break;
            }
        }
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
