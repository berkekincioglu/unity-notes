// =============================================================================
// AudioPool.cs — Pooled AudioSources for high-frequency SFX
// =============================================================================
// Pre-creates N AudioSources and reuses them. Avoids the allocation cost of
// PlayClipAtPoint / SFXPlayer.PlayAt when sounds fire many times per second.
//
// USE FOR:
//   - Gunshots, machine-gun fire
//   - Footsteps of many NPCs simultaneously
//   - Impacts from physics-heavy scenes
//   - Particle-system-driven SFX
//
// FOR ONE-OFF SFX, use SFXPlayer.cs — pooling is overkill.
//
// SCENE REQUIREMENTS:
//   - Attach to a persistent GameObject (same as AudioManager)
//   - Assign sfxMixerGroup so pooled sources route through the SFX bus
// =============================================================================

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioPool : MonoBehaviour
{
    [Header("Pool Config")]
    [SerializeField] private int poolSize = 16;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    [Header("Default SFX Behaviour")]
    [SerializeField] private float defaultPitchRange = 0.05f;

    private List<AudioSource> pool;

    void Awake()
    {
        pool = new List<AudioSource>(poolSize);

        for (int i = 0; i < poolSize; i++)
        {
            var go = new GameObject($"PooledAudio_{i}");
            go.transform.SetParent(transform);

            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            src.outputAudioMixerGroup = sfxMixerGroup;

            pool.Add(src);
        }
    }

    // =========================================================================
    // PLAY — 3D spatial, at a world position
    // =========================================================================

    public void PlayAt(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return;

        var src = GetFreeSource();
        if (src == null) return; // pool exhausted, drop this sound

        src.transform.position = position;
        src.spatialBlend = 1f;
        src.clip = clip;
        src.volume = volume;
        src.pitch = 1f + Random.Range(-defaultPitchRange, defaultPitchRange);
        src.outputAudioMixerGroup = sfxMixerGroup;
        src.Play();
    }

    // =========================================================================
    // PLAY — 2D, non-spatial (UI, menu, non-positional SFX)
    // =========================================================================

    public void Play2D(AudioClip clip, float volume = 1f, AudioMixerGroup group = null)
    {
        if (clip == null) return;

        var src = GetFreeSource();
        if (src == null) return;

        src.transform.position = transform.position;
        src.spatialBlend = 0f;
        src.clip = clip;
        src.volume = volume;
        src.pitch = 1f;
        src.outputAudioMixerGroup = group != null ? group : sfxMixerGroup;
        src.Play();
    }

    // =========================================================================
    // POOL INTERNALS
    // =========================================================================

    private AudioSource GetFreeSource()
    {
        // Naive linear scan — fine for pool sizes up to ~64.
        // For larger pools, track an index or a free list for O(1) pickup.
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].isPlaying) return pool[i];
        }

        // Pool exhausted. Choice: drop this sound (current), or steal the
        // longest-running one. Dropping is safer — prevents audible cutoffs.
        return null;
    }
}

// =============================================================================
// INSPECTOR SETUP:
//
// 1. Attach to the AudioManager GameObject (or any persistent object).
//
// 2. Pool Size:
//    - 16  : light SFX load (platformer, puzzle)
//    - 32  : moderate action (TPS, brawler)
//    - 48+ : dense combat (shooters, bullet hell)
//
// 3. SFX Mixer Group:
//    - Drag the SFX group from your AudioMixer asset.
//    - All pooled playback routes here by default.
//
// USAGE FROM GAMEPLAY CODE:
//
//   AudioManager.Instance.PlaySFX(gunshotClip, firePoint.position);
//
// That call inside AudioManager forwards to AudioPool.PlayAt. No allocations
// after initial pool creation.
//
// SIZING THE POOL:
//   Count the max simultaneous SFX in your worst-case moment.
//   Multiply by 1.5–2 for safety.
//   If you see audible cutoffs during playtest, increase. If you rarely see
//   more than half in use, decrease.
//
// =============================================================================
