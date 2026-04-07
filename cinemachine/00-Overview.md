# Cinemachine 3.1 — Overview

## Architecture

Cinemachine has 3 core elements:

```
┌──────────────────────────────────────────────────────────────┐
│  Main Camera                                                  │
│  + CinemachineBrain          "Director" — decides which       │
│                               virtual camera controls the     │
│                               Unity camera                    │
└──────────────┬───────────────────────────────────────────────┘
               │
               │  Highest priority wins
               │
  ┌────────────┼────────────────────┐
  ▼            ▼                    ▼
┌──────┐  ┌──────┐            ┌──────┐
│VCam A│  │VCam B│            │VCam C│
│Pri:10│  │Pri: 0│            │Pri: 0│
│ACTIVE│  │      │            │      │
└──────┘  └──────┘            └──────┘
```

1. **CinemachineBrain** — Attached to Main Camera. Only ONE. Monitors all virtual cameras
   and picks the one with highest priority.

2. **CinemachineCamera** — Added to empty GameObjects. As many as you want. Each defines
   a "shot" — where the camera is, what it looks at, how it behaves.

3. **Priority** — Determines which virtual camera is active. Change priority at runtime
   to switch cameras with automatic blending.

## CinemachineCamera Properties

| Property         | Description                                              |
|------------------|----------------------------------------------------------|
| Priority         | Higher value = this camera wins. Negative values allowed |
| Tracking Target  | The GameObject the camera follows                        |
| Look At Target   | Optional separate target for rotation (aiming)           |
| Lens / FOV       | Field of view, near/far clip, Dutch angle                |
| Standby Update   | Never / Always / Round Robin — update rate when inactive |

## Procedural Behaviors

A CinemachineCamera is passive by default. You add behaviors to make it move:

```
CinemachineCamera
├── Position Control   ← WHERE the camera is (Follow, Orbital, ThirdPerson...)
├── Rotation Control   ← WHERE the camera looks (Composer, HardLookAt, PanTilt...)
├── Noise              ← Continuous shake (Perlin noise)
└── Extensions         ← Extra features (Deoccluder, Confiner, Impulse...)
```

See: 01-Position-Control.md, 02-Rotation-Control.md, 03-Extensions.md

## Blending (Camera Transitions)

When priority changes, CinemachineBrain blends between cameras automatically.

```
VCam A (Priority 10)  →  VCam B (Priority 20)

Frame:  ████████████░░░░░░░░░░░░  
        ← VCam A ──→←── Blend ──→← VCam B ──→
                     (default 2s)
```

Default blend is configurable. Custom blends per camera pair possible.
See: 04-Blending.md

## Reading Order

1. This file (architecture overview)
2. 01-Position-Control.md (6 position behaviors)
3. 02-Rotation-Control.md (4 rotation behaviors)
4. 03-Extensions.md (Deoccluder, Confiner, Impulse, etc.)
5. 04-Blending.md (transitions between cameras)
6. 05-Genre-Recipes.md (game-type specific setups)
7. .cs files (working code examples per genre)
