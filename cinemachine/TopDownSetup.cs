// =============================================================================
// TopDownSetup.cs — Top-Down / RTS Camera
// =============================================================================
// Setup: Follow (World Space binding) + fixed rotation
//
// Camera looks down at the player from a fixed angle.
// No rotation control needed — camera angle is constant.
//
// REQUIRED COMPONENTS on VCam GameObject:
//   - CinemachineCamera
//   - CinemachineFollow   (Position Control)
//
// SCENE REQUIREMENTS:
//   - Main Camera with CinemachineBrain
//   - Player or focus point GameObject
// =============================================================================

using UnityEngine;
using Unity.Cinemachine;

public class TopDownSetup : MonoBehaviour
{
    [SerializeField] private Transform player;

    void Start()
    {
        var cam = GetComponent<CinemachineCamera>();
        var follow = GetComponent<CinemachineFollow>();

        // --- CinemachineCamera ---
        cam.Target.TrackingTarget = player;

        // --- Follow (Position Control) ---
        // World Space binding: camera doesn't rotate with target
        // Offset: above and slightly behind the player
        follow.FollowOffset = new Vector3(0, 15, -8);

        // Damping: smooth follow
        follow.TrackerSettings.PositionDamping = new Vector3(2, 2, 2);
    }
}

// =============================================================================
// INSPECTOR SETUP GUIDE:
//
// 1. Create empty GameObject → name "TopDownVCam"
//
// 2. Set its ROTATION in Transform:
//    - X: 60 (look down at 60 degree angle)
//    - Y: 0
//    - Z: 0
//    This is the fixed viewing angle. Cinemachine Follow won't change it
//    in World Space mode.
//
// 3. Add Component: CinemachineCamera
//    - Tracking Target: Player
//
// 4. Add Component: CinemachineFollow
//    - Binding Mode: World Space
//    - Follow Offset: (0, 15, -8)
//      → 15 units above, 8 units behind
//    - Position Damping: (2, 2, 2)
//      → Smooth but responsive
//
// VARIATIONS:
//
//   Isometric:
//     - Rotation: X=30, Y=45, Z=0
//     - Offset: (0, 12, -12)
//
//   Directly overhead:
//     - Rotation: X=90, Y=0, Z=0
//     - Offset: (0, 20, 0)
//
//   RTS with zoom:
//     - Add a script that changes Follow Offset Y based on scroll wheel
//     - Clamp between min/max zoom values
//
// OPTIONAL EXTENSIONS:
//   - Confiner 3D: restrict camera to playable area
//   - Follow Zoom: adjust FOV to keep consistent view size
// =============================================================================
