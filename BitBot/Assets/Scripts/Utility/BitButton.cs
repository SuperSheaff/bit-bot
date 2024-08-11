using UnityEngine;
using System.Collections;

public class BitButton : MonoBehaviour
{
    public enum ButtonType
    {
        Permanent,
        Temporary
    }

    public ButtonType buttonType;
    public BitLight[] indicatorLights; // Array of custom lights to turn on when the button is pressed
    public Color buttonPressColor = Color.green; // Color to change the lights to when the button is pressed
    public string soundName; // Optional sound to play when the button is pressed
    public LayerMask playerLayer; // Layer to detect the player
    public BitDoor[] linkedDoors; // Doors that this button is linked to
    public Transform buttonTransform; // Transform of the button for visual pressing effect
    public Vector3 pressOffset; // Offset for the button press visual effect
    public float pressSpeed = 5f; // Speed of the button press animation

    public bool isPressed = false;
    private Vector3 initialPosition;
    private int objectsOnButton = 0; // Counter for objects on the button
    private bool wasInitializedWithObject = false;

    public bool IsPressed { get { return isPressed; } }

    private void Start()
    {
        initialPosition = buttonTransform.localPosition;
    }

    private void OnEnable()
    {
        Debug.Log("BitButton OnEnable: Resetting state");
        objectsOnButton = 0;
        wasInitializedWithObject = false;
        Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2, Quaternion.identity, playerLayer);

        // Check if there's already an object on the button
        if (colliders.Length > 0)
        {
            objectsOnButton = colliders.Length;
            isPressed = true;
            wasInitializedWithObject = true; // Set flag indicating button was initialized with an object on it
            SetButtonPressedState(true);
            buttonTransform.localPosition = initialPosition + pressOffset;

            // Ensure doors stay open if the button should be pressed
            foreach (BitDoor door in linkedDoors)
            {
                if (!door.IsOpen)
                {
                    door.OnButtonPressed(this);
                }
            }
        }
        else
        {
            isPressed = false;
            buttonTransform.localPosition = initialPosition;
            SetButtonPressedState(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other))
        {
            Debug.Log("BitButton OnTriggerEnter: Player entered");
            objectsOnButton++;
            if (objectsOnButton == 1 && !wasInitializedWithObject) // Only activate if this is the first object on the button and it wasn't already initialized with an object
            {
                ActivateButton();
            }
            PressButton();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsPlayer(other))
        {
            Debug.Log("BitButton OnTriggerExit: Player exited");
            objectsOnButton--;
            if (objectsOnButton == 0)
            {
                ReleaseButton();
                if (buttonType == ButtonType.Temporary)
                {
                    DeactivateButton();
                }
            }
        }
    }

    private bool IsPlayer(Collider other)
    {
        return ((1 << other.gameObject.layer) & playerLayer) != 0;
    }

    private void PressButton()
    {
        StopAllCoroutines();
        StartCoroutine(MoveButton(initialPosition + pressOffset));
        if (!wasInitializedWithObject)
        {
            PlayButtonSound();
        }
    }

    private void ReleaseButton()
    {
        StopAllCoroutines();
        StartCoroutine(MoveButton(initialPosition));
    }

    private void ActivateButton()
    {
        isPressed = true;
        SetButtonPressedState(true);

        if (linkedDoors != null)
        {
            foreach (BitDoor door in linkedDoors)
            {
                door.OnButtonPressed(this);
            }
        }
    }

    private void DeactivateButton()
    {
        isPressed = false;
        SetButtonPressedState(false);

        if (linkedDoors != null)
        {
            foreach (BitDoor door in linkedDoors)
            {
                door.OnButtonReleased(this);
            }
        }
    }

    private void SetButtonPressedState(bool pressed)
    {
        if (indicatorLights != null)
        {
            foreach (BitLight light in indicatorLights)
            {
                if (light != null)
                {
                    if (pressed)
                    {
                        light.SetColor(buttonPressColor);
                        light.TurnOn();
                    }
                    else
                    {
                        light.TurnOff();
                    }
                }
            }
        }
    }

    private void PlayButtonSound()
    {
        if (!string.IsNullOrEmpty(soundName))
        {
            SoundManager.instance.PlaySound(soundName, transform);
        }
    }

    private IEnumerator MoveButton(Vector3 targetPosition)
    {
        while (Vector3.Distance(buttonTransform.localPosition, targetPosition) > 0.01f)
        {
            buttonTransform.localPosition = Vector3.Lerp(buttonTransform.localPosition, targetPosition, Time.deltaTime * pressSpeed);
            yield return null;
        }
        buttonTransform.localPosition = targetPosition;
    }
}
