using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class IntroVideoPlayer : MonoBehaviour
{
    [SerializeField] private string videoFileName;
    [SerializeField] private Material targetMaterial; // Reference to the material to change color

    private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer)
        {
            string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
            videoPlayer.url = videoPath;

            // Subscribe to video events
            videoPlayer.prepareCompleted += OnVideoPrepared;
            videoPlayer.loopPointReached += OnVideoFinished;
        }
        else
        {
            Debug.Log("Video Player Component not found.");
        }

        if (targetMaterial != null)
        {
            targetMaterial.color = Color.black; // Reset the material color to black when the video finishes
        }
    }

    public void PlayVideo()
    {
        videoPlayer.Play();

        if (targetMaterial != null)
        {
            StartCoroutine(ChangeColorOverTime(Color.black, Color.white, 1f));
        }
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        // You can add any additional logic here if needed when the video is prepared
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        Destroy(gameObject);
    }

    private IEnumerator ChangeColorOverTime(Color fromColor, Color toColor, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            targetMaterial.color = Color.Lerp(fromColor, toColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        targetMaterial.color = toColor; // Ensure the color is set to the target color at the end
    }

}