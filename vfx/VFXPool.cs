// =============================================================================
// VFXPool.cs — Pooled ParticleSystem prefabs
// =============================================================================
// Avoids the allocation cost of Instantiate/Destroy for high-frequency VFX
// (gunshots, footstep dust, hit sparks). Pre-spawns N copies per registered
// prefab and reuses them.
//
// Mirrors AudioPool.cs in the audio/ folder.
//
// USE FOR:
//   - Effects firing more than ~5 times per second
//   - Mobile / low-end performance targets
//   - Any prefab where the cost of Instantiate becomes visible in Profiler
//
// FOR ONE-OFF EFFECTS (boss death, scripted explosions), Instantiate via
// VFXManager.Spawn is fine.
// =============================================================================

using System.Collections.Generic;
using UnityEngine;

public class VFXPool : MonoBehaviour
{
    [System.Serializable]
    public class PooledPrefab
    {
        public GameObject prefab;
        public int initialCount = 8;
    }

    [SerializeField] private List<PooledPrefab> pooledPrefabs = new();

    // Map from original prefab → list of cached instances
    private readonly Dictionary<GameObject, Queue<GameObject>> pools = new();

    void Awake()
    {
        foreach (var entry in pooledPrefabs)
        {
            if (entry.prefab == null) continue;
            var queue = new Queue<GameObject>(entry.initialCount);

            for (int i = 0; i < entry.initialCount; i++)
            {
                queue.Enqueue(CreateInstance(entry.prefab));
            }
            pools[entry.prefab] = queue;
        }
    }

    // =========================================================================
    // PUBLIC API
    // =========================================================================

    public bool TryPlay(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (prefab == null) return false;
        if (!pools.TryGetValue(prefab, out var queue))
        {
            // Prefab wasn't pre-registered — caller can fall back to Instantiate
            return false;
        }

        GameObject instance;
        if (queue.Count > 0)
        {
            instance = queue.Dequeue();
        }
        else
        {
            // Pool exhausted — grow rather than drop, since VFX cutoff is jarring
            instance = CreateInstance(prefab);
        }

        instance.transform.SetParent(parent, worldPositionStays: false);
        instance.transform.position = position;
        instance.transform.rotation = rotation;
        instance.SetActive(true);

        // Restart any ParticleSystems on the instance from frame 0
        foreach (var ps in instance.GetComponentsInChildren<ParticleSystem>(includeInactive: true))
        {
            ps.Clear();
            ps.Play(withChildren: true);
        }

        // Auto-return to pool after the longest particle system finishes
        float lifetime = ComputeMaxLifetime(instance);
        StartCoroutine(ReturnAfter(instance, prefab, lifetime));

        return true;
    }

    // =========================================================================
    // INTERNALS
    // =========================================================================

    private GameObject CreateInstance(GameObject prefab)
    {
        var go = Instantiate(prefab, transform);
        go.SetActive(false);
        return go;
    }

    private static float ComputeMaxLifetime(GameObject go)
    {
        float max = 0f;
        foreach (var ps in go.GetComponentsInChildren<ParticleSystem>(includeInactive: true))
        {
            var main = ps.main;
            float candidate = main.duration + main.startLifetime.constantMax;
            if (candidate > max) max = candidate;
        }
        return Mathf.Max(0.5f, max);
    }

    private System.Collections.IEnumerator ReturnAfter(GameObject instance, GameObject prefab, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Return(instance, prefab);
    }

    private void Return(GameObject instance, GameObject prefab)
    {
        if (instance == null) return;
        instance.SetActive(false);
        instance.transform.SetParent(transform, worldPositionStays: false);

        if (!pools.TryGetValue(prefab, out var queue))
        {
            queue = new Queue<GameObject>();
            pools[prefab] = queue;
        }
        queue.Enqueue(instance);
    }
}

// =============================================================================
// SETUP:
//
// 1. Add this component on the same GO as VFXManager (or a child).
//
// 2. In Inspector → Pooled Prefabs → add an entry per high-frequency prefab:
//    - Prefab: e.g., HitImpactPrefab
//    - Initial Count: 8–32 depending on rate of fire
//
// 3. From VFXManager.Spawn, the pool is consulted first. If the prefab isn't
//    registered here, VFXManager falls back to Instantiate.
//
// SIZING:
//   - Footstep dust at 5/sec, lifetime 0.5s → ~3 simultaneous → pool 4–8
//   - Gunshot impact at 20/sec, lifetime 0.5s → ~10 simultaneous → pool 16
//   - Bullet trails (long-lived) → pool generously, 32+
//
// LIFETIME:
//   ComputeMaxLifetime auto-detects the max (Duration + Start Lifetime). If
//   your effect has trailing sub-emitters that outlive the parent, increase
//   the parent's Duration so this calc covers them.
//
// =============================================================================
