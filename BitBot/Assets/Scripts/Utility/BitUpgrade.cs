using System.Collections;
using TMPro;
using UnityEngine;

public enum UpgradeType
{
    Jump,
    Sneak,
    Push,
    LedgeGrab,
    None
    // Add more upgrade types as needed
}

public class BitUpgrade : MonoBehaviour
{
    public PlayerController playerController; // Reference to the PlayerController script
    public LayerMask playerLayer; // Layer to detect the player
    public UpgradeType upgradeType; // The type of upgrade this button provides

    public TMP_Text PressJump;
    public TMP_Text UpgradeTitle; // TextMeshPro object for the upgrade title
    public TMP_Text UpgradeDescription; // TextMeshPro object for upgrade instructions
    public GameObject upgradeUI; // UI GameObject for displaying the upgrade info

    public string titleText; // Title text for the upgrade
    public string descriptionText; // Description text for the upgrade

    public float floatAmplitude = 0.5f; // Amplitude of the floating effect
    public float floatFrequency = 1f; // Frequency of the floating effect
    public float rotateSpeed = 45f; // Speed of rotation
    public Vector3 rotationAxis = Vector3.up; 

    public ParticleSystem ambientParticles; // Ambient particle system
    public ParticleSystem sparkleParticles; // Sparkle particle system
    public ParticleSystem explosionParticles; // Explosion particle system

    private Vector3 startPosition;
    private bool isActivated = false;
    private Camera mainCamera;

    void Start()
    {
        startPosition = transform.position;
        mainCamera = Camera.main;

        if (explosionParticles != null)
        {
            explosionParticles.Stop(); // Ensure explosion particles are stopped at start
        }

        if (sparkleParticles != null)
        {
            sparkleParticles.Stop();
        }
        
        if (ambientParticles != null)
        {
            ambientParticles.Play(); // Start ambient particles
        }

        if (upgradeUI != null)
        {
            upgradeUI.SetActive(false); // Ensure the upgrade UI is hidden at start
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
            StartCoroutine(UpgradeSequence());
        }
    }

    private bool IsPlayer(Collider other)
    {
        return ((1 << other.gameObject.layer) & playerLayer) != 0;
    }

    private void FloatAndRotate()
    {
        transform.position = startPosition + new Vector3(0, Mathf.Sin(Time.time * floatFrequency) * floatAmplitude, 0);
        transform.Rotate(rotationAxis * rotateSpeed * Time.deltaTime);
    }

    private IEnumerator UpgradeSequence()
    {
        playerController.stateMachine.ChangeState(playerController.pauseState);
        SoundManager.instance.PlaySound("BIT_CARTRIDGE", transform);

        if (ambientParticles != null)
        {
            ambientParticles.Stop();
        }

        if (sparkleParticles != null)
        {
            sparkleParticles.Play();
        }

        // Spiral movement parameters
        float spiralDuration = 2.8f;
        float spiralHeight = 4f;
        float initialSpiralRadius = 4f;
        float finalSpiralRadius = 0.1f;
        float spiralSpeed = 5f;

        Vector3 initialPosition = transform.position;
        Vector3 targetPosition = playerController.transform.position + Vector3.up * spiralHeight;

        float elapsed = 0f;
        while (elapsed < spiralDuration)
        {
            elapsed += Time.deltaTime;
            float angle = elapsed * spiralSpeed;
            float radius = Mathf.Lerp(initialSpiralRadius, finalSpiralRadius, elapsed / spiralDuration);
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            float y = Mathf.Lerp(initialPosition.y, targetPosition.y, elapsed / spiralDuration);
            transform.position = new Vector3(playerController.transform.position.x + x, y, playerController.transform.position.z + z);

            Vector3 direction = mainCamera.transform.position - transform.position;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(-direction) * Quaternion.Euler(-90, 0, 0);

            yield return null;
        }

        // Pause above player
        float pauseDuration = 0.1f;
        yield return new WaitForSeconds(pauseDuration);

        // Fly into player's head with ease-in-out
        elapsed = 0f;
        Vector3 finalPosition = playerController.transform.position + Vector3.up * 2f;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / 0.5f);
            transform.position = Vector3.Lerp(targetPosition, finalPosition, t);
            yield return null;
        }

        if (explosionParticles != null)
        {
            explosionParticles.transform.position = transform.position;
            explosionParticles.Play();
        }

        if (sparkleParticles != null)
        {
            sparkleParticles.Stop();
        }

        DisplayUpgradeInfo();
        yield return new WaitUntil(() => playerController.inputHandler.JumpPressed);

        ApplyUpgrade();
        HideUpgradeInfo();

        yield return new WaitForSeconds(0.5f);
        playerController.stateMachine.ChangeState(playerController.idleState);

        Destroy(gameObject);
    }

    private void ApplyUpgrade()
    {
        switch (upgradeType)
        {
            case UpgradeType.Jump:
                playerController.EnableJump();
                break;

            case UpgradeType.Sneak:
                playerController.EnableSneak();
                break;

            case UpgradeType.Push:
                playerController.EnablePush();
                break;

            case UpgradeType.LedgeGrab:
                playerController.EnableLedgeGrab();
                break;

            case UpgradeType.None:
                break;

            // Add more cases for other upgrade types
        }
    }

    private void DisplayUpgradeInfo()
    {
        if (upgradeUI != null)
        {
            UpgradeTitle.text = titleText;
            UpgradeDescription.text = descriptionText;

            upgradeUI.SetActive(true);
            StartCoroutine(ScaleUI(upgradeUI.transform, Vector3.zero, Vector3.one, 0.5f));
        }
    }

    private void HideUpgradeInfo()
    {
        if (upgradeUI != null)
        {
            upgradeUI.SetActive(false);
        }
    }

    private IEnumerator ScaleUI(Transform uiTransform, Vector3 fromScale, Vector3 toScale, float duration, System.Action onComplete = null)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            uiTransform.localScale = Vector3.Lerp(fromScale, toScale, t);
            yield return null;
        }
        uiTransform.localScale = toScale;

        if (onComplete != null)
        {
            onComplete.Invoke();
        }
    }
}
