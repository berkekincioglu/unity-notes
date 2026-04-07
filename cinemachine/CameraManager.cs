// =============================================================================
// CameraManager.cs — Centralized Camera Switching (Singleton)
// =============================================================================
// Manages priority-based switching between CinemachineCameras.
// Follows the same Singleton pattern as InputManager.
//
// USAGE:
//   CameraManager.Instance.SwitchTo(cameraType);
//   CameraManager.Instance.SwitchTo(CameraType.ThirdPerson);
//
// HOW IT WORKS:
//   - All virtual cameras start with Priority 0
//   - SwitchTo() sets the target camera to Priority 10, others to 0
//   - CinemachineBrain detects the change and blends automatically
//   - You configure blend style/time on the Brain, not here
// =============================================================================

using UnityEngine;
using Unity.Cinemachine;

public enum CameraType
{
    ThirdPerson,
    Aim,
    TopDown,
    Cutscene
    // Add more as needed
}

public class CameraManager : MonoBehaviour
{
    // =========================================================================
    // SINGLETON
    // =========================================================================

    public static CameraManager Instance { get; private set; }

    // =========================================================================
    // CAMERA REFERENCES — Assign in Inspector
    // =========================================================================
    // Each field holds a reference to a CinemachineCamera in the scene.
    // Drag them from the Hierarchy into these Inspector slots.
    // =========================================================================

    [Header("Virtual Cameras")]
    [SerializeField] private CinemachineCamera thirdPersonCam;
    [SerializeField] private CinemachineCamera aimCam;
    [SerializeField] private CinemachineCamera topDownCam;
    [SerializeField] private CinemachineCamera cutsceneCam;

    // Active priority value. Inactive cameras get 0.
    private const int ActivePriority = 10;
    private const int InactivePriority = 0;

    // =========================================================================
    // INITIALIZATION
    // =========================================================================

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Default to third person on game start
        SwitchTo(CameraType.ThirdPerson);
    }

    // =========================================================================
    // CAMERA SWITCHING
    // =========================================================================
    // Only ONE camera should have high priority at a time.
    // CinemachineBrain on Main Camera handles the blend automatically.
    //
    // The blend style and duration are configured on:
    //   Main Camera > CinemachineBrain > Default Blend (or Custom Blends asset)
    // =========================================================================

    public void SwitchTo(CameraType type)
    {
        // Reset all to inactive
        SetAllInactive();

        // Activate the requested camera
        switch (type)
        {
            case CameraType.ThirdPerson:
                thirdPersonCam.Priority = ActivePriority;
                break;
            case CameraType.Aim:
                aimCam.Priority = ActivePriority;
                break;
            case CameraType.TopDown:
                topDownCam.Priority = ActivePriority;
                break;
            case CameraType.Cutscene:
                cutsceneCam.Priority = ActivePriority;
                break;
        }
    }

    private void SetAllInactive()
    {
        if (thirdPersonCam != null) thirdPersonCam.Priority = InactivePriority;
        if (aimCam != null) aimCam.Priority = InactivePriority;
        if (topDownCam != null) topDownCam.Priority = InactivePriority;
        if (cutsceneCam != null) cutsceneCam.Priority = InactivePriority;
    }
}

// =============================================================================
// UNITY SETUP:
//
// 1. Create empty GameObject → name it "CameraManager"
// 2. Attach this script
// 3. In Inspector, drag your CinemachineCameras into the slots
// 4. Make sure Main Camera has CinemachineBrain component
// 5. Configure blend style/time on CinemachineBrain (not here)
//
// EXAMPLE USAGE FROM OTHER SCRIPTS:
//
//   // Player aims down sights
//   CameraManager.Instance.SwitchTo(CameraType.Aim);
//
//   // Player stops aiming
//   CameraManager.Instance.SwitchTo(CameraType.ThirdPerson);
//
//   // Cutscene triggered
//   CameraManager.Instance.SwitchTo(CameraType.Cutscene);
// =============================================================================
