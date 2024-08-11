using System.Collections;
using UnityEngine;

public class FinalDoorController : MonoBehaviour
{
    public Transform doorTransform; // Transform of the door
    public Vector3 doorOpenOffset; // Offset to move the door to open it
    public float doorOpenSpeed = 2f; // Speed at which the door opens
    public int totalSlots = 4; // Total number of slots to fill
    public AudioClip openSound; // Sound to play when the door opens

    private int slotsFilled = 0;
    private bool isOpening = false;
    private Vector3 closedPosition;
    private Vector3 openPosition;
    private AudioSource audioSource;

    void Start()
    {
        closedPosition = doorTransform.position;
        openPosition = closedPosition + doorOpenOffset;
        audioSource = GetComponent<AudioSource>();
    }

    public void IncrementSlotsFilled()
    {
        slotsFilled++;
        if (slotsFilled >= totalSlots && !isOpening)
        {
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        isOpening = true;
        audioSource.PlayOneShot(openSound);
        StartCoroutine(OpenDoorCoroutine());
    }

    private IEnumerator OpenDoorCoroutine()
    {
        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * doorOpenSpeed;
            doorTransform.position = Vector3.Lerp(closedPosition, openPosition, time);
            yield return null;
        }
    }
}
