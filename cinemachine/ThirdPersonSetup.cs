// =============================================================================
// ThirdPersonSetup.cs — 3rd Person Action Camera (Dark Souls / Zelda style)
// =============================================================================
// Setup: Orbital Follow + Rotation Composer + Deoccluder
//
// Player orbits the camera around the character using mouse/right stick.
// Camera avoids walls and obstacles automatically.
//
// REQUIRED COMPONENTS on VCam GameObject:
//   - CinemachineCamera
//   - CinemachineOrbitalFollow      (Position Control)
//   - CinemachineRotationComposer   (Rotation Control)
//   - CinemachineDeoccluder         (Extension)
//   - CinemachineInputAxisController (for player input)
//
// SCENE REQUIREMENTS:
//   - Main Camera with CinemachineBrain
//   - Player GameObject (assigned as Tracking Target)
//   - Obstacles must have Colliders for Deoccluder to work
// =============================================================================

using UnityEngine;
using Unity.Cinemachine;

// This script shows how to create and configure a 3rd person camera via code.
// In practice, you'd set most of this up in the Inspector.
// This serves as a reference for what each setting does.

public class ThirdPersonSetup : MonoBehaviour
{
    [SerializeField] private Transform player;

    void Start()
    {
        // --- Get components (already added in Inspector) ---
        var cam = GetComponent<CinemachineCamera>();
        var orbital = GetComponent<CinemachineOrbitalFollow>();
        var composer = GetComponent<CinemachineRotationComposer>();

        // --- CinemachineCamera ---
        cam.Priority = 10;
        cam.Target.TrackingTarget = player;

        // --- Orbital Follow (Position Control) ---
        // Sphere mode: camera can orbit freely around target
        orbital.OrbitStyle = CinemachineOrbitalFollow.OrbitStyles.Sphere;

        // Binding mode: camera rotates WITH player's yaw but ignores pitch/roll
        // This means when the player turns, the camera follows the turn
        // but doesn't tilt when the player looks up/down
        orbital.TargetOffset = new Vector3(0, 1.5f, 0); // Aim at head height

        // --- Rotation Composer (Rotation Control) ---
        // Keep target slightly above center for a cinematic feel
        composer.Composition.ScreenPosition = new Vector2(0, 0.3f);
        composer.Damping = new Vector2(1f, 0.5f);

        // --- Deoccluder is configured in Inspector ---
        // Strategy: Pull Camera Forward
        // Camera Radius: 0.3
        // Damping: 0.5
        // Ignore Tag: "Player" (so player collider doesn't block camera)
    }
}

// =============================================================================
// INSPECTOR SETUP GUIDE:
//
// 1. Create empty GameObject → name "ThirdPersonVCam"
// 2. Add Component: CinemachineCamera
//    - Tracking Target: Player
//    - Priority: 10
//
// 3. Add Component: CinemachineOrbitalFollow
//    - Orbit Style: Sphere
//    - Radius: 5
//    - Target Offset: (0, 1.5, 0)
//    - Position Damping: (1, 1, 1)
//
// 4. Add Component: CinemachineRotationComposer
//    - Screen Position: (0, 0.3)
//    - Dead Zone: (0.1, 0.1)
//    - Damping: (1, 0.5)
//
// 5. Add Component: CinemachineDeoccluder
//    - Strategy: Pull Camera Forward
//    - Camera Radius: 0.3
//    - Damping: 0.5
//    - Ignore Tag: "Player"
//
// 6. Add Component: CinemachineInputAxisController
//    - Auto-detects Input System actions for camera rotation
//    - Adjust Gain for mouse sensitivity
// =============================================================================
