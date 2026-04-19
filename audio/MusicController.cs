// =============================================================================
// MusicController.cs — Music crossfade and mood snapshots
// =============================================================================
// Handles two kinds of music transitions:
//   1. Track swap — crossfade from one AudioClip to another (e.g., level music
//      changing when entering a new area).
//   2. Mood shift — blend between AudioMixer snapshots (e.g., Calm → Combat).
//
// Track crossfade uses two AudioSources (A/B) that ping-pong so the next fade
// always has a free source to fade in on.
//
// Mood snapshots don't change tracks — they change the mixer state. See
// 04-AudioMixer.md for how to set up snapshots.
//
// REQUIRED COMPONENTS:
//   - Two AudioSource components on this GameObject (sourceA, sourceB)
//   - Both should be routed to the Music mixer group
//   - Play On Awake = OFF, Loop = ON
//
// SCENE REQUIREMENTS:
//   - Assigned via AudioManager.cs
// =============================================================================

using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MusicController : MonoBehaviour
{
    [SerializeField] private AudioSource sourceA;
    [SerializeField] private AudioSource sourceB;
    [SerializeField] private float defaultFadeSeconds = 1.5f;

    private AudioSource active;   // currently audible
    private AudioSource standby;  // silent, next to fade in
    private Coroutine currentFade;

    void Awake()
    {
        active = sourceA;
        standby = sourceB;
    }

    // =========================================================================
    // TRACK CROSSFADE
    // =========================================================================

    public void CrossfadeTo(AudioClip clip, float fadeSeconds = -1f)
    {
        if (clip == null) return;
        if (fadeSeconds < 0f) fadeSeconds = defaultFadeSeconds;

        // If the same clip is already active, do nothing
        if (active.clip == clip && active.isPlaying) return;

        // Set up standby with the new clip, then swap
        standby.clip = clip;
        standby.volume = 0f;
        standby.Play();

        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeRoutine(active, standby, fadeSeconds));

        // Swap roles AFTER starting the coroutine — coroutine uses the local refs
        (active, standby) = (standby, active);
    }

    public void FadeOut(float fadeSeconds = -1f)
    {
        if (fadeSeconds < 0f) fadeSeconds = defaultFadeSeconds;

        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeRoutine(active, null, fadeSeconds));
    }

    private IEnumerator FadeRoutine(AudioSource fromSrc, AudioSource toSrc, float duration)
    {
        float t = 0f;
        float startFromVol = fromSrc != null ? fromSrc.volume : 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / duration);

            if (fromSrc != null) fromSrc.volume = Mathf.Lerp(startFromVol, 0f, k);
            if (toSrc != null)   toSrc.volume   = Mathf.Lerp(0f, 1f, k);

            yield return null;
        }

        if (fromSrc != null)
        {
            fromSrc.Stop();
            fromSrc.volume = 0f;
        }
        if (toSrc != null) toSrc.volume = 1f;

        currentFade = null;
    }

    // =========================================================================
    // MOOD SNAPSHOT TRANSITIONS (Mixer-driven, not clip-driven)
    // =========================================================================

    public void TransitionToSnapshot(AudioMixerSnapshot snapshot, float seconds = 1.5f)
    {
        if (snapshot != null) snapshot.TransitionTo(seconds);
    }

    // Blend between multiple snapshots. Weights should sum to 1 for a clean mix.
    // Example: 70% Calm + 30% Tension feels like "something is off, but not yet combat".
    public void BlendSnapshots(AudioMixerSnapshot[] snapshots, float[] weights, float seconds = 1.5f)
    {
        if (snapshots == null || weights == null) return;
        if (snapshots.Length != weights.Length) return;

        // All snapshots belong to the same mixer — read it from the first
        var mixer = snapshots[0].audioMixer;
        mixer.TransitionToSnapshots(snapshots, weights, seconds);
    }
}

// =============================================================================
// INSPECTOR SETUP:
//
// 1. Attach this to the same GameObject as AudioManager (or a child).
//
// 2. Add TWO AudioSource components:
//    - Play On Awake: OFF
//    - Loop: ON
//    - Output: Music mixer group
//    - Spatial Blend: 0 (2D)
//    - Assign each to sourceA and sourceB fields.
//
// 3. To change level music:
//    AudioManager.Instance.PlayMusic(levelTheme, fadeSeconds: 2f);
//
// 4. To enter combat mood:
//    AudioManager.Instance.TransitionMood(combatSnapshot, seconds: 1f);
//
// 5. For 3-way blend (Calm + Tension + Combat):
//    musicController.BlendSnapshots(
//        new[] { calm, tension, combat },
//        new[] { 0.5f, 0.5f, 0f },
//        1.5f);
//
// =============================================================================
