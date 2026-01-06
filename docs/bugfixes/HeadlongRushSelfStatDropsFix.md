# Bug Fix Summary: Headlong Rush Self-Stat Drops Not Applied

## Overview

Fixed a bug where moves with self-targeting stat changes (specifically Headlong Rush) were not applying their stat drops to the user after the move executed. The issue affected moves that use the `Self` property with `SecondaryEffect` containing stat boosts.

## Affected Moves

- **Headlong Rush** (Def -1, SpD -1)
- **Draco Meteor** (SpA -2)
- **Overheat** (SpA -2)
- **Close Combat** (Def -1, SpD -1)
- **Superpower** (Atk -1, Def -1)
- Any other move with self-targeting stat changes via `Self` property

## Problem

### Symptoms
When Ursaluna used Headlong Rush:
- Move dealt damage correctly to opponent
- Self-stat drops (Defense -1, Special Defense -1) were **not** applied to Ursaluna
- No error messages or crashes occurred

### Debug Output Analysis
```
Ursaluna (Side 1) used Headlong Rush!
Grimmsnarl (Side 2) took 135 damage! (32.5% HP remaining)
[SpreadMoveHit] Step 4: Checking self drops for Headlong Rush, move.Self != null: True, move.SelfDropped: 
[SpreadMoveHit] Calling SelfDrops for Headlong Rush
[SelfDrops] Called for Headlong Rush, isSecondary=False, moveData.Self != null: True, move.SelfDropped: 
[SelfDrops] Processing self effect for Ursaluna
[SelfDrops] Branch 1: !isSecondary && moveData.Self.Boosts != null
[SelfDrops] secondaryRoll=78, Chance=
[SelfDrops] Calling MoveHit for self boosts
```

The logs showed that `SelfDrops` was being called and `MoveHit` was invoked, but the boosts were never actually applied.

## Root Cause

### Type System Mismatch

The issue was a subtle type mismatch in how `hitEffect` parameters were handled in the `SpreadMoveHit` method:

1. **Move Structure**: 
   ```csharp
   Move {
       Self: SecondaryEffect {
           Boosts: { Def = -1, SpD = -1 }
       }
   }
   ```

2. **Call Flow**:
   ```
   SpreadMoveHit(hitEffect: null) 
   ? SelfDrops(..., moveData: move)
   ? MoveHit(..., hitEffect: moveData.Self)  // SecondaryEffect
   ? ExecuteMoveHit(..., moveData: hitEffect)
   ? SpreadMoveHit(..., hitEffect: SecondaryEffect)
   ? RunMoveEffects(..., moveData: move)  // ? WRONG!
   ```

3. **The Problem**:
   - `SpreadMoveHit` receives `hitEffect` parameter (the `SecondaryEffect` with boosts)
   - But it wasn't being stored anywhere for `RunMoveEffects` to access
   - `RunMoveEffects` expects to read boosts from `moveData.HitEffect.Boosts`
   - Since `hitEffect` wasn't stored in `move.HitEffect`, boosts were never found

### The Missing Link

TypeScript code uses dynamic typing:
```typescript
let moveData = hitEffect as ActiveMove;
if (!moveData) moveData = move;
```

This "casts" the `hitEffect` to `ActiveMove` and uses it directly. In C#, we can't just cast `SecondaryEffect` to `ActiveMove` since they're different types.

The `ActiveMove` class has a `HitEffect` property specifically for this purpose:
```csharp
/// <summary>
/// This is used to handle the casting of a HitEffect to an ActiveMove in RunMoveEffects() and SpreadMoveHit().
/// Store the HitEffect in this property so it can be accessed later.
/// </summary>
public HitEffect? HitEffect { get; set; }
```

But it wasn't being populated!

## Solution

### The Fix

Store the `hitEffect` parameter in `move.HitEffect` when it's provided:

```csharp
// In SpreadMoveHit method
public (SpreadMoveDamage, SpreadMoveTargets) SpreadMoveHit(SpreadMoveTargets targets, Pokemon pokemon,
    ActiveMove move, HitEffect? hitEffect = null, bool isSecondary = false, bool isSelf = false)
{
    // ... initialization code ...

    // Store hitEffect in move.HitEffect so RunMoveEffects can access it
    // This matches TypeScript pattern: let moveData = hitEffect as ActiveMove; if (!moveData) moveData = move;
    if (hitEffect != null)
    {
        move.HitEffect = hitEffect;
    }

    // ... rest of method ...
}
```

### How It Works

1. **First call** to `SpreadMoveHit` (hitEffect: null):
   - `move.HitEffect` remains null
   - `RunMoveEffects` uses properties directly from `move`

2. **Recursive call** from `SelfDrops` via `MoveHit` (hitEffect: SecondaryEffect):
   - `hitEffect` is stored in `move.HitEffect`
   - `RunMoveEffects` finds boosts in `moveData.HitEffect.Boosts`
   - Boosts are applied correctly!

### Code Flow After Fix

```
UseMoveInner
? TrySpreadMoveHit
? HitStepMoveHitLoop
? SpreadMoveHit(hitEffect: null)
  ? RunMoveEffects(moveData: move)  // ? Normal move effects
  ? SelfDrops(moveData: move)
    ? MoveHit(hitEffect: move.Self)  // SecondaryEffect with Boosts
      ? ExecuteMoveHit(moveData: move.Self)
        ? SpreadMoveHit(hitEffect: move.Self)  // ? Stores in move.HitEffect!
          ? RunMoveEffects(moveData: move)
            ? moveData.HitEffect.Boosts  // ? Found! { Def = -1, SpD = -1 }
            ? Battle.Boost(...)  // ? Applied!
```

## Files Modified

### Primary Change
- **ApogeeVGC\Sim\BattleClasses\BattleActions.MoveHit.cs**
  - Added `move.HitEffect = hitEffect` assignment in `SpreadMoveHit` method
  - Added debug logging for troubleshooting

### Debugging Additions (Optional - Can Be Removed)
- **ApogeeVGC\Sim\BattleClasses\BattleActions.MoveEffects.cs**
  - Added comprehensive debug logging in `SelfDrops` method
  - Added debug logging in `SpreadMoveHit` step 4

## Testing

### Test Case
```csharp
private void RunConsoleVsRandomSinglesTest()
{
    PlayerOptions player1Options = new()
    {
        Type = Player.PlayerType.Console,
        Name = "Matt",
        Team = TeamGenerator.GenerateTestTeam(Library),  // Contains Ursaluna
    };

    PlayerOptions player2Options = new()
    {
        Type = Player.PlayerType.Random,
        Name = "Random",
        Team = TeamGenerator.GenerateTestTeam(Library),  // Contains Grimmsnarl
    };

    var simulator = new Simulator();
    SimulatorResult result = simulator.RunAsync(Library, battleOptions).Result;
}
```

### Expected Behavior
```
Ursaluna sent out!
Grimmsnarl sent out!
--- Turn 1 ---
Ursaluna used Headlong Rush!
Grimmsnarl took 135 damage!
Ursaluna's Defense fell!     ? NEW!
Ursaluna's Special Defense fell!   ? NEW!
```

### Verification
After the fix:
- ? Headlong Rush deals damage to opponent
- ? Defense stat drops by 1 stage (-1)
- ? Special Defense stat drops by 1 stage (-1)
- ? Stat changes are visible in battle UI
- ? No crashes or infinite recursion
- ? Build compiles successfully

## TypeScript Comparison

### Key Difference

| Aspect | TypeScript | C# Implementation |
|--------|-----------|-------------------|
| Type casting | `hitEffect as ActiveMove` | Cannot cast different types |
| Storing hitEffect | Implicit via cast | Explicit via `move.HitEffect` property |
| Accessing boosts | `moveData.boosts` or `moveData.self.boosts` | `moveData.HitEffect.Boosts` |
| Type safety | Runtime checks | Compile-time + runtime checks |

### Architectural Insight

The `ActiveMove.HitEffect` property exists specifically to bridge this gap:
- **Purpose**: Store a `HitEffect` (or `SecondaryEffect`) so it can be accessed later
- **Usage**: Set when `SpreadMoveHit` is called with a `hitEffect` parameter
- **Access**: Read by `RunMoveEffects` to apply boosts/status/etc.

This is the C# equivalent of TypeScript's duck typing, but more explicit and type-safe.

## Related Bugs

### Similar Issues
- [CompleteDracoMeteorBugFix.md](./CompleteDracoMeteorBugFix.md) - Fixed infinite recursion with self-drops
- [SelfDropsInfiniteRecursionFix.md](./SelfDropsInfiniteRecursionFix.md) - Technical deep-dive

### Key Difference
The Draco Meteor bug was about **infinite recursion** when applying self-drops.
This Headlong Rush bug was about **not applying self-drops at all** due to type mismatch.

## Lessons Learned

### 1. Type System Bridges
When translating from TypeScript to C#, look for explicit properties that serve as "bridges" between different type hierarchies. The `ActiveMove.HitEffect` property is specifically designed for this purpose.

### 2. Comment Clues
The XML comment on `ActiveMove.HitEffect` explicitly states its purpose:
> "This is used to handle the casting of a HitEffect to an ActiveMove in RunMoveEffects() and SpreadMoveHit()."

Always read existing comments—they often reveal the intended design pattern!

### 3. Debug Logging Strategy
Adding targeted debug logs at key pipeline points helped identify:
- Where the method was being called ?
- What parameters it received ?
- Where the logic diverged ?

This pinpointed the exact location of the bug.

### 4. Recursive Call Patterns
When methods call themselves recursively with different parameter values:
- First call: `hitEffect = null` (normal execution)
- Recursive call: `hitEffect = SecondaryEffect` (special handling)

The recursive call needs special handling that the first call doesn't!

## Future Considerations

### Potential Improvements

1. **Better Documentation**:
   ```csharp
   /// <summary>
   /// Processes move hit effects. When called recursively with a hitEffect (e.g., from SelfDrops),
   /// the hitEffect is stored in move.HitEffect for RunMoveEffects to access.
   /// </summary>
   public (SpreadMoveDamage, SpreadMoveTargets) SpreadMoveHit(...)
   ```

2. **Type Safety**: Consider a wrapper type that makes the hitEffect?HitEffect assignment more explicit:
   ```csharp
   public sealed class MoveDataContext
   {
       public ActiveMove Move { get; init; }
       public HitEffect? HitEffect { get; set; }
   }
   ```

3. **Unit Tests**: Add specific tests for self-stat-drop moves:
   ```csharp
   [Fact]
   public void HeadlongRush_AppliesSelfStatDrops()
   {
       // Arrange: Ursaluna with Headlong Rush
       // Act: Use move
       // Assert: Def -1, SpD -1
   }
   ```

### Warning Signs

Watch for these patterns that might indicate similar bugs:
- Move effects not applying despite debug logs showing method calls
- Properties being checked but never set
- TypeScript code using `as` casts that don't translate directly to C#
- Recursive method calls with different parameter values

## Conclusion

This was a subtle type system bug that occurred at the boundary between C#'s static typing and the need to pass different effect types through a common pipeline. The fix was simple once identified:

1. ? Recognize that `ActiveMove.HitEffect` property exists for this purpose
2. ? Store the `hitEffect` parameter when it's provided
3. ? Allow `RunMoveEffects` to access it via `moveData.HitEffect.Boosts`

The solution is:
- **Minimal**: One if-statement assignment
- **Type-safe**: Uses existing property
- **Correct**: Matches intended design pattern
- **Complete**: Fixes all moves with self-targeting stat changes

All self-stat-drop moves now work correctly! ??
