---
name: unity-animation
description: Unity Animation system expert — Animator Controller, Animation Clips, Blend Trees, animation layers, IK, animation events, Timeline, Playable API. Use when creating animation state machines, configuring blend trees, scripting animation control, building Timeline cutscenes, or using Playable API for dynamic animation.
---

# Unity Animation & Timeline

## Animator Controller

State machine with states (clips), transitions (conditions), parameters.

```csharp
// Set parameters
animator.SetBool("IsRunning", true);
animator.SetTrigger("Attack");
animator.SetFloat("Speed", 5.5f);
animator.SetInteger("ComboStep", 2);

// Get current state info
AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
if (state.IsName("Base.Locomotion.Run"))
    Debug.Log("Running");
```

## Transitions & Conditions

- **Has Exit Time**: Waits for clip to finish before transitioning. Off = instant on condition.
- **Transition Duration**: Blend time between states (seconds).
- **Interruption Source**: Which transitions can interrupt this one.
- **Solo/Mute**: Debugging tools for testing transitions.

## Blend Trees

For blending animations by parameter (e.g., speed → idle/walk/run).

```csharp
// 1D Blend Tree: Speed parameter blends Idle→Walk→Run
// 2D Blend Tree: Speed + Direction (strafe) = 9-way locomotion

// Set blend values
animator.SetFloat("Speed", currentSpeed);
animator.SetFloat("Direction", inputDirection); // -1 to 1
```

**Threshold** = parameter value where clip is at full weight. **Motion** = the clip.

## Animation Layers

For playing different animations on different body parts.

```
Layer 0: Full Body (locomotion)
Layer 1: Upper Body (attack/aim)  ← Mask: UpperBody
Layer 2: Additive (breathing)      ← Blending: Additive
```

```csharp
// Layer weight (0 = off, 1 = full override)
animator.SetLayerWeight(1, 0.8f); // Upper body at 80%
```

## Animation Events

```csharp
// In Animation Clip (add event at specific frame):
// Function name → called at that frame
public void OnAttackHit()
{
    // Deal damage at the frame the weapon connects
    DealDamage();
}

public void OnFootstep(AnimationEvent animEvent)
{
    // animEvent.floatParameter, intParameter, stringParameter
    PlayFootstepSound();
}
```

## IK (Inverse Kinematics)

```csharp
void OnAnimatorIK(int layerIndex)
{
    // Set IK position/weight for left hand (0 = off, 1 = full)
    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
    animator.SetIKPosition(AvatarIKGoal.LeftHand, targetHandPos);
    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
    animator.SetIKRotation(AvatarIKGoal.LeftHand, targetHandRot);

    // Look IK (head turns toward target)
    animator.SetLookAtWeight(0.5f);
    animator.SetLookAtPosition(lookTarget);
}
```

Requires `IK Pass` checked on the animation layer.

## Timeline

Cinematic sequencing tool. Create: `Create > Timeline`.

```csharp
// Play Timeline via PlayableDirector
var director = GetComponent<PlayableDirector>();
director.Play();

// Wait for timeline to finish
yield return new WaitUntil(() => director.state == PlayState.Paused);

// Set time
director.time = 5.5;
```

**Track types:** Activation (show/hide objects), Animation, Audio, Signal, Control (nested timelines), Playable.

## Playable API (Dynamic Animation)

```csharp
// Mix two clips at runtime
var graph = PlayableGraph.Create();
var mixer = AnimationPlayableOutput.Create(graph, "Mixer", animator).GetPlayable();

var clipA = AnimationClipPlayable.Create(graph, idleClip);
var clipB = AnimationClipPlayable.Create(graph, runClip);

clipA.SetInputWeight(0, 0.5f);
clipB.SetInputWeight(0, 0.5f);

graph.Play();
```

## Key Gotchas

1. `SetTrigger` auto-resets after consumption. `SetBool` stays until changed.
2. Cross-fade time: 0.1-0.25s is typical. Too long = sluggish, too short = poppy.
3. `Apply Root Motion`: On = animation drives character position (good for organic movement). Off = script controls position.
4. Blend Tree thresholds must be sorted (0, 0.5, 1.0) — unsorted = broken blending.
5. Timeline `Signal` = AnimationEvent equivalent. Create SignalEmitter + SignalReceiver.
6. `Animator.StringToHash("StateName")` cached once → use `IsName(hash)` for faster state checks.
7. Playable API is more flexible but more verbose — use for dynamic blending (e.g., aim while walking).
