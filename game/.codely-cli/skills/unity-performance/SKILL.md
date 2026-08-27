---
name: unity-performance
description: Unity performance optimization expert — Profiler, memory management, GC optimization, draw call batching, LOD, object pooling, texture compression, GPU Instancing, SRP Batcher. Use when analyzing performance bottlenecks, optimizing rendering pipelines, reducing GC pressure, optimizing mobile frame rates, or preparing for release.
---

# Unity Performance Optimization

## Profiler

```
Window > Analysis > Profiler
```

Key modules:
- **CPU Usage**: Main thread time per frame. Look for spikes > 16.6ms (60fps target).
- **GPU Usage**: Render time. Check overdraw, shader complexity.
- **Memory**: Total/GC/Texture/Mesh memory. Look for leaks.
- **Rendering**: Draw calls, batches, triangles, vertices.

**Profiling tips:**
- Always profile on target device (mobile ≠ editor performance).
- Deep Profile (expensive) for per-function breakdown.
- `Profiler.BeginSample("MyCode")` / `EndSample()` for custom markers.

## Draw Call Reduction

| Technique | Effect |
|-----------|--------|
| Static/Dynamic Batching | Combine small meshes (≤300 verts for static) |
| GPU Instancing | Same mesh+material, different transforms |
| SRP Batcher | URP/HDRP: persist CBUFFER across draw calls |
| Atlas Textures | Combine sprites → fewer materials |
| Mesh.CombineMeshes | Manual batching for static geometry |

**Priority:** SRP Batcher > GPU Instancing > Dynamic Batching > Static Batching.

## GC Optimization

```csharp
// BAD — allocates every frame
void Update()
{
    foreach (var enemy in GameObject.FindObjectsOfType<Enemy>()) // GC alloc!
        enemy.Move();
    var pos = transform.position + new Vector3(1, 0, 0); // struct, OK
}

// GOOD — cache and reuse
private List<Enemy> _enemies = new();
void Start() { _enemies = FindObjectsOfType<Enemy>().ToList(); }
void Update()
{
    foreach (var enemy in _enemies) enemy.Move(); // no alloc
}

// StringBuilder for string concat
var sb = new StringBuilder(256);
sb.Append("Score: ").Append(score);
text.text = sb.ToString();
```

**Common GC sources:**
- `foreach` on non-struct collections → use `for` with index
- `string` concatenation → `StringBuilder`
- `new List<T>()` in Update → cache and `.Clear()`
- `GetComponent` in Update → cache in Start
- LINQ → manual loops
- `Camera.main` → cache reference
- `yield return new WaitForEndOfFrame()` → cache the yield instruction

## Object Pooling

```csharp
public class Pool<T> where T : Component
{
    private readonly T _prefab;
    private readonly Queue<T> _pool = new();
    private readonly Transform _parent;

    public Pool(T prefab, int initialSize, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;
        for (int i = 0; i < initialSize; i++)
        {
            var obj = Object.Instantiate(prefab, _parent);
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }
    }

    public T Get()
    {
        if (_pool.Count > 0)
        {
            var obj = _pool.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }
        return Object.Instantiate(_prefab, _parent);
    }

    public void Release(T obj)
    {
        obj.gameObject.SetActive(false);
        _pool.Enqueue(obj);
    }
}
```

## Memory Management

```csharp
// Force unload unused assets (loading screen, scene transition)
Resources.UnloadUnusedAssets();
GC.Collect(); // Only when safe (not during gameplay)

// Texture memory — the #1 memory consumer on mobile
// Compress: ASTC for mobile (better than ETC2), BC7 for desktop
// Max size: 2048 on mobile, 4096 on desktop
// Mipmaps: ON for 3D, OFF for UI

// Audio: use Streaming for long clips, DecompressOnLoad for short clips
```

## LOD & Culling

```
LOD 0: 100% detail (close, <10m)
LOD 1: 50% detail (medium, 10-30m)
LOD 2: 25% detail (far, 30-100m)
Culled: beyond 100m
```

- Use `LOD Group` component on prefabs.
- Set up `Occlusion Culling` (Window > Rendering > Occlusion Culling > Bake).
- Frustum culling is automatic.

## Mobile-Specific

1. **Texture compression**: ASTC 6x6 (quality) or 4x4 (speed).
2. **Vertex count**: <100k total visible per frame.
3. **Shader**: Use URP Simple Lit or Unlit on mobile. Avoid complex Surface Shaders.
4. **Overdraw**: Check in Scene View (Overdraw draw mode). Reduce transparent particles.
5. **Fill rate**: Reduce post-processing on mobile. Keep RTs at device resolution.
6. **Physics**: Fixed Timestep = 0.02 (50fps). Increase if needed. Use Layer Collision Matrix.

## Key Gotchas

1. Editor performance ≠ device performance. Always profile on target.
2. `Camera.main` wraps `FindObjectWithTag` — cache it.
3. `Instantiate`/`Destroy` causes GC spikes — use object pooling for frequent spawns.
4. `Debug.Log` is expensive in builds — wrap with `#if UNITY_EDITOR` or use conditional `[Conditional("UNITY_EDITOR")]`.
5. `transform.position` getter allocates nothing (it's a property returning a struct). But `transform.position.x = 5` is invalid — must do `transform.position = new Vector3(5, y, z)`.
6. SRP Batcher requires shader compatibility (CBUFFER_START/END). Check `Frame Debugger`.
