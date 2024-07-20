using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EndZone : MonoBehaviour
{
    [Header("Fade Settings")]
    public RawImage fadeImage; // UI Image used for fading
    public TextMeshProUGUI endTimer; // Second text to display at the end
    public TextMeshProUGUI endText; // First text to display at the end
    public TextMeshProUGUI endText2; // Second text to display at the end
    public float fadeDuration = 2.0f; // Duration for the fade effect
    public float textFadeDelay = 1.0f; // Delay before fading in the first text
    public float secondTextFadeDelay = 1.0f; // Delay before fading in the second text

    private void Start()
    {
        // Ensure the image and text are fully transparent at the start
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);
        endText.color = new Color(endText.color.r, endText.color.g, endText.color.b, 0);
        endText2.color = new Color(endText2.color.r, endText2.color.g, endText2.color.b, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(HandleEndSequence());
            GameController.instance.player.stateMachine.ChangeState(GameController.instance.player.endState);
            GameTimer.instance.StopTimer();
        }
    }

    private IEnumerator HandleEndSequence()
    {
        yield return StartCoroutine(FadeImage(true, fadeDuration));
        yield return new WaitForSeconds(textFadeDelay);
        yield return StartCoroutine(FadeText(endText, true, fadeDuration));
        yield return StartCoroutine(FadeText(endTimer, true, fadeDuration));
        yield return new WaitForSeconds(secondTextFadeDelay);
        yield return StartCoroutine(FadeText(endText2, true, fadeDuration));
    }

    private IEnumerator FadeImage(bool fadeIn, float duration)
    {
        float startAlpha = fadeIn ? 0 : 1;
        float endAlpha = fadeIn ? 1 : 0;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, endAlpha);
    }

    private IEnumerator FadeText(TextMeshProUGUI text, bool fadeIn, float duration)
    {
        float startAlpha = fadeIn ? 0 : 1;
        float endAlpha = fadeIn ? 1 : 0;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        text.color = new Color(text.color.r, text.color.g, text.color.b, endAlpha);
    }
}
