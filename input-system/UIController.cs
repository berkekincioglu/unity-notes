// =============================================================================
// UIController.cs — Hybrid Approach (Interface + Polling)
// =============================================================================
// Interface → Compiler FORCES you to handle new actions
// Polling   → All reads happen in Update, logic stays in one place
//
// Can ONLY see UI actions. CANNOT access Player/Vehicle actions.
// =============================================================================

using UnityEngine;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour, GameInputActions.IUIActions
{
    // -------------------------------------------------------------------------
    // Private variables
    // -------------------------------------------------------------------------
    private GameInputActions.UIActions uiInput;

    // =========================================================================
    // INITIALIZATION
    // =========================================================================

    void Start()
    {
        uiInput = InputManager.Instance.UI;
        uiInput.SetCallbacks(this);
    }

    // =========================================================================
    // INTERFACE METHODS — Required, left empty
    // =========================================================================
    // If you add a "Scroll" action to the UI map and don't write OnScroll here:
    //   → COMPILE ERROR! You can't forget.
    // =========================================================================

    public void OnNavigate(InputAction.CallbackContext context) { }
    public void OnSubmit(InputAction.CallbackContext context) { }
    public void OnCancel(InputAction.CallbackContext context) { }
    public void OnClick(InputAction.CallbackContext context) { }

    // =========================================================================
    // UPDATE + HANDLERS
    // =========================================================================

    void Update()
    {
        HandleNavigation();
        HandleSubmit();
        HandleCancel();
        HandleClick();
    }

    private void HandleNavigation()
    {
        Vector2 nav = uiInput.Navigate.ReadValue<Vector2>();
        if (nav != Vector2.zero)
        {
            Debug.Log($"Menu navigation: {nav}");
        }
    }

    private void HandleSubmit()
    {
        if (uiInput.Submit.WasPressedThisFrame())
        {
            Debug.Log("Menu item selected!");
        }
    }

    private void HandleCancel()
    {
        if (uiInput.Cancel.WasPressedThisFrame())
        {
            Debug.Log("Closing menu...");
            InputManager.Instance.SwitchToPlayer();
        }
    }

    private void HandleClick()
    {
        if (uiInput.Click.WasPressedThisFrame())
        {
            Debug.Log("UI click!");
        }
    }

    // =========================================================================
    // PUBLIC API
    // =========================================================================

    public void OpenMenu()
    {
        InputManager.Instance.SwitchToUI();
    }

    // =========================================================================
    // CLEANUP
    // =========================================================================

    void OnDisable()
    {
        uiInput.RemoveCallbacks(this);
    }
}
