# Particle System — Basics

A Particle System is a single `MonoBehaviour` that spawns, moves, and renders
many small visual elements (particles). Add one with `GameObject → Effects →
Particle System`. The Inspector shows ~22 modules; only three are enabled by
default. This file covers the Main module + the three default modules. Other
modules live in [02-Modules-Deep-Dive.md](02-Modules-Deep-Dive.md).

## Default Module State

```
☑ Main Module          ← always present (top, no checkbox header)
☑ Emission             ← how many particles spawn
☑ Shape                ← where they spawn
☑ Renderer             ← what they look like
☐ all other 18 modules ← off by default
```

## Main Module

| Property              | What it does                                         | Typical values                         |
|-----------------------|------------------------------------------------------|----------------------------------------|
| **Duration**          | System runtime in seconds (loops if Looping)         | 5s                                      |
| **Looping**           | Restart at end                                       | ON (ambient) / OFF (one-shot)          |
| **Prewarm**           | Pretend system already ran when scene starts         | ON for rain / fog                      |
| **Start Delay**       | Wait before first emission                           | 0                                       |
| **Start Lifetime**    | How long each particle lives                         | 1–5s                                    |
| **Start Speed**       | Initial speed in shape's emit direction              | 1 (snow) – 50 (bullet trail)            |
| **3D Start Size**     | Per-axis size if needed                              | OFF                                     |
| **Start Size**        | Initial size                                         | 0.1–2                                   |
| **3D Start Rotation** | Per-axis rotation                                    | OFF                                     |
| **Start Rotation**    | Initial rotation in degrees                          | Random Between Two Constants            |
| **Flip Rotation**     | Half particles rotate opposite                       | OFF (effects), ~0.5 (smoke variation)   |
| **Start Color**       | Initial RGBA                                         | White                                   |
| **Gravity Modifier**  | Scales project gravity for these particles           | 0 (space), 1 (real)                     |
| **Simulation Space**  | **Local / World / Custom** (see below)               | depends — read carefully                |
| **Simulation Speed**  | Time multiplier (slow-mo or fast)                    | 1                                       |
| **Delta Time**        | Scaled / Unscaled (Pause menu UI usually Unscaled)   | Scaled                                  |
| **Scaling Mode**      | Local / Hierarchy / Shape — how transform scale applies | Local (default)                       |
| **Play On Awake**     | Start at scene load                                  | ON for ambient, OFF for prefab effects  |
| **Emitter Velocity**  | What "moving emitter" means for child sims           | Rigidbody / Transform                   |
| **Max Particles**     | Hard cap — extra spawns are dropped                  | 1000 default, 10000 for weather         |
| **Auto Random Seed**  | Different seed per Play                              | ON (variation each fire)                |
| **Stop Action**       | What happens when system stops (None / Disable / Destroy / Callback) | Destroy for instantiated prefabs |
| **Culling Mode**      | How off-screen sims behave (Pause / Always Simulate) | Pause (perf)                            |
| **Ring Buffer Mode**  | Reuse oldest particle instead of waiting             | OFF                                     |

## Simulation Space — Critical

```
LOCAL SPACE                          WORLD SPACE
┌──────────────────┐                 ┌──────────────────┐
│  emitter moves   │                 │  emitter moves   │
│      ●━━━━>      │                 │      ●━━━━>      │
│      ┃           │                 │  ●●●●            │
│      ┃ smoke     │                 │  smoke trails    │
│      ┃ follows   │                 │  stays behind    │
│      ┃ emitter   │                 │  in world        │
│      ↓           │                 │                  │
└──────────────────┘                 └──────────────────┘
torch carried by character           rocket leaving a trail
```

| Pick     | Use for                                                  |
|----------|----------------------------------------------------------|
| Local    | Effect attached to a moving GO (torch, hand magic, aura) |
| World    | Trails, dust kicked up, emitter that may be destroyed    |
| Custom   | Reference an arbitrary Transform — niche                 |

**Most common bug:** rocket exhaust set to Local — looks like it's glued to the
nose of the rocket instead of trailing behind. Switch to World.

## Emission Module

| Property             | What it does                                         |
|----------------------|------------------------------------------------------|
| **Rate over Time**   | Particles per second (continuous)                    |
| **Rate over Distance**| Particles per metre travelled (footprints, trails)  |
| **Bursts**           | Spawn N particles at specific times — see [04-Burst-Effects.md](04-Burst-Effects.md) |

Two emission strategies coexist. A campfire might use Rate over Time = 50 for
flames AND a burst every 0.5s for crackling sparks.

## Shape Module — Where Particles Are Born

| Shape          | Description                              | Common use                       |
|----------------|------------------------------------------|----------------------------------|
| **Sphere**     | Full sphere volume / shell               | Omnidirectional explosion        |
| **Hemisphere** | Half sphere                              | Ground burst, dust kick          |
| **Cone**       | Cone in `+Z`                             | Fire (default), spray, fountain  |
| **Donut**      | Torus                                    | Ring of magic                    |
| **Box**        | Axis-aligned box volume                  | Rain (wide flat), area effect    |
| **Mesh**       | Sample any 3D mesh's surface             | Damage on enemy mesh             |
| **Mesh Renderer** | Mesh + the renderer's material         | Rich mesh-based emit             |
| **Skinned Mesh Renderer** | Animated mesh                  | Blood from a moving character    |
| **Sprite**     | 2D sprite shape                          | 2D pickup glow                   |
| **Sprite Renderer** | Existing sprite renderer            | 2D character VFX                 |
| **Circle**     | 2D circle (flat)                         | 2D burst on a plane              |
| **Edge**       | A line segment                           | 2D ground particles              |
| **Rectangle**  | 2D rectangle                             | 2D screen / area emit            |

| Property        | Meaning                                              |
|-----------------|------------------------------------------------------|
| Emit From       | Volume / Shell / Edge / Vertex / Triangle (per-shape)|
| Radius / Angle  | Shape size controls                                  |
| Position / Rotation / Scale | Local transform of the shape             |
| Align To Direction | Particles rotate to match emit direction          |
| Randomize Direction | Adds randomness to emit direction                |

## Renderer Module — Drawing the Particles

| Property            | What it does                                          |
|---------------------|-------------------------------------------------------|
| **Render Mode**     | Billboard / Stretched / Mesh / None                   |
| **Material**        | Shader + texture used                                 |
| **Trail Material**  | Material used by the Trails module if enabled        |
| **Sort Mode**       | Draw order: Distance, Oldest, Youngest                |
| **Sorting Fudge**   | Manual offset for sort ties (2D layering)             |
| **Min/Max Particle Size** | Clamp on screen                                 |
| **Render Alignment**| Billboard alignment (View, World, Velocity, Local)    |
| **Cast / Receive Shadows** | Shadow flags                                  |
| **Use GPU Instancing** | Performance boost for Mesh render mode             |
| **Sorting Layer / Order in Layer** | 2D draw order                          |

### Render Modes

```
Billboard  — flat quad always facing the camera (most particles)
Stretched  — quad stretched along velocity (rain, motion blur)
Mesh       — render a 3D mesh per particle (debris chunks, leaves)
None       — invisible — useful for sub-emitter triggers
```

### Common Particle Shaders

| Shader                     | When to use                              |
|----------------------------|------------------------------------------|
| Particles/Unlit            | Fast, ignore lighting (smoke, weather)   |
| Particles/Standard Unlit   | Color/transparency aware, still cheap    |
| Particles/Standard Surface | Lighting-aware (fire glow on terrain)    |
| Custom (URP/HDRP)          | Use Shader Graph for stylised effects    |

## Scene View Controls

When a Particle System is selected, an overlay appears: Play / Pause / Stop /
Restart, plus Playback Speed and Time. Useful for previewing without entering
Play mode.

## When to Use This

- Adding the first Particle System to a scene
- Picking Local vs World space (most common confusion)
- Choosing a shape that matches the effect (Cone for fire, Box for weather)
- Tweaking Max Particles to avoid runaway counts
- Setting Stop Action: Destroy on prefab effects so they self-clean
