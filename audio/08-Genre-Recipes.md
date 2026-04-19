# Genre Recipes

A quick-reference mapping between game genres and the audio stack that
typically fits. Pick a row, then dig into the referenced files and code
examples. These are starting points, not rules.

## Recipe Table

| Genre              | Music             | Ambient            | SFX pattern                         | Mixer Snapshots              | Example files                         |
|--------------------|-------------------|--------------------|-------------------------------------|------------------------------|---------------------------------------|
| **FPS / Shooter**  | Adaptive combat   | Looped 3D layers   | Pooled gunshots + PlayOneShot       | Calm / Alerted / Combat      | `AudioPool.cs`, `SFXPlayer.cs`        |
| **2D Platformer**  | Single track loop | Level-specific loop| PlayOneShot (jump, pickup)          | Default / Boss / Death       | `SFXPlayer.cs`, `AudioManager.cs`     |
| **Racing**         | Looped high-energy| Engine loop (pitch from RPM) | PlayOneShot (skid, crash) | Default / Menu / PostRace    | `SFXPlayer.cs`                        |
| **Stealth / Horror**| Tension stems    | Sparse layered     | PlayClipAtPoint (surprise cues)     | Normal / Detected / Chase    | `MusicController.cs`, `SFXPlayer.cs`  |
| **RTS / Top-down** | Adaptive by battle| Map-wide loop      | Pooled unit sounds + dialog priority| Peace / Combat               | `AudioPool.cs`                        |
| **Cinematic / Narrative** | Scored cues | Minimal            | Timeline events + PlayOneShot       | Per-scene                    | `AudioManager.cs`                     |
| **Rhythm**         | THE gameplay      | Optional           | Precise PlayScheduled calls         | Pre / Play / Results         | Custom beat-accurate scheduling       |

## FPS — Detail

- **Listener** on Player (not Camera — first-person head position)
- **Weapons** pool at least 8 AudioSources, assigned per weapon
- **Footsteps** use Animation Events + random clip per surface material
- **Ambient** 2–4 spatialized loops per area (wind, machinery, distant)
- **Mixer** Weapons group with compressor, Music group ducked by Dialog
- **Snapshots** Calm (exploration), Alerted (enemy nearby), Combat (active fight)

## 2D Platformer — Detail

- **Listener** on Main Camera
- **Music** single track, Play On Awake + Loop
- **SFX** per-action AudioSources on the player OR static `SFXPlayer.PlayAt`
- **Ambient** one 2D loop for level feel; 3D loops on hazards (lava, saw)
- **Special: Cave / interior transitions** — place Audio Reverb Zones

## Racing — Detail

- **Listener** on Camera (third-person) or driver (first-person)
- **Engine loop** AudioSource on vehicle with `pitch` driven by RPM
- **Wind** Low Pass Filter strength inversely proportional to speed
- **Doppler Level** 1 on other vehicles for pass-bys
- **Skids / impacts** PlayOneShot with pitch randomization

## Stealth / Horror — Detail

- **Music** three-snapshot system: Calm / Tension / Chase; use snapshots blend
- **Ambient** sparse, long, looped 3D sources set at key points
- **Stings** PlayClipAtPoint for sudden scares (won't be interrupted by destroy)
- **Mixer** Low Pass on Master when hiding (Peek / Cover) for muffled POV
- **Dialog** highest priority, drives ducking on Music and Ambient groups

## RTS / Top-down — Detail

- **Listener** on Camera (follows cursor/focus)
- **Unit sounds** prioritized — selected units = priority 0, enemies off-screen = 192+
- **Ambient** wide map-level 2D loop + spot 3D loops on buildings
- **Music** Peace / Combat snapshot blend based on active engagement

## Cinematic / Narrative — Detail

- **Timeline** is your friend — Timeline can trigger AudioSources with Signal Emitters
- **Dialog** separate mixer group, always priority 0, routed through Dialog bus
- **Music** often scored per-scene, swap via snapshots on scene load

## Rhythm Games — Special Note

Standard `Play()` is not sample-accurate. Use `AudioSource.PlayScheduled(dsptime)`
with `AudioSettings.dspTime` for beat-perfect timing. This is niche — out of
scope for most projects but worth knowing exists.

## Starting a New Project — Minimum Audio Setup

Regardless of genre:

```
1. Create AudioMixer with Master/Music/SFX/UI groups
2. Expose MusicVolume, SFXVolume, UIVolume on Master's children
3. Add AudioManager.cs to a bootstrap scene (DontDestroyOnLoad)
4. Route EVERY AudioSource you make to a mixer group
5. Implement volume sliders in Options menu that call AudioManager
```

If you do only this, your game already sounds more shipped than most student
projects.

## When to Use This

- Starting a new project and sketching the audio architecture
- Debating pool vs PlayClipAtPoint for your specific game
- Deciding what snapshots to define
- Choosing what belongs on the mixer vs the source
