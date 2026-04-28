# Unity VFX — Overview

This folder contains reference notes for Unity's two visual-effects systems
(Unity 6.x): **Particle System** (component-based, CPU) and **VFX Graph**
(node-based, GPU). Start here, then follow the numbered files. Code examples
live in the `.cs` files alongside these notes.

## Two Systems, Two Purposes

```
┌─────────────────────────────┐    ┌─────────────────────────────┐
│   PARTICLE SYSTEM           │    │   VFX GRAPH                 │
│   (component, CPU)          │    │   (asset, GPU)              │
├─────────────────────────────┤    ├─────────────────────────────┤
│ Inspector-driven            │    │ Visual node graph           │
│ Thousands of particles      │    │ Millions of particles       │
│ Physics interaction: YES    │    │ Physics interaction: NO     │
│ Beginner-friendly           │    │ Steeper learning curve      │
│ Works on all hardware       │    │ Modern GPU required         │
└─────────────────────────────┘    └─────────────────────────────┘
```

Performance reference (Unity Learn): 1,000,000 particles ran at ~5 FPS in a
Particle System and at ~100 FPS in VFX Graph.

## Decision Table

| Need                               | Pick                |
|------------------------------------|---------------------|
| Beginner project                   | Particle System     |
| Particles must collide / bounce    | Particle System     |
| Mobile / low-end target            | Particle System     |
| Simple to medium effects           | Particle System     |
| Cinematic, stylised, dense effects | VFX Graph           |
| Millions of particles              | VFX Graph           |
| GPU-bound performance critical     | VFX Graph           |

## Categories of VFX

| Category       | Definition                                     | Examples                              |
|----------------|------------------------------------------------|---------------------------------------|
| Environmental  | Continuous, atmospheric                        | Rain, snow, fog, fire, waterfall mist |
| Gameplay       | Triggered by player action / game event        | Explosion, pickup sparkle, hit feedback |

Both systems handle both categories. The split matters for *integration*:
environmental effects sit in the scene; gameplay effects spawn on demand.

## Reading Order

1. [01-Particle-System-Basics.md](01-Particle-System-Basics.md) — Main module + the 3 default modules
2. [02-Modules-Deep-Dive.md](02-Modules-Deep-Dive.md) — Color/Size over Lifetime, Noise, Sub Emitters, Trails, Lights, Texture Sheet
3. [03-Environmental-Effects.md](03-Environmental-Effects.md) — Rain, snow, fog, dust recipes
4. [04-Burst-Effects.md](04-Burst-Effects.md) — Explosions, pickups, hit effects
5. [05-VFX-Graph.md](05-VFX-Graph.md) — Node graph, 4 contexts, properties, events
6. [06-2D-VFX.md](06-2D-VFX.md) — 2D-specific settings (Gravity Source, sorting, sprite shape)
7. [07-Scripting-Patterns.md](07-Scripting-Patterns.md) — Instantiate / Play / Emit / SendEvent
8. [08-Performance-and-Recipes.md](08-Performance-and-Recipes.md) — Optimization checklist + genre recipes

## Code Examples

| File                        | Demonstrates                                             |
|-----------------------------|----------------------------------------------------------|
| `VFXManager.cs`             | Singleton with typed PlayEffect API + pool routing       |
| `VFXPool.cs`                | Pooled ParticleSystem prefabs for high-frequency effects |
| `ExplosionEffect.cs`        | Multi-system Instantiate + Destroy pattern               |
| `HitEffectSpawner.cs`       | Surface-aware effect picking (parallel to FootstepPlayer) |
| `VFXGraphController.cs`     | Driving a VisualEffect with SendEvent + properties       |

## When to Use This Folder

- Choosing Particle System vs VFX Graph for a new effect
- Building rain / snow / explosion from scratch with a known recipe
- Wiring an effect to fire from gameplay code (Instantiate, Play, Emit)
- Diagnosing FPS drops caused by particles
- Picking the right Stop Action and pooling strategy
