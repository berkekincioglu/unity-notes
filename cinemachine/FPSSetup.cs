// =============================================================================
// FPSSetup.cs — First Person Camera
// =============================================================================
// Setup: Hard Lock to Target + Pan Tilt
//
// Camera is locked to player's head position. Player rotates the camera
// freely with mouse/stick via Pan Tilt.
//
// REQUIRED COMPONENTS on VCam GameObject:
//   - CinemachineCamera
//   - CinemachineHardLockToTarget     (Position Control)
//   - CinemachinePanTilt              (Rotation Control)
//   - CinemachineInputAxisController  (for mouse look)
//
// SCENE REQUIREMENTS:
//   - Main Camera with CinemachineBrain
//   - Player with a child "CameraTarget" at head height
//     (this is the Tracking Target, not the player root)
// =============================================================================

using UnityEngine;
using Unity.Cinemachine;

public class FPSSetup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTarget; // Empty GO at player's head

    [Header("Look Settings")]
    [SerializeField] private float tiltMin = -80f;  // How far down player can look
    [SerializeField] private float tiltMax = 80f;   // How far up player can look

    void Start()
    {
        var cam = GetComponent<CinemachineCamera>();
        var panTilt = GetComponent<CinemachinePanTilt>();

        // --- CinemachineCamera ---
        // Track the head position, not the player root
        // Create an empty child GO on the player at head height (e.g., Y = 1.7)
        cam.Target.TrackingTarget = cameraTarget;

        // --- Hard Lock to Target (Position Control) ---
        // No configuration needed — camera position = target position, always

        // --- Pan Tilt (Rotation Control) ---
        // Configured via CinemachineInputAxisController in Inspector
        // Tilt limits are set on the Input Axis Controller's vertical axis
    }
}

// =============================================================================
// INSPECTOR SETUP GUIDE:
//
// 1. On Player GameObject:
//    - Create empty child → name "CameraTarget"
//    - Position: (0, 1.7, 0) — eye/head height
//
// 2. Create empty GameObject → name "FPSVCam"
//
// 3. Add Component: CinemachineCamera
//    - Tracking Target: Player/CameraTarget (the child, not the player root!)
//
// 4. Add Component: CinemachineHardLockToTarget
//    (No settings — position always matches target)
//
// 5. Add Component: CinemachinePanTilt
//    (Rotation driven by Input Axis Controller)
//
// 6. Add Component: CinemachineInputAxisController
//    - Horizontal: Mouse X / Right Stick X (pan)
//    - Vertical: Mouse Y / Right Stick Y (tilt)
//    - Set Gain for mouse sensitivity
//    - Set vertical axis range to clamp tilt (e.g., -80 to 80)
//
// OPTIONAL:
//   - Add Noise behavior for subtle head bob while walking
//   - Amplitude: 0.1, Frequency: 1 (very subtle)
//
// NOTE ON PLAYER ROTATION:
//   The Pan Tilt controls camera rotation only.
//   You still need a script on the Player to rotate the character body
//   to match the camera's horizontal rotation:
//
//   void LateUpdate()
//   {
//       // Rotate player body to match camera horizontal look
//       transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
//   }
// =============================================================================
