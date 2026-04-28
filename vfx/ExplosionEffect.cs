// =============================================================================
// ExplosionEffect.cs — Multi-system explosion driver
// =============================================================================
// A real-feeling explosion is several Particle Systems stacked under one
// parent. This script lives on the parent prefab and:
//   - Plays all child Particle Systems together
//   - Self-destructs the prefab after the longest-living system finishes
//
// Use this when:
//   - The prefab has 2+ Particle Systems (flash, fireball, smoke, debris)
//   - You can't rely on a single root Stop Action: Destroy because children
//     have different durations
//
// PREFAB STRUCTURE EXAMPLE:
//   ExplosionPrefab (this script + Transform)
//   ├── Flash      (Particle System)  - 0.1s
//   ├── Fireball   (Particle System)  - 0.5s
//   ├── Smoke      (Particle System)  - 3.0s   ← longest, drives lifetime
//   ├── Debris     (Particle System)  - 1.5s
//   └── Sparks     (Particle System)  - 0.7s
//
// The whole prefab self-cleans after ~3 seconds (Smoke's lifetime).
// =============================================================================

using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    [Header("Optional References")]
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private float audioVolume = 1f;

    [Header("Camera Shake (optional)")]
    [SerializeField] private float shakeStrength = 0f;

    private ParticleSystem[] systems;

    void Awake()
    {
        systems = GetComponentsInChildren<ParticleSystem>(includeInactive: true);
    }

    void OnEnable()
    {
        // Reset and replay every system from frame 0
        foreach (var ps in systems)
        {
            ps.Clear();
            ps.Play(withChildren: true);
        }

        // Audio (independent of VFX lifecycle)
        if (explosionSound != null)
        {
            // Goes through audio/SFXPlayer.cs if present in your project,
            // or AudioSource.PlayClipAtPoint as a fallback.
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, audioVolume);
        }

        // Camera shake (Cinemachine Impulse if present in project)
        // Implementation hook — see cinemachine/03-Extensions.md (Impulse)

        // Schedule destruction after the longest-living system finishes
        float lifetime = ComputeMaxLifetime();
        Destroy(gameObject, lifetime);
    }

    private float ComputeMaxLifetime()
    {
        float max = 0f;
        foreach (var ps in systems)
        {
            var main = ps.main;
            float lifetime = main.duration + main.startLifetime.constantMax;
            if (lifetime > max) max = lifetime;
        }
        return Mathf.Max(0.5f, max);
    }
}

// =============================================================================
// PREFAB ASSEMBLY:
//
// 1. Empty parent GO → name "ExplosionPrefab"
//    - Add this script
//    - (Optional) Assign explosionSound clip
//
// 2. Children, each a Particle System:
//    a. Flash    (very short, 1-2 huge bright particles)
//    b. Fireball (orange burst, grows then shrinks)
//    c. Smoke    (gray, slow rise, longest lived) ← drives parent lifetime
//    d. Debris   (mesh particles, gravity, collision planes)
//    e. Sparks   (stretched billboard, fast outward)
//
// 3. On EACH child:
//    - Looping: OFF
//    - Play On Awake: OFF (this script calls Play)
//
// 4. Save as Prefab. Drag into VFXManager's explosionPrefab field.
//
// USAGE:
//
//   VFXManager.Instance.PlayExplosion(transform.position);
//
// PERFORMANCE NOTE:
//   For frequent explosions, register the prefab in VFXPool. The pool calls
//   Play on the same instances rather than Instantiate-Destroy each time.
//
// =============================================================================
