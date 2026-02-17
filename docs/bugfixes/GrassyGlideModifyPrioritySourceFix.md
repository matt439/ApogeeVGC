# Grassy Glide ModifyPriority Source Parameter Fix

## Summary
**Severity**: Medium  
**Systems Affected**: Move-specific OnModifyPriority handlers (e.g., Grassy Glide)

## Problem
Grassy Glide's `OnModifyPriority` handler was throwing a `NullReferenceException` when checking if the Pokemon using the move is grounded in Grassy Terrain.

### Exception Details
```
System.NullReferenceException: Object reference not set to an instance of an object.
   at ApogeeVGC.Data.Moves.Moves.<>c.<CreateMovesGhi>b__10_5(Battle battle, Int32 priority, Pokemon source, Pokemon _, ActiveMove _) in MovesGHI.cs:line 387
```

The handler was receiving `null` for the `source` parameter:
```csharp
OnModifyPriority = new OnModifyPriorityEventInfo((battle, priority, source, _, _) =>
{
    if (battle.Field.IsTerrain(ConditionId.GrassyTerrain, source) &&
        (source.IsGrounded() ?? false))  // NullReferenceException here!
    {
        return priority + 1;
    }
    return new VoidReturn();
}),
```

## Root Cause Analysis

### TypeScript Event Handler Pattern
In Pokémon Showdown, `singleEvent` passes arguments to callbacks as:
```typescript
const args = [target, source, sourceEffect];
if (hasRelayVar) args.unshift(relayVar);
// Result: [relayVar, target, source, sourceEffect]
```

The `ModifierSourceMove` handler pattern has this signature:
```typescript
ModifierSourceMove: (
    this: Battle, relayVar: number, source: Pokemon, target: Pokemon, move: ActiveMove
) => number | void;
```

When `singleEvent` is called for ModifyPriority:
```typescript
priority = this.singleEvent('ModifyPriority', move, null, action.pokemon, null, null, priority);
```

The handler receives:
- `relayVar` = `priority`
- `source` (handler param) = `target` (singleEvent arg) = `action.pokemon`
- `target` (handler param) = `source` (singleEvent arg) = `null`
- `move` = `sourceEffect` = `null`

So the handler's `source` parameter receives `action.pokemon` because of parameter position mapping.

### C# Implementation Issue
In C#, `GetActionSpeed` was calling `SingleEvent` like this:
```csharp
RelayVar? singleEventResult = SingleEvent(
    EventId.ModifyPriority,
    move,
    null,
    moveAction.Pokemon,  // target
    null,                // source - THIS WAS NULL!
    null,
    priority
);
```

The `EventHandlerAdapter` resolves handler parameters by name:
- Parameter named `source` ? `context.SourcePokemon`
- `SourcePokemon` comes from the `source` argument to `SingleEvent`, which was `null`

Unlike TypeScript's positional mapping, C# uses named parameter resolution, so the `source` parameter in the handler got `null` instead of the Pokemon using the move.

## Solution
Pass `moveAction.Pokemon` as both the `target` AND `source` parameter to `SingleEvent`:

```csharp
RelayVar? singleEventResult = SingleEvent(
    EventId.ModifyPriority,
    move,
    null,
    moveAction.Pokemon,
    moveAction.Pokemon,  // Pass Pokemon as source too
    null,
    priority
);
```

This ensures that handlers using the `ModifierSourceMove` pattern can access the Pokemon using the move via the `source` parameter name.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/Battle.Sorting.cs` - Added `moveAction.Pokemon` as source parameter in SingleEvent call

## Affected Moves
Any move with an `OnModifyPriority` handler that accesses the `source` parameter:
- **Grassy Glide** - Gains +1 priority when user is grounded in Grassy Terrain

## Testing
Run random battle evaluation to verify Grassy Glide no longer causes exceptions when used in Grassy Terrain.

## Related Issues
This is similar to other event handler parameter resolution issues documented in:
- Facade BasePower Event Parameter Fix
- Immunity Event Parameter Conversion Fix

## Keywords
`ModifyPriority`, `Grassy Glide`, `SingleEvent`, `source`, `target`, `NullReferenceException`, `EventHandlerAdapter`, `ModifierSourceMove`
