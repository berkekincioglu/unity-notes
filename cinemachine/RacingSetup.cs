// =============================================================================
// RacingSetup.cs — Racing Camera
// =============================================================================
// Setup: Follow (Lock to Target) + Rotate With Follow Target + FreeLook Modifier
//
// Camera follows behind the vehicle, rotates with it.
// Optional: FOV increases with speed for a sense of velocity.
//
// REQUIRED COMPONENTS on VCam GameObject:
//   - CinemachineCamera
//   - CinemachineFollow                (Position Control)
//   - CinemachineRotateWithFollowTarget (Rotation Control)
//
// OPTIONAL:
//   - CinemachineFreeLookModifier      (varies FOV with orbit — used for speed FOV)
//   - Noise                            (vibration at high speed)
//
// SCENE REQUIREMENTS:
//   - Main Camera with CinemachineBrain
//   - Vehicle GameObject
// =============================================================================

using UnityEngine;
using Unity.Cinemachine;

public class RacingSetup : MonoBehaviour
{
    [SerializeField] private Transform vehicle;
    [SerializeField] private float baseFOV = 60f;
    [SerializeField] private float maxFOV = 85f;
    [SerializeField] private float fovLerpSpeed = 3f;

    private CinemachineCamera cam;

    void Start()
    {
        cam = GetComponent<CinemachineCamera>();
        var follow = GetComponent<CinemachineFollow>();

        // --- CinemachineCamera ---
        cam.Target.TrackingTarget = vehicle;
        cam.Lens.FieldOfView = baseFOV;

        // --- Follow (Position Control) ---
        // Lock to Target: camera follows vehicle's rotation
        // When vehicle turns, camera turns with it
        follow.FollowOffset = new Vector3(0, 3, -8); // Above and behind

        // Low damping on Z for responsive acceleration feel
        // Higher damping on X/Y for stable cornering
        follow.TrackerSettings.PositionDamping = new Vector3(2, 1, 0.5f);
    }

    // =========================================================================
    // SPEED-BASED FOV (optional — call from vehicle controller)
    // =========================================================================
    // Wider FOV at high speed creates a sense of velocity.
    // Call this every frame from VehicleController with current speed ratio.
    //
    // Example:
    //   racingSetup.UpdateSpeedFOV(currentSpeed / maxSpeed);
    // =========================================================================

    public void UpdateSpeedFOV(float speedRatio01)
    {
        // speedRatio01: 0 = stopped, 1 = max speed
        float targetFOV = Mathf.Lerp(baseFOV, maxFOV, speedRatio01);
        cam.Lens.FieldOfView = Mathf.Lerp(cam.Lens.FieldOfView, targetFOV, Time.deltaTime * fovLerpSpeed);
    }
}

// =============================================================================
// INSPECTOR SETUP GUIDE:
//
// 1. Create empty GameObject → name "RacingVCam"
//
// 2. Add Component: CinemachineCamera
//    - Tracking Target: Vehicle
//    - Lens > FOV: 60
//
// 3. Add Component: CinemachineFollow
//    - Binding Mode: Lock To Target
//    - Follow Offset: (0, 3, -8)
//    - Position Damping: (2, 1, 0.5)
//
// 4. Add Component: CinemachineRotateWithFollowTarget
//    - Damping: 1
//    (Camera faces same direction as vehicle)
//
// SPEED-BASED EFFECTS:
//
//   FOV:
//     Call UpdateSpeedFOV() from your vehicle script.
//     60° at rest → 85° at max speed
//
//   Noise (shake at speed):
//     Add Noise behavior to VCam.
//     Set Amplitude Gain to 0 by default.
//     From vehicle script: set Amplitude Gain = speedRatio * 0.3
//     → Subtle vibration at high speed
//
// CAMERA ANGLES FOR DIFFERENT FEEL:
//
//   Close & low (arcade):
//     Offset: (0, 1.5, -4), FOV: 70-90
//
//   High & far (simulation):
//     Offset: (0, 5, -12), FOV: 50-60
//
//   Bumper cam:
//     Offset: (0, 0.8, 0.5), FOV: 90
//     (Camera almost at hood level, very immersive)
// =============================================================================
