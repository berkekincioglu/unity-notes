# Particle System — Modules Deep Dive

The non-default modules. Most effects need 2–4 of these enabled. Toggle each
module's checkbox in the Inspector to activate.

## Color over Lifetime

Particles change color/alpha as they age. Drives the "fade out" most VFX needs.

```
Lifetime %:    0%      25%     50%     75%     100%
              ●yellow ●orange ●red    ●gray   ○transparent
                      (alpha 1.0)            (alpha 0.0)
```

**Gradient editor:**
- Top row of pins → **Alpha keys** (transparency)
- Bottom row of pins → **Color keys** (RGB)

**Common pattern:** start opaque/colored, end with `alpha = 0` so particles
fade out smoothly instead of vanishing abruptly.

## Color by Speed

Color depends on particle speed, not age. Useful for impact debris (fast =
hot, slow = cold) or parallax-like layers. Less common than Color over Lifetime.

## Size over Lifetime

```
Size
 1 │      ╱╲
   │    ╱    ╲
   │  ╱        ╲___
 0 │╱             ╲___
   └──────────────────▶  Lifetime %
   0%                100%
```

Curve editor. Typical shape: small → grow → shrink to 0. Combined with alpha
fade, particles visually "puff out and dissipate" — basic smoke / dust pattern.

## Size by Speed

Size depends on speed, not age. Sparks from a grinder — faster particles
appear bigger, looking hotter.

## Velocity over Lifetime

Add velocity in X/Y/Z (Local or World). Adds drift, gravity-like pull, or
upward float (smoke).

| Use case        | Settings                                                    |
|-----------------|-------------------------------------------------------------|
| Smoke rising    | Linear Y: 1, Local                                          |
| Wind drift      | Linear X: 0.5 (random between two constants for variety)    |
| Upward gust     | Orbital Y or Linear Y over time curve                       |

## Limit Velocity over Lifetime

Caps particle speed. Useful when initial Start Speed is high but you want
particles to slow down (drag-like). Set Speed to a low value and Dampen to 1
for a "fluid resistance" feel.

## Inherit Velocity

When the emitter moves, do particles inherit some of that velocity?

```
Inherit Velocity = 0    →  particles start at Start Speed only (sparks shoot in cone)
Inherit Velocity = 1    →  particles get full emitter velocity (smoke trails behind a car)
```

Useful for trails on moving objects.

## Force over Lifetime

A constant force in X/Y/Z. Wind, gravity-direction-other-than-down, magnetic
pull. Differs from Velocity over Lifetime: this is acceleration.

## Rotation over Lifetime / Rotation by Speed

Spin particles. Smoke benefits from slow random rotation; gears or debris from
spin tied to speed.

## External Forces

Listens to global Wind Zones in the scene (Component → Wind Zone). Rare for
gameplay; common in cinematics.

## Noise Module

Adds smooth, organic variation to particle motion. Single most useful "make
this look natural" tool.

```
Noise OFF                       Noise ON
   │                              │
   │ smoke                        │  smoke
   │  ↑                           │   ╲╲╲
   │  ↑                           │   ↗↖↗
   │  ↑                           │  ↑↗↖
   │                              │
```

| Property      | What it does                                 |
|---------------|----------------------------------------------|
| Strength      | How far particles wander from their path     |
| Frequency     | How fast the noise pattern changes           |
| Scroll Speed  | Pattern movement over time                   |
| Damping       | Smooths abrupt direction changes             |
| Octaves       | Layers of noise — more detail, more cost     |
| Quality       | Low (cheap) – High (smooth)                  |
| Position / Rotation / Size Amount | Per-axis noise application |

Recipe: fire flames → Strength 0.3, Frequency 0.5, Scroll Speed 1.

## Collision Module

CPU-only feature. Particles bounce off scene colliders. World-space sim
required.

| Setting        | Meaning                                          |
|----------------|--------------------------------------------------|
| Type           | Planes (cheap) / World (full physics)            |
| Mode           | 3D / 2D                                          |
| Dampen         | Velocity multiplier on bounce                    |
| Bounce         | Bounce factor (1 = perfect)                      |
| Lifetime Loss  | % lifetime consumed per collision                |
| Min Kill Speed | Particles slower than this die                   |
| Send Collision Messages | Triggers `OnParticleCollision()` callback |

Performance: World collision is expensive. For dense effects, prefer Planes
mode with manually placed planes (a single ground plane often suffices).

## Triggers Module

Inverse of Collision: detect when particles enter/exit/pass through a trigger
collider. Calls `OnParticleTrigger()`. Useful for lava-tile mechanics or
spell-zone effects.

## Sub Emitters

Particles can spawn other Particle Systems on key events.

```
Trigger condition:    Sub-emitter spawns a:
  Birth         →  trail particle (smoke from a flying spark)
  Collision     →  burst (impact dust when raindrop hits ground)
  Death         →  small explosion (a sub-explosion at the end of a fuse)
  Trigger       →  custom (when entering trigger volume)
  Manual        →  triggered by ParticleSystem.TriggerSubEmitter() in code
```

Use case: a rocket. Sub-emitter on Birth = smoke trail. Sub-emitter on Death =
explosion when it detonates.

## Texture Sheet Animation

Animate particles by stepping through a sprite sheet (e.g., 4×4 fire frames).

| Property              | Meaning                                       |
|-----------------------|-----------------------------------------------|
| Mode                  | Grid (sheet) / Sprites (list of clips)        |
| Tiles X, Y            | Grid dimensions                               |
| Animation             | Whole Sheet / Single Row                      |
| Frame Over Time       | Curve: which frame at which lifetime %        |
| Random Start Frame    | Each particle picks a different first frame   |
| Cycles                | Animation loops per particle lifetime         |

A 4×4 fire texture sheet with Random Start Frame = ON makes every flame look
different. Cheap variety.

## Lights Module

Each particle can spawn a real-time Light. Glowing fire that lights the floor.

| Property        | Meaning                                          |
|-----------------|--------------------------------------------------|
| Light Prefab    | A prefab containing a Light component            |
| Ratio           | Fraction of particles that get lights (0–1)      |
| Random Distribution | Random selection vs evenly spaced            |
| Use Particle Color | Match light color to particle                  |
| Size / Range / Intensity / Lifetime Multipliers | Scale Light props |
| Maximum Lights  | Hard cap (very important — see below)            |

**Performance:** Forward rendering culls extra lights painfully. Set Maximum
Lights to 4–8 for forward, more is fine on Deferred / URP Forward+.

## Trails Module

A streak follows each particle.

```
Particle:           ●          (head)
                  ─╲
                    ─╲          (trail width tapers)
                      ─╲
                        ─╲╳     (tail end)
```

| Mode             | Meaning                                  |
|------------------|------------------------------------------|
| Particles        | Trail per particle                       |
| Ribbon           | One trail across all particles (chain)   |
| Particles & Ribbon | Both                                   |

Ratio = fraction of particles that get a trail (0.5 = half).

**Don't forget:** Renderer module → Trail Material slot must be assigned, or
trails render hot pink.

## Custom Data

Pass per-particle vector/color data into custom shaders (Shader Graph).
Advanced; ignore until you start writing custom particle shaders.

## When to Use This

- Adding fade-out (Color over Lifetime + alpha to 0)
- Smoke that grows then dissipates (Size over Lifetime curve)
- Fire that wavers naturally (Noise)
- Rocket trails / bouncing physics particles (Sub Emitters / Collision)
- Animated fire-frame flames (Texture Sheet Animation)
- Glowing torch that lights surroundings (Lights — sparingly)
