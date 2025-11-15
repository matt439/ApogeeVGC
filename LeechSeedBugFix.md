# Leech Seed Bug Fix - Summary

## Problem
Battle ends prematurely as a tie when Leech Seed move is used on Turn 1. The battle stops after processing only the first move action without completing the turn.

## Symptoms
```
[TurnLoop] Processing action 1: Move
[Simulator.RunAsync] Completed task: choiceProcessing
[Simulator.RunAsync] Battle.Ended = False
[Simulator.RunAsync] Choice processing completed
Battle has ended: False
WARNING: DetermineWinner called but battle hasn't ended yet
[Driver] Battle completed with result: Tie
```

- Battle stops after processing first move
- No error messages or exceptions logged
- `Battle.Ended = False` but battle loop exits anyway
- Turn 1, so no residual damage should be happening yet

## Root Cause
The Leech Seed move in `ApogeeVGC/Data/Moves.cs` was missing the `VolatileStatus` field. Without this field, the move engine doesn't know to apply the LeechSeed volatile condition to the target when the move hits.

### Original Code (Broken)
```csharp
[MoveId.LeechSeed] = new()
{
    Id = MoveId.LeechSeed,
    Num = 73,
    Accuracy = 90,
    BasePower = 0,
    Category = MoveCategory.Status,
    Name = "Leech Seed",
    BasePp = 10,
    Priority = 0,
  Flags = new MoveFlags
    {
 Protect = true,
        Reflectable = true,
        Mirror = true,
        Metronome = true,
    },
    Condition = _library.Conditions[ConditionId.LeechSeed],  // Only has Condition
    OnTryImmunity = new OnTryImmunityEventInfo((_, target, _, _) =>
        !target.HasType(PokemonType.Grass)),
    Secondary = null,
    Target = MoveTarget.Normal,
    Type = MoveType.Grass,
},
```

## Solution
Added the `VolatileStatus` field to the Leech Seed move definition. This field tells the move execution engine (specifically `BattleActions.MoveEffects.RunMoveEffects`) to apply the volatile condition to the target.

### Fixed Code
```csharp
[MoveId.LeechSeed] = new()
{
    Id = MoveId.LeechSeed,
    Num = 73,
    Accuracy = 90,
    BasePower = 0,
    Category = MoveCategory.Status,
    Name = "Leech Seed",
    BasePp = 10,
    Priority = 0,
    Flags = new MoveFlags
    {
        Protect = true,
    Reflectable = true,
        Mirror = true,
        Metronome = true,
    },
    VolatileStatus = ConditionId.LeechSeed,  // ? ADDED THIS
    Condition = _library.Conditions[ConditionId.LeechSeed],
    OnTryImmunity = new OnTryImmunityEventInfo((_, target, _, _) =>
        !target.HasType(PokemonType.Grass)),
    Secondary = null,
    Target = MoveTarget.Normal,
 Type = MoveType.Grass,
},
```

## How Volatile Status Moves Work

### Move Execution Flow
1. Move is executed via `BattleActions.UseMove` ? `UseMoveInner`
2. `TryMoveHit` or `TrySpreadMoveHit` is called
3. `MoveHit` is called, which calls `ExecuteMoveHit`
4. `ExecuteMoveHit` calls `RunMoveEffects`
5. **`RunMoveEffects`** checks for `moveData.VolatileStatus` and applies it:

```csharp
// From BattleActions.MoveEffects.cs
if (moveData.VolatileStatus != null)
{
    RelayVar volatileResult = target.AddVolatile(moveData.VolatileStatus.Value, source, move);
    hitResult = volatileResult switch
    {
        BoolRelayVar brv => BoolIntUndefinedUnion.FromBool(brv.Value),
        IntRelayVar irv => BoolIntUndefinedUnion.FromInt(irv.Value),
 _ => BoolIntUndefinedUnion.FromUndefined()
    };
    didSomething = CombineResults(didSomething, hitResult);
}
```

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
    volatileStatus: 'leechseed',  // ? This is what was missing
    condition: {
        onStart(target) {
   this.add('-start', target, 'move: Leech Seed');
        },
        onResidualOrder: 8,
   onResidual(pokemon) {
            const target = this.getAtSlot(pokemon.volatiles['leechseed'].sourceSlot);
            // ...
     }
    },
// ...
}
```

## Other Affected Components

### LeechSeed Condition
The condition itself (in `Conditions.cs`) was already correct:
- Has `OnStart` event handler
- Has `OnResidual` event handler with proper healing logic
- Sets `ImmuneTypes = [PokemonType.Grass]`
- `OnResidual` accesses `pokemon.Volatiles[ConditionId.LeechSeed].SourceSlot` to find the source Pokemon

The condition was fine - the problem was that it never got applied because the move didn't specify `VolatileStatus`.

### Similar Moves for Reference
Other volatile status moves in the codebase that work correctly:
- **Protect**: Has `VolatileStatus = ConditionId.Protect`
- **Confusion**: Secondary effect applies via `VolatileStatus`

## Files Modified
- `ApogeeVGC/Data/Moves.cs` - Added `VolatileStatus = ConditionId.LeechSeed` to Leech Seed move

## Testing Notes
- Should test that Leech Seed properly:
  1. Applies the volatile condition on hit
  2. Shows "-start" message in battle log
  3. Drains HP during residual phase on subsequent turns
  4. Heals the source Pokemon
  5. Fails against Grass-type Pokemon
  6. Properly handles source Pokemon switching out/fainting

## Additional Context
The investigation initially considered:
- Simultaneous faints from residual damage (ruled out - Turn 1)
- Missing immunity checks (already present via `OnTryImmunity`)
- `SourceSlot` not being set (would be set automatically by `AddVolatile`)
- Exception handling swallowing errors (no try-catch found in move execution)

The actual issue was simpler: the move never attempted to apply the volatile because the `VolatileStatus` field was missing.
