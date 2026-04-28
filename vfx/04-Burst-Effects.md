# Burst Effects

One-shot effects that spawn many particles instantly: explosions, hit
feedback, pickup sparkles, ability cast flashes. Triggered from gameplay code
or by simply instantiating the prefab.

## The Burst Module

```
Emission Module:
  Rate over Time: 0          ← no continuous emission
  Bursts:
   ┌──────┬───────┬────────┬──────────┬─────────┐
   │ Time │ Count │ Cycles │ Interval │ Probabil│
   ├──────┼───────┼────────┼──────────┼─────────┤
   │ 0.0  │  50   │   1    │   0      │   100%  │   single burst
   │ 0.0  │  10   │   3    │   0.2    │   100%  │   3-pulse burst
   │ 0.0  │  20   │   1    │   0      │    50%  │   sometimes
   └──────┴───────┴────────┴──────────┴─────────┘
```

| Field        | Meaning                                                          |
|--------------|------------------------------------------------------------------|
| **Time**     | When (in seconds) within the system's Duration the burst fires   |
| **Count**    | How many particles to spawn (constant or random range)           |
| **Cycles**   | How many times this burst repeats                                |
| **Interval** | Time between cycles                                              |
| **Probability** | Chance each cycle actually fires                              |

## Generic Explosion Recipe

```
Main Module:
  Looping:        OFF
  Duration:       2 (long enough for tail particles to die)
  Start Speed:    Random Between 5 and 15
  Start Lifetime: Random Between 0.5 and 1.5
  Start Size:     Random Between 0.3 and 0.8
  Start Color:    Bright orange → handled in Color over Lifetime
  Stop Action:    Destroy   ← cleans itself up after instantiating

Shape Module:
  Shape:          Sphere
  Radius:         0.1
  Emit From:      Shell (or Volume)

Emission Module:
  Rate over Time: 0
  Bursts:         Time 0, Count 50, Cycles 1

Color over Lifetime:
  yellow → orange → red → black-transparent

Size over Lifetime:
  curve: 0 → 1 → 1.5 → 0  (grow then shrink)

Renderer:
  Material: smoke / fire texture, Additive blend
```

**Stop Action: Destroy** is mandatory for one-shot effects. Without it,
instantiated GameObjects pile up in the hierarchy.

## Multi-System Explosion (More Realistic)

A single Particle System can't sell a real explosion. Stack three under one
parent:

```
ExplosionPrefab
├── Flash      (Particle System: 1 particle, huge, 0.1s, bright white)
├── Fireball   (Particle System: 30 burst, orange, 0.5s, grow-shrink)
├── Smoke      (Particle System: 20 burst, gray, 3s, slow rise + Noise)
├── Debris     (Particle System: 15 burst, mesh chunks, gravity, Collision)
└── Sparks     (Particle System: 50 burst, stretched billboard, fast, fade)
```

Each child is its own ParticleSystem. Parent has `ExplosionEffect.cs` which
sets Stop Action: Destroy on the longest-lived child so the whole prefab
self-cleans when smoke finishes. See `ExplosionEffect.cs`.

## Hit Effect (Bullet Impact)

```
Main:
  Looping:        OFF
  Duration:       0.5
  Start Speed:    Random 2–6
  Start Lifetime: Random 0.2–0.5
  Start Size:     Random 0.05–0.15
  Stop Action:    Destroy

Shape:
  Hemisphere       ← burst away from the surface
  Radius:          0.05

Emission:
  Burst at 0:      Count 12, Cycles 1

Color over Lifetime:
  bright → faded

Renderer:
  Stretched billboard for spark feel
```

Spawn at the contact point with `Quaternion.LookRotation(hit.normal)` so the
hemisphere fires *away from* the surface, not into it.

## Pickup Sparkle / Coin Collect

Soft, friendly burst with shimmer.

```
Main:
  Duration:       1
  Start Speed:    Random 1–3
  Start Lifetime: 0.5
  Start Size:     Random 0.05–0.1

Shape:
  Sphere

Emission:
  Burst: Count 20, Cycles 1

Color over Lifetime:
  sparkle gradient (gold)
  alpha fade out

Texture Sheet Animation:
  4×4 sparkle sheet, Random Start Frame
```

## Random Between Two Constants — The Variation Trick

Almost every property has a ▾ menu. Set Start Speed to "Random Between Two
Constants" with values 5 and 15: every particle gets a different speed in that
range. Without this, a 50-particle burst feels uniform and cheap.

```
Constant                      ← single value (boring)
Curve                         ← changes during system runtime
Random Between Two Constants  ← per-particle randomization ✓ (most useful)
Random Between Two Curves     ← curves randomized per particle
```

Apply to: Start Speed, Start Lifetime, Start Size, Start Rotation, Start
Color, and module-level properties like Velocity over Lifetime.

## Triggering Bursts from Code

```csharp
// Already-spawned ParticleSystem in scene
particleSystem.Play();          // restart and fire its bursts

// Manual extra burst on top of configured emission
particleSystem.Emit(50);        // spawn 50 right now

// Spawn a fresh prefab at a position
Instantiate(explosionPrefab, hitPoint, Quaternion.LookRotation(hit.normal));
```

See [07-Scripting-Patterns.md](07-Scripting-Patterns.md).

## When to Use This

- Explosion, gun-fire muzzle flash, impact debris
- Pickup feedback, ability cast, level-up flash
- Building multi-layered effects (flash + fireball + smoke + debris)
- Adding per-particle variety to a flat burst (Random Between Two Constants)
- Wiring Stop Action: Destroy on every prefab burst effect
