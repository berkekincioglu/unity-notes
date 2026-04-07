// =============================================================================
// CutsceneSetup.cs — Cutscene / Cinematic Camera
// =============================================================================
// Setup: Spline Dolly + Rotation Composer
//
// Camera moves along a predefined path (spline) while looking at a target.
// Multiple VCams with blending create a cinematic sequence.
//
// REQUIRED COMPONENTS on VCam GameObject:
//   - CinemachineCamera
//   - CinemachineSplineDolly        (Position Control)
//   - CinemachineRotationComposer   (Rotation Control)
//
// SCENE REQUIREMENTS:
//   - Main Camera with CinemachineBrain
//   - Spline Container (the path for the camera to follow)
//   - Look At target (character, point of interest, etc.)
// =============================================================================

using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Splines;

public class CutsceneSetup : MonoBehaviour
{
    [SerializeField] private SplineContainer dollyPath;
    [SerializeField] private Transform lookAtTarget;
    [SerializeField] private float dollySpeed = 0.5f;

    private CinemachineCamera cam;
    private CinemachineSplineDolly dolly;
    private bool isPlaying;

    void Start()
    {
        cam = GetComponent<CinemachineCamera>();
        dolly = GetComponent<CinemachineSplineDolly>();

        // --- CinemachineCamera ---
        cam.Target.TrackingTarget = lookAtTarget;

        // --- Spline Dolly (Position Control) ---
        dolly.Spline = dollyPath;

        // Start at the beginning of the spline
        dolly.CameraPosition = 0;
    }

    // =========================================================================
    // CUTSCENE PLAYBACK
    // =========================================================================

    public void PlayCutscene()
    {
        isPlaying = true;
        dolly.CameraPosition = 0;
        cam.Priority = 20; // Higher than gameplay cameras
    }

    public void StopCutscene()
    {
        isPlaying = false;
        cam.Priority = 0; // Return to gameplay camera
    }

    void Update()
    {
        if (!isPlaying) return;

        // Move camera along the spline
        dolly.CameraPosition += dollySpeed * Time.deltaTime;

        // Stop at the end of the spline (normalized: 1 = end)
        if (dolly.CameraPosition >= 1f)
        {
            StopCutscene();
        }
    }
}

// =============================================================================
// INSPECTOR SETUP GUIDE:
//
// STEP 1 — CREATE THE SPLINE PATH:
//   - GameObject > Spline > Draw Spline Container
//   - Click in Scene View to place knots (control points)
//   - Adjust tangent handles for smooth curves
//   - This defines the camera's flight path
//
// STEP 2 — CREATE CUTSCENE VCAM:
//   - Create empty GameObject → name "CutsceneVCam"
//   - Add Component: CinemachineCamera
//     - Tracking Target: (what camera should look at)
//     - Priority: 0 (inactive until cutscene triggers)
//
//   - Add Component: CinemachineSplineDolly
//     - Spline: (drag the SplineContainer here)
//     - Position Units: Normalized (0-1 across entire path)
//     - Camera Rotation: Default or Path
//     - Damping: (0.5, 0.5, 0.5)
//
//   - Add Component: CinemachineRotationComposer
//     - Screen Position: (0, 0) for centered
//     - Damping: (2, 2) for smooth rotation
//
// STEP 3 — TRIGGER FROM CODE:
//   cutsceneSetup.PlayCutscene();
//   // Camera takes Priority 20, blends in via CinemachineBrain
//   // Moves along spline, looking at target
//   // At spline end, priority drops, blends back to gameplay cam
//
// MULTI-SHOT CUTSCENES:
//   For complex cutscenes with multiple angles:
//   - Create multiple VCams, each with its own spline or fixed position
//   - Use Timeline to sequence them (activate/deactivate over time)
//   - Timeline overrides CinemachineBrain priority for precise control
//
// ALTERNATIVE: AUTOMATIC DOLLY
//   Instead of manual playback in Update, use Auto-Dolly:
//   - Spline Dolly > Automatic Dolly > Fixed Speed
//   - Set speed in Inspector
//   - No code needed — camera moves on its own when active
// =============================================================================
