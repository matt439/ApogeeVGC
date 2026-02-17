# Collection Modified During Enumeration Fix

**Date**: 2025-01-XX  
**Severity**: High  
**Systems Affected**: Battle lifecycle, switching mechanics, phazing

## Problem

During random battle testing, the simulator threw a `System.InvalidOperationException: Collection was modified; enumeration operation may not execute.` error in the `RunAction` method of `Battle.Lifecycle.cs` at line 719.

### Error Stack Trace
```
System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
   at ApogeeVGC.Sim.BattleClasses.Battle.RunAction(IAction action) Line 719
   at ApogeeVGC.Sim.BattleClasses.Battle.TurnLoop() Line 372
```

### Root Cause

The `RunAction` method had four `foreach` loops that iterated over `side.Active`:

1. **Phazing loop (lines 716-729)**: Handles forced switches from moves like Roar, Dragon Tail, etc.
2. **Cancel fainted actions loop (lines 737-747)**: Removes queued actions for fainted Pokemon
3. **Revival Blessing check loop (lines 788-804)**: Checks for Revival Blessing conditions
4. **BeforeSwitchOut event loop (lines 821-843)**: Triggers BeforeSwitchOut events

The problem occurred in the **phazing loop** when `Actions.DragIn()` was called, which ultimately calls `BattleActions.SwitchIn()`. The `SwitchIn` method modifies `side.Active[pos]` on line 159 of `BattleActions.Switch.cs`:

```csharp
side.Active[pos] = pokemon;
```

This modification occurred **while the foreach loop was still iterating** over `side.Active`, causing the collection modification exception.

## Solution

Created **snapshot copies** of `side.Active` before each foreach loop iteration. This ensures that even if the original collection is modified during iteration (via `DragIn` ? `SwitchIn`), the loop continues safely over the snapshot.

### Changes Made

Modified all four foreach loops in `RunAction` to use snapshots:

```csharp
// Before (unsafe):
foreach (Pokemon? pokemon in side.Active)
{
    // Code that might trigger DragIn/SwitchIn
}

// After (safe):
Pokemon?[] activeSnapshot = [.. side.Active];
foreach (Pokemon? pokemon in activeSnapshot)
{
    // Code that might trigger DragIn/SwitchIn
}
```

### Affected Code Sections

1. **Phazing loop** (lines ~717-730)
2. **Cancel fainted actions loop** (lines ~738-748)
3. **Revival Blessing check loop** (lines ~791-805)
4. **BeforeSwitchOut event loop** (lines ~823-844)

## Technical Notes

- Used C# 12 collection expression syntax `[.. side.Active]` to create array snapshots
- Snapshots are created immediately before each loop to minimize staleness
- The snapshot is a shallow copy (array of references), which is appropriate since we're only reading Pokemon properties and occasionally modifying the Active collection itself
- This pattern is safe because:
  - We're iterating over Pokemon references (not modifying them)
  - We're only modifying the collection structure (`side.Active[pos] = ...`)
  - The Pokemon objects themselves remain valid throughout

## Testing

- Verified compilation with no errors
- Tested with random battle simulations using `RunRndVsRndVgcRegIEvaluation`
- Exception no longer occurs during phazing scenarios

## Related Issues

This is similar to other collection modification issues that can occur in event-driven systems where callbacks modify collections being iterated. The fix follows the standard pattern of creating a snapshot before iteration when modification during iteration is possible.

## Keywords

`collection modified`, `enumeration`, `concurrent modification`, `InvalidOperationException`, `foreach`, `side.Active`, `phazing`, `DragIn`, `SwitchIn`, `Roar`, `Dragon Tail`
