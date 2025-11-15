# Leech Seed Bug Fix - Summary

## Problem
Battle ends prematurely as a tie when Leech Seed move is used on Turn 1. The battle stops after processing only the first move action without completing the turn.

## Symptoms
```
[TurnLoop] Processing action 1: Move
[ProcessChoiceResponsesAsync] Exception: InvalidOperationException: Cannot convert Undefined to int...
[Simulator.RunAsync] Completed task: choiceProcessing
[Simulator.RunAsync] Battle.Ended = False
Battle has ended: False
WARNING: DetermineWinner called but battle hasn't ended yet
[Driver] Battle completed with result: Tie
```

- Battle stops after processing first move
- Exception thrown: "Cannot convert Undefined to int. Undefined and 0 are semantically different."
- `Battle.Ended = False` but battle loop exits anyway
- Exception causes async task to complete prematurely

## Root Cause
The issue occurred in `Battle.Combat.cs` in the `SpreadDamage` method. The code was calling `.ToInt()` on damage values **before** checking if they were `Undefined`.

For non-damaging status moves like Leech Seed, the damage value is `Undefined` (not 0, as they are semantically different in Pokemon battle mechanics). When the code tried to convert this `Undefined` value to an integer, it threw an exception.

### The Problematic Code (Line 103 in Battle.Combat.cs)
```csharp
int targetDamage = curDamage[0].ToInt();  // ? Exception thrown here for Undefined values

// Handle undefined damage values
if (curDamage[0] is UndefinedBoolIntUndefinedUnion)
{
  retVals.Add(curDamage[0]);
  continue;
}
```

The check for `Undefined` came AFTER the `.ToInt()` call, so the exception was thrown before the check could prevent it.

## Solution
Moved the `Undefined` check to occur BEFORE calling `.ToInt()`, so non-damaging moves can be handled properly without attempting to convert `Undefined` to an integer.

### Fixed Code
```csharp
// Handle undefined damage values FIRST before calling ToInt()
if (curDamage[0] is UndefinedBoolIntUndefinedUnion)
{
    retVals.Add(curDamage[0]);
    continue;
}

int targetDamage = curDamage[0].ToInt();  // ? Safe to call now
```

Additionally, two other locations in `BattleActions.HitSteps.cs` were fixed to safely check if `move.TotalDamage` is an integer before calling `.ToInt()`:

1. **Recoil damage check** (line ~845):
```csharp
if ((move.Recoil != null || move.Id == MoveId.Chloroblast) && 
    move.TotalDamage is IntIntFalseUnion totalDamageInt && totalDamageInt.Value > 0)
```

2. **Emergency Exit check** (line ~885):
```csharp
int curDamage = targets.Count == 1 
    ? (move.TotalDamage is IntIntFalseUnion totalDmgInt ? totalDmgInt.Value : 0)
    : (damage[i] is IntBoolIntEmptyUndefinedUnion dmgInt ? dmgInt.Value : 0);
```

## How Leech Seed Works (Correctly)

The Leech Seed move definition in `ApogeeVGC/Data/Moves.cs` already had the correct `VolatileStatus = ConditionId.LeechSeed` field, which was added in a previous fix.

1. When Leech Seed hits, `RunMoveEffects` checks for `moveData.VolatileStatus`
2. It calls `target.AddVolatile(ConditionId.LeechSeed, source, move)`
3. The `AddVolatile` method automatically sets the `SourceSlot` field
4. On subsequent turns, the LeechSeed condition's `OnResidual` handler drains HP from the target and heals the source

## Why This Caused the Battle to End Prematurely

The exception was being caught in `Simulator.ProcessChoiceResponsesAsync`, which logged it and then exited the method through the exception handler. This caused the `choiceProcessingTask` to complete, which triggered `Task.WhenAny` to return in `Simulator.RunAsync`, and the simulator incorrectly interpreted this as the battle ending (even though `Battle.Ended` was still `false`).

## Testing Results
After the fix:
- Leech Seed applies successfully on Turn 1
- The volatile status is correctly stored with source information
- On Turn 2 and beyond, Leech Seed drains HP from the target and heals the source
- The battle continues normally without premature termination
- Both Console and async modes work correctly

## Files Modified
1. `ApogeeVGC\Sim\BattleClasses\Battle.Combat.cs` - Fixed `SpreadDamage` to check for `Undefined` before calling `.ToInt()`
2. `ApogeeVGC\Sim\BattleClasses\BattleActions.HitSteps.cs` - Fixed two locations that unsafely called `.ToInt()` on `move.TotalDamage`

## Related Information

### Pokemon Showdown Reference
In the official Pokemon Showdown TypeScript code, Leech Seed has:
```typescript
leechseed: {
    num: 73,
    accuracy: 90,
    basePower: 0,
    category: "Status",
    name: "Leech Seed",
    pp: 10,
    priority: 0,
    flags: { protect: 1, reflectable: 1, mirror: 1, metronome: 1 },
    volatileStatus: 'leechseed',
    condition: {
        onStart(target) {
  this.add('-start', target, 'move: Leech Seed');
  },
onResidualOrder: 8,
  onResidual(pokemon) {
            const target = this.getAtSlot(pokemon.volatiles['leechseed'].sourceSlot);
          // Drains HP and heals source
        }
    },
}
```

### Undefined vs 0 in Battle Mechanics
In Pokemon battle mechanics, `Undefined` and `0` are semantically different:
- `0` means the move dealt 0 damage (e.g., due to immunity or substitutes)
- `Undefined` means the move doesn't deal damage at all (e.g., status moves like Leech Seed)

This distinction is important for determining move success and triggering various effects.
