// =============================================================================
// SFXPlayer.cs — Static helper for one-shot SFX
// =============================================================================
// A lightweight wrapper for fire-and-forget sound effects. Fixes two problems
// with PlayClipAtPoint: no mixer routing and no pitch randomization.
//
// USE FOR:
//   - One-off gameplay sounds where the emitter may be destroyed
//   - UI clicks, pickups, small impacts
//   - When pooling is overkill (low-frequency SFX)
//
// FOR HIGH-FREQUENCY SFX (gunshots, footsteps of 20 NPCs), use AudioPool.cs.
//
// USAGE:
//   SFXPlayer.PlayAt(clipRef, transform.position);
//   SFXPlayer.PlayAt(clipRef, transform.position, volume: 0.7f, pitchRange: 0.1f);
//   SFXPlayer.Play2D(uiClickClip);
// =============================================================================

using UnityEngine;
using UnityEngine.Audio;

public static class SFXPlayer
{
    // Optional: set once from AudioManager on boot so routing is automatic
    public static AudioMixerGroup DefaultSFXGroup;

    // =========================================================================
    // 3D Spatial SFX at a world position
    // =========================================================================

    public static void PlayAt(AudioClip clip, Vector3 position, float volume = 1f, float pitchRange = 0.05f)
    {
        if (clip == null) return;

        var go = new GameObject($"SFX_{clip.name}");
        go.transform.position = position;

        var src = go.AddComponent<AudioSource>();
        src.clip = clip;
        src.spatialBlend = 1f;
        src.volume = volume;
        src.pitch = 1f + Random.Range(-pitchRange, pitchRange);

        if (DefaultSFXGroup != null)
            src.outputAudioMixerGroup = DefaultSFXGroup;

        src.Play();

        // Self-destruct after clip finishes. Accounts for pitch so clips pitched
        // up finish sooner and aren't cut off early.
        Object.Destroy(go, clip.length / Mathf.Max(0.01f, src.pitch));
    }

    // =========================================================================
    // 2D SFX (UI, non-positional)
    // =========================================================================

    public static void Play2D(AudioClip clip, float volume = 1f, AudioMixerGroup routeTo = null)
    {
        if (clip == null) return;

        var go = new GameObject($"SFX2D_{clip.name}");

        var src = go.AddComponent<AudioSource>();
        src.clip = clip;
        src.spatialBlend = 0f;
        src.volume = volume;

        src.outputAudioMixerGroup = routeTo != null ? routeTo : DefaultSFXGroup;

        src.Play();
        Object.Destroy(go, clip.length);
    }

    // =========================================================================
    // Random-variant convenience
    // =========================================================================

    public static void PlayRandomAt(AudioClip[] variants, Vector3 position, float volume = 1f, float pitchRange = 0.05f)
    {
        if (variants == null || variants.Length == 0) return;
        PlayAt(variants[Random.Range(0, variants.Length)], position, volume, pitchRange);
    }
}

// =============================================================================
// PATTERN NOTES:
//
// Why not just use AudioSource.PlayClipAtPoint?
//   - PlayClipAtPoint gives you no mixer routing. No ducking, no volume sliders.
//   - PlayClipAtPoint has no pitch randomization parameter.
//   - This wrapper adds both in ~30 lines.
//
// Lifetime:
//   The temporary GameObject destroys itself using Destroy(go, delay).
//   Delay accounts for pitch > 1 (clip finishes sooner) to avoid cutting off.
//
// AOT / Script Order:
//   SFXPlayer.DefaultSFXGroup should be assigned once on game boot.
//   Do it from AudioManager.Awake():
//       SFXPlayer.DefaultSFXGroup = sfxGroup;
//
// When to switch to AudioPool.cs:
//   If you see "SFX_xxx" GameObjects spawning by the dozen per second in the
//   Hierarchy during gameplay, pool them instead.
//
// =============================================================================
