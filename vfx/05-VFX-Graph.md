# VFX Graph

VFX Graph is the GPU-driven, node-based alternative to Particle System. Use it
for effects that need millions of particles, complex stylised shaders, or
heavy GPU work. It cannot interact with CPU physics.

## Setup

```
Window → Package Manager → Visual Effect Graph → Install
Project → Create → Visual Effects → Visual Effect Graph    (creates the asset)
GameObject → Visual Effects → Visual Effect              (creates a scene VFX)
Drag the asset into the Visual Effect component's Asset Template field
```

Open the graph by double-clicking the asset.

## Architecture: Four Contexts

A VFX Graph is a directed flow of *contexts* (the big colored boxes), each
holding *blocks* (operations).

```
┌─────────────────────────────┐
│  SPAWN                      │   How often / how many to emit
│   Constant Spawn Rate        │
└──────────────┬──────────────┘
               ▼
┌─────────────────────────────┐
│  INITIALIZE PARTICLE        │   Properties at birth
│   Set Velocity Random        │   - shape, position
│   Set Lifetime               │   - velocity, size, color
│   Set Size                   │
└──────────────┬──────────────┘
               ▼
┌─────────────────────────────┐
│  UPDATE PARTICLE            │   Behaviour over lifetime
│   Turbulence                 │   - color/size over life
│   Color over Life            │   - turbulence, collision
│   Collide with Plane         │
└──────────────┬──────────────┘
               ▼
┌─────────────────────────────┐
│  OUTPUT PARTICLE            │   How to draw
│   Quad / Mesh / Lit / Unlit │   - material assignment
│                              │   - render mode
└─────────────────────────────┘
```

Read the graph top to bottom: spawn rule → initial state → frame-by-frame
update → render.

## Context Cheat-Sheet

| Context             | Particle System equivalent                |
|---------------------|-------------------------------------------|
| Spawn               | Emission module (Rate over Time, Bursts)  |
| Initialize Particle | Main module + Shape module                |
| Update Particle     | Most over-lifetime modules + Noise        |
| Output Particle     | Renderer module                           |

## Blocks

Blocks are the operations inside a context. Right-click a context → Create
Block. Examples:

```
Initialize Particle:
  Set Velocity Random (range)
  Set Lifetime Random (range)
  Set Size
  Set Color Random
  Set Position : Sphere

Update Particle:
  Turbulence
  Add Velocity over Life
  Conform to Sphere
  Collide with Sphere
  Multiply Size over Life

Output Particle Quad:
  Set Color over Life
  Texture Sample
```

Drag connections between blocks to feed values.

## Properties (Inspector-Exposed Variables)

Open the *Blackboard* (left panel) → `+` → Float / Vector3 / Color / Texture2D.
Mark `Exposed`. Now the property shows up on the Visual Effect component in
the Inspector AND can be set from script:

```csharp
[SerializeField] private VisualEffect vfx;

vfx.SetFloat("ExplosionRadius", 5f);
vfx.SetVector3("TargetPosition", target.position);
vfx.SetTexture("FlowMap", customTexture);
```

Properties drive any block input — connect "ExplosionRadius" to a Sphere
block's radius input and you can size explosions per-call.

## Events

Default events:
```
OnPlay  → triggered by VisualEffect.Play()
OnStop  → triggered by VisualEffect.Stop()
```

Custom events:
```
Add an Event context to the graph, name it "Explode".
Wire it as the input to a Spawn context that does a burst.

In script: vfx.SendEvent("Explode");
```

This is how you trigger one-shot bursts in VFX Graph.

## Spawn Modes

Spawn contexts can use different blocks:

| Block              | Behavior                                              |
|--------------------|-------------------------------------------------------|
| Constant Spawn Rate| Steady N particles per second                          |
| Variable Spawn Rate| Animated curve of spawn rate                           |
| Single Burst       | One-time N particles                                   |
| Periodic Burst     | N particles every M seconds                            |

Connect events to control these from script.

## Output Particle Variants

| Output type           | Use for                              |
|-----------------------|--------------------------------------|
| Output Particle Quad  | Standard billboard quad              |
| Output Particle Mesh  | 3D mesh per particle (debris)        |
| Output Particle Strip | Trails as connected strip geometry   |
| Output Particle Lit   | URP/HDRP-aware lit particles         |
| Output Particle Unlit | Faster, ignores lighting             |

## Coordinate Spaces

VFX Graph contexts have a Space toggle (Local / World) at the top-right of
each context. Same idea as Particle System Simulation Space, but per-context.

## Compute Operators (Math in the Graph)

Right-click → Create Node:
- Add, Multiply, Subtract, Divide, Lerp, Saturate
- Sample Curve, Sample Gradient
- Time, Random Number
- Get Position, Get Velocity, Get Age

Wiring Sample Curve into Set Size = "particle size follows this curve over its
lifetime" — same as Particle System's Size over Lifetime, but explicit.

## Visual Effect Component (Scene-side)

Drop the asset onto a GameObject's Visual Effect component. Inspector exposes:

| Property              | Meaning                                   |
|-----------------------|-------------------------------------------|
| Asset Template        | Which graph asset                         |
| Initial Event Name    | Which event fires on enable               |
| Random Seed           | Deterministic playback                    |
| Update Mode           | Fixed Delta Time / Delta Time             |
| Culling Flags         | When to simulate / skip                   |
| Reseed on Play        | New seed every Play() call                |
| (Exposed Properties)  | Whatever you marked on the Blackboard     |

## Limitations

- **No CPU physics** — particles cannot collide with Rigidbodies. Use plane
  / sphere / sdf colliders defined inside the graph instead.
- **No `OnParticleCollision`** message receiver. Detect collisions on the GPU
  (e.g., Conform to Plane) but you can't get a callback per-particle in C#.
- Older mobile GPUs may not run VFX Graph well; profile early.
- Built-in Render Pipeline support is limited; URP/HDRP is the home.

## When VFX Graph Wins

- Magic spells, environmental particles by the millions
- Detail-rich cinematic effects
- Stylised shaders that need Shader Graph integration
- Boss arena ambiance with dense effects
- Anything where Particle System's CPU bound is your bottleneck

## When to Stay on Particle System

- Particles must collide with Rigidbody / scene colliders
- Targeting low-end mobile / web
- Effect is simple (a single fire, a smoke puff)
- Team is unfamiliar with node graphs and time pressure is real

## When to Use This File

- Deciding whether to migrate an effect to VFX Graph
- Wiring a graph and forgetting which block goes in which context
- Triggering a graph from script (events, properties)
- Reading an existing graph someone else built
