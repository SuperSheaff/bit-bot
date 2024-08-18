using System.Collections;
using TMPro;
using UnityEngine;
using Unity.Cinemachine;

public class BitSecret : MonoBehaviour
{
    public PlayerController playerController; // Reference to the PlayerController script
    public LayerMask playerLayer; // Layer to detect the player

    public TMP_Text SecretTitle; // TextMeshPro object for the secret title
    public TMP_Text SecretMessage; // TextMeshPro object for the secret message
    public GameObject secretUI; // UI GameObject for displaying the secret info

    [TextArea(3, 10)] // Allows for multiline input in the Unity inspector
    public string titleText; // Title text for the secret

    [TextArea(5, 20)] // Allows for multiline input in the Unity inspector
    public string messageText; // Message text for the secret

    public CinemachineCamera secretCamera; // The Cinemachine camera to switch to during the secret sequence
    private CinemachineCamera previousCamera; // Store the previous camera
    public Camera mainCamera; // The main camera

    public float floatAmplitude = 0.5f; // Amplitude of the floating effect
    public float floatFrequency = 1f; // Frequency of the floating effect
    public float rotateSpeed = 45f; // Speed of rotation
    public Vector3 rotationAxis = Vector3.up; // Axis of rotation

    public Material newMaterial; // The new material to apply on trigger enter
    private Renderer secretRenderer; // Renderer component of the secret object

    private Vector3 startPosition;
    private bool isActivated = false;

    void Start()
    {
        startPosition = transform.position;
        previousCamera = CameraController.instance.defaultCamera; // Initialize with default camera
        secretRenderer = GetComponent<Renderer>(); // Get the Renderer component

        if (secretUI != null)
        {
            secretUI.SetActive(false); // Ensure the secret UI is hidden at start
        }
    }

    void Update()
    {
        if (!isActivated)
        {
            FloatAndRotate();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other) && !isActivated)
        {
            isActivated = true;
            StartCoroutine(SecretSequence());

            // Increment the secret score in the GameController
            GameController.instance.AddSecret();

            // Change the material of the object
            if (newMaterial != null && secretRenderer != null)
            {
                secretRenderer.material = newMaterial;
            }
        }
    }

    private bool IsPlayer(Collider other)
    {
        return ((1 << other.gameObject.layer) & playerLayer) != 0;
    }

    private void FloatAndRotate()
    {
        // Floating effect
        transform.position = startPosition + new Vector3(0, Mathf.Sin(Time.time * floatFrequency) * floatAmplitude, 0);
        // Rotating effect
        transform.Rotate(rotationAxis * rotateSpeed * Time.deltaTime);
    }

    private IEnumerator SecretSequence()
    {
        playerController.stateMachine.ChangeState(playerController.secretState);

        yield return new WaitForSeconds(1f); // Adjust delay as needed

        // Switch to the secret Cinemachine camera
        if (secretCamera != null)
        {
            previousCamera = CameraController.instance.activeCamera; // Store the previous camera
            CameraController.instance.SetActiveCamera(secretCamera, LayerMask.NameToLayer("Nothing"));
        }

        yield return new WaitForSeconds(1f); // Adjust delay as needed

        DisplaySecretInfo();

        yield return new WaitUntil(() => playerController.inputHandler.JumpPressed);

        HideSecretInfo();

        // Switch back to the previous camera
        if (previousCamera != null)
        {
            CameraController.instance.SetActiveCamera(previousCamera, LayerMask.NameToLayer("Nothing"));
        }

        playerController.ResetJumpBuffer();
        playerController.stateMachine.ChangeState(playerController.idleState);

        Destroy(gameObject);
    }

    private void DisplaySecretInfo()
    {
        if (secretUI != null)
        {
            SecretTitle.text = titleText;
            SecretMessage.text = string.Empty; // Start with an empty message
            secretUI.SetActive(true);

            StartCoroutine(TypewriteEffect(messageText));
        }
    }

    private IEnumerator TypewriteEffect(string fullText)
    {
        for (int i = 0; i < fullText.Length; i++)
        {
            SecretMessage.text += fullText[i];
            SoundManager.instance.PlaySound("TYPING_SOUND", transform); // Play sound for each character
            yield return new WaitForSeconds(0.05f); // Adjust typing speed as needed
        }
    }

    private void HideSecretInfo()
    {
        if (secretUI != null)
        {
            secretUI.SetActive(false);
        }
    }
}
