# AudioSource — Inspector Properties

The AudioSource component plays AudioClips from a GameObject's position. Every
sound-producing object in your scene has one. This file covers the non-spatial
properties — see [02-Spatial-Audio.md](02-Spatial-Audio.md) for 3D distance
behaviour.

## Non-Spatial Properties

| Property              | Default | What it does                                             | When to change                                      |
|-----------------------|---------|----------------------------------------------------------|-----------------------------------------------------|
| **AudioResource**     | (none)  | The clip (or Random Container) to play                   | Always assign one, or `Play()` does nothing         |
| **Output**            | (none)  | AudioMixerGroup routing destination                      | Route to Music / SFX / UI group (see 04)            |
| **Mute**              | Off     | Silences without stopping playback                       | Debugging                                           |
| **Bypass Effects**    | Off     | Skip filters attached to this AudioSource                | Temporarily A/B a filter                            |
| **Bypass Listener FX**| Off     | Skip effects on the AudioListener                        | Dialog clarity — bypass global reverb on VO         |
| **Bypass Reverb Zones**| Off    | Ignore Reverb Zones this source passes through           | UI sounds that shouldn't echo in caves              |
| **Play On Awake**     | **On**  | Start playback automatically when the scene loads        | OFF for event-triggered SFX. ON for ambient / music |
| **Loop**              | Off     | Restart the clip when it ends                            | ON for music, ambient drones, fire crackle          |
| **Priority**          | 128     | 0 = never stolen. 256 = first to be dropped.             | Music/dialog = 0. Distant ambient = 200+            |
| **Volume**            | 1.0     | 0 = silent, 1 = max                                      | Usually keep at 1 and balance via mixer             |
| **Pitch**             | 1.0     | Playback speed AND frequency (1.5 = +7 semitones)        | Randomize ±0.1 to break repetition (see 05)         |
| **Stereo Pan**        | 0       | -1 = left, 0 = center, 1 = right (2D sounds only)        | Left/right UI cues                                  |
| **Spatial Blend**     | 0       | 0 = 2D, 1 = 3D                                           | See [02-Spatial-Audio.md](02-Spatial-Audio.md)      |
| **Reverb Zone Mix**   | 1.0     | How much wet signal Reverb Zones contribute              | Lower to reduce reverb effect without disabling     |

## Priority Matters

Unity plays a limited number of voices at once (32 on most platforms). When you
exceed that, the system silences sources with the HIGHEST priority number first.

```
Priority 0   ← Music, dialog (never steal these)
Priority 128 ← Default — general SFX
Priority 256 ← Distant ambient, low-importance sounds (OK to drop)
```

Rule of thumb: if a sound disappearing would ruin the scene, lower its priority.

## Play On Awake Trap

`Play On Awake` is **on by default**. If you add an AudioSource for a one-shot
SFX (gunshot, jump) and leave this on, the sound plays the moment the scene
loads — often unwanted.

```
Ambient / music / scene loop  →  Play On Awake: ON,  Loop: ON
One-shot event (jump, hit)    →  Play On Awake: OFF, Loop: OFF
                                  (trigger via script, see 05)
```

## Volume vs Mixer

Setting `Volume` per source gets unwieldy fast. Prefer:

1. Keep `Volume` at 1.0 on the source
2. Route through an AudioMixerGroup (`Output` field)
3. Balance relative levels at the Mixer
4. Let player settings control the Mixer's exposed parameters

See [04-AudioMixer.md](04-AudioMixer.md).

## Pitch Has Two Effects

Unlike some engines, Unity's `pitch` changes both speed AND frequency together.

```
pitch = 1.0  → normal
pitch = 1.5  → 50% faster, up about 7 semitones (chipmunk)
pitch = 0.5  → half speed, down an octave (slow-motion)
pitch = -1.0 → plays in reverse (curiosity more than a tool)
```

For randomized-but-same-speed variation, use multiple clips + a Random Container
(Unity 6) or random clip selection in script (see [07-Advanced.md](07-Advanced.md)).

## When to Use This

- Choosing Play On Awake vs script-triggered per sound
- Deciding priority tiers across your project
- Understanding why a sound randomly cuts out under load (priority)
- Routing every source to a mixer group before shipping (always)
