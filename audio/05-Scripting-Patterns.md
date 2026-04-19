# Scripting Patterns

Inspector setup gets you far — `Play On Awake` handles music and ambient. But
event-driven SFX (a gunshot when the player fires, a sound when you pick up a
coin) need scripting. This file covers the patterns.

## The Three Play Methods

```csharp
audioSource.Play();                  // plays the clip on the source
audioSource.PlayOneShot(clip);       // plays clip ON TOP of whatever is already playing
AudioSource.PlayClipAtPoint(clip, pos); // static — creates a temp source at position
```

| Method               | Starts from scratch? | Overlaps? | Needs AudioSource? |
|----------------------|---------------------|-----------|--------------------|
| `Play()`             | Yes — restarts      | No        | Yes                |
| `PlayOneShot(clip)`  | No — layered        | Yes       | Yes                |
| `PlayClipAtPoint`    | N/A — new temp GO   | Yes       | No (static)        |

### When to Use Each

- **Play()** — looping sounds, music, anything you'll also `Stop()`.
- **PlayOneShot()** — high-frequency SFX on a known source (machine-gun,
  footsteps on a character).
- **PlayClipAtPoint()** — fire-and-forget sounds from an object that might get
  destroyed (explosion, coin pickup). Unity creates a hidden GameObject,
  plays the clip, destroys it when done.

## Cache GetComponent in Awake

```csharp
public class Coin : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        audioSource.Play();
    }
}
```

Rule: anything you call more than once, cache once in `Awake`. Calling
`GetComponent<AudioSource>()` inside `OnTriggerEnter` is a common slow-down.

## The Destruct-On-Death Trap

```csharp
// ❌ BAD — the AudioSource gets destroyed with the GameObject
void OnCollisionEnter(Collision c)
{
    audioSource.Play();
    Destroy(gameObject);      // cuts the sound off instantly
}
```

`Destroy(gameObject)` runs after the current frame, BUT an AudioSource cannot
keep playing after its GameObject is gone. Two fixes:

### Fix 1: Play the sound from something that survives

```csharp
// ✅ The collider is something that isn't being destroyed (e.g., the enemy)
void OnCollisionEnter(Collision c)
{
    c.gameObject.GetComponent<AudioSource>()?.Play();
    Destroy(gameObject);
}
```

### Fix 2: PlayClipAtPoint (simplest for explosions, pickups)

```csharp
[SerializeField] private AudioClip deathClip;

void OnCollisionEnter(Collision c)
{
    AudioSource.PlayClipAtPoint(deathClip, transform.position);
    Destroy(gameObject);
}
```

Unity spawns a temporary GameObject with an AudioSource, plays the clip, and
destroys itself when the clip ends. You don't manage the lifecycle.

**Downside:** no mixer routing (goes straight to the listener), no pitch
randomization out of the box. For that, use `SFXPlayer.cs` (helper wrapper).

## Pitch Randomization (Break Repetition)

Same clip played 5 times in a row sounds robotic. Vary the pitch:

```csharp
audioSource.pitch = Random.Range(0.95f, 1.05f);
audioSource.PlayOneShot(footstepClip);
```

- `±0.05` — subtle, natural variation
- `±0.10` — noticeable but still the same sound
- `±0.20` — arcade / stylized

Randomize the clip too if you have 3–4 variations:

```csharp
[SerializeField] private AudioClip[] footstepClips;
audioSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)]);
```

See `FootstepPlayer.cs` for the animation-event-driven version.

## PlayOneShotHelpers — The PlayOneShot Volume Argument

```csharp
audioSource.PlayOneShot(clip, volumeScale: 0.6f);
```

`volumeScale` multiplies this one-shot's volume without touching the
AudioSource's own Volume field. Useful when one clip is naturally loud.

## Stop, Pause, UnPause

```csharp
audioSource.Stop();      // stops and rewinds
audioSource.Pause();     // stops, keeps position
audioSource.UnPause();   // resumes from paused position
audioSource.isPlaying;   // bool you can check
audioSource.time;        // current playhead in seconds
```

Pause/UnPause is mostly for implementing a Pause menu; most of the time you
just stop or let clips finish.

## Subscribing to "Finished" (There's No Event)

Unity has no `onClipFinished` callback. Two workarounds:

```csharp
// 1. Coroutine
yield return new WaitWhile(() => audioSource.isPlaying);
// continue...

// 2. Scheduled task based on clip.length
Invoke(nameof(OnClipDone), clip.length);
```

Use coroutines for longer / interruptible clips; `Invoke` for simple fire-once
timing.

## When to Use This

- Triggering SFX from gameplay code (pickup, hit, jump)
- Avoiding the cut-sound-on-destroy bug
- Making repeated SFX feel less robotic (pitch / clip randomization)
- Building a pause / stop / resume system
- Choosing PlayOneShot vs PlayClipAtPoint based on lifecycle
