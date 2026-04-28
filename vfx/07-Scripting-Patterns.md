# Scripting Patterns

Wiring VFX to gameplay. Six common patterns, when to use each, and the
gotchas. Code examples assume `using UnityEngine;` (and `using
UnityEngine.VFX;` for VFX Graph).

## Pattern 1: Instantiate Prefab + Stop Action: Destroy

The most common one-shot pattern.

```csharp
[SerializeField] private GameObject explosionPrefab;

void OnDeath()
{
    Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    Destroy(gameObject);
}
```

**Critical:** the prefab's Particle System must have `Stop Action = Destroy`
on its Main module so it self-destructs when finished. Without it, every
explosion leaves a dead GameObject in the hierarchy forever.

If the prefab has multiple particle systems, set Stop Action: Destroy on the
**root** (the longest-living one usually). See `ExplosionEffect.cs` for the
multi-system case.

### Orientation

For directional bursts (hits, sprays), use the surface normal:

```csharp
Instantiate(hitPrefab, hit.point, Quaternion.LookRotation(hit.normal));
```

The hemisphere shape will then fire away from the surface.

## Pattern 2: Pre-placed System with Play() / Stop()

For effects already in the scene that need on/off control.

```csharp
[SerializeField] private ParticleSystem fireEffect;

public void IgniteFire()
{
    fireEffect.Play();
}

public void Extinguish()
{
    fireEffect.Stop();   // stops emission, lets existing particles die naturally
}
```

| Method                          | Effect                                                       |
|---------------------------------|--------------------------------------------------------------|
| `Play()`                        | Start emission. If already playing, ignored. Use `Play(true)` to also restart children. |
| `Stop()`                        | Stop emission. Existing particles continue lifecycle.        |
| `Stop(true, StopBehavior.StopEmittingAndClear)` | Stop emission AND remove existing particles.    |
| `Pause()` / `Pause(true)`       | Pause time. Particles freeze.                                |
| `Clear()`                       | Remove all live particles immediately.                       |

This is more performant than Instantiate for repeating effects (a torch you
ignite/extinguish during gameplay).

## Pattern 3: Manual Burst with Emit()

When the system is configured for ongoing emission but you want extra
particles on demand.

```csharp
[SerializeField] private ParticleSystem dust;

public void OnLand()
{
    dust.Emit(20);
}
```

Or with full per-particle control:

```csharp
var emitParams = new ParticleSystem.EmitParams
{
    position = transform.position,
    velocity = Vector3.up * 5f,
    startColor = Color.red,
    startSize = 0.5f,
    startLifetime = 1f,
};
dust.Emit(emitParams, count: 10);
```

Skips Shape/Main module random ranges entirely — useful for precise placement.

## Pattern 4: Pooled Particle Systems

For high-frequency effects (gunshots, footstep dust, hit sparks). Allocating a
new GameObject per shot tanks performance.

```
shoot → pool.Get() → set position → Play() → release after lifetime
```

See `VFXPool.cs` for the full implementation. Mirrors the audio pool pattern
in `audio/AudioPool.cs`.

Use when:
- Effect fires more than ~5 times per second
- Mobile target where allocations cost frames
- Multi-system effects where Instantiate is expensive

## Pattern 5: VFX Graph SendEvent + Properties

For VFX Graph instances.

```csharp
using UnityEngine.VFX;

[SerializeField] private VisualEffect graph;

public void Explode(float radius)
{
    graph.SetFloat("ExplosionRadius", radius);
    graph.SetVector3("Origin", transform.position);
    graph.SendEvent("Explode");
}
```

The graph has an Event context named "Explode" connected to a burst Spawn
context. SendEvent triggers it.

```csharp
graph.Play();          // built-in OnPlay event
graph.Stop();          // built-in OnStop event
graph.Reinit();        // reset all particles + state
```

## Pattern 6: Toggle Emission Module

For "moving = trail / standing = no trail" cases without play/stop.

```csharp
[SerializeField] private ParticleSystem trail;

void Update()
{
    var em = trail.emission;
    em.enabled = playerVelocity.sqrMagnitude > 0.1f;
}
```

`emission` returns a struct, not a reference. Always assign back via the
local var when changing it. (Same pattern for any module: `var main =
ps.main;` etc.)

## Common Gotchas

### Modules are Structs, Not References

```csharp
// ❌ DOES NOTHING
ps.main.startSpeed = 5f;

// ✅
var main = ps.main;
main.startSpeed = 5f;     // assignment to local; written back automatically because it's a property setter
```

Some modules (`emission`, `shape`, `main`, `colorOverLifetime`...) are
"module structs" — accessing them returns a wrapper that writes through.
Just store them in a local var for readability.

### Random Curves and Min-Max Curves

```csharp
var main = ps.main;
main.startSpeed = new ParticleSystem.MinMaxCurve(5f, 15f);  // Random Between Two Constants
```

Use `MinMaxCurve` and `MinMaxGradient` for properties that support random
ranges.

### OnParticleCollision (Particle System Only)

When Collision module is enabled and `Send Collision Messages` is checked,
this fires per-particle on the OBJECT THAT WAS HIT (not the emitter):

```csharp
void OnParticleCollision(GameObject other)
{
    // 'other' is the emitter
    health -= 5;
}
```

VFX Graph has no equivalent C# callback — handle collisions inside the graph.

### Length of an Effect

```csharp
float effectDuration = ps.main.duration + ps.main.startLifetime.constantMax;
Destroy(gameObject, effectDuration);
```

If you can't rely on Stop Action: Destroy, compute the lifetime manually.

### Sub-Emitters Don't Auto-Play

If a child Particle System is a Sub Emitter (referenced in the parent's Sub
Emitters module), do NOT also set it to `Play On Awake` — it will double-fire.

## Pattern Selection Cheat-Sheet

```
Where is the effect?              Use:
──────────────────────────────    ──────────────────────────────
One-off, prefab, fire-and-forget  Instantiate + Stop Action: Destroy
Already in scene, toggle          Play() / Stop()
Already in scene, frequent extra  Emit(N)
Very frequent, perf matters       Pool (see VFXPool.cs)
VFX Graph asset                   SendEvent + SetFloat / SetVector3
Conditional ambient (moving etc)  emission.enabled flag
```

## When to Use This

- Wiring an explosion to a death event
- Fixing the "Particle System cuts off when GameObject dies" bug (see
  `audio/05-Scripting-Patterns.md` for the audio version of the same trap)
- Choosing between Instantiate, Play, and pooling
- Working with VFX Graph from script (events + properties)
- Reading or writing to module structs without confusion
