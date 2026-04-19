# Audio Effects and Filters

Unity has two ways to color sound: **Audio Reverb Zones** (environmental, spatial)
and **Audio Filter components** attached to an AudioSource (per-source, direct).
Plus, any filter can live on the AudioMixer for global processing — see
[04-AudioMixer.md](04-AudioMixer.md).

## Audio Reverb Zone

A reverb zone is an invisible bubble in space. When the AudioListener enters it,
everything audible gets colored with the zone's reverb.

**Setup:**
```
Hierarchy → right-click → Audio → Audio Reverb Zone
```

| Property         | What it does                                              |
|------------------|-----------------------------------------------------------|
| Min Distance     | Inside this radius, reverb is at 100%                     |
| Max Distance     | Past this, no reverb. Between min and max, it blends.     |
| Reverb Preset    | Picks character (Cave, Arena, Hangar, etc.)               |

```
                  outer zone edge (Max)
         ╱─────────────────────────╲
       ╱     blending zone           ╲
      │  ╱──────────────╲             │
      │ │ full reverb    │            │
      │ │   (Min)        │            │
      │  ╲──────────────╱             │
       ╲                             ╱
         ╲─────────────────────────╱

When Listener crosses Max → reverb starts fading in
When Listener crosses Min → reverb at 100%
```

## Reverb Presets

| Preset          | Character                       | Good for                        |
|-----------------|---------------------------------|---------------------------------|
| Off             | Dry                             | Open air                        |
| Generic         | Light echo                      | Default indoor                  |
| Padded Cell     | Very damp, no echo              | Studio, isolation room          |
| Room            | Small reflections               | Bedroom                         |
| Bathroom        | Hard tiled reflections          | Tile bathroom                   |
| Livingroom      | Mid reflections                 | Carpeted lounge                 |
| Stone Room      | Crisp, sustained                | Stone interiors                 |
| Auditorium      | Concert hall                    | Theater, opera                  |
| Concert Hall    | Large, airy                     | Symphony hall                   |
| Cave            | Long, dark tail                 | Caves, mines                    |
| Arena           | Huge open interior              | Stadium, arena                  |
| Hangar          | Very long tail, huge volume     | Aircraft hangars, warehouses    |
| Carpeted Hallway| Dampened long space             | Hotel hallway                   |
| Hallway         | Crisp long space                | School hallway                  |
| Stone Corridor  | Sharp and long                  | Castle corridor                 |
| Alley           | Tight outdoor reflections       | Urban alley                     |
| Forest          | Dampened outdoor                | Forest                          |
| City            | Mild ambient reflection         | City street                     |
| Mountains       | Distant reflections             | Mountain valley                 |
| Quarry          | Wide stone echo                 | Open quarry                     |
| Plain           | Open flat ground                | Desert, plains                  |
| Parking Lot     | Flat concrete                   | Garage, parking structure       |
| Sewer Pipe      | Tube resonance                  | Sewers, pipes                   |
| Underwater      | Muffled, filtered               | Swimming, aquarium              |
| User            | Custom manual settings          | Hand-tuned                      |

## Audio Filter Components

These are added to an AudioSource (or to the AudioListener for global effects).
Process order follows component order in the Inspector.

```
AudioSource
  └─ Clip played
     └─ Low Pass Filter?
        └─ High Pass Filter?
           └─ Echo Filter?
              └─ Distortion Filter?
                 └─ Chorus Filter?
                    └─ Reverb Filter?
                       → mixed into scene → AudioListener
```

| Filter             | Effect                           | Common uses                              |
|--------------------|----------------------------------|------------------------------------------|
| **Low Pass Filter**| Cut high frequencies             | Underwater, behind walls, distant sounds |
| **High Pass Filter**| Cut low frequencies             | Telephone / radio voice effect           |
| **Echo Filter**    | Delay + repeat                   | Deep wells, caves, chant-like effect     |
| **Distortion Filter**| Drive / crush                  | Radios, damaged speakers, horror stings  |
| **Chorus Filter**  | Layer slightly detuned copies    | Rich pads, choirs, magical effects       |
| **Reverb Filter**  | Apply reverb per-source (not zone)| Individual dialog reverb without a zone |

### Filter Gotchas

- Applying a filter to the AudioSource colors **that source only**. Applying to
  the AudioListener (same filter types work there) colors **everything**.
- Filters use CPU. Don't put a Reverb Filter on 50 sources. Prefer a single
  Audio Reverb Zone or route through an AudioMixer effect instead.
- Runtime-toggle filters by setting `enabled = false` on the component.

## Which to Use

```
"I want a cave to echo when the player enters."
    → Audio Reverb Zone with Cave preset, positioned in the cave.

"I want the gunshot SFX to sound distorted (damaged speaker)."
    → Distortion Filter on the gunshot AudioSource.

"I want EVERYTHING muffled when the player is underwater."
    → Low Pass Filter on the AudioListener (enable/disable by script).

"I want all SFX to sit in a slight room reverb."
    → AudioMixer group with a Reverb effect — see 04-AudioMixer.md.
```

## When to Use This

- Bringing spatial character to interior spaces (Reverb Zones)
- Shaping individual sources for storytelling (filters)
- Making a "global" effect via AudioListener filter (e.g., underwater toggle)
- Deciding between zone / filter / mixer — pick the broadest level that works
