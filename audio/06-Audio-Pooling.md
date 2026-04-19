# Audio Pooling

`PlayClipAtPoint` creates a new GameObject for every sound. Fine for a few
explosions. Bad for a machine-gun firing 10 rounds per second, or a particle
system that spawns 50 impacts per frame. Pooling reuses a fixed number of
AudioSources instead.

## The Problem PlayClipAtPoint Has

```
gunshot → new GameObject → new AudioSource → Play → Destroy
          (allocation)   (allocation)            (GC pressure)
```

Repeated 600 times per minute = thousands of allocations + destructions. Frame
drops on low-end hardware, GC spikes on mobile.

Also: `PlayClipAtPoint` can't route to an AudioMixerGroup — its temp source has
no Output wired. No ducking, no volume slider integration. Dealbreaker for a
shippable game.

## The Pooling Idea

Pre-create N AudioSources. When you need to play a SFX, grab a free one, play
on it, release it back when done. Zero allocations after startup.

```
Pool of 16 AudioSources:
  [ idle  idle  idle  idle  PLAY  idle  idle  idle  idle  idle  idle  idle  idle  idle  idle  idle ]

                                ▲
                                │
                  PlaySFX(clip, pos) grabs a free one
```

## Minimal Pool (Full version in AudioPool.cs)

```csharp
public class AudioPool : MonoBehaviour
{
    [SerializeField] private int poolSize = 16;
    [SerializeField] private AudioMixerGroup sfxGroup;

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
            src.outputAudioMixerGroup = sfxGroup;
            pool.Add(src);
        }
    }

    public void PlayAt(AudioClip clip, Vector3 pos, float volume = 1f)
    {
        var src = GetFreeSource();
        if (src == null) return; // pool exhausted — dropping the sound

        src.transform.position = pos;
        src.clip = clip;
        src.volume = volume;
        src.spatialBlend = 1f;
        src.pitch = Random.Range(0.95f, 1.05f);
        src.Play();
    }

    private AudioSource GetFreeSource()
    {
        foreach (var s in pool) if (!s.isPlaying) return s;
        return null;
    }
}
```

## Sizing the Pool

- Count peak simultaneous SFX in your worst-case moment (combat, explosion)
- Multiply by 1.5–2 for safety
- 16–32 is typical for indie games; 64+ for shooters with dense action

Too small → sounds get dropped at peaks. Too large → wasted memory (each
AudioSource is cheap but the GameObject overhead adds up on mobile).

## When to Pool vs PlayClipAtPoint vs Existing AudioSource

```
One occasional SFX per scene (ambient mushroom pop once every 10s)
    → Existing AudioSource on the object with PlayOneShot

Fire-and-forget event where the emitter is destroyed (explosion, coin)
    → PlayClipAtPoint (few times per frame max)
    OR SFXPlayer helper that routes to mixer (see SFXPlayer.cs)

High-frequency SFX (gunshots, footsteps of 20 NPCs, ball bounces)
    → Pool
```

## Advanced Variants

- **Priority queue:** when pool is full, steal the source with the lowest
  AudioSource.priority instead of dropping the new sound
- **2D/3D segregation:** separate pool for UI (2D) and world (3D) sounds
- **Group by category:** weapon pool, footstep pool, impact pool — each with
  its own mixer group, easier to balance
- **Per-emitter ownership:** each emitter (e.g., a gun) reserves 3–4 pooled
  sources so it can always overlap its own shots

## Gotchas

- Pool objects under a persistent GameObject (`DontDestroyOnLoad` friendly)
  or recreate per scene
- Pooled 3D sources need their position set BEFORE `Play()` so they start at
  the right location
- Reset fields that carry over between uses (pitch, volume, clip, mute)
- If you set `loop = true` on a pooled source, **release it explicitly** —
  otherwise it never returns to the pool

## When to Use This

- Replacing `PlayClipAtPoint` once performance bites or mixer routing matters
- Shooters, bullet-hell, rhythm games, particle-heavy scenes
- Any sound that can fire dozens of times per second
- Mobile builds where allocations cost frames
