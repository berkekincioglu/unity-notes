# Input System Best Practice — Folder Structure

This folder contains the professional usage pattern for Unity's Input System.
Use this as a reference in future projects.

## Folder Contents

```
input-system/
│
├── 00-Folder-Structure.md          ← This file (overview)
├── 01-Setup.md                     ← Step-by-step setup (what to do in Unity Editor)
│
├── InputManager.cs                 ← Centralized input control (Singleton)
│                                      Sole owner of all input
│                                      Map switching is done here
│
├── PlayerController.cs             ← Player movement (Hybrid: Interface + Polling)
│                                      Interface → new actions can't be forgotten
│                                      Polling → all logic in Update, one place
│                                      Can ONLY see Player actions
│
├── UIController.cs                 ← Menu / UI control (Hybrid: Interface + Polling)
│                                      Can ONLY see UI actions
│                                      Cannot access Player/Vehicle actions
│
└── VehicleController.cs            ← Vehicle control (Hybrid: Interface + Polling)
                                       Can ONLY see Vehicle actions
                                       Includes enter/exit flow example
```

## Placement in a Real Unity Project

```
Assets/
├── Input/
│   ├── GameInputActions.inputactions    ← Action Asset (create in Unity Editor)
│   └── GameInputActions.cs              ← Auto-generated (created when you click Apply)
│
├── Scripts/
│   ├── Input/
│   │   └── InputManager.cs
│   ├── Player/
│   │   └── PlayerController.cs
│   ├── UI/
│   │   └── UIController.cs
│   └── Vehicle/
│       └── VehicleController.cs
│
└── Scenes/
    └── GameScene.unity
```

## Architecture Overview

```
                 GameInputActions.inputactions
                 (DEFINITION of all actions)
                           │
                    Generate C# Class
                           │
                           ▼
                 GameInputActions.cs (Auto-generated)
                 ├── .Player   (PlayerActions struct)
                 ├── .UI       (UIActions struct)
                 └── .Vehicle  (VehicleActions struct)
                           │
                           ▼
                    InputManager.cs (Singleton)
                    ONE instance, ONE owner
                    ├── .Player  property
                    ├── .UI      property
                    └── .Vehicle property
                     │         │         │
                     ▼         ▼         ▼
              Player       UI        Vehicle
              Controller   Controller Controller
              knows        knows      knows
              .Player      .UI        .Vehicle
              ONLY         ONLY       ONLY
```

## Reading Order

1. This file (overview)
2. 01-Setup.md (what to do in Unity Editor)
3. InputManager.cs (understand the centralized structure)
4. PlayerController.cs (hybrid approach — interface + polling)
5. UIController.cs (different map usage)
6. VehicleController.cs (map switching flow)

## What is the Hybrid Approach?

All controllers use the same pattern:

  Implement interface → write empty methods (compiler safety net)
  Poll in Update      → all logic is read in one place

This way:
  - Adding a new action triggers a COMPILE ERROR (you can't forget)
  - But logic doesn't scatter — everything reads top-to-bottom in Update
