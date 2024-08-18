using System.Collections;
using UnityEngine;
using TMPro;

public class PushHandle : MonoBehaviour
{
    public PushableObject pushableObject; // Reference to the pushable object
    public Vector3 pushDirection = Vector3.forward; // Direction to push in local space
    public Transform playerLockPosition; // Position where the player should lock to
    public GameObject interactTextPrefab; // Prefab with a TextMeshPro component
    public Vector3 textOffset = new Vector3(0, 2, 0); // Custom offset for the text position
    private GameObject interactTextInstance;
    private TextMeshPro interactText;

    private bool playerInTriggerZone = false;
    private PlayerController playerController;

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

        // Initialize interact text (if a prefab is provided)
        if (interactTextPrefab != null)
        {
            // Apply the custom offset to the text position
            interactTextInstance = Instantiate(interactTextPrefab, transform.position + textOffset, Quaternion.identity);
            interactTextInstance.transform.SetParent(transform);
            interactText = interactTextInstance.GetComponent<TextMeshPro>();
            interactText.color = new Color(interactText.color.r, interactText.color.g, interactText.color.b, 0f); // Completely off at start
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && PlayerCanPush(other))
        {
            playerController = other.GetComponent<PlayerController>();
            if (playerController != null && playerController.stateMachine.CurrentState != playerController.pushingState)
            {
                FadeInInteractText();
                playerInTriggerZone = true; // Set flag to true to start checking in Update
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && playerController.PushEnabled)
        {
            FadeOutInteractText();
            playerInTriggerZone = false; // Stop checking in Update
            playerController = null; // Clear the reference
        }
    }

    private void Update()
    {
        if (playerInTriggerZone && playerController != null && playerController.stateMachine.CurrentState == playerController.pushingState)
        {
            FadeOutInteractText();
            playerInTriggerZone = false; // Stop checking in Update after fading out
        }

        if (!playerInTriggerZone && playerController != null && playerController.stateMachine.CurrentState != playerController.pushingState)
        {
            FadeInInteractText();
            playerInTriggerZone = true; // Stop checking in Update after fading out
        }
    }

    private bool PlayerCanPush(Collider player)
    {
        // Assuming the player has a script with boolean 'PushEnabled' and 'stateMachine.CurrentState != playerController.pushingState' variables
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            return playerController.PushEnabled;
        }
        return false;
    }

    private void FadeInInteractText()
    {
        if (interactText != null)
        {
            StopAllCoroutines();
            StartCoroutine(FadeTextToFullAlpha(0.5f, interactText));
        }
    }

    private void FadeOutInteractText()
    {
        if (interactText != null)
        {
            StopAllCoroutines();
            StartCoroutine(FadeTextToZeroAlpha(0.5f, interactText));
        }
    }

    private IEnumerator FadeTextToFullAlpha(float duration, TextMeshPro text)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0f);
        while (text.color.a < 1.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (Time.deltaTime / duration));
            yield return null;
        }
    }

    private IEnumerator FadeTextToZeroAlpha(float duration, TextMeshPro text)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);
        while (text.color.a > 0.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime / duration));
            yield return null;
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
