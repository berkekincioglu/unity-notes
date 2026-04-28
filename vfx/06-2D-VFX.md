# VFX in 2D Projects

Both Particle System and VFX Graph work in 2D. The same modules, the same
properties — with a few project-level settings to tune for 2D rendering. This
is more about *configuration* than *different tooling*.

## Main Module — Gravity Source

```
Gravity Source:
  3D Physics  ← default; uses Project Settings → Physics gravity
  2D Physics  ← uses Project Settings → Physics 2D gravity
```

Set this to **2D Physics** in 2D projects. Otherwise particle gravity won't
match what your Rigidbody2D objects experience.

## Shape Module — 2D-Friendly Shapes

3D shapes (Sphere, Cone, Box) still work but emit into 3D space — particles
end up at varied Z positions, which then sort weirdly against sprites.

Use 2D-flat shapes:

| Shape       | Description                  | Use                                  |
|-------------|------------------------------|--------------------------------------|
| Circle      | Disc on the XY plane         | Burst around a point                  |
| Rectangle   | Rectangle on XY plane         | Wide screen / area emit              |
| Edge        | Line segment                  | Ground particles, treadmark           |
| Sprite      | Emit from a sprite shape      | 2D character ability                  |
| Sprite Renderer | Emit from existing sprite | 2D pickup, hit on a sprite            |

If you must use a 3D shape (e.g., Sphere for radial burst), set the system's
transform `Z = 0` and limit emission to `Emit From: Edge` or `Shell` so
particles stay near the plane.

## Renderer Module — Sorting in 2D

2D rendering uses Sorting Layer + Order in Layer instead of Z-distance.

| Property                   | Use                                     |
|----------------------------|-----------------------------------------|
| **Sorting Layer**          | Pick a layer (`Background`, `Effects`, `UI`...) |
| **Order in Layer**         | Higher = drawn on top                    |
| **Sorting Fudge**          | Tiebreaker offset within the same layer  |
| **Sort Mode (within particle)**| Distance / Oldest / Youngest         |

**Common bug:** particles render *behind* the player sprite. Fix:

```
Renderer:
  Sorting Layer:   Effects
  Order in Layer:  10  (greater than the Player's Order in Layer)
```

## Material / Shader

2D effects typically use:
- `Sprites/Default` (basic 2D-friendly)
- `Particles/Unlit` with a transparent texture
- URP-specific particle shaders if you're on URP 2D

For pixel-art games, set the texture's import settings to:
- Filter Mode: **Point (no filter)** — keeps pixel-art crispness
- Compression: **None** — preserves texture sharpness

## Spatial Audio Reminder

A 2D *visual* particle effect can still emit a 3D *sound*. The two systems are
independent. Place an AudioSource child with `Spatial Blend = 1` and the
explosion will sound directional even in a 2D side-scroller. See
`audio/02-Spatial-Audio.md`.

## VFX Graph in 2D

Same package, same workflow. The only nuance: choose `Output Particle Unlit
Quad` and orient outputs so they face the camera (Set Orient → Face Camera
Plane). Sorting in URP 2D Renderer is handled by Renderer Layer (URP setting).

## Recipes

### 2D Hit Spark

```
Main:
  Looping: OFF
  Duration: 0.5
  Start Speed: Random 2–5
  Start Lifetime: Random 0.2–0.5
  Stop Action: Destroy
  Gravity Source: 2D Physics
  Gravity Modifier: 1

Shape:
  Circle, Radius: 0.05

Emission:
  Burst at 0: Count 8

Renderer:
  Sorting Layer: Effects
  Order in Layer: 10
  Material: 2D spark texture
```

### 2D Pickup Sparkle

```
Main:
  Duration: 1
  Start Speed: Random 1–2
  Start Lifetime: 0.5
  Stop Action: Destroy

Shape:
  Sprite (use the pickup's sprite as the emit shape)

Emission:
  Burst at 0: Count 15

Texture Sheet Animation:
  4×4 sparkle, Random Start Frame
```

### 2D Trail (Footprint / Skid)

```
Main:
  Looping: OFF (triggered by movement)
  Start Lifetime: 1
  Start Size: Random 0.05–0.1
  Gravity Modifier: 0
  Simulation Space: World

Emission:
  Rate over Distance: 5  (one particle every 0.2 units of movement)

Shape: Edge (or Circle, small)

Color over Lifetime: fade out alpha
```

Attach to the player. As they move, the trail spawns world-space marks that
stay behind.

## When to Use This

- Configuring a 2D project's particles to feel native (Gravity Source, sorting)
- Fixing "particles drawn behind sprite" bugs (Sorting Layer / Order in Layer)
- Picking pixel-art-friendly material settings
- Adding 3D audio to 2D effects (don't conflate visuals and audio dimensions)
