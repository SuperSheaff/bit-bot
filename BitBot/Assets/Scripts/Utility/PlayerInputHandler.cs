using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    #region Input Variables

    private PlayerInput playerInput;
    public Vector2 Move { get; private set; }
    public bool Jump { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool JumpReleased { get; private set; }

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        InitializeInput();
    }

    private void OnDestroy()
    {
        DisposeInput();
    }

    private void LateUpdate()
    {
        ResetJumpFlags();
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
    }

    // Disposes input actions and unsubscribes from events
    private void DisposeInput()
    {
        playerInput.Player.Move.performed -= OnMovePerformed;
        playerInput.Player.Move.canceled -= OnMoveCanceled;

        playerInput.Player.Jump.performed -= OnJumpPerformed;
        playerInput.Player.Jump.canceled -= OnJumpCanceled;

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
    }

    // Callback for jump input canceled
    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        Jump = false;
        JumpReleased = true;
    }

    #endregion

    #region Helper Methods

    // Resets the JumpPressed and JumpReleased flags at the end of each frame
    private void ResetJumpFlags()
    {
        JumpPressed = false;
        JumpReleased = false;
    }

    #endregion
}
