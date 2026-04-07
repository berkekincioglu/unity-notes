// =============================================================================
// Platformer2DSetup.cs — 2D Platformer Camera (Celeste / Hollow Knight style)
// =============================================================================
// Setup: Position Composer + Confiner 2D
//
// Camera follows player with dead zone (breathing room).
// Confiner keeps screen edges within the level boundary.
//
// REQUIRED COMPONENTS on VCam GameObject:
//   - CinemachineCamera
//   - CinemachinePositionComposer    (Position Control)
//   - CinemachineConfiner2D          (Extension)
//
// SCENE REQUIREMENTS:
//   - Main Camera with CinemachineBrain (Orthographic projection)
//   - Player GameObject
//   - Empty GameObject with PolygonCollider2D (level boundary shape)
// =============================================================================

using UnityEngine;
using Unity.Cinemachine;

public class Platformer2DSetup : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Collider2D levelBounds; // PolygonCollider2D for confiner

    void Start()
    {
        var cam = GetComponent<CinemachineCamera>();
        var composer = GetComponent<CinemachinePositionComposer>();
        var confiner = GetComponent<CinemachineConfiner2D>();

        // --- CinemachineCamera ---
        cam.Target.TrackingTarget = player;

        // Orthographic lens for 2D
        cam.Lens.OrthographicSize = 5f;

        // --- Position Composer (Position Control) ---
        // Camera Distance: how far the camera is from the 2D plane
        composer.CameraDistance = 10f;

        // Damping: higher = smoother, lazier follow
        composer.Damping = new Vector3(1.5f, 1f, 0);

        // --- Confiner 2D (Extension) ---
        confiner.BoundingShape2D = levelBounds;
    }
}

// =============================================================================
// INSPECTOR SETUP GUIDE:
//
// 1. LEVEL BOUNDARY SETUP:
//    - Create empty GameObject → name "LevelBounds"
//    - Add Component: PolygonCollider2D
//    - Edit the polygon to match your level shape
//    - Set "Is Trigger" = true (so it doesn't collide with anything)
//    - Set to a layer that physics ignores
//
// 2. MAIN CAMERA:
//    - Projection: Orthographic
//    - Add Component: CinemachineBrain
//
// 3. Create empty GameObject → name "Platformer2DVCam"
//
// 4. Add Component: CinemachineCamera
//    - Tracking Target: Player
//    - Lens > Orthographic Size: 5 (adjust for your pixel density)
//
// 5. Add Component: CinemachinePositionComposer
//    - Camera Distance: 10
//    - Dead Zone: (0.15, 0.1)
//      → Player moves this much before camera reacts
//      → Gives "breathing room" — camera doesn't chase every step
//    - Soft Zone: (0.6, 0.6)
//      → Camera starts following with damping in this zone
//    - Screen Position: (0, 0)
//      → Player centered. Adjust to (0, -0.1) to show more sky
//    - Damping: (1.5, 1, 0)
//      → X slightly lazier than Y for natural platformer feel
//    - Lookahead Time: 0.3 (optional)
//      → Camera subtly moves ahead of player's direction
//      → Can cause jitter — test and lower if needed
//
// 6. Add Component: CinemachineConfiner2D
//    - Bounding Shape 2D: LevelBounds (the PolygonCollider2D)
//    - Damping: 0.5
//    - Slowing Distance: 0.3
//
// DEAD ZONE VISUAL:
//
//   ┌──────────────────────────────┐
//   │         SOFT ZONE            │
//   │    ┌──────────────────┐      │
//   │    │    DEAD ZONE     │      │
//   │    │       ●          │      │  Player in dead zone
//   │    │    (player)      │      │  → camera is still
//   │    └──────────────────┘      │
//   │                              │  Player walks to soft zone
//   │                              │  → camera starts following
//   └──────────────────────────────┘
//
// OPTIONAL:
//   - Pixel Perfect extension: prevents sub-pixel jitter in pixel art
//   - Noise: subtle breathing bob (very low amplitude ~0.02)
// =============================================================================
