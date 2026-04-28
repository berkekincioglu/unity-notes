// =============================================================================
// HitEffectSpawner.cs — Surface-aware impact effect picker
// =============================================================================
// When a projectile / weapon hits something, the effect should match the
// surface: dust on dirt, sparks on metal, splash on water, blood on flesh.
//
// This is the VFX parallel to audio/FootstepPlayer.cs — both pick a
// material-aware variant on demand.
//
// USAGE:
//   - Attach to anything that fires (gun, magic staff, fist)
//   - On hit, call SpawnAt(point, normal, surfaceTag)
//   - The spawner picks the right prefab and forwards to VFXManager
//
// SURFACE DETECTION:
//   Tag colliders in your scene with a "SurfaceTag" component (your own tiny
//   MonoBehaviour with a string field). Read it from the RaycastHit.
// =============================================================================

using UnityEngine;

public class HitEffectSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SurfaceMapping
    {
        public string surfaceId;          // e.g., "metal", "wood", "flesh"
        public GameObject impactPrefab;
    }

    [Header("Surface → Prefab Mapping")]
    [SerializeField] private SurfaceMapping[] surfaces;

    [Header("Fallback")]
    [SerializeField] private GameObject defaultImpactPrefab;

    // =========================================================================
    // PUBLIC API
    // =========================================================================

    public void SpawnAt(Vector3 position, Vector3 normal, string surfaceId = null)
    {
        var prefab = ResolvePrefab(surfaceId);
        if (prefab == null) return;

        var rotation = Quaternion.LookRotation(normal);

        // Route through VFXManager so pooling is honored
        if (VFXManager.Instance != null)
        {
            VFXManager.Instance.Spawn(prefab, position, rotation);
        }
        else
        {
            Instantiate(prefab, position, rotation);
        }
    }

    public void SpawnFromHit(RaycastHit hit)
    {
        // Try to read a SurfaceTag component from the hit collider
        string surfaceId = null;
        var tag = hit.collider.GetComponent<SurfaceTag>();
        if (tag != null) surfaceId = tag.surfaceId;

        SpawnAt(hit.point, hit.normal, surfaceId);
    }

    // =========================================================================
    // INTERNALS
    // =========================================================================

    private GameObject ResolvePrefab(string surfaceId)
    {
        if (string.IsNullOrEmpty(surfaceId)) return defaultImpactPrefab;

        for (int i = 0; i < surfaces.Length; i++)
        {
            if (surfaces[i].surfaceId == surfaceId && surfaces[i].impactPrefab != null)
                return surfaces[i].impactPrefab;
        }
        return defaultImpactPrefab;
    }
}

// Minimal companion class — drop one of these on each "tagged" surface in
// the scene. Could also be a ScriptableObject if you want shared definitions.
public class SurfaceTag : MonoBehaviour
{
    public string surfaceId;
}

// =============================================================================
// PREFAB SETUP:
//
// 1. Build per-surface impact prefabs:
//    - HitFx_Metal    : yellow sparks, stretched billboard, short lifetime
//    - HitFx_Wood     : brown splinters, mesh particles, gravity
//    - HitFx_Flesh    : red mist + drops, world space
//    - HitFx_Concrete : gray dust burst
//    - HitFx_Water    : splash + drops sub-emitter
//
// 2. Each prefab uses a Hemisphere shape so particles emit AWAY from the surface
//    (orientation comes from the LookRotation(normal) at spawn).
//
// 3. Register prefabs in this component's Inspector array.
//
// 4. Tag colliders in the scene:
//    - Add SurfaceTag component
//    - Set surfaceId to "metal" / "wood" / "flesh" etc.
//
// USAGE FROM A WEAPON:
//
//   void Fire()
//   {
//       if (Physics.Raycast(muzzle.position, muzzle.forward, out var hit, 100f))
//       {
//           hitEffectSpawner.SpawnFromHit(hit);
//           target.TakeDamage(damage);
//       }
//   }
//
// =============================================================================
