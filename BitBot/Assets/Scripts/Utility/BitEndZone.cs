using UnityEngine;
using TMPro;
using System.Collections;
using Unity.Cinemachine;

public class EndZone : MonoBehaviour
{
    [Header("End Sequence Settings")]
    public GameObject endScreenObject; // The end screen object to activate
    public CinemachineCamera endCamera; // The camera to switch to during the end sequence
    public TMP_Text titleText; // Text object for the title
    public TMP_Text messageText; // Text object for the message

    public float cameraTransitionDelay = 0.5f; // Delay before starting the typewriter effect
    public float textDelay = 1.0f; // Delay between typing the title and the message
    public float typeSpeed = 0.05f; // Speed of the typewriter effect

    private string title = "BIT_BOTv9423";
    private string message;

    private void Start()
    {
        // Ensure the end screen object is inactive at the start
        endScreenObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Retrieve the player's time and secrets found
            float playerTime = GameController.instance.playerTime;
            int secretsFound = GameController.instance.secretScore;

            // Create the message string with placeholders for dynamic content
            message = $"Diagnostics:\nInitial evaluation successful in <color=#FFFFFF>{playerTime:F2} seconds</color>\n<color=#FFFFFF>{secretsFound}/4 BIT_BOT logs</color> accessed\nYou seek answers?\nBIT_TECH has recorded your curiosity\nFurther assessment required";

            StartCoroutine(HandleEndSequence());
            GameController.instance.player.stateMachine.ChangeState(GameController.instance.player.endState);
            GameTimer.instance.StopTimer();
        }
    }

    private IEnumerator HandleEndSequence()
    {
        // Switch to the end camera
        CameraController.instance.SetActiveCamera(endCamera, LayerMask.NameToLayer("Nothing"));
        
        yield return new WaitForSeconds(cameraTransitionDelay);

        // Activate the end screen object
        endScreenObject.SetActive(true);

        yield return StartCoroutine(TypewriteEffect(titleText, title));

        yield return new WaitForSeconds(textDelay);

        yield return StartCoroutine(TypewriteEffect(messageText, "Diagnostics:\nInitial evaluation successful in "));

        yield return new WaitForSeconds(1f);
        messageText.text += $"<color=#FFFFFF>{GameController.instance.playerTime:F2} seconds</color><\n";
        SoundManager.instance.PlaySound("TYPING_SOUND", transform); // Play sound for each character
        yield return new WaitForSeconds(1f);
        messageText.text += $"<color=#FFFFFF>{GameController.instance.secretScore}/4 BIT_BOT logs</color> accessed<\n";
        SoundManager.instance.PlaySound("TYPING_SOUND", transform); // Play sound for each character

        yield return StartCoroutine(TypewriteEffect(messageText, "You seek answers?<\nBIT_TECH has recorded your curiosity<\nFurther assessment required<"));
    }

    private IEnumerator TypewriteEffect(TMP_Text textObject, string content)
    {
        for (int i = 0; i < content.Length; i++)
        {
            textObject.text += content[i];
            SoundManager.instance.PlaySound("TYPING_SOUND", transform); // Play sound for each character
            yield return new WaitForSeconds(typeSpeed); // Control the typing speed
        }
    }
}
