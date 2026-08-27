---
name: unity-vfx-graph
description: Unity VFX Graph visual effects system expert — GPU particles, point caches, subgraphs, events, output contexts, property binding, HLSL custom nodes. Use when creating complex particle effects (fire, explosions, rain, magic), writing custom VFX nodes, optimizing GPU particle performance, or binding VFX properties to gameplay scripts.
---

# Unity VFX Graph

## Overview

VFX Graph uses GPU-side simulation for massive particle counts (millions). Replaces Particle System for next-gen effects. Requires compute shader support (not on old mobile).

## Contexts (The Pipeline)

| Context | Role | Executes |
|---------|------|----------|
| **Spawn** | Controls emission rate/burst | CPU |
| **Initialize** | Sets up particle birth state | GPU |
| **Update** | Per-frame simulation (forces, collisions) | GPU |
| **Output** | Renders particles (Quad/Mesh/Line) | GPU |

Flow: Spawn → Initialize → [Update → Output]

## Core Workflow

1. Create VFX Graph asset: `Create > Visual Effects > Visual Effect Graph`.
2. Open VFX Graph editor (`Window > Visual Effects > Visual Effect Graph`).
3. Connect blocks in context blocks.
4. Assign to `VisualEffect` component on a GameObject.

## Properties & Exposition

```csharp
// Expose properties to C# scripts (check "Exposed" in VFX editor)
vfx.SetFloat("EmissionRate", 500f);
vfx.SetVector3("SpawnPosition", transform.position);
vfx.SetBool("IsBurning", true);
vfx.SetGradient("ColorOverLife", gradient);
vfx.SetTexture("NoiseTex", texture);

// Get back
float rate = vfx.GetFloat("EmissionRate");

// Events — trigger burst from C#
vfx.SendEvent("OnExplode");
```

## Common Blocks

**Initialize:** Set custom position (sphere/cone), Set lifetime, Set velocity.

**Update:** Force (gravity/wind), Turbulence, Collision (sphere/plane), Kill (by age/bounds).

**Output (Quad):** Set color over life, Set size over life, Set alpha over life.

## Subgraphs

Reusable node groups. Create: `Create > Visual Effects > Subgraph Block`.
- Drag `.vfxblock` into any context.
- Good for: damage numbers, spawn patterns, color gradients shared across effects.

## Point Cache (PCache)

For complex meshes/sculpted shapes:
1. Bake point data: `VFX > Point Cache Bake Tool`.
2. Set as Position source in Initialize.
3. Each particle spawns at a vertex point.

## HLSL Custom Nodes

```csharp
// In VFX Graph, add "Custom Function" node
// Mode: String → write HLSL inline
float3 swirl(float3 pos, float strength, float time)
{
    float angle = time * strength;
    float s = sin(angle), c = cos(angle);
    return float3(pos.x * c - pos.z * s, pos.y, pos.x * s + pos.z * c);
}
```

## Performance Optimization

1. **Capacity**: Set in Spawn context. Too high = wasted GPU memory. Profile with VFX Editor profiler.
2. **Output strips**: Use `Output Particle Strips` for trails/ribbons (cheaper than line output).
3. **Lod VFX**: Different VFX assets per LOD distance. Swap via script.
4. **Kill bounds**: Always set Kill block — particles that fly away forever waste GPU.
5. **Avoid Update collisions** for dense effects — use rough approximations instead.
6. **Batch**: One VFX with 100k particles > 100 VFX with 1k particles. Merge similar effects.

## Key Gotchas

1. VFX Graph requires compute shaders — won't work on old OpenGL ES 2.0 devices.
2. `VisualEffect` component does NOT auto-play. Call `vfx.Play()` or check `Play On Awake`.
3. Property names are case-sensitive. Must be marked "Exposed" in the VFX editor.
4. `SendEvent` with no associated Spawn burst = nothing happens. Configure event in Spawn context.
5. VFX Graph and Particle System are separate systems. Can't mix them in one component.
6. Mesh output requires the mesh to be readable (Read/Write Enabled in import settings).
