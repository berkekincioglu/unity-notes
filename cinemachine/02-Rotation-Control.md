# Rotation Control — Where the Camera Looks

These behaviors control the camera's rotation (aim). Add one to a CinemachineCamera alongside a Position Control behavior.

---

## 1. Rotation Composer

Rotates camera to face the Look At target using dead zone / soft zone composition.

```
┌──────────────────────────────┐
│         SOFT ZONE            │
│    ┌──────────────────┐      │
│    │    DEAD ZONE     │      │
│    │       ●          │      │  Target in dead zone
│    │     Target       │      │  → camera does NOT rotate
│    └──────────────────┘      │
│                              │  Target enters soft zone
│                              │  → camera rotates SLOWLY
└──────────────────────────────┘
```

### Key Properties

| Property        | Description                                          |
|-----------------|------------------------------------------------------|
| Target Offset   | Aim point offset in target's local space              |
| Damping         | Rotation responsiveness (lower = faster)              |
| Screen Position | Where to frame target (0 = center, ±0.5 = edges)     |
| Dead Zone       | Frame area where camera doesn't adjust               |
| Soft Zone       | Frame area where camera starts adjusting             |
| Hard Limits     | Absolute boundary target cannot exceed               |
| Lookahead       | Predicts movement to pre-rotate (can cause jitter)   |
| Center On Activate | Auto-center target when camera becomes active     |

**Use for:** 3rd person action, anything needing composed framing

---

## 2. Hard Look At

Always centers the Look At target in the frame. No dead zone, no composition.

```
  📷 ─────────────→ ● Target (always centered)
```

Simple and reliable. Target is always dead-center.

**Use for:** Lock-on systems, simple follow cameras, boss tracking

---

## 3. Pan Tilt

Player input controls camera rotation directly. No target tracking.

```
  Mouse/Stick → Pan (horizontal) + Tilt (vertical)

  📷 rotates freely based on input
```

Requires CinemachineInputAxisController to receive player input.

**Use for:** FPS mouse look, free camera exploration

---

## 4. Rotate With Follow Target

Camera adopts the tracking target's rotation.

```
  Target rotates 45° right → Camera rotates 45° right

  ●→    📷→     (always facing same direction as target)
```

**Use for:** Vehicle cameras, airplane/spaceship following, rail grinding
