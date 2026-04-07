# Blending — Camera Transitions

When the active camera changes (priority shift), CinemachineBrain blends between them automatically.

---

## Default Blend

Set on the CinemachineBrain component:

```
CinemachineBrain (on Main Camera):
  Default Blend:
    Style: Ease In Out
    Time:  2 seconds
```

This applies to ALL camera transitions unless overridden.

---

## Blend Styles

| Style        | Description                                          |
|--------------|------------------------------------------------------|
| Cut          | Instant switch, no blend (0 seconds)                 |
| Linear       | Uniform interpolation — constant speed               |
| Ease In Out  | S-curve — smooth start and end (most natural)        |
| Ease In      | Linear exit, eased entry to new camera               |
| Ease Out     | Eased exit, linear entry to new camera               |
| Hard In      | Eased exit, accelerated entry                        |
| Hard Out     | Accelerated exit, eased entry                        |
| Custom       | User-defined animation curve                         |

```
Linear:      ╱─────────── (constant speed)
Ease In Out: ╱⌒──────⌒── (slow start, slow end — most common)
Cut:         │            (instant)
```

---

## Custom Blends (Per Camera Pair)

Create a **Cinemachine Blender Settings** asset to override default blends for specific transitions.

```
Assets > Create > Cinemachine > Blender Settings
```

Then assign it to CinemachineBrain's Custom Blends field.

### Example Configuration

| From         | To           | Style       | Time |
|--------------|--------------|-------------|------|
| FollowCam    | AimCam       | Ease In     | 0.3s |
| AimCam       | FollowCam    | Ease Out    | 0.8s |
| ANY CAMERA   | CutsceneCam  | Cut         | 0s   |
| CutsceneCam  | ANY CAMERA   | Ease In Out | 1.5s |

- Use "ANY CAMERA" as a wildcard
- More specific entries override less specific ones
- First match wins if equal specificity

---

## Blend Hints (on CinemachineCamera)

Fine-tune HOW the blend interpolates:

| Hint                           | Effect                                     |
|--------------------------------|--------------------------------------------|
| Spherical Position             | Path curves around target (orbiting feel)  |
| Cylindrical Position           | Same but only horizontal curve             |
| Screen Space Aim When Targets Differ | Blend aim in screen space              |
| Inherit Position               | Start from current Unity camera position   |
| Ignore Target                  | Don't blend targets, just positions        |

---

## Triggering Blends from Code

```csharp
// Simply change priority — Brain handles the blend automatically
followCam.Priority = 0;
aimCam.Priority = 10;
// Brain detects the change and blends using configured style/time
```

No manual interpolation needed. This is the power of Cinemachine — just set priorities.
