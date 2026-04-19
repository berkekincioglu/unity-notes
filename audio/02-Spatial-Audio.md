# Spatial (3D) Audio

Spatial audio means sound behaves like it does in real life: quieter with
distance, coming from a direction, fading as you turn away. This works the same
whether your game is visually 2D or 3D — **3D audio describes how sound behaves
in space, not what your game looks like**.

## Spatial Blend

The single most important property on an AudioSource.

```
Spatial Blend = 0.0  ──────────────────────  Spatial Blend = 1.0
     "2D"                                          "3D"
Constant volume everywhere              Distance + direction apply
Music, UI, menus                        Footsteps, gunshots, ambient
```

You can pick any value in between. `0.7` is a soft 3D — subtle directionality
with little distance loss. Useful for important tactical sounds that should
still be audible even when far away.

## AudioListener

- Exactly **one** AudioListener per scene (Unity warns otherwise).
- Lives on Main Camera by default.
- Moving it to the player (first-person / third-person) is often more natural
  — what you see is not always where your ears should be.

```
Third-person camera decision:
  Listener on Camera  → sound scales with camera zoom (can feel off)
  Listener on Player  → sound scales with character (more natural)
```

## Min & Max Distance

A 3D AudioSource shows two wire spheres in the Scene view:

```
                       MAX DISTANCE (outer sphere)
                 ╱─────────────────────────╲
               ╱                             ╲
              │      ATTENUATION ZONE         │   volume drops here
              │                               │
              │     ╱──────────────╲          │
              │    │  MIN DISTANCE  │         │   full volume inside
              │    │      ◉         │         │
              │    │   (source)     │         │
              │     ╲──────────────╱          │
               ╲                             ╱
                 ╲─────────────────────────╱
```

- **Min Distance** — inside this radius, the sound is at full volume. Should
  roughly match the physical size of the thing making the sound. A waterfall
  might be 3–5 units; a footstep 0.5.
- **Max Distance** — past this radius, the sound is effectively silent (exact
  behaviour depends on rolloff curve).

## Volume Rolloff

The shape of the falloff between Min and Max:

```
Volume
  1 │●────╮
    │      ╲
    │        ╲_
    │           ╲____________             Logarithmic (real-world)
    │                        ╲___________
  0 │                                    ╲___
    └──────────────────────────────────────────▶ Distance
      min                                     max


Volume
  1 │●───────────────╮
    │                  ╲
    │                    ╲                Linear (predictable)
    │                      ╲
  0 │                        ╲
    └──────────────────────────────────────────▶ Distance
      min                                     max
```

| Rolloff        | Feel                           | Use for                                     |
|----------------|--------------------------------|---------------------------------------------|
| **Logarithmic**| Starts dropping fast, long tail| Realistic sounds, most ambient              |
| **Linear**     | Even fade, predictable         | Gameplay-critical audio where distance matters linearly (pickup ping) |
| **Custom**     | Curve editor                   | Set a sweet spot — e.g. full volume for a range, then drop hard |

Unity's default is Logarithmic with steep initial fall — good for most cases
but unrecognizable at medium distances. For gameplay-important sounds, switch
to Linear or a Custom curve.

## Spread

How wide the sound is distributed across the stereo field.

```
Spread = 0°    → Fully positional. Panning is aggressive.
                  Tiny left/right movements cause hard speaker switches.
Spread = 60–120° → Natural. Sound has width.
Spread = 180°  → Fully spread — no directional info left.
Spread = 360°  → Inverted (special effect territory).
```

Tiny sources (a coin) can take Spread 0. Large sources (a crowd, an engine)
benefit from 90°+ so they don't snap between speakers.

## Doppler Level

Simulates frequency shift when source or listener is moving fast.

```
     approaching ambulance → higher pitch
     passing               → neutral
     receding              → lower pitch
```

| Doppler Level | Meaning                                |
|---------------|----------------------------------------|
| 0             | Off. Use for stationary sounds.        |
| 1             | Realistic. Use for vehicles, projectiles.|
| > 1           | Exaggerated. Arcade racing, sci-fi pass-bys.|

Global Doppler speed is tuned under `Edit > Project Settings > Audio > Doppler Factor`.

## 2D Visuals, 3D Audio

Common mistake: "my game is 2D, so I set Spatial Blend to 0 on everything."
Wrong — a side-scroller enemy should still feel on the correct side. Keep
spatial blend at 1 (or around 0.7) for positional SFX in 2D games. The scene
still has a 3D coordinate system underneath.

## When to Use This

- Scaling Min/Max Distance to your scene (waterfall vs footstep vs bomb)
- Picking rolloff based on gameplay vs realism
- Preventing "speaker snapping" on tiny sources (Spread)
- Adding Doppler to vehicles / projectiles for life
- Deciding whether the AudioListener goes on Camera or Player
