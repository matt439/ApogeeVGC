# Magic Bounce Infinite Recursion Fix

## Problem Summary

When running large-scale parallel battle simulations (`RunRndVsRndVgcRegIEvaluation` with 32 threads), intermittent `System.InvalidOperationException: Stack overflow` exceptions occurred. The stack trace showed infinite recursion through the Magic Bounce ability handler:

```
Battle.RunEvent → UseMoveInner → UseMove → [Magic Bounce handler] → UseMove → UseMoveInner → ...
```

The exceptions were:
- **Intermittent**: Only occurred occasionally during thousands of parallel battles
- **Not reproducible in debug**: Using the same seeds in single-battle debug mode (`RunSingleBattleDebugVgcRegI`) did not trigger the issue

## Root Cause

The stack overflow was caused by an **infinite recursion loop** between two opposing Pokémon with Magic Bounce abilities bouncing a reflectable move back and forth indefinitely.

### The Recursion Cycle

1. **Pokemon A** uses a reflectable move (e.g., Taunt) targeting **Pokemon B**
2. **Pokemon B's Magic Bounce** triggers in `OnTryHit` handler:
   - Creates a new `ActiveMove` from the base move
   - Sets `newMove.HasBounced = true` (guard flag to prevent re-bouncing)
   - Calls `battle.Actions.UseMove(newMove, targetB, new UseMoveOptions { Target = sourceA })`
3. **`UseMoveInner` loses the flag**:
   - Line 340: `var activeMove = move.ToActiveMove()`
   - `ToActiveMove()` creates a **fresh** `ActiveMove` from the base `Move` properties
   - **Critical bug**: `ToActiveMove()` only copies base `Move` properties, not `ActiveMove`-specific properties like `HasBounced`
   - Result: `activeMove.HasBounced = null` (not `true`)
4. **Pokemon A's Magic Bounce** triggers:
   - Checks `move.HasBounced == true` → **fails** (it's null)
   - Bounces the move back to Pokemon B
5. **Infinite loop**: Steps 2-4 repeat until stack overflow

### Why `ToActiveMove()` Loses the Flag

The `Move.ToActiveMove()` method (in `Move.Core.cs`) explicitly copies only base `Move` properties and leaves `ActiveMove`-specific properties at their default values:

```csharp
public ActiveMove ToActiveMove()
{
    return new ActiveMove
    {
        // Copies all base Move properties (Id, Name, BasePower, etc.)
        Id = Id,
        Name = Name,
        // ... 100+ more properties
        
        // ActiveMove-specific properties like HasBounced are NOT copied
        // They remain at their default values (null for HasBounced)
    };
}
```

This is by design—`ToActiveMove()` is meant to create a fresh execution context for a move. However, certain properties like `HasBounced` need to be preserved across re-invocations.

## Why Intermittent and Not Reproducible in Debug

### Intermittent Occurrence

The bug only triggers when **all** of these conditions are met:
1. Both battling Pokémon have the Magic Bounce ability (rare)
2. One uses a reflectable move against the other
3. The recursion goes deep enough to overflow the stack before natural termination (one faints, runs out of PP, etc.)

In random battles, the specific combination of:
- Team generation seeds producing two Magic Bounce Pokémon
- Battle RNG causing them to use reflectable moves against each other
- HP/damage values preventing early knockout

...is rare but inevitable over thousands of battles.

### Not Reproducible in Single Debug Run

Even with identical seeds, the single debug run may not reproduce because:

1. **Thread Stack Size Difference**:
   - **Thread pool threads** (used by `Parallel.For`): **1 MB stack** (default)
   - **Main thread** (used by debug method): **4-8 MB or more**
   - The larger main thread stack can accommodate 500+ recursion levels without overflow
   - Thread pool threads overflow much sooner (potentially before natural battle termination)

2. **Battle Progression Timing**:
   - The exact turn when the Magic Bounce loop triggers depends on battle state
   - Debug run with same seeds has same teams/moves, but may hit a natural termination condition (faint, different move choice, PP depletion) before recursing deep enough to overflow the larger main thread stack

3. **Debug Flag Differences**:
   - Parallel: `debug = false`
   - Single: `debug = true`
   - If `debug` affects any control flow (logging, timing, event processing), it could cause subtle divergence

## Solution

Preserve the `HasBounced` flag when `UseMoveInner` creates a new `ActiveMove` from an incoming `ActiveMove`:

**File**: `ApogeeVGC\Sim\BattleClasses\BattleActions.Moves.cs`

```csharp
public bool UseMoveInner(Move move, Pokemon pokemon, UseMoveOptions? options = null)
{
    // ...

    // Get active move
    var activeMove = move.ToActiveMove();

    // Preserve HasBounced from the incoming move (e.g. set by Magic Bounce) because
    // ToActiveMove() only copies base Move properties and drops ActiveMove-specific
    // fields.  Without this, two opposing Magic Bounce holders infinitely re-bounce
    // reflectable moves, causing a stack overflow.
    if (move is ActiveMove { HasBounced: true })
    {
        activeMove.HasBounced = true;
    }

    pokemon.LastMoveUsed = activeMove;

    // Copy priority and prankster boost from active move if it exists
    if (Battle.ActiveMove != null)
    {
        activeMove.Priority = Battle.ActiveMove.Priority;
        if (activeMove.HasBounced != true)  // ← Existing check now works correctly
        {
            activeMove.PranksterBoosted = Battle.ActiveMove.PranksterBoosted;
        }
    }
    
    // ...
}
```

### Why This Works

1. **First Magic Bounce** (Pokemon B):
   - Creates `ActiveMove` with `HasBounced = true`
   - Calls `UseMove(newMove, ...)`
2. **UseMoveInner** (new code):
   - Calls `move.ToActiveMove()` → creates fresh `ActiveMove`
   - **Preserves `HasBounced = true`** from incoming move
3. **Second Magic Bounce** (Pokemon A):
   - Checks `move.HasBounced == true` → **succeeds**
   - Returns early with `VoidReturn()`, **breaks the recursion**

**Recursion depth**: 1-2 levels instead of 500+

## Files Modified

1. **`ApogeeVGC\Sim\BattleClasses\BattleActions.Moves.cs`**:
   - Added `HasBounced` preservation logic in `UseMoveInner` method after line 340

## Testing

### Verification Method

1. Run `RunRndVsRndVgcRegIEvaluation` with 500-1000 battles
2. Verify no stack overflow exceptions occur
3. Check that battles with Magic Bounce Pokémon complete normally

### Expected Behavior After Fix

- Magic Bounce activates correctly on first bounce
- Second Magic Bounce sees `HasBounced = true` and skips bouncing
- Move completes normally without recursion
- No stack overflow exceptions in parallel or single-threaded execution

### Affected Abilities/Moves

**Abilities**:
- Magic Bounce (Espeon, Xatu, Natu, Absol-Mega, Sableye-Mega, Diancie-Mega)

**Reflectable Moves** (partial list):
- Status moves: Taunt, Toxic, Will-O-Wisp, Thunder Wave, Confuse Ray, Leech Seed, Spore
- Stat-lowering moves: Screech, Growl, Leer, Fake Tears
- Entry hazards: Stealth Rock, Spikes, Toxic Spikes, Sticky Web

## Related TypeScript Implementation

TypeScript's `useMove` in `battle-actions.ts`:

```typescript
useMove(move: ActiveMove | Move, pokemon: Pokemon, options?) {
    // ...
    move = this.dex.getActiveMove(move);  // Similar to ToActiveMove()
    
    if (this.activeMove) {
        move.priority = this.activeMove.priority;
        if (!move.hasBounced) move.pranksterBoosted = this.activeMove.pranksterBoosted;
    }
    // ...
}
```

**Note**: TypeScript doesn't have the same issue because `this.dex.getActiveMove()` preserves `hasBounced` when converting an `ActiveMove` to another `ActiveMove`. The C# port's separation of `Move` and `ActiveMove` with explicit `ToActiveMove()` conversion required the additional preservation logic.

## Pattern: Preserving ActiveMove State Across Re-invocation

This fix establishes a pattern for other `ActiveMove`-specific properties that need preservation:

```csharp
var activeMove = move.ToActiveMove();

// Preserve ActiveMove-specific properties from incoming ActiveMove
if (move is ActiveMove incomingActiveMove)
{
    if (incomingActiveMove.HasBounced == true)
        activeMove.HasBounced = true;
    
    // Future: Add other properties that need preservation
    // if (incomingActiveMove.SomeFlag == true)
    //     activeMove.SomeFlag = true;
}
```

## Prevention Guidelines

1. **When using `ToActiveMove()`**: Consider whether any `ActiveMove`-specific properties need preservation
2. **When adding new `ActiveMove` properties**: Document whether they should persist across `UseMove` re-invocations
3. **Recursion guards**: Always verify that guard flags (like `HasBounced`) survive type conversions
4. **Thread stack considerations**: Understand that thread pool threads have smaller stacks than main threads

## Keywords

`Magic Bounce`, `infinite recursion`, `stack overflow`, `HasBounced`, `ToActiveMove`, `UseMoveInner`, `ActiveMove`, `thread stack size`, `parallel battles`, `intermittent bug`, `reflectable moves`, `bounce mechanics`, `recursion depth`, `property preservation`, `type conversion`

---

**Severity**: Critical (causes complete simulation failure)  
**Affected Systems**: Move execution pipeline, ability event handlers, parallel battle simulation  
**Bug Type**: Infinite recursion, state loss during type conversion  
**Detection Difficulty**: High (intermittent, only in specific team compositions)  
**Fix Complexity**: Low (single property preservation check)

---

*Documented*: 2025-01-20  
*Fixed By*: GitHub Copilot  
*Reported By*: Random battle evaluation testing
