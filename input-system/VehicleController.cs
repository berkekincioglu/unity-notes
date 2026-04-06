// =============================================================================
// VehicleController.cs — Hybrid Approach (Interface + Polling)
// =============================================================================
// Interface → Compiler FORCES you to handle new actions
// Polling   → All reads happen in Update, logic stays in one place
//
// Can ONLY see Vehicle actions. CANNOT access Player/UI actions.
// =============================================================================

using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleController : MonoBehaviour, GameInputActions.IVehicleActions
{
    // -------------------------------------------------------------------------
    // Inspector settings
    // -------------------------------------------------------------------------
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float brakeForce = 10f;
    [SerializeField] private float turnSpeed = 100f;

    // -------------------------------------------------------------------------
    // Private variables
    // -------------------------------------------------------------------------
    private GameInputActions.VehicleActions vehicleInput;
    private float currentSpeed;

    // =========================================================================
    // INITIALIZATION
    // =========================================================================

    void Start()
    {
        vehicleInput = InputManager.Instance.Vehicle;
        vehicleInput.SetCallbacks(this);
    }

    // =========================================================================
    // INTERFACE METHODS — Required, left empty
    // =========================================================================
    // If you add a "Horn" action to the Vehicle map and don't write OnHorn here:
    //   → COMPILE ERROR! You can't forget.
    // =========================================================================

    public void OnSteer(InputAction.CallbackContext context) { }
    public void OnAccelerate(InputAction.CallbackContext context) { }
    public void OnBrake(InputAction.CallbackContext context) { }
    public void OnExit(InputAction.CallbackContext context) { }

    // =========================================================================
    // UPDATE + HANDLERS
    // =========================================================================

    void Update()
    {
        HandleSteering();
        HandleAcceleration();
        HandleBrake();
        HandleExit();
        ApplyMovement();
    }

    private void HandleSteering()
    {
        Vector2 steer = vehicleInput.Steer.ReadValue<Vector2>();
        transform.Rotate(Vector3.up, steer.x * turnSpeed * Time.deltaTime);
    }

    private void HandleAcceleration()
    {
        float gas = vehicleInput.Accelerate.ReadValue<float>();
        currentSpeed += gas * acceleration * Time.deltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
    }

    private void HandleBrake()
    {
        if (vehicleInput.Brake.IsPressed())
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, brakeForce * Time.deltaTime);
        }
    }

    private void HandleExit()
    {
        if (vehicleInput.Exit.WasPressedThisFrame())
        {
            ExitVehicle();
        }
    }

    private void ApplyMovement()
    {
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }

    // =========================================================================
    // ENTER / EXIT
    // =========================================================================

    public void EnterVehicle()
    {
        InputManager.Instance.SwitchToVehicle();
        Debug.Log("Entered vehicle!");
    }

    private void ExitVehicle()
    {
        currentSpeed = 0;
        InputManager.Instance.SwitchToPlayer();
        Debug.Log("Exited vehicle!");
    }

    // =========================================================================
    // CLEANUP
    // =========================================================================

    void OnDisable()
    {
        vehicleInput.RemoveCallbacks(this);
    }
}
