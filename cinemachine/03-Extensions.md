# Extensions — Extra Camera Features

Extensions are added to a CinemachineCamera to provide additional capabilities on top of Position and Rotation control.

---

## 1. Deoccluder (Obstacle Avoidance)

Prevents the camera from being blocked by obstacles. Uses Physics Raycasting.

```
Without Deoccluder:              With Deoccluder:

  Wall──┐                        Wall──┐
        │  📷 stuck               │
        │  behind wall            │  📷 pulled forward
        │  can't see player       │  player visible
  ● Player                     ● Player
────────┘                      ────────┘
```

### Avoidance Strategies

| Strategy               | Description                                     |
|------------------------|-------------------------------------------------|
| Pull Camera Forward    | Moves camera along Z until in front of obstacle  |
| Preserve Camera Height | Relocates camera but keeps original height       |
| Preserve Camera Distance | Relocates camera but keeps original distance   |

### Key Properties

| Property              | Description                                      |
|-----------------------|--------------------------------------------------|
| Distance Limit        | Max raycast distance (0 = use target distance)   |
| Camera Radius         | Clearance distance from obstacles                |
| Damping               | Return speed after occlusion clears              |
| Damping When Occluded | Responsiveness during obstacle avoidance         |
| Maximum Effort        | Max obstacle hits to process (4 is usually enough)|
| Ignore Tag            | Skip obstacles with this tag (use for target)    |

**Use for:** Any 3rd person camera in environments with walls/objects

---

## 2. Confiner 2D

Constrains the camera within a 2D polygon boundary. Screen edges never show outside the shape.

```
┌─────────────────────────────────┐
│  Polygon Collider 2D           │
│  (defines the game world)       │
│                                 │
│    ┌───────────────────┐        │
│    │   Camera View     │        │
│    │       ● Player    │        │
│    └───────────────────┘        │
│                                 │
│  Camera stops at boundary →     │
│  player sees only game world    │
└─────────────────────────────────┘
```

### Key Properties

| Property         | Description                                        |
|------------------|----------------------------------------------------|
| Bounding Shape 2D | The PolygonCollider2D or CompositeCollider2D       |
| Damping          | Smoothness around corners (higher = more gradual)  |
| Slowing Distance | Camera decelerates near boundary edges             |
| Max Window Size  | Performance optimization for cache calculations    |

**Important:** Call `InvalidateBoundingShapeCache()` when the polygon changes at runtime.

**Tip:** Use multiple CinemachineCameras with different bounding shapes and blend between them instead of changing a single Confiner's shape.

**Use for:** 2D platformers, any game with defined world boundaries

---

## 3. Confiner 3D

Same concept as Confiner 2D but uses a 3D Collider volume.

**Use for:** 3D games with camera-restricted zones (rooms, arenas)

---

## 4. Noise (Basic Multi Channel Perlin)

Continuous camera shake using Perlin noise. Simulates handheld camera feel.

### Key Properties

| Property       | Description                                       |
|----------------|---------------------------------------------------|
| Noise Profile  | Predefined shake pattern asset (Unity has built-in)|
| Amplitude Gain | Shake intensity (0 = off, 1 = default)            |
| Frequency Gain | Shake speed (higher = faster vibrations)          |
| Pivot Offset   | Offset for rotation-based shake                   |

Both Amplitude and Frequency can be animated at runtime (e.g., increase shake as player takes damage).

**Use for:** Running camera bob, idle breathing, tension building

---

## 5. Impulse System (Event-Based Shake)

Camera shake triggered by game events (explosions, hits, impacts).

```
  Impulse Source               Impulse Listener
  ┌──────────────┐            ┌──────────────────┐
  │  Explosion   │ ~~~wave~~→ │  CinemachineCamera│
  │  GameObject  │            │  + Listener       │
  └──────────────┘            └──────────────────┘

  Shake intensity decreases with distance from source.
```

### Two Components

| Component                        | Role                                  |
|----------------------------------|---------------------------------------|
| CinemachineImpulseSource         | General purpose — trigger from code   |
| CinemachineCollisionImpulseSource| Auto-triggers on collision events     |
| CinemachineImpulseListener       | Extension on VCam — "hears" impulses  |

### Setup

1. Add Impulse Source to GameObjects that cause shake (explosion prefab, weapon, etc.)
2. Add Impulse Listener extension to CinemachineCameras that should react

**Use for:** Explosions, weapon recoil, landing impact, boss attacks

---

## 6. Group Framing

Dynamically adjusts zoom/position to keep multiple targets in frame.

```
  Before:                    After (auto-zoomed out):
  ┌──────────┐              ┌──────────────────────┐
  │ ●A       │              │                      │
  │          │    zoom out  │  ●A            ●B    │
  │    ●B out│  ──────────→ │                      │
  └──────────┘              └──────────────────────┘
```

### Requirements

1. CinemachineTargetGroup component (separate GameObject)
2. Add members (GameObjects) with Weight and Radius
3. Set CinemachineCamera's Tracking Target to the Target Group
4. Add Group Framing extension

### Target Group Properties

| Property      | Description                                     |
|---------------|-------------------------------------------------|
| Weight        | Influence on group center (0 = ignored)         |
| Radius        | Used for bounding box calculation                |
| Position Mode | Group Center (bounding box) or Group Average    |

### Group Framing Properties

| Property         | Description                                    |
|------------------|------------------------------------------------|
| Framing Size     | Screen occupancy (1.0 = full, 0.5 = half)     |
| Damping          | Adjustment speed                               |
| Size Adjustment  | Zoom Only / Dolly Only / Dolly Then Zoom       |

**Use for:** Co-op multiplayer, boss fights, sports games, party games

---

## 7. Other Notable Extensions

| Extension          | Description                                       |
|--------------------|---------------------------------------------------|
| FreeLook Modifier  | Varies FOV/noise/damping based on orbit position  |
| Third Person Aim   | Steady crosshair for TPS shooters                 |
| Pixel Perfect      | Prevents sub-pixel movement for pixel art games   |
| Post Processing    | Per-camera post-process effects                   |
| Follow Zoom        | Adjusts FOV to maintain target size on screen     |
| Recomposer         | Runtime composition adjustments                   |
