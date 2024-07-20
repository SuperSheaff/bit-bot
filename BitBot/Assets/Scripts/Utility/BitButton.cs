using UnityEngine;

// Script to handle button functionality, including optional light and sound effects, and linking to doors
public class BitButton : MonoBehaviour
{
    public Light indicatorLight; // Optional light to turn on when the button is pressed
    public string soundName; // Optional sound to play when the button is pressed
    public LayerMask playerLayer; // Layer to detect the player
    public BitDoor[] linkedDoors; // Doors that this button is linked to

    private bool isPressed = false;

    public bool IsPressed { get { return isPressed; } }

    // Triggered when another collider enters the trigger collider attached to this object
    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other) && !isPressed)
        {
            isPressed = true;
            ActivateButton();
        }
    }

    // Check if the collider belongs to a player
    private bool IsPlayer(Collider other)
    {
        return ((1 << other.gameObject.layer) & playerLayer) != 0;
    }

    // Activate the button by turning on the light, playing the sound, and notifying linked doors
    private void ActivateButton()
    {
        // Turn on the light if there is one
        if (indicatorLight != null)
        {
            indicatorLight.enabled = true;
        }

        // Play the sound if there is one
        if (!string.IsNullOrEmpty(soundName))
        {
            SoundManager.instance.PlaySound(soundName, transform);
        }

        // Notify linked doors
        foreach (BitDoor door in linkedDoors)
        {
            door.OnButtonPressed();
        }

        SoundManager.instance?.PlaySound("Button", this.transform);
    }
}
