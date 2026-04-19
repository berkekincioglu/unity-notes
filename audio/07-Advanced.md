# Advanced Topics

Concepts beyond the basic "play a clip" setup — Animation Events, adaptive
music, modern Unity 6 audio features, and a few industry-standard tools
(FMOD / Wwise).

## Animation Events

Problem: a character animation shows the foot hitting the ground at exactly
frame 14 of a run cycle. How do you play a footstep clip in sync?

Answer: **Animation Events** — call a method on a MonoBehaviour from a
specific animation frame.

**Steps:**

1. Select a clip in the Animation window (Window → Animation).
2. Scrub to the frame where the foot hits.
3. Click the `Add Event` icon in the timeline ribbon.
4. In Inspector, pick a function from any MonoBehaviour on the animated
   GameObject (e.g., `PlayStep`).
5. Parameters are limited to: `int`, `float`, `string`, `Object`,
   `AnimationEvent`.

Your method signature:

```csharp
public void PlayStep()
{
    audioSource.pitch = Random.Range(0.95f, 1.05f);
    audioSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)]);
}
```

This works identically for 2D sprite animations — spritesheet frames can
trigger events too.

See `FootstepPlayer.cs` for the full pattern.

### Gotchas

- Animation Events only fire while the animation is playing. If you use a
  state machine that blends or interrupts, an event might be skipped.
- Events with `string` parameters are useful for "surface type" routing:
  `PlayStep("wood")`, `PlayStep("metal")`.
- Function must be on a component on the SAME GameObject as the Animator,
  OR on a child with appropriate hierarchy.

## Adaptive / Dynamic Music

Goal: music that responds to gameplay — calmer in exploration, tense near
enemies, full combat track during fights.

### Approach 1: AudioMixer Snapshots (simplest)

Use three snapshots (`Calm`, `Tension`, `Combat`) that differ in track volume.
All three music stems play simultaneously at all times; snapshot transitions
just change their mix levels.

```
Music/Calm      track playing at  0dB in Calm,   -80dB elsewhere
Music/Tension   track playing at -80dB in Calm,  0dB in Tension
Music/Combat    track playing at -80dB in Calm,  0dB in Combat
```

Blend between snapshots with:

```csharp
mixer.TransitionToSnapshots(
    new[] { calmSnap, tensionSnap, combatSnap },
    new[] { 0.7f, 0.3f, 0f },   // weights
    1.5f);                       // seconds
```

See `MusicController.cs`.

### Approach 2: Layered Stems via Code

Multiple AudioSources playing the same track at different intensities. Fade
volumes individually by script. More manual, more control.

### Approach 3: Middleware

For branching / looping / randomized music blocks beyond snapshot mixing,
drop into FMOD or Wwise (see below).

## AudioResource + Random Containers (Unity 6)

Unity 6 added `AudioResource` as a new type that `AudioSource.resource` can
point to. It replaces the old `AudioSource.clip` (which still works).

**Audio Random Container** is an asset that holds multiple clips with
randomization rules (pick random, sequential, shuffle) and per-clip pitch /
volume ranges. Point an AudioSource at one and it picks a different clip
each `Play()`.

```
Project → Create → Audio → Audio Random Container
  Clips: footstep_01, footstep_02, footstep_03, footstep_04
  Volume Randomization: 0.9 – 1.0
  Pitch Randomization: 0.95 – 1.05
  Trigger Mode: Random No-Repeat
```

Assign the container as the AudioSource's resource, and every `Play()`
pulls a different randomized variant. Replaces the "random clip in script"
pattern from a library of repeated code.

## Load Types (Performance)

Set on each AudioClip's import settings:

| Load Type               | Memory                       | CPU                    | Use for                          |
|-------------------------|------------------------------|------------------------|----------------------------------|
| Decompress On Load      | Full uncompressed in RAM     | Zero decode cost       | Short SFX — machine-gun, footsteps |
| Compressed In Memory    | Compressed in RAM, decoded live | Small per-play cost | Medium clips — ambient loops     |
| Streaming               | Tiny — streams from disk     | I/O + decode overhead  | Long tracks — music, long dialog |

Getting this wrong ruins performance: a 3-minute music track as
"Decompress On Load" eats tens of MB of RAM. A 0.2s gunshot as "Streaming"
has hitching at playback start.

## Compression Formats

On the AudioClip import inspector:

- **PCM** — uncompressed. Highest quality, largest size.
- **Vorbis** — lossy but good quality; configurable `Quality` slider (0.0–1.0).
  Default choice for most SFX and music.
- **ADPCM** — lossy, fast to decode. Older / mobile / speech.

Rule: Vorbis ~0.5 for SFX, Vorbis 0.7–1.0 for music, PCM for impacts where
attack transient matters.

## 3D Audio Settings (Project-wide)

`Edit → Project Settings → Audio`:

| Setting                  | What it does                                      |
|--------------------------|---------------------------------------------------|
| Global Volume            | Master multiplier (before mixer)                  |
| Volume Rolloff Scale     | Scales distance-based attenuation                 |
| Doppler Factor           | Global Doppler strength                           |
| Default Speaker Mode     | Stereo / 5.1 / 7.1                                |
| DSP Buffer Size          | Best Latency / Good / Best Performance            |
| Sample Rate              | 44100 / 48000 — target platform dependent         |

DSP Buffer Size matters for latency-sensitive games (rhythm, VR). Larger =
less CPU but more input-to-sound delay.

## FMOD and Wwise (Middleware)

For games with complex audio needs, third-party middleware replaces Unity's
audio system with a far more powerful one:

- **FMOD Studio** — free up to a revenue threshold. Parameter-driven events,
  live tweaking during play, built-in ducking, randomization.
- **Wwise (Audiokinetic)** — industry standard for AAA. Rich authoring, very
  deep pipeline.

When to consider middleware:
- Audio designer is a separate person who wants to tweak without touching code
- 100+ sounds with conditional / parameterized behaviour
- Complex adaptive music (multi-stem, transitions, crossfades across states)
- Porting to many platforms and needing consistent audio authoring

Both have Unity packages and are well-documented. For hobby / small indie
projects, Unity's built-in is enough. Don't add middleware to your first
project.

## When to Use This

- Syncing sounds to animation frames (Animation Events)
- Building a music system that reacts to gameplay (Snapshots)
- Cutting variation-code with Random Containers
- Tuning memory / CPU tradeoffs (Load Type + Compression)
- Deciding whether your game has outgrown the built-in audio system
