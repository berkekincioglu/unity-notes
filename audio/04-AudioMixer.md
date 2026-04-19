# AudioMixer

The AudioMixer is Unity's bus-and-routing system. You build a tree of
AudioMixerGroups, route every AudioSource to one of them, and control the
whole game's audio from one asset. It is the single most important tool for
shipping a game that doesn't sound amateur.

## Create One

```
Project → right-click → Create → Audio Mixer
```

Open the asset with `Window > Audio > Audio Mixer`.

## Group Structure (Typical)

```
Master
  ├── Music
  ├── SFX
  │    ├── Weapons
  │    ├── Footsteps
  │    └── Ambient
  ├── Voice
  └── UI
```

Every AudioSource in the scene has an `Output` field. Drag an AudioMixerGroup
into it. Now that source is routed.

```
AudioSource.Output → AudioMixerGroup → parent group → … → Master → AudioListener
```

## Why This Matters

Without the mixer, every source has its own `Volume` field. You ship the game,
the player moves the music slider, and you've wired up nothing. With the mixer:

1. Mixer group volume controls all sources routed to it.
2. One parameter, **exposed** to script, drives a slider in your Options menu.
3. You balance once, at the mixer, not per-source.

## Effects on Groups

Right-click a group → `Add Effect`. Same filter types as per-source (Low Pass,
High Pass, Reverb, Chorus, etc.) — but applied to the summed output of that
group. More efficient than per-source filters when many sources share the
same effect.

Common patterns:
- Reverb effect on the SFX group → all SFX share the same room character
- Low Pass on Master → global muffle when paused / menu opened
- Compressor on Music → keeps music loud at low listener volumes
- Send/Receive → route a copy of SFX into a side chain for ducking

## Exposed Parameters (For Volume Sliders)

Steps to make a settings-menu slider actually work:

1. In the AudioMixer window, select a group (e.g., Music).
2. Inspector → right-click on `Volume` → `Expose ... to script`.
3. Top-right of Mixer window: rename the exposed param to something readable
   (`MusicVolume`).
4. From script:

```csharp
mixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20f);
```

Mixer volume is in **decibels** (-80 to 0 typically), but a UI slider is
linear 0–1. Convert with `Mathf.Log10(linear) * 20f`. Clamp `sliderValue > 0`
before the log.

See `AudioManager.cs` for the full implementation.

## Snapshots (Dynamic Mood Changes)

A Snapshot is a saved state of the entire mixer (all volumes, all effect
parameters). You can transition between them at runtime.

```
Snapshots on the mixer:
  Default      (normal play)
  MenuOpen     (music up, SFX down, slight low-pass on Master)
  LowHealth    (music muffled, heartbeat bus up)
  Combat       (SFX punchier, ambient quieter)
  Paused       (everything ducked 20dB, low-pass on Master)
```

Transition from script:

```csharp
[SerializeField] private AudioMixerSnapshot defaultSnapshot;
[SerializeField] private AudioMixerSnapshot combatSnapshot;

combatSnapshot.TransitionTo(timeInSeconds: 1.5f);
```

For blending between multiple snapshots at once (e.g., 40% Combat + 60% Default):

```csharp
AudioMixer.TransitionToSnapshots(snapshots, weights, timeToReach);
```

See `MusicController.cs` for a combat-blend example.

## Ducking (Auto-Dip on Events)

Ducking = automatically lower one bus when another plays. Example: music dips
when dialog plays so speech is clear.

**Approach (side-chain ducking):**
1. Mixer group `Music` has a `Duck Volume` effect added.
2. Mixer group `Dialog` has a `Send` effect targeting the Music's Duck Volume.
3. Whenever the Dialog group has audio, it triggers the duck on Music.
4. Ratio, threshold, attack, release configured on the Duck Volume.

No scripting required once wired — purely a mixer graph.

## Mixing Discipline

- Every AudioSource → route to a group. No exceptions. "Master only" is a code
  smell.
- Balance using the mixer sliders during playtest, not by editing source
  volumes.
- Decide -6dB / -12dB / -20dB zones for categories (music vs SFX vs ambient)
  and enforce them.
- Use snapshots instead of hand-crafting fades from script when possible.

## When to Use This

- Shipping settings-menu volume sliders that actually work
- Combat / menu / pause music transitions
- Keeping dialog intelligible over music (ducking)
- Global effects (pause low-pass, underwater muffle)
- Re-balancing levels without touching the scene
