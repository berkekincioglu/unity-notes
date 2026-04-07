# Genre Recipes — Quick Reference

Which Cinemachine setup for which game type? Find your genre, grab the .cs file.

---

## Quick Reference Table

| Genre            | Position Control      | Rotation Control     | Extensions              | File                  |
|------------------|-----------------------|----------------------|-------------------------|-----------------------|
| 3rd Person Action| Orbital Follow        | Rotation Composer    | Deoccluder              | ThirdPersonSetup.cs   |
| TPS Shooter      | Third Person Follow   | Hard Look At         | Deoccluder, 3rd P. Aim  | TPSShooterSetup.cs    |
| FPS              | Hard Lock to Target   | Pan Tilt             | (none or Noise)         | FPSSetup.cs           |
| Top-Down / RTS   | Follow (World Space)  | (none, fixed angle)  | (optional Confiner 3D)  | TopDownSetup.cs       |
| 2D Platformer    | Position Composer     | (none, 2D)           | Confiner 2D             | Platformer2DSetup.cs  |
| Racing           | Follow (Lock Target)  | Rotate With Target   | FreeLook Modifier       | RacingSetup.cs        |
| Cutscene         | Spline Dolly          | Rotation Composer    | (Post Processing)       | CutsceneSetup.cs      |
| Multi-target     | Follow + Group Framing| Rotation Composer    | Group Framing           | (see 03-Extensions.md)|

---

## Common Patterns

### Switching Cameras at Runtime
All genres benefit from a centralized CameraManager.cs that handles priority switching.
See: CameraManager.cs

### Adding Shake to Any Genre
1. Add Noise behavior (continuous) OR Impulse Listener extension (event-based)
2. Works with any position/rotation setup
3. See: 03-Extensions.md (Noise & Impulse sections)

### Obstacle Avoidance for Any 3D Genre
Add the Deoccluder extension to any 3D CinemachineCamera. It works regardless of which position control behavior is used.

---

## Input Controller Reminder

Cameras that need player input (Orbital Follow, Pan Tilt) require:
- **CinemachineInputAxisController** component on the same GameObject
- Automatically integrates with Unity's Input System package
- Configure gain (sensitivity), accel/decel time in Inspector
