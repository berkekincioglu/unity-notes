# Performance and Genre Recipes

Two parts: the optimization checklist (apply per-effect and per-scene), and a
quick-reference table for genre-specific VFX stacks.

## Performance Checklist

Run through this before shipping. Each item is a real frame-eater.

```
□ Max Particles capped sensibly (1000 default, 10000+ only when verified)
□ Stop Action: Destroy on every instantiated prefab
□ Culling Mode: Pause when Offscreen for non-critical effects
□ Renderer Material uses Particles/Unlit unless lighting needed
□ No Lights Module unless absolutely required (Maximum Lights: 4)
□ Collision Module set to Planes (not World) where possible
□ Texture sizes: 256×256 / 512×512 — not 2048×2048 for a smoke puff
□ Mip maps disabled on particle textures (single distance use)
□ High-frequency effects pooled (VFXPool)
□ Soft Particles disabled unless needed (depth read costs)
□ GPU Instancing enabled on Mesh-mode particles
□ World-space simulation used only when necessary
□ Sub-Emitter chains aren't deeper than 2 levels
□ VFX Graph used for >10k simultaneous particles
□ Profiler: Particle System Update doesn't dominate CPU frame
□ Mobile: tested at target FPS with worst-case effect density
```

## Common Performance Mistakes

| Mistake                                           | Fix                                                      |
|---------------------------------------------------|----------------------------------------------------------|
| Rate over Time = 1000 for "more particles"        | Lower it. Few large particles look better than many tiny |
| Offscreen weather still simulating                | Culling Mode: Pause (or Pause and Catch-up)              |
| Forgot Stop Action on burst prefab                | Set to Destroy. Hierarchy fills with dead GOs            |
| Lights Module spawning 50 lights                  | Drop Maximum Lights and Ratio                            |
| Real-time particle shadows on every effect        | Cast Shadows: Off for VFX                                |
| 4K explosion texture                              | Compress and downscale; particles are small on screen    |
| New Instantiate every shot                        | Pool. Especially on mobile.                              |
| Mesh particles without GPU Instancing             | Enable GPU Instancing in Renderer module                 |

## LOD-Like Tricks

There's no formal LOD system for particles, but you can approximate:

- Multiple prefabs (`HitFx_High`, `HitFx_Med`, `HitFx_Low`) selected per
  platform / quality setting
- Disable secondary effects (sparks, debris) on low quality
- Reduce Rate over Time multiplier in `MainModule.startSpeedMultiplier`
- VFX Graph: use exposed quality-tier properties; flip them at startup

## Profiling

```
Window → Analysis → Profiler
   CPU Usage → look for "ParticleSystem.Update"
   GPU Usage → look for VFX render time
   Memory   → Particle System asset count / texture memory
```

Frame Debugger:
```
Window → Analysis → Frame Debugger
   Step through draw calls
   See how many "Render Particle System" calls per frame
   → consolidate effects with same material to save batches
```

## Genre Recipes (Quick Reference)

| Genre              | Environmental                  | Gameplay (events)                             | Notes                                |
|--------------------|--------------------------------|-----------------------------------------------|--------------------------------------|
| **FPS / Shooter**  | Dust motes, fog, weather       | Muzzle flash, hit sparks, blood, explosions   | Pool ALL combat VFX                  |
| **2D Platformer**  | Background snow, ambient mist  | Jump dust, pickup sparkle, death burst        | Watch Sorting Layer for layering     |
| **Racing**         | Speed lines, dirt particles    | Skid smoke, crash spark + fire                | Inherit Velocity from car            |
| **Stealth/Horror** | Volumetric fog, dust, candles  | Heartbeat overlay (UI VFX), discovery flash   | Subtle and slow > loud bursts        |
| **RTS / Top-down** | Map ambient (snow on biomes)   | Selection ring, explosion, building damage    | Distance LOD important               |
| **Cinematic**      | Per-shot atmospheric           | Triggered by Timeline Signal Emitters         | Timeline owns the timing             |
| **Magic / Fantasy**| Floating motes, fireflies      | Spell cast, projectile trail, impact          | Best fit for VFX Graph               |
| **Mobile / Casual**| Minimal — score frames first   | Tap feedback, success burst                   | Aggressively pool, low Max Particles |

## FPS Detail

```
Muzzle flash:    Pre-placed, Play() per shot, very short duration (0.05s)
Casing eject:    Mesh particle with gravity + Collision Planes
Hit on flesh:    Burst, bright red, Stretched, Stop Action: Destroy
Hit on metal:    Burst, yellow sparks, Sub Emitter for ricochet
Smoke trail:     World space, Rate over Distance, on bullet GO
Explosion:       Multi-system prefab (flash + ball + smoke + debris)
Atmosphere:      Sparse fog particles + dust motes per room
```

## 2D Platformer Detail

```
Jump:      Burst on takeoff (5 dust particles, Rectangle shape, ground level)
Land:      Burst on landing (10 particles, faster outward)
Dash:      World-space short-lived trail with stretched render
Pickup:    Sprite-shaped burst, Texture Sheet sparkle
Hit:       Hit-stop + burst, screen shake from elsewhere (cinemachine impulse)
```

## Racing Detail

```
Engine:   Continuous emission off the exhaust (Rate over Distance)
Skid:     Activated when handbrake; world-space tire marks (decals or trails)
Speed:    Stretched billboard particles streaming past camera at high speed
Crash:    Big multi-system burst + post-effect screen shake
```

## Stealth / Horror Detail

```
Ambient candles:  Tiny flame + glow + Light module (1 light max)
Footprint dust:   Rate over Distance from player, very subtle
Detection flash:  UI VFX (VFX Graph in screen space) for "spotted!"
Vignette pulse:   Renderer-side, not particles, but pairs with VFX
```

## VFX-Graph-First Genres

Pick VFX Graph when:
- Magic/fantasy with dense particle clouds (Genshin, Hades vibe)
- Cinematic, slow-mo set pieces
- Boss arenas with persistent dense effects
- Stylised particle systems where you'd write Shader Graph anyway

Otherwise, Particle System is faster to build and more compatible.

## When to Use This File

- Running a perf pass before shipping
- Diagnosing why a scene drops to 30 FPS during combat
- Building VFX for a new genre — find the row, follow the recipe
- Deciding when to invest in VFX Graph migration
