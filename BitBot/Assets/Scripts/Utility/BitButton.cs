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
    public BitLight[] indicatorLights;
    public Color buttonPressColor = Color.green;
    public string soundName;
    public LayerMask playerLayer;
    public BitDoor[] linkedDoors;
    public Transform buttonTransform;
    public Vector3 pressOffset;
    public float pressSpeed = 5f;

    public bool isPressed = false;
    public bool permanentPressed = false;
    private Vector3 initialPosition;
    private int objectsOnButton = 0;
    private bool wasInitializedWithObject = false;
    private bool shouldCheckInUpdate = false; // Flag to control whether Update should check for objects

    public bool IsPressed { get { return isPressed; } }

    private void Start()
    {
        initialPosition = buttonTransform.localPosition;
    }

    private void OnEnable()
    {
        Debug.Log("BitButton OnEnable: Resetting state");
        CheckForObjectsOnButton();

        // Set the flag to true if there are objects on the button after OnEnable runs
        shouldCheckInUpdate = objectsOnButton > 0;
    }

    private void Update()
    {
        if (shouldCheckInUpdate)
        {
            CheckForObjectsOnButton();
        }
    }

    private void CheckForObjectsOnButton()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2, Quaternion.identity, playerLayer);

        if (colliders.Length > 0)
        {
            objectsOnButton = colliders.Length;
            wasInitializedWithObject = true;

            if (!isPressed)
            {
                ActivateButton();
                PressButton();
            }
        }
        else
        {
            if (isPressed && buttonType == ButtonType.Temporary)
            {
                objectsOnButton = 0;
                ReleaseButton();
                if (buttonType == ButtonType.Temporary)
                {
                    DeactivateButton();
                }

                // Disable the flag when there are no objects on the button
                shouldCheckInUpdate = false;
            }
            wasInitializedWithObject = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other))
        {
            objectsOnButton++;
            if (objectsOnButton == 1 && !wasInitializedWithObject)
            {
                ActivateButton();
            }
            PressButton();

            if (buttonType == ButtonType.Permanent)
            {
                permanentPressed = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsPlayer(other))
        {
            objectsOnButton--;
            if (objectsOnButton <= 0 && buttonType == ButtonType.Temporary)
            {
                objectsOnButton = 0;
                ReleaseButton();
                if (buttonType == ButtonType.Temporary)
                {
                    DeactivateButton();
                }

                // Reset the flag when an object moves off the button
                shouldCheckInUpdate = false;
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
        if (!string.IsNullOrEmpty(soundName) && !permanentPressed)
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
