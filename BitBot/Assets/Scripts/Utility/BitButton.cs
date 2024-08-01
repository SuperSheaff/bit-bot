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
    public BitLight indicatorLight; // Custom light to turn on when the button is pressed
    public Color buttonPressColor = Color.green; // Color to change the light to when the button is pressed
    public string soundName; // Optional sound to play when the button is pressed
    public LayerMask playerLayer; // Layer to detect the player
    public BitDoor[] linkedDoors; // Doors that this button is linked to
    public Transform buttonTransform; // Transform of the button for visual pressing effect
    public Vector3 pressOffset; // Offset for the button press visual effect
    public float pressSpeed = 5f; // Speed of the button press animation

    private bool isPressed = false;
    private Vector3 initialPosition;
    private int objectsOnButton = 0; // Counter for objects on the button

    public bool IsPressed { get { return isPressed; } }

    private void Start()
    {
        initialPosition = buttonTransform.localPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other))
        {
            objectsOnButton++;
            if (objectsOnButton == 1) // Only activate if this is the first object on the button
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
    }

    private void ReleaseButton()
    {
        StopAllCoroutines();
        StartCoroutine(MoveButton(initialPosition));
    }

    private void ActivateButton()
    {
        isPressed = true;

        if (indicatorLight != null)
        {
            indicatorLight.SetColor(buttonPressColor);
            indicatorLight.TurnOn();
        }

        bool doorStateChanged = false;
        foreach (BitDoor door in linkedDoors)
        {
            if (buttonType == ButtonType.Temporary || !door.IsOpen)
            {
                door.OnButtonPressed();
                doorStateChanged = true;
            }
        }

        if (doorStateChanged && !string.IsNullOrEmpty(soundName))
        {
            SoundManager.instance.PlaySound(soundName, transform);
        }
    }

    private void DeactivateButton()
    {
        isPressed = false;

        if (indicatorLight != null)
        {
            indicatorLight.TurnOff();
        }

        bool doorStateChanged = false;
        foreach (BitDoor door in linkedDoors)
        {
            if (door.IsOpen)
            {
                door.OnButtonReleased();
                doorStateChanged = true;
            }
        }

        if (doorStateChanged && !string.IsNullOrEmpty(soundName))
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
