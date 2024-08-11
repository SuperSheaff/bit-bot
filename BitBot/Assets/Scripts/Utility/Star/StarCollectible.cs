using System.Collections;
using UnityEngine;

public class StarCollectible : MonoBehaviour
{
    public float floatAmplitude = 0.5f; // Amplitude of the floating effect
    public float floatFrequency = 1f; // Frequency of the floating effect
    public float rotateSpeed = 50f; // Speed of rotation around player (slowed down)
    public Vector3 rotationAxis = Vector3.up; // Axis of rotation
    public float followSpeed = 2f; // Speed at which the star follows the player
    public float rotationDistance = 1.5f; // Distance from the player while rotating
    public float heightAbovePlayer = 2f; // Height above the player for the star to spin
    public Transform player; // Reference to the player
    public bool isCollected = false; // Check if the star is collected
    public Vector3 rotationOffset; // Rotation offset to apply when facing the camera

    private Vector3 startPosition;
    private float rotationAngle;
    private Camera mainCamera;
    private bool soundPlayed = false; // Flag to ensure sound plays only once

    void Start()
    {
        startPosition = transform.position;
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (!isCollected)
        {
            FloatAndRotate();
        }
        else
        {
            RotateAbovePlayer();
        }
    }

    private void FloatAndRotate()
    {
        transform.position = startPosition + new Vector3(0, Mathf.Sin(Time.time * floatFrequency) * floatAmplitude, 0);
        transform.Rotate(rotationAxis * rotateSpeed * Time.deltaTime);
    }

    private void RotateAbovePlayer()
    {
        if (player != null)
        {
            // Calculate position above and around the player
            rotationAngle += (rotateSpeed / 10) * Time.deltaTime; // Reduced rotation speed
            Vector3 offset = new Vector3(Mathf.Cos(rotationAngle), heightAbovePlayer, Mathf.Sin(rotationAngle)) * rotationDistance;
            transform.position = player.position + offset;

            // Face the camera with rotation offset
            Vector3 directionToCamera = (mainCamera.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(directionToCamera) * Quaternion.Euler(rotationOffset);
            transform.rotation = lookRotation;

            // Apply floating effect
            transform.position += Vector3.up * Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectStar(other.transform);
        }
    }

    private void CollectStar(Transform playerTransform)
    {
        if (!soundPlayed)
        {
            isCollected = true;
            player = playerTransform;
            transform.SetParent(player); // Parent the star to the player
            SoundManager.instance.PlaySound("STAR_COLLECT", transform);
            soundPlayed = true; // Ensure sound only plays once
        }
    }

    public void MoveToSlot(Transform slotTransform)
    {
        StartCoroutine(MoveToSlotCoroutine(slotTransform));
    }

    private IEnumerator MoveToSlotCoroutine(Transform slotTransform)
    {
        transform.SetParent(null); // Unparent the star from the player
        float journeyTime = 3f; // Slower movement
        float arcHeight = 5f; // Higher arc
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;

        while (elapsedTime < journeyTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / journeyTime;

            // Calculate the arc position
            Vector3 currentPos = Vector3.Lerp(startPosition, slotTransform.position, t);
            currentPos.y += arcHeight * Mathf.Sin(t * Mathf.PI);

            transform.position = currentPos;

            // Face the camera with rotation offset
            Vector3 directionToCamera = (mainCamera.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(directionToCamera) * Quaternion.Euler(rotationOffset);
            transform.rotation = lookRotation;

            yield return null;
        }

        // Ensure the star ends at the slot position
        transform.position = slotTransform.position;
    }
}
