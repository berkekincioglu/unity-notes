// =============================================================================
// VFXManager.cs — Central VFX controller (Singleton)
// =============================================================================
// One manager per project. Spawns gameplay VFX prefabs through a pool, with
// a typed API so call sites don't reach for prefabs directly.
//
// Mirrors the AudioManager / InputManager singleton pattern in this repo.
//
// RESPONSIBILITIES:
//   - Spawn one-shot effects (explosions, hits, pickups) by ID
//   - Route through the VFXPool for high-frequency / repeated effects
//   - Persist across scene loads (DontDestroyOnLoad)
//
// SCENE REQUIREMENTS:
//   - One GameObject in the bootstrap scene with this component
//   - VFXPool component on the same or child GameObject, wired in Inspector
//   - Prefab references assigned per VFX type
// =============================================================================

using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance { get; private set; }

    [Header("Prefab References (one-shot effects)")]
    [SerializeField] private GameObject hitImpactPrefab;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject pickupSparklePrefab;
    [SerializeField] private GameObject muzzleFlashPrefab;

    [Header("Pool")]
    [SerializeField] private VFXPool pool;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // =========================================================================
    // ONE-SHOT EFFECT API
    // =========================================================================

    public void PlayHitImpact(Vector3 position, Vector3 normal)
    {
        Spawn(hitImpactPrefab, position, Quaternion.LookRotation(normal));
    }

    public void PlayExplosion(Vector3 position)
    {
        Spawn(explosionPrefab, position, Quaternion.identity);
    }

    public void PlayPickupSparkle(Vector3 position)
    {
        Spawn(pickupSparklePrefab, position, Quaternion.identity);
    }

    public void PlayMuzzleFlash(Transform muzzle)
    {
        // Muzzle flash usually attaches to the gun — different lifecycle
        Spawn(muzzleFlashPrefab, muzzle.position, muzzle.rotation, parent: muzzle);
    }

    // =========================================================================
    // GENERIC SPAWN — for one-off prefabs not in the typed list
    // =========================================================================

    public void Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (prefab == null) return;

        // Prefer pool for repeatable effects. Falls back to Instantiate when no
        // pooled instance is available or the prefab isn't pre-registered.
        if (pool != null && pool.TryPlay(prefab, position, rotation, parent))
            return;

        // Fallback: vanilla Instantiate. Prefab must have Stop Action: Destroy
        // on its root ParticleSystem to avoid leaking GameObjects.
        Instantiate(prefab, position, rotation, parent);
    }
}

// =============================================================================
// SETUP:
//
// 1. Create empty GameObject "VFXManager" in your bootstrap scene
// 2. Add this component
// 3. Add VFXPool component on a child or same GO; assign in Inspector
// 4. Build prefabs for each effect type:
//    - Particle System(s) parented under one GameObject
//    - Root ParticleSystem Main → Stop Action = Destroy
//    - Drag prefab into VFXManager's prefab slots
// 5. Pre-register prefabs you want pooled: VFXPool's pooledPrefabs list
//
// CALL SITES (typical):
//
//   void OnHit(RaycastHit hit)
//   {
//       VFXManager.Instance.PlayHitImpact(hit.point, hit.normal);
//   }
//
//   void OnDeath()
//   {
//       VFXManager.Instance.PlayExplosion(transform.position);
//   }
//
//   void OnPickup()
//   {
//       VFXManager.Instance.PlayPickupSparkle(transform.position);
//   }
// =============================================================================
