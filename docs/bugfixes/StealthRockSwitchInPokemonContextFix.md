# Stealth Rock SwitchIn Pokemon Context Fix

## Problem Summary
When a Pokemon switched in with Stealth Rock (or any other side condition with an OnSwitchIn handler) on their side of the field, the battle crashed with:

```
System.InvalidOperationException: Event SwitchIn adapted handler failed on effect Stealth Rock (Condition)
Inner exception: InvalidOperationException: Event SwitchIn: Parameter 1 (Pokemon pokemon) is non-nullable but no Pokemon found in context (TargetPokemon=False, SourcePokemon=False)
```

## Root Cause
The `FieldEvent` method was collecting side condition event handlers **twice** for SwitchIn events:

1. **First collection (lines 594-598)**: Without Pokemon context
   ```csharp
   if (side.N < 2)
   {
       handlers.AddRange(
           FindSideEventHandlers(side, sideEventId, EventPrefix.None, getKey));
   }
   ```
   - This created handlers with `EffectHolder = side` (a `SideEffectHolder`)
   - When invoked, these handlers produced a `SideSingleEventTarget`
   - The `EventContext` had `TargetPokemon = null`

2. **Second collection (line 615)**: With Pokemon context
   ```csharp
   handlers.AddRange(FindSideEventHandlers(side, eventId, customHolder: active));
   ```
   - This created handlers with `EffectHolder = active` (a `PokemonEffectHolder`)  
   - When invoked, these handlers produced a `PokemonSingleEventTarget`
   - The `EventContext` had `TargetPokemon = <switching Pokemon>`

**The Problem**: The first handler (without Pokemon context) executed first and failed because Stealth Rock's OnSwitchIn handler has a non-nullable Pokemon parameter, but the EventContext didn't have a TargetPokemon.

## Why This Matters
Side condition handlers like Stealth Rock's OnSwitchIn need to know **which specific Pokemon** is switching in, not just that a switch occurred on a side. The damage calculation depends on the Pokemon's type, whether it has Heavy-Duty Boots, etc.

## Solution
Modified `FieldEvent` to skip collecting side condition handlers without Pokemon context for SwitchIn events:

```csharp
// Skip this for SwitchIn events since side condition handlers need Pokemon context
if (side.N < 2 && eventId != EventId.SwitchIn)
{
    handlers.AddRange(
        FindSideEventHandlers(side, sideEventId, EventPrefix.None, getKey));
}
```

This ensures that for SwitchIn events:
- Side condition handlers are ONLY collected with `customHolder: active` (line 615)
- All side condition handlers have the switching Pokemon as their EffectHolder
- EventContext.TargetPokemon is properly set to the switching Pokemon
- Handlers can access Pokemon-specific properties (type, item, etc.)

## Files Modified
- `ApogeeVGC\Sim\BattleClasses\Battle.Events.cs` - Added condition to skip side handler collection without Pokemon context for SwitchIn events

## Testing
After applying the fix, built successfully and the error should no longer occur when Pokemon switch in with Stealth Rock or other entry hazards on the field.

## Impact
- Fixes crash when switching in with Stealth Rock, Sticky Web, Toxic Spikes on the field
- Ensures all side condition OnSwitchIn handlers receive proper Pokemon context
- May affect other side conditions with SwitchIn handlers (Toxic Spikes absorption, Sticky Web speed drop, etc.)

## Pattern Recognition
This follows a similar pattern to other event handler collection issues:
- **SideResidual Pokemon Parameter Nullability Fix**: Side-level events need Pokemon context when affecting individual Pokemon
- **FieldResidual Pokemon Parameter Nullability Fix**: Field-level events need Pokemon context when affecting individual Pokemon  

**General Rule**: When an event handler operates on individual Pokemon (checking stats, types, items, applying damage), it should be collected with `customHolder: pokemon` to ensure the Pokemon is available in the EventContext.

## Related Issues
- Previous "SwitchIn Null Pokemon Parameter Fix" added null validation that caught this issue
- That fix converted a silent `NullReferenceException` into a descriptive error
- This fix addresses the root cause by ensuring Pokemon context is properly provided

## Keywords
`FieldEvent`, `SwitchIn`, `side condition`, `Stealth Rock`, `Sticky Web`, `Toxic Spikes`, `entry hazards`, `customHolder`, `EffectHolder`, `EventContext`, `TargetPokemon`, `duplicate handlers`, `event collection`
