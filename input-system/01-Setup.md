# Input System Best Practice — Setup Guide (Unity 6.3)

## Overview

By using this structure:
- Each system (Player, UI, Vehicle) can ONLY access its OWN actions
- Access to other systems' actions is BLOCKED at the compiler level
- No string-based lookups, no typo risk
- When a new action is added, the interface warns you — you can't forget

---

## Step 1: Create an Input Action Asset

```
Project Window > right-click > Create > Input Actions
Name: "GameInputActions"
```

This creates a `GameInputActions.inputactions` file.
Double-click it to open the **Input Actions Editor**.

---

## Step 2: Define Action Maps and Actions

When the editor opens, you'll see 3 panels:

```
┌─────────────────┬──────────────────────┬───────────────────┐
│  (A) ACTION MAPS│  (B) ACTIONS         │ (C) PROPERTIES    │
│                 │                      │                   │
│  + to add new   │  + to add new action │  Settings for the │
│  map            │                      │  selected action/ │
│                 │                      │  binding          │
│  Player     [x] │  Move    [+]         │                   │
│  UI         [x] │  Jump    [+]         │  Action Type:     │
│  Vehicle    [x] │  Attack  [+]         │  [ Button ▼ ]     │
│                 │  Look    [+]         │                   │
└─────────────────┴──────────────────────┴───────────────────┘
```

### 2a: Adding Action Maps

Click the (+) button in the left panel. Create these maps:

```
1. Player     — Player movement, jump, attack
2. UI         — Menu navigation, submit, cancel
3. Vehicle    — Vehicle control
```

### 2b: Adding Actions to Each Map

Select a map (e.g. "Player"), then click (+) in the middle panel to add actions:

#### PLAYER Action Map:

| Action Name | Action Type | Control Type | Description             |
|-------------|-------------|--------------|--------------------------|
| Move        | Value       | Vector2      | WASD / Left Stick        |
| Look        | Value       | Vector2      | Mouse / Right Stick      |
| Jump        | Button      | (default)    | Space / Gamepad South    |
| Attack      | Button      | (default)    | Left Click / Gamepad West|
| Sprint      | Button      | (default)    | Left Shift / Gamepad East|

#### UI Action Map:

| Action Name | Action Type | Control Type | Description             |
|-------------|-------------|--------------|--------------------------|
| Navigate    | Value       | Vector2      | Arrow Keys / D-Pad       |
| Submit      | Button      | (default)    | Enter / Gamepad South    |
| Cancel      | Button      | (default)    | Escape / Gamepad East    |
| Click       | Button      | (default)    | Left Click               |

#### VEHICLE Action Map:

| Action Name | Action Type | Control Type | Description             |
|-------------|-------------|--------------|--------------------------|
| Steer       | Value       | Vector2      | AD / Left Stick          |
| Accelerate  | Value       | Axis         | W / Right Trigger        |
| Brake       | Button      | (default)    | S / Left Trigger         |
| Exit        | Button      | (default)    | E / Gamepad North        |

### 2c: Adding Bindings (Key Mapping)

Click the (+) next to an action to add bindings:

**For a single key:**
- Select "Add Binding"
- Click the "Path" dropdown in the Properties panel
- Press the "Listen" button → press the key → auto-detected
- Or manually select from the list (e.g. Keyboard > W)

**For WASD / 2D Vector (like Move):**
- Select "Add Up/Down/Left/Right Composite" (or "Add 2D Vector Composite")
- 4 sub-bindings are created: Up, Down, Left, Right
- Click each one and assign a key via Path:

```
Move action:
├── WASD (2D Vector Composite)
│   ├── Up:    Keyboard > W
│   ├── Down:  Keyboard > S
│   ├── Left:  Keyboard > A
│   └── Right: Keyboard > D
│
└── Left Stick (Binding)
    └── Path: Gamepad > Left Stick
```

**For multiple device support:**
Add multiple bindings to the same action. For example, Move:
1. One 2D Vector Composite (WASD) → keyboard
2. One regular Binding (Left Stick) → gamepad

Unity uses whichever device is active.

---

## Step 3: Generate the C# Class

1. In the Project window, SELECT `GameInputActions.inputactions` (single click, not double)
2. In the Inspector, you'll see these settings:

```
Inspector:
┌─────────────────────────────────────┐
│ GameInputActions (Input Action Asset)│
│                                     │
│ ☑ Generate C# Class                 │  ← ENABLE THIS
│                                     │
│ C# Class File:                      │
│   GameInputActions.cs               │  ← Auto-filled
│                                     │
│ C# Class Name:                      │
│   GameInputActions                   │  ← Keep as is
│                                     │
│ C# Class Namespace:                  │
│   (leave empty or "MyGame.Input")   │  ← Optional
│                                     │
│            [ Apply ]                 │  ← CLICK THIS
└─────────────────────────────────────┘
```

3. Click "Apply" → Unity auto-generates `GameInputActions.cs`

IMPORTANT: Every time you add/change Action Maps or Actions,
the file auto-updates. Do NOT edit it manually — it will be overwritten.

---

## Step 4: Use in Scripts

Example files in this folder:
- InputManager.cs       → Centralized control (Singleton)
- PlayerController.cs   → Player movement
- UIController.cs       → Menu control
- VehicleController.cs  → Vehicle control

---

## Quick Reference: Action Type Selection

```
Continuous value reading?    → Value        (Move, Look, Steer)
One-time trigger?            → Button       (Jump, Attack, Submit)
Unfiltered raw input?        → Pass-Through (special cases)
```

## Quick Reference: Reading Methods

```csharp
// Continuous value (Value type action)
Vector2 dir = playerInput.Move.ReadValue<Vector2>();

// Is it held down? (true every frame while pressed)
bool running = playerInput.Sprint.IsPressed();

// Was it pressed this frame? (true for only 1 frame)
bool jumped = playerInput.Jump.WasPressedThisFrame();

// Was it released this frame? (true for only 1 frame)
bool released = playerInput.Attack.WasReleasedThisFrame();
```
