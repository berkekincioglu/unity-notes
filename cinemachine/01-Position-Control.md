# Position Control — Where the Camera Is

Cinemachine offers 6 position behaviors. Add one to a CinemachineCamera to control its position.

---

## 1. Follow

Maintains a fixed offset from the tracking target with damping.

```
Camera stays at offset (0, 5, -10) from player:

  Side view:
       📷
      ╱
     ╱  offset
    ╱
   ● Player
```

### Key Properties

| Property         | Description                                           |
|------------------|-------------------------------------------------------|
| Follow Offset    | Camera distance from target (default: 0, 0, -10)     |
| Position Damping | Smoothness per axis. Higher = slower, smoother        |
| Binding Mode     | How offset is interpreted relative to target rotation |

### Binding Modes (Critical Concept)

| Mode                         | Behavior                                         |
|------------------------------|--------------------------------------------------|
| World Space                  | Offset in world coords. Target rotates → camera stays |
| Lock To Target               | Camera rotates WITH target. Offset maintained in local space |
| Lock To Target No Roll       | Same but ignores target's roll                   |
| Lock To Target With World Up | Only yaw affects camera, ignores pitch/roll       |
| Lock To Target On Assign     | Locks offset at activation, then keeps world space |
| Lazy Follow                  | Minimizes movement like a human operator          |

**Use for:** Top-down, side-scroller, RTS, simple 3rd person

---

## 2. Orbital Follow

Camera orbits around the target. Player can control the orbit with mouse/stick via CinemachineInputAxisController.

```
        📷 (top)
       ╱
      ╱
📷 ──●── 📷     Player rotates camera around target
      ╲
       ╲
        📷 (bottom)
```

### Modes

| Mode       | Description                                              |
|------------|----------------------------------------------------------|
| Sphere     | Camera can be at any point on a sphere around the target |
| ThreeRig   | 3 circular orbits (top/mid/bottom) connected by spline   |

### Key Properties

| Property       | Description                                           |
|----------------|-------------------------------------------------------|
| Horizontal     | Y-axis rotation (degrees)                             |
| Vertical       | X-axis rotation (degrees in Sphere, arbitrary in Rig) |
| Radius         | Distance multiplier from target                       |
| Recentering    | Auto-return to center when input stops                |
| Binding Mode   | Same 6 modes as Follow                               |

**Use for:** 3rd person action/RPG (Dark Souls, Zelda style)

---

## 3. Third Person Follow

Shoulder camera with a 4-point mini-rig. Built-in obstacle avoidance.

```
Mini-rig structure:

  Side view:              Top view:
       C ── D(📷)            ● Target
       │                     │
       │                   B ┤ (shoulder offset)
       B                     │
       │                     D 📷 (behind)
       A(●)

  A = Origin (target position)
  B = Shoulder (horizontal offset)
  C = Hand (vertical offset from shoulder)
  D = Camera (behind hand, at Camera Distance)
```

### Key Properties

| Property                 | Description                                    |
|--------------------------|------------------------------------------------|
| Shoulder Offset          | X,Y,Z offset from target (the "shoulder")      |
| Vertical Arm Length      | Vertical offset for the "hand" pivot            |
| Camera Side              | 0 = left shoulder, 1 = right shoulder           |
| Camera Distance          | How far behind the hand the camera sits         |
| Camera Collision Filter  | Which layers block the camera                   |
| Camera Radius            | Minimum distance from obstacles                 |
| Damping Into Collision   | Smoothness when avoiding obstacles              |

When obstacles block the view, the rig bends to keep the camera outside while maintaining target visibility.

**Use for:** TPS shooters (The Last of Us, Gears of War style)

---

## 4. Position Composer

Keeps the target at a specific screen-space position using dead zone / soft zone system.

```
┌─────────────────────────────────┐
│          HARD LIMIT (red)       │
│   ┌──────────────────────┐      │
│   │    SOFT ZONE (blue)  │      │
│   │  ┌──────────────┐   │      │
│   │  │  DEAD ZONE   │   │      │
│   │  │      ●       │   │      │  Target in dead zone
│   │  │   Target      │   │      │  → camera does NOT move
│   │  └──────────────┘   │      │
│   │                      │      │  Target enters soft zone
│   └──────────────────────┘      │  → camera moves SLOWLY
│                                 │
│                                 │  Hard limit = absolute boundary
└─────────────────────────────────┘
```

### Key Properties

| Property        | Description                                          |
|-----------------|------------------------------------------------------|
| Camera Distance | Z-axis separation from target                        |
| Dead Zone       | Area where camera doesn't react                      |
| Soft Zone       | Area where camera starts following (with damping)    |
| Screen Position | Where to place target on screen (0 = center)         |
| Damping         | Smoothness per axis                                  |
| Lookahead       | Predicts target movement to adjust early             |

**Use for:** 2D platformers (Celeste, Hollow Knight), side-scrollers

---

## 5. Hard Lock to Target

Camera position = target position. Exactly. No offset, no damping.

```
📷 = ● (camera IS the target)
```

**Use for:** FPS cameras (camera = player's head), vehicle interiors

---

## 6. Spline Dolly

Camera moves along a predefined spline path.

```
Spline path:
  ○───────○───────○───────○
  knot0   knot1   knot2   knot3
              📷
         (camera slides along path)
```

### Position Units

| Unit       | Description                            |
|------------|----------------------------------------|
| Knot       | Integer/decimal knot index (0, 1, 2.5) |
| Distance   | World units from spline start          |
| Normalized | 0-1 across entire spline               |

### Auto-Dolly Modes

| Mode                    | Description                              |
|-------------------------|------------------------------------------|
| Fixed Speed             | Constant velocity along spline           |
| Nearest Point To Target | Camera goes to closest point on spline   |

### Rotation Options

| Mode                     | Description                              |
|--------------------------|------------------------------------------|
| Default                  | No rotation modification                 |
| Path                     | Follows spline tangent and up vector     |
| Path No Roll             | Same but no roll                         |
| Follow Target            | Adopts tracking target's rotation        |
| Follow Target No Roll    | Same but no roll                         |

**Use for:** Cutscenes, cinematic fly-throughs, racing game track cameras
