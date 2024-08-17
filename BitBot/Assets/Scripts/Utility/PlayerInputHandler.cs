using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    #region Input Variables

    private PlayerInput playerInput;
    public Vector2 Move         { get; private set; }
    public bool Jump            { get; private set; }
    public bool JumpPressed     { get; private set; }
    public bool JumpReleased    { get; private set; }
    public bool Crouch          { get; private set; }
    public bool CrouchPressed   { get; private set; }
    public bool CrouchReleased  { get; private set; }
    public bool Push            { get; private set; }
    public bool PushPressed     { get; private set; }
    public bool PushReleased    { get; private set; }
    public bool Pause           { get; private set; }
    public bool PausePressed    { get; private set; }
    public bool PauseReleased   { get; private set; }
    #endregion

    #region References

    // private PauseMenuManager pauseMenuManager;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        InitializeInput();
    }

    private void Start()
    {
        // pauseMenuManager = FindAnyObjectByType<PauseMenuManager>();
    }

    private void OnDestroy()
    {
        DisposeInput();
    }

    private void LateUpdate()
    {
        ResetFlags();
    }

    #endregion

    #region Input Initialization

    // Initializes input actions and subscribes to events
    private void InitializeInput()
    {
        playerInput = new PlayerInput();
        playerInput.Player.Enable();

        playerInput.Player.Move.performed += OnMovePerformed;
        playerInput.Player.Move.canceled += OnMoveCanceled;

        playerInput.Player.Jump.performed += OnJumpPerformed;
        playerInput.Player.Jump.canceled += OnJumpCanceled;

        playerInput.Player.Crouch.performed += OnCrouchPerformed;
        playerInput.Player.Crouch.canceled += OnCrouchCanceled;

        playerInput.Player.Push.performed += OnPushPerformed;
        playerInput.Player.Push.canceled += OnPushCanceled;
        
        playerInput.Player.Pause.performed += OnPausePerformed;
    }

    // Disposes input actions and unsubscribes from events
    private void DisposeInput()
    {
        playerInput.Player.Move.performed -= OnMovePerformed;
        playerInput.Player.Move.canceled -= OnMoveCanceled;

        playerInput.Player.Jump.performed -= OnJumpPerformed;
        playerInput.Player.Jump.canceled -= OnJumpCanceled;

        playerInput.Player.Crouch.performed -= OnCrouchPerformed;
        playerInput.Player.Crouch.canceled -= OnCrouchCanceled;

        playerInput.Player.Push.performed -= OnPushPerformed;
        playerInput.Player.Push.canceled -= OnPushCanceled;
        
        playerInput.Player.Pause.performed -= OnPausePerformed;

        playerInput.Player.Disable();
    }

    #endregion

    #region Input Callbacks

    // Callback for move input performed
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>();
    }

    // Callback for move input canceled
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        Move = Vector2.zero;
    }

    // Callback for jump input performed
    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        Jump = true;
        JumpPressed = true;
        Invoke("ResetFlags", 0.5f);
    }

    // Callback for jump input canceled
    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        Jump = false;
        JumpReleased = true;
    }

    // Callback for crouch input performed
    private void OnCrouchPerformed(InputAction.CallbackContext context)
    {
        Crouch = true;
        CrouchPressed = true;
    }

    // Callback for crouch input canceled
    private void OnCrouchCanceled(InputAction.CallbackContext context)
    {
        Crouch = false;
        CrouchReleased = true;
    }

     // Callback for push input performed
    private void OnPushPerformed(InputAction.CallbackContext context)
    {
        Push = true;
        PushPressed = true;
        Invoke("ResetFlags", 0.5f);
    }

    // Callback for push input canceled
    private void OnPushCanceled(InputAction.CallbackContext context)
    {
        Push = false;
        PushReleased = true;
    }
    
     // Callback for Pause input performed
    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        Pause = true;
        PausePressed = true;
    }

    // Callback for Pause input canceled
    private void OnPauseCanceled(InputAction.CallbackContext context)
    {
        Pause = false;
        PauseReleased = true;
    }

    #endregion

    #region Helper Methods

    // Resets the flags at the end of each frame
    private void ResetFlags()
    {
        Jump = false;
        JumpPressed = false;
        JumpReleased = false;
        CrouchPressed = false;
        CrouchReleased = false;
        Push = false;
        PushPressed = false;
        PushReleased = false;
        Pause = false;
        PausePressed = false;
        PauseReleased = false;
    }

    #endregion
}
