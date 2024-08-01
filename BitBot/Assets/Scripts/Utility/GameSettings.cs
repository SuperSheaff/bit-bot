using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "GameSettings", order = 1)]
public class GameSettings : ScriptableObject
{
    #region Movement Settings

    [Header("Movement Settings")]
    [Tooltip("Movement speed of the player.")]
    public float moveSpeed = 5f;

    [Tooltip("Rotation speed of the player.")]
    public float rotationSpeed = 10f;

    #endregion

    #region Jump Settings

    [Header("Jump Settings")]
    [Tooltip("Desired jump height.")]
    public float jumpHeight = 2f;

    [Tooltip("Desired time to reach the peak of the jump.")]
    public float jumpDuration = 0.5f;

    [Tooltip("Speed of the player while in the air.")]
    public float jumpMoveSpeed = 3f;

    [Tooltip("Maximum time to hold jump button for a higher jump.")]
    public float jumpHoldTime = 0.2f;

    [Tooltip("Gravity applied when holding jump button.")]
    public float jumpHoldGravity = -15f;

    [Tooltip("Time window to buffer jump input.")]
    public float jumpBufferTime = 0.1f;

    [Tooltip("Time window to allow jump after leaving ground.")]
    public float coyoteTime = 0.2f;

    [Tooltip("Increased gravity when falling.")]
    public float extraFallGravity = -30f;

    #endregion

    #region Air Control Settings

    [Header("Air Control Settings")]
    [Tooltip("Control influence in the air (0.0 to 1.0).")]
    public float airControlInfluence = 0.5f;

    #endregion

    #region Crouch Settings

    [Header("Crouch Settings")]
    [Tooltip("Crouching speed of the player.")]
    public float crouchMoveSpeed = 2f;

    #endregion

    #region Push Settings

    [Header("Push Settings")]
    [Tooltip("Pushing speed of the player.")]
    public float pushingMoveSpeed = 2f;

    #endregion

    #region Air Control Settings

    [Header("Air Control Settings")]
    [HideInInspector] public float gravity; // Calculated gravity based on jump settings
    [HideInInspector] public float jumpForce; // Calculated jump force based on jump settings

    #endregion

    public float pushDistanceCheck = 2f;

    #region Debug Settings

    [Header("Debug Settings")]
    [Tooltip("Enable or disable debug mode for additional logging.")]
    public bool debugMode = false;

    [Header("Skip Intro")]
    public bool skipIntro = false;

    #endregion
}