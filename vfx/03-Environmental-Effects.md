# Environmental Effects

Continuous, atmospheric effects that live in the scene and don't react to a
single event — weather, ambient dust, fog, fire pits. The patterns here all
loop and use Prewarm so they're "already running" when the scene loads.

## Snow

```
Main Module:
  Looping:          ON
  Prewarm:          ON
  Start Speed:      1
  Start Lifetime:   8 (long enough to fall to ground)
  Start Size:       0.1
  Start Color:      White (alpha 0.7)
  Gravity Modifier: 0  (Start Speed handles fall)
  Max Particles:    10000
  Simulation Space: World

Shape Module:
  Shape:    Box
  Emit From: Volume
  Scale:    X=10, Y=1, Z=10  (wide thin sheet above the player)
  Position: ~10 units above scene

Emission Module:
  Rate over Time: 150

Renderer Module:
  Material: SnowMaterial (or Particles/Unlit + a soft white texture)

Noise Module (optional but better):
  Strength:  0.2
  Frequency: 0.3
  → drifty, non-vertical fall
```

Attach the Particle System to the player so it follows along. With
`Simulation Space = World`, particles stay where they were emitted, so you
don't see snow visibly chasing the player.

## Rain

```
Main Module:
  Same as snow except:
  Start Speed:    10
  Start Lifetime: 1.5
  Start Size:     0.05

Renderer Module:
  Render Mode:    Stretched Billboard
  Length Scale:   0.1  (long thin streaks instead of dots)
  Material:       RaindropMaterial

Emission Module:
  Rate over Time: 800–1500
```

**Stretched Billboard** is the rain trick. It elongates each particle along its
velocity vector, producing the streak look without animating textures.

### Splash on Ground (optional, advanced)

Add a Sub Emitter on Collision to spawn a small "splash" Particle System where
each raindrop hits.

## Fog / Volumetric Mist

Real volumetric fog is a rendering feature (Window → Rendering → Lighting →
Environment). For *visible drifting fog*, particles work:

```
Main Module:
  Looping:          ON
  Start Lifetime:   10
  Start Speed:      0.3
  Start Size:       Random Between 3 and 6  (large blobs)
  Start Color:      White, alpha 0.05  (very faint)
  Max Particles:    100  (big sparse particles, not many)
  Simulation Space: World

Shape Module:
  Shape:    Box, large
  Emit From: Volume

Velocity over Lifetime:
  Linear X / Z: small drift

Color over Lifetime:
  Fade in at 0–10%, fade out at 80–100%

Renderer Module:
  Material: soft blob / cloud texture
  Render Mode: Billboard
```

Few large semi-transparent particles drifting slowly = passable fog. Combine
with the rendering Volumetric Fog feature for real depth.

## Dust Motes (Sunlight Beams)

Tiny floating specks visible in light shafts.

```
Main Module:
  Start Size:     Random 0.005–0.02
  Start Speed:    0.05
  Start Lifetime: 5
  Max Particles:  200
  Simulation Space: World

Shape:
  Box, fits the room

Noise:
  Strength: 0.1
  Frequency: 0.1
  → particles meander, not still
```

Pair with a soft white particle texture and Additive blend for the "glittering
in sunlight" feel.

## Waterfall Mist / Spray

Continuous spray at the bottom of a waterfall.

```
Main Module:
  Start Speed:    Random 2–4
  Start Lifetime: Random 1.5–3
  Start Size:     Random 0.5–1.5
  Looping:        ON

Shape:
  Cone, Angle: 25, Radius: 1

Color over Lifetime:
  Alpha 0 → 0.6 → 0   (fade in and out, never fully solid)

Velocity over Lifetime:
  Linear Y: 1 (floats up slightly)
  Linear (random per axis) for drift

Renderer:
  Material: soft cloud / smoke
```

Pair with a 3D AudioSource for the waterfall sound (see `audio/`).

## Fire / Campfire

Multiple Particle Systems combined under a parent.

```
Campfire (parent GameObject)
├── Flames    (Particle System)
│   - Cone shape, narrow angle
│   - Start Speed 1, Lifetime 1
│   - Color: yellow → orange → red → transparent
│   - Size over Lifetime: 1 → 0
│   - Noise on
│   - Additive blend
├── Smoke     (Particle System)
│   - Cone shape, wider angle
│   - Start Speed 0.5
│   - Lifetime 4
│   - Color: dark gray → transparent
│   - Size over Lifetime: small → big
│   - Noise on
└── Sparks    (Particle System)
    - Burst occasionally (every 0.5s, 3 particles)
    - Random direction
    - Stretched Billboard
    - Bright orange
```

Add a Point Light child that flickers (Light Probes or a script driving
intensity with `Mathf.PerlinNoise`).

## Sand Wind / Heat Haze

For deserts or hot zones — particles drifting with strong horizontal force,
distortion shader optional.

```
Main:
  Start Speed:    Random 1–3
  Start Lifetime: Random 2–4
  Simulation Space: World

Shape: Box, oriented across the scene

Force over Lifetime:
  X (or wind direction): 2–5

Renderer: stretched billboard, faint warm color
```

## Common Environmental Pitfalls

- Forgetting `Prewarm` — first 5 seconds look empty after scene load
- Using Local space — weather chases the player visibly
- Max Particles too low — Rate over Time keeps trying but particles drop
- Rendering thousands of particles off-screen — set Culling Mode to `Pause`
- Not lowering `Rate over Time` for low-end targets — mobile drops frames

## When to Use This

- Adding weather to a level (rain, snow)
- Making a room feel atmospheric (fog, dust motes)
- Hand-rolling a campfire / torch fire
- Creating a misty waterfall
- Enriching a desert / wasteland (sand wind)
