// =============================================================================
// TPSShooterSetup.cs — Third Person Shooter Camera (TLOU / Gears of War style)
// =============================================================================
// Setup: Third Person Follow + Hard Look At + Deoccluder + Third Person Aim
//
// Over-the-shoulder camera. Built-in obstacle avoidance via the mini-rig.
// Crosshair stays centered with ThirdPersonAim extension.
//
// REQUIRED COMPONENTS on VCam GameObject:
//   - CinemachineCamera
//   - CinemachineThirdPersonFollow   (Position Control)
//   - CinemachineHardLookAt          (Rotation Control)
//   - CinemachineDeoccluder          (Extension — extra wall avoidance)
//   - CinemachineThirdPersonAim      (Extension — steady crosshair)
//
// SCENE REQUIREMENTS:
//   - Main Camera with CinemachineBrain
//   - Player GameObject (assigned as Tracking Target)
//   - Aim target or crosshair UI element
// =============================================================================

using UnityEngine;
using Unity.Cinemachine;

public class TPSShooterSetup : MonoBehaviour
{
    [SerializeField] private Transform player;

    void Start()
    {
        var cam = GetComponent<CinemachineCamera>();
        var tpFollow = GetComponent<CinemachineThirdPersonFollow>();

        // --- CinemachineCamera ---
        cam.Target.TrackingTarget = player;

        // --- Third Person Follow (Position Control) ---
        // Mini-rig: Origin → Shoulder → Hand → Camera
        //
        // Shoulder Offset: right side (positive X), slightly above (positive Y)
        tpFollow.ShoulderOffset = new Vector3(0.8f, 0.3f, 0);

        // Vertical arm length: hand height above shoulder
        tpFollow.VerticalArmLength = 0.3f;

        // Camera side: 1 = right shoulder, 0 = left shoulder
        tpFollow.CameraSide = 1f;

        // Camera distance: how far behind the "hand" the camera sits
        tpFollow.CameraDistance = 3f;

        // Camera collision: which layers can block the camera
        // Set this in Inspector using the layer dropdown
        // tpFollow.CameraCollisionFilter = LayerMask.GetMask("Default", "Environment");

        // Camera radius: minimum clearance from obstacles
        tpFollow.CameraRadius = 0.2f;

        // Damping: how quickly camera reacts to collision
        tpFollow.DampingIntoCollision = 0.1f;  // Fast reaction when blocked
        tpFollow.DampingFromCollision = 0.5f;  // Slow return after clear
    }
}

// =============================================================================
// INSPECTOR SETUP GUIDE:
//
// 1. Create empty GameObject → name "TPSShooterVCam"
// 2. Add Component: CinemachineCamera
//    - Tracking Target: Player
//
// 3. Add Component: CinemachineThirdPersonFollow
//    - Shoulder Offset: (0.8, 0.3, 0)
//    - Vertical Arm Length: 0.3
//    - Camera Side: 1 (right shoulder)
//    - Camera Distance: 3
//    - Camera Radius: 0.2
//    - Damping Into Collision: 0.1
//    - Damping From Collision: 0.5
//
// 4. Add Component: CinemachineHardLookAt
//    (No settings — always centers Look At target)
//
// 5. Add Component: CinemachineDeoccluder
//    - Strategy: Pull Camera Forward
//    - Camera Radius: 0.3
//
// 6. Add Component: CinemachineThirdPersonAim
//    - Aim Target Reticle: (optional — UI crosshair RectTransform)
//    - Keeps crosshair steady despite camera shake/movement
//
// SWITCHING BETWEEN HIP AND AIM:
//   Use two VCams — one for hip-fire (wider), one for ADS (closer, tighter FOV).
//   Switch with CameraManager.Instance.SwitchTo(CameraType.Aim)
//   Configure different FOV and Camera Distance on each.
// =============================================================================
