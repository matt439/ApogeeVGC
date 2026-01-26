# Effectiveness Event PokemonType Parameter Fix

## Issue
`InvalidOperationException` when Iron Ball item's Effectiveness event handler was invoked:
```
Event Effectiveness: Parameter 3 (PokemonType _) is non-nullable but no matching value found in context
```

The Iron Ball item has an `OnEffectiveness` event handler that modifies type effectiveness calculations. When this handler was invoked, the EventHandlerAdapter couldn't resolve the `PokemonType` parameter required by the handler signature.

## Root Cause
The Effectiveness event is called from `Pokemon.RunEffectiveness` with a `PokemonType` as the event source:
```csharp
RelayVar? runEventResult = Battle.RunEvent(
    EventId.Effectiveness,
    this,       // target: Pokemon
    type,       // source: PokemonType (the type being checked)
    move,       // sourceEffect: ActiveMove
    typeMod     // relayVar: int
);
```

However, the event context system was not properly handling `PokemonType` sources:

1. `RunEventSource` had support for `PokemonType` via `TypeRunEventSource`
2. `SingleEventSource` had support for `PokemonType` via `PokemonTypeSingleEventSource`
3. **BUT** `Battle.RunEvent` only converted `PokemonRunEventSource` to `PokemonSingleEventSource`, dropping `TypeRunEventSource`
4. **AND** `EventContext` had no field for `SourceType`
5. **AND** `EventHandlerAdapter.ResolveParameter` had no logic to extract `PokemonType` from context

This caused the `PokemonType` parameter to be lost during event invocation, resulting in the parameter resolution failure.

## Handler Signature
The `OnEffectivenessEventInfo` expects:
```csharp
Func<Battle, int, Pokemon?, PokemonType, ActiveMove, IntVoidUnion> handler
```

Example from Iron Ball:
```csharp
OnEffectiveness = new OnEffectivenessEventInfo((battle, typeMod, target, type, move) =>
{
    if (target == null || /* ... */)
        return typeMod;
    if (move.Type == MoveType.Ground && target.HasType(PokemonType.Flying))
        return 0; // Ground moves hit Flying-types holding Iron Ball
    return typeMod;
}),
```

## Solution
Added support for `PokemonType` throughout the event context system:

### 1. EventContext (Sim/Events/EventContext.cs)
Added `SourceType` property to hold the `PokemonType` source:
```csharp
/// <summary>
/// Source type (for type-specific events like Effectiveness).
/// </summary>
public PokemonType? SourceType { get; init; }
```

### 2. Battle.Events.cs (Sim/BattleClasses/Battle.Events.cs)
Updated `RunEvent` to convert `TypeRunEventSource` to `PokemonTypeSingleEventSource`:
```csharp
Source = source switch
{
    PokemonRunEventSource pokemonSource => new PokemonSingleEventSource(pokemonSource.Pokemon),
    TypeRunEventSource typeSource => new PokemonTypeSingleEventSource(typeSource.Type), // NEW
    _ => null,
},
```

### 3. EventInvocationContext.cs (Sim/Events/EventInvocationContext.cs)
Updated `ToEventContext` to extract `PokemonType` from `PokemonTypeSingleEventSource`:
```csharp
SourceType = Source switch
{
    PokemonTypeSingleEventSource t => t.Type,
    _ => null
},
```

### 4. EventHandlerAdapter.cs (Sim/Events/EventHandlerAdapter.cs)
Added `PokemonType` resolution logic to `ResolveParameter`:
```csharp
// Add support for PokemonType
if (paramType == typeof(PokemonType) || typeof(PokemonType).IsAssignableFrom(paramType))
{
    return context.SourceType;
}
```

## Impact
- **Effectiveness event**: Can now properly handle `PokemonType` parameters
- **Iron Ball item**: Effectiveness handler now works correctly
- **Other items/abilities**: Any Effectiveness event handlers that use `PokemonType` parameters will now work

## Testing
Build successful. The fix allows Iron Ball's Effectiveness event to properly receive the `PokemonType` parameter and modify type effectiveness calculations.

## Files Modified
1. `ApogeeVGC\Sim\Events\EventContext.cs` - Added `SourceType` property
2. `ApogeeVGC\Sim\BattleClasses\Battle.Events.cs` - Convert `TypeRunEventSource` to `PokemonTypeSingleEventSource`
3. `ApogeeVGC\Sim\Events\EventInvocationContext.cs` - Extract `PokemonType` from source
4. `ApogeeVGC\Sim\Events\EventHandlerAdapter.cs` - Resolve `PokemonType` parameters

## Related Fixes
This fix follows the same pattern as previous event parameter resolution fixes:
- ModifyAccuracyEventParameterNullabilityFix.md
- ImmunityEventParameterConversionFix.md
- VoidFalseUnionReturnConversionFix.md
