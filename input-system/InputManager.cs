// =============================================================================
// InputManager.cs — Centralized Input Control (Singleton)
// =============================================================================
// This script is the SOLE OWNER of all input actions.
// No other script should create a GameInputActions instance directly.
//
// USAGE:
//   InputManager.Instance.Player  → Access Player actions
//   InputManager.Instance.UI      → Access UI actions
//   InputManager.Instance.Vehicle → Access Vehicle actions
//
// MAP SWITCHING:
//   InputManager.Instance.SwitchToPlayer()
//   InputManager.Instance.SwitchToUI()
//   InputManager.Instance.SwitchToVehicle()
// =============================================================================

using UnityEngine;

public class InputManager : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Singleton Pattern
    // -------------------------------------------------------------------------
    // Only one InputManager exists in the scene.
    // Any script accesses it via "InputManager.Instance".
    //
    // WHY SINGLETON?
    // If each script created its own GameInputActions instance:
    //   - The same key press would be processed multiple times
    //   - Map enable/disable would be out of sync
    //   - Wasted memory
    // -------------------------------------------------------------------------
    public static InputManager Instance { get; private set; }

    // -------------------------------------------------------------------------
    // Input Action Asset — single instance
    // -------------------------------------------------------------------------
    // Instance of the GameInputActions class generated via "Generate C# Class".
    // This class contains all maps and actions from the .inputactions file.
    // -------------------------------------------------------------------------
    private GameInputActions inputs;

    // -------------------------------------------------------------------------
    // Expose ONLY the relevant map, not the entire asset
    // -------------------------------------------------------------------------
    // PlayerController can only access the "Player" property.
    // UIController can only access the "UI" property.
    // This way each script only sees its own actions.
    //
    // "=>" (expression body) is shorthand for:
    //   public GameInputActions.PlayerActions Player { get { return inputs.Player; } }
    // -------------------------------------------------------------------------
    public GameInputActions.PlayerActions Player => inputs.Player;
    public GameInputActions.UIActions UI => inputs.UI;
    public GameInputActions.VehicleActions Vehicle => inputs.Vehicle;

    // -------------------------------------------------------------------------
    // Awake — Runs earliest, before other scripts' Start methods
    // -------------------------------------------------------------------------
    void Awake()
    {
        // Singleton check: if an InputManager already exists, destroy this duplicate
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Persist across scene changes — lives for the entire game session
        DontDestroyOnLoad(gameObject);

        // Create input actions
        inputs = new GameInputActions();

        // Player map is active by default on game start
        SwitchToPlayer();
    }

    // -------------------------------------------------------------------------
    // Map Switching Methods
    // -------------------------------------------------------------------------
    // IMPORTANT: Disable other maps before enabling one!
    // If two maps are active at the same time, the same key can trigger
    // two different actions.
    //
    // Example: If Space is bound to both "Jump" (Player) and "Submit" (UI),
    //   and both maps are active → pressing Space fires both = BUG!
    // -------------------------------------------------------------------------

    public void SwitchToPlayer()
    {
        inputs.UI.Disable();
        inputs.Vehicle.Disable();
        inputs.Player.Enable();
        Debug.Log("[InputManager] Player action map active");
    }

    public void SwitchToUI()
    {
        inputs.Player.Disable();
        inputs.Vehicle.Disable();
        inputs.UI.Enable();
        Debug.Log("[InputManager] UI action map active");
    }

    public void SwitchToVehicle()
    {
        inputs.Player.Disable();
        inputs.UI.Disable();
        inputs.Vehicle.Enable();
        Debug.Log("[InputManager] Vehicle action map active");
    }

    // -------------------------------------------------------------------------
    // Disable all input (for cutscenes, loading screens, etc.)
    // -------------------------------------------------------------------------
    public void DisableAllInput()
    {
        inputs.Player.Disable();
        inputs.UI.Disable();
        inputs.Vehicle.Disable();
        Debug.Log("[InputManager] All input disabled");
    }

    // -------------------------------------------------------------------------
    // Cleanup — prevent memory leaks
    // -------------------------------------------------------------------------
    // GameInputActions uses unmanaged memory.
    // If Dispose() is not called, a memory leak occurs.
    // -------------------------------------------------------------------------
    void OnDestroy()
    {
        inputs?.Dispose();
    }
}

// =============================================================================
// UNITY SETUP:
//
// 1. Create an empty GameObject → name it "InputManager"
// 2. Attach this script (Add Component > InputManager)
// 3. Done! The singleton works automatically.
//
// NOTE: Because DontDestroyOnLoad is used, InputManager survives
//       scene transitions. Do NOT add a new one in every scene!
// =============================================================================
