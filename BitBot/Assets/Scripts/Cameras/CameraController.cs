using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.Video;
using TMPro;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    public CinemachineCamera defaultCamera; // The default camera to use
    private CinemachineCamera activeCamera; // The currently active camera
    public Camera mainCamera; // The main camera

    public CinemachineCamera introCamera1; // The camera to use for the intro sequence
    public CinemachineCamera introCamera2; // The camera to use for the intro sequence
    public CinemachineCamera firstAreaCamera; // The camera to use after the intro sequence
    public float introPauseBeforeVideo = 2f; // Duration to stay zoomed in
    public float introVideoLength = 2f; // Duration to stay zoomed in

    private bool introCompleted = false; // Flag to track if the intro sequence is completed
    private CinemachineBrain cinemachineBrain;

    public IntroVideoPlayer introVideoPlayer;

    public TMP_Text PressStart;
    public TMP_Text PressKeys;

    void Awake()
    {
        // Singleton pattern implementation
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        ResetCullingMask();
    }

    void Start()
    {
        if (GameController.instance.gameSettings.skipIntro)
        {
            CompleteIntroSequenceImmediately();
        }
        else
        {
            // Set the intro camera as the active camera at the start
            if (introCamera1 != null)
            {
                SetIntroCamera(introCamera1, LayerMask.NameToLayer("Area 0"));
            }
            else
            {
                Debug.LogError("No intro camera set in CameraController.");
            }
        }
    }

    public void StartHandleIntroSequence()
    {
        if (!GameController.instance.gameSettings.skipIntro)
        {
            StartCoroutine(HandleIntroSequence());
        }
        else
        {
            CompleteIntroSequenceImmediately();
        }
    }

    private void CompleteIntroSequenceImmediately()
    {
        SetIntroCamera(firstAreaCamera, LayerMask.NameToLayer("Area 0"));
        GameController.instance.player.IsIntroFinished = true;
        introCompleted = true;
        GameController.instance.stateMachine.ChangeState(GameController.instance.gamePlayState);
    }

    public IEnumerator HandleIntroSequence()
    {

        StartCoroutine(FadeOutText(PressStart, 2f));

        // Switch to the second intro camera
        if (introCamera2 != null)
        {
        Debug.Log("HandleIntroSequence");

            SetIntroCamera(introCamera2, LayerMask.NameToLayer("Area 0"));
            yield return new WaitForSeconds(1f);
            introVideoPlayer.PlayVideo();
            yield return new WaitForSeconds(introVideoLength);
        }
        else
        {
            Debug.LogError("No second intro camera set in CameraController.");
        }

        // Set the first area camera as the active camera
        if (firstAreaCamera != null)
        {
            SetIntroCamera(firstAreaCamera, LayerMask.NameToLayer("Area 0"));
        }
        else
        {
            Debug.LogError("No first area camera set in CameraController.");
        }

        yield return new WaitForSeconds(5f);
        
        GameController.instance.player.IsIntroFinished = true;
        introCompleted = true;

        yield return new WaitForSeconds(4f);
        StartCoroutine(FadeInText(PressKeys, 2f));
        yield return new WaitForSeconds(1f);
        GameController.instance.stateMachine.ChangeState(GameController.instance.gamePlayState);
    }

    public void SetIntroCamera(CinemachineCamera camera, LayerMask excludeLayers)
    {
        if (activeCamera != null)
        {
            activeCamera.Priority = 0; // Lower priority to deactivate the current active camera
        }

        activeCamera = camera;
        activeCamera.Priority = 10; // Higher priority to activate the new camera

        // Update the culling mask of the main camera to exclude specified layers
        mainCamera.cullingMask &= ~excludeLayers.value;
    }

    public void SetActiveCamera(CinemachineCamera camera, LayerMask excludeLayers)
    {
        if (introCompleted)
        {
            if (activeCamera != null)
            {
                activeCamera.Priority = 0; // Lower priority to deactivate the current active camera
            }

            activeCamera = camera;
            activeCamera.Priority = 10; // Higher priority to activate the new camera

            // Update the culling mask of the main camera to exclude specified layers
            mainCamera.cullingMask &= ~excludeLayers.value;
        }
    }

    public void ResetCullingMask()
    {
        // Reset the culling mask to default
        mainCamera.cullingMask = ~0;
    }

     // Fade in the text over the specified duration
    public IEnumerator FadeInText(TMP_Text textMeshPro, float duration)
    {
        float elapsedTime = 0f;
        Color color = textMeshPro.color;

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Clamp01(elapsedTime / duration);
            textMeshPro.color = new Color(color.r, color.g, color.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        textMeshPro.color = new Color(color.r, color.g, color.b, 1f); // Ensure the text is fully visible
    }

    // Fade out the text over the specified duration
    public IEnumerator FadeOutText(TMP_Text textMeshPro, float duration)
    {
        float elapsedTime = 0f;
        Color color = textMeshPro.color;

        while (elapsedTime < duration)
        {
            float alpha = 1f - Mathf.Clamp01(elapsedTime / duration);
            textMeshPro.color = new Color(color.r, color.g, color.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        textMeshPro.color = new Color(color.r, color.g, color.b, 0f); // Ensure the text is fully invisible
    }
}
