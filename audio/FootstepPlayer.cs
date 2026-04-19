// =============================================================================
// FootstepPlayer.cs — Animation Event-driven footsteps
// =============================================================================
// Ties together three concepts from this folder:
//   - Animation Events (07-Advanced.md) → the trigger
//   - Random clip selection (05-Scripting-Patterns.md) → variation
//   - Optional surface routing via string parameter → step type switching
//
// HOW IT WORKS:
//   The character's run/walk animation has an Animation Event on the frames
//   where a foot hits the ground. The event calls PlayStep() on this script.
//   Each call picks a random clip + randomized pitch so steps don't feel
//   robotic.
//
// REQUIRED:
//   - Animator (on this GameObject or a parent)
//   - AudioSource (on this GameObject) — routed to SFX mixer group
//   - One or more clip arrays (default + per-surface optional)
//
// OPTIONAL:
//   - Surface detection via raycast or trigger zones that set currentSurface
// =============================================================================

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FootstepPlayer : MonoBehaviour
{
    [Header("Default Surface")]
    [SerializeField] private AudioClip[] defaultClips;

    [Header("Optional Per-Surface Clip Sets")]
    [SerializeField] private AudioClip[] grassClips;
    [SerializeField] private AudioClip[] metalClips;
    [SerializeField] private AudioClip[] waterClips;

    [Header("Variation")]
    [SerializeField] private float pitchRange = 0.05f;
    [SerializeField] private float volumeRange = 0.1f;

    private AudioSource audioSource;
    private string currentSurface = "default";

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update currentSurface from a raycast or physics trigger on your character.
    // Then the next PlayStep call picks from that surface's clip set.
    public void SetSurface(string surface)
    {
        currentSurface = surface;
    }

    // =========================================================================
    // ANIMATION EVENT ENTRY POINTS
    // =========================================================================
    // Bound via the Animation window: Add Event at the frame the foot lands,
    // pick PlayStep (or PlayStepOnSurface with a string parameter).
    // =========================================================================

    public void PlayStep()
    {
        var clips = GetClipsFor(currentSurface);
        PlayRandomClip(clips);
    }

    // Alternate signature that accepts the surface name as the event parameter,
    // for animation authors who prefer putting the surface in the event itself.
    public void PlayStepOnSurface(string surface)
    {
        var clips = GetClipsFor(surface);
        PlayRandomClip(clips);
    }

    // =========================================================================
    // INTERNALS
    // =========================================================================

    private AudioClip[] GetClipsFor(string surface)
    {
        switch (surface)
        {
            case "grass": return grassClips != null && grassClips.Length > 0 ? grassClips : defaultClips;
            case "metal": return metalClips != null && metalClips.Length > 0 ? metalClips : defaultClips;
            case "water": return waterClips != null && waterClips.Length > 0 ? waterClips : defaultClips;
            default:      return defaultClips;
        }
    }

    private void PlayRandomClip(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];

        audioSource.pitch = 1f + Random.Range(-pitchRange, pitchRange);
        float volume = 1f - Random.Range(0f, volumeRange);

        audioSource.PlayOneShot(clip, volume);
    }
}

// =============================================================================
// ANIMATION EVENT SETUP:
//
// 1. Open Animation window (Window → Animation → Animation).
// 2. Select the character, pick the Run/Walk clip.
// 3. Scrub to the frame where the left foot plants.
// 4. Click "Add Event" (the chevron icon in the timeline ribbon).
// 5. In Inspector for that event:
//    - Function: PlayStep
//      OR
//    - Function: PlayStepOnSurface, String: "grass"
// 6. Repeat for the right foot plant frame.
//
// SURFACE DETECTION (optional):
//
//   In your character controller:
//
//     void Update()
//     {
//         if (Physics.Raycast(transform.position, Vector3.down, out var hit, 1.5f))
//         {
//             // Example: read a SurfaceTag MonoBehaviour on the hit collider
//             var tag = hit.collider.GetComponent<SurfaceTag>();
//             footsteps.SetSurface(tag != null ? tag.surfaceId : "default");
//         }
//     }
//
// UNITY 6 ALTERNATIVE:
//
//   Replace the clip arrays with a single AudioResource pointing to an Audio
//   Random Container asset. The container handles randomization internally —
//   no code needed for variation. See 07-Advanced.md.
//
// =============================================================================
