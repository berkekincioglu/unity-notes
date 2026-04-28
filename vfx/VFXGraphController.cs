// =============================================================================
// VFXGraphController.cs — Drive a VisualEffect (VFX Graph) from script
// =============================================================================
// VFX Graph is triggered by Events and parameterized by Properties exposed in
// the graph's Blackboard. This script wraps both behind a small typed API.
//
// REQUIRED ON THE SAME GO:
//   - VisualEffect component with a graph asset assigned
//
// REQUIRED IN THE GRAPH:
//   - An Event node named "Explode" (or whatever name you want to trigger)
//   - Exposed properties: ExplosionRadius (float), Origin (Vector3), TintColor (Color)
//     — names must match the strings used in Set* calls
//
// PACKAGE:
//   Window → Package Manager → Visual Effect Graph → Install
// =============================================================================

using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class VFXGraphController : MonoBehaviour
{
    [Header("Defaults")]
    [SerializeField] private float defaultRadius = 3f;
    [SerializeField] private Color defaultTint = Color.white;

    [Header("Property Names (must match graph Blackboard)")]
    [SerializeField] private string radiusProperty = "ExplosionRadius";
    [SerializeField] private string originProperty = "Origin";
    [SerializeField] private string tintProperty   = "TintColor";

    [Header("Event Names (must match graph Event nodes)")]
    [SerializeField] private string explodeEvent = "Explode";

    private VisualEffect graph;

    void Awake()
    {
        graph = GetComponent<VisualEffect>();
    }

    // =========================================================================
    // PUBLIC API
    // =========================================================================

    public void TriggerExplosion(Vector3 origin, float radius = -1f, Color? tint = null)
    {
        if (graph == null) return;

        graph.SetVector3(originProperty, origin);
        graph.SetFloat(radiusProperty, radius > 0f ? radius : defaultRadius);
        graph.SetVector4(tintProperty, tint ?? defaultTint);

        graph.SendEvent(explodeEvent);
    }

    public void Play()
    {
        if (graph != null) graph.Play();   // built-in OnPlay event
    }

    public void Stop()
    {
        if (graph != null) graph.Stop();   // built-in OnStop event
    }

    public void Reset()
    {
        if (graph != null) graph.Reinit(); // wipe particles + state
    }

    // =========================================================================
    // PROPERTY HELPERS — generic passthroughs
    // =========================================================================

    public void SetFloat(string name, float value)   => graph?.SetFloat(name, value);
    public void SetVector3(string name, Vector3 v)   => graph?.SetVector3(name, v);
    public void SetColor(string name, Color c)       => graph?.SetVector4(name, c);
    public void SetTexture(string name, Texture t)   => graph?.SetTexture(name, t);
    public void SendEvent(string name)               => graph?.SendEvent(name);
}

// =============================================================================
// GRAPH SETUP:
//
// 1. Create a VFX Graph asset:
//    Project > Create > Visual Effects > Visual Effect Graph
//
// 2. Open the graph. Add to the Blackboard (left panel "+"):
//    - ExplosionRadius (Float, Exposed)
//    - Origin (Vector3, Exposed)
//    - TintColor (Color, Exposed)
//
// 3. Wire properties into the relevant blocks:
//    - Initialize Particle → Set Position : Sphere → connect Origin to Center
//    - Initialize Particle → Set Position : Sphere → connect ExplosionRadius
//    - Output Particle Quad → Set Color → connect TintColor
//
// 4. Add an Event context, name it "Explode".
//    Connect its output to a Spawn context that does a Burst (e.g., 5000 particles).
//
// 5. In the scene:
//    - Empty GO with Visual Effect component + this script
//    - Asset Template: drag your graph asset
//
// USAGE:
//
//   [SerializeField] private VFXGraphController magicVfx;
//
//   void OnSpellCast(Vector3 target)
//   {
//       magicVfx.TriggerExplosion(target, radius: 5f, tint: Color.cyan);
//   }
//
// PROPERTY HAS-CHECK (defensive):
//   Before SetFloat, you can guard with graph.HasFloat(name) if your graphs
//   are interchangeable and may not all have the same exposed properties.
//
// =============================================================================
