// =============================================================================
// PlayerController.cs — Hybrid Approach (Interface + Polling)
// =============================================================================
// Interface → Compiler FORCES you to handle new actions, you can't forget
// Polling   → All reads happen in Update, logic stays in one place, clean
//
// REQUIRED COMPONENTS (on the same GameObject):
//   - CharacterController
//   - This script
//
// REQUIRED SCENE OBJECT:
//   - InputManager (separate GameObject, Singleton)
// =============================================================================

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, GameInputActions.IPlayerActions
{
    // -------------------------------------------------------------------------
    // Inspector settings
    // -------------------------------------------------------------------------
    [SerializeField] private float speed = 5f;
    [SerializeField] private float sprintMultiplier = 2f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float terminalVelocity = -50f;

    // -------------------------------------------------------------------------
    // Private variables
    // -------------------------------------------------------------------------
    private CharacterController controller;
    private GameInputActions.PlayerActions playerInput;
    private Vector3 velocity;

    // =========================================================================
    // INITIALIZATION
    // =========================================================================

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerInput = InputManager.Instance.Player;

        // Register interface — compiler will warn if a new action is added
        playerInput.SetCallbacks(this);
    }

    // =========================================================================
    // INTERFACE METHODS — Required, must be written even if empty
    // =========================================================================
    // Why empty?
    // Because we DON'T store values here.
    // We read via Polling in Update → logic stays in one place.
    //
    // These methods exist solely as a "compiler safety net".
    // If you add a "Crouch" action tomorrow and don't write OnCrouch here:
    //   → COMPILE ERROR! You can't forget.
    //
    // Then you also add the crouch read logic in the appropriate handler.
    // =========================================================================

    public void OnMove(InputAction.CallbackContext context) { }
    public void OnLook(InputAction.CallbackContext context) { }
    public void OnJump(InputAction.CallbackContext context) { }
    public void OnAttack(InputAction.CallbackContext context) { }
    public void OnSprint(InputAction.CallbackContext context) { }

    // =========================================================================
    // UPDATE + HANDLERS
    // =========================================================================

    void Update()
    {
        HandleMovement();
        HandleGravity();
        HandleJump();
        HandleAttack();
        ApplyGravity();
    }

    private void HandleMovement()
    {
        Vector2 moveInput = playerInput.Move.ReadValue<Vector2>();
        bool isSprinting = playerInput.Sprint.IsPressed();

        float currentSpeed = isSprinting ? speed * sprintMultiplier : speed;

        Vector3 move = transform.right * moveInput.x
                     + transform.forward * moveInput.y;
        controller.Move(move * currentSpeed * Time.deltaTime);
    }

    private void HandleGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    private void HandleJump()
    {
        if (playerInput.Jump.WasPressedThisFrame() && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void HandleAttack()
    {
        if (playerInput.Attack.WasPressedThisFrame())
        {
            Debug.Log("Attack!");
        }
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        if (velocity.y < terminalVelocity)
        {
            velocity.y = terminalVelocity;
        }
        controller.Move(velocity * Time.deltaTime);
    }

    // =========================================================================
    // CLEANUP
    // =========================================================================

    void OnDisable()
    {
        playerInput.RemoveCallbacks(this);
    }
}
