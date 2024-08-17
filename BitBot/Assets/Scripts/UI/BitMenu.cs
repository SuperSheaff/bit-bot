using UnityEngine;
using TMPro;

public class BitMenu : MonoBehaviour
{
    public TMP_Text[] menuItems; // Array to hold the menu items (TMP_Text objects)
    public Color selectedColor = Color.yellow; // Color for the selected item
    public Color defaultColor = Color.white; // Color for non-selected items
    public float inputDelay = 0.2f; // Delay between inputs in seconds

    private int currentIndex = 0; // Track the current selected item
    private PlayerController player;
    private PlayerInputHandler inputHandler;
    private float lastInputTime; // Tracks the last time input was registered

    private void Start()
    {
        player = FindObjectOfType<PlayerController>(); // Reference to the player controller
        inputHandler = player.inputHandler; // Reference to the input handler
    }

    private void OnEnable()
    {
        // Reset the currentIndex and update the menu item colors
        currentIndex = 0;
        lastInputTime = Time.time; // Reset the input timer
        UpdateMenuColors();
    }

    private void Update()
    {
        HandleMenuNavigation();
    }

    private void HandleMenuNavigation()
    {
        if (Time.time - lastInputTime >= inputDelay)
        {
            if (inputHandler.Move.y > 0)
            {
                // Move up the menu
                currentIndex = (currentIndex > 0) ? currentIndex - 1 : menuItems.Length - 1;
                lastInputTime = Time.time; // Reset the input timer
                UpdateMenuColors();
            }
            else if (inputHandler.Move.y < 0)
            {
                // Move down the menu
                currentIndex = (currentIndex < menuItems.Length - 1) ? currentIndex + 1 : 0;
                lastInputTime = Time.time; // Reset the input timer
                UpdateMenuColors();
            }
        }

        // Check for selection
        if (inputHandler.Jump && Time.time - lastInputTime >= inputDelay)
        {
            ActivateMenuItem();
            lastInputTime = Time.time; // Reset the input timer after selection
        }
    }

    private void UpdateMenuColors()
    {
        // Update the color of each menu item based on the current selection
        for (int i = 0; i < menuItems.Length; i++)
        {
            menuItems[i].color = (i == currentIndex) ? selectedColor : defaultColor;
        }
    }

    private void ActivateMenuItem()
    {
        switch (currentIndex)
        {
            case 0:
                ResumeGame();
                break;
            case 1:
                RespawnPlayer();
                break;
            case 2:
                QuitGame();
                break;
        }
    }

    private void ResumeGame()
    {
        // Logic to resume the game
        player.stateMachine.ChangeState(player.inAirState);
        player.ResetJumpBuffer();
    }

    private void RespawnPlayer()
    {
        player.Respawn();
        player.stateMachine.ChangeState(player.inAirState);
        player.ResetJumpBuffer();
    }

    private void QuitGame()
    {
        // Logic to quit the game
        Application.Quit();
    }
}
