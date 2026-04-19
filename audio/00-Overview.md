# Unity Audio — Overview

This folder contains reference notes for Unity's built-in audio system (Unity 6.x).
Start here, then follow the numbered files. Code examples live in the `.cs` files
alongside these notes.

## Architecture

Unity audio has three core pieces:

```
  ┌─────────────────┐      ┌─────────────────┐      ┌─────────────────┐
  │   AudioClip     │─────▶│   AudioSource   │─────▶│  AudioListener  │
  │  (asset)        │      │  (component)    │      │   (component)   │
  │                 │      │                 │      │                 │
  │ .wav .mp3 .ogg  │      │ Emits sound     │      │ The "ears"      │
  │ imported file   │      │ from a GO       │      │ One per scene   │
  └─────────────────┘      └─────────────────┘      └─────────────────┘
```

- **AudioClip** — the imported audio file in `Assets/`. An asset, not a component.
- **AudioSource** — a component on a GameObject that plays clips. Many per scene.
- **AudioListener** — a component that "hears". Exactly ONE per scene. Attached to
  Main Camera by default.

Distance, orientation, and filters between Source and Listener decide how the
sound is rendered.

## Supported Formats

| Format | Use For                    | Notes                                    |
|--------|----------------------------|------------------------------------------|
| `.wav` | Short SFX (shots, impacts) | Uncompressed. Low latency, bigger file.  |
| `.ogg` | Music, long ambient loops  | Open format. Good compression. Preferred.|
| `.mp3` | Music                      | Compressed. Works but `.ogg` preferred.  |
| `.aiff`| Legacy / Apple workflows   | Uncompressed like `.wav`.                |

## Import Settings (Inspector on a clip)

| Setting             | Options                                | When to pick which                          |
|---------------------|----------------------------------------|---------------------------------------------|
| **Load Type**       | Decompress On Load                     | Short SFX that play often (instant start)   |
|                     | Compressed In Memory                   | Medium clips, save RAM                      |
|                     | Streaming                              | Long music tracks, read from disk           |
| **Compression**     | PCM / Vorbis / ADPCM                   | Vorbis = smallest, PCM = fastest            |
| **Preload Audio Data** | On / Off                            | On = faster first play, uses RAM            |
| **Force To Mono**   | On / Off                               | SFX: On (saves memory, spatialization uses mono anyway). Music: Off. |
| **Load In Background**| On / Off                             | On for long Streaming clips                 |

See [07-Advanced.md](07-Advanced.md) for deeper load-type discussion.

## Diegetic vs Non-Diegetic

| Type          | In the game world? | Example                                 |
|---------------|--------------------|-----------------------------------------|
| Diegetic      | Yes                | Footsteps, waterfall, NPC dialogue      |
| Non-Diegetic  | No                 | Menu music, UI click, boss soundtrack   |

Diegetic sounds usually want 3D spatial blend; non-diegetic usually 2D.

## Reading Order

1. [01-AudioSource.md](01-AudioSource.md) — Inspector properties of AudioSource
2. [02-Spatial-Audio.md](02-Spatial-Audio.md) — 3D positional audio
3. [03-Effects-and-Filters.md](03-Effects-and-Filters.md) — Reverb zones + filter components
4. [04-AudioMixer.md](04-AudioMixer.md) — Groups, snapshots, ducking, exposed params
5. [05-Scripting-Patterns.md](05-Scripting-Patterns.md) — Play / PlayOneShot / PlayClipAtPoint
6. [06-Audio-Pooling.md](06-Audio-Pooling.md) — Pooling AudioSources for high-frequency SFX
7. [07-Advanced.md](07-Advanced.md) — Animation events, adaptive music, load types, FMOD/Wwise
8. [08-Genre-Recipes.md](08-Genre-Recipes.md) — Per-genre audio setups

## Code Examples

| File                  | Demonstrates                                      |
|-----------------------|---------------------------------------------------|
| `AudioManager.cs`     | Singleton with typed PlayMusic / PlaySFX API      |
| `SFXPlayer.cs`        | Fire-and-forget 3D SFX with pitch randomization   |
| `MusicController.cs`  | Crossfade tracks + AudioMixer Snapshot transitions|
| `AudioPool.cs`        | Pooled AudioSource for high-frequency SFX         |
| `FootstepPlayer.cs`   | Animation Event-driven footsteps                  |

## When to Use This Folder

- Starting a new Unity project and planning audio architecture
- Debugging "why is this sound silent / too quiet / wrong direction"
- Deciding between PlayClipAtPoint vs pooling vs single AudioSource
- Building a pause menu with working volume sliders
- Implementing adaptive music (calm → combat transitions)
