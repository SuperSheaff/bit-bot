using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour
{
    public Transform startPoint; // The starting point of the platform
    public Transform endPoint; // The end point of the platform
    public float speed = 2.0f; // The speed at which the platform moves
    public float waitTime = 1.0f; // Time to wait at each point
    public AnimationCurve easeCurve; // Curve for ease-in-out movement

    private Vector3 targetPosition; // The current target position
    private bool isWaiting = false; // To check if the platform is waiting
    private float waitTimer = 0f; // Timer to manage wait time
    private float moveTimer = 0f; // Timer to manage movement

    void Start()
    {
        if (startPoint != null && endPoint != null)
        {
            transform.position = startPoint.position; // Initialize the platform's position to the start point
            targetPosition = endPoint.position; // Set the initial target position to the end point
        }
    }

    void FixedUpdate()
    {
        if (startPoint != null && endPoint != null)
        {
            if (isWaiting)
            {
                waitTimer += Time.fixedDeltaTime;
                if (waitTimer >= waitTime)
                {
                    waitTimer = 0f;
                    isWaiting = false;
                    targetPosition = (targetPosition == endPoint.position) ? startPoint.position : endPoint.position;
                    moveTimer = 0f;
                }
            }
            else
            {
                MovePlatform();
            }
        }
    }

    private void MovePlatform()
    {
        float journeyLength = Vector3.Distance(startPoint.position, endPoint.position);
        moveTimer += Time.fixedDeltaTime;
        float fractionOfJourney = (moveTimer * speed) / journeyLength;

        // Apply ease-in-out curve
        float easedFraction = easeCurve.Evaluate(fractionOfJourney);

        transform.position = Vector3.Lerp(transform.position, targetPosition, easedFraction);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;
            isWaiting = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }
}