# SwitchIn Null Pokemon Parameter Fix

## Problem Summary
When a Pokemon with Stealth Rock on their side of the field switched in, the battle crashed with `NullReferenceException` at line 657 of `ConditionsSTU.cs` when trying to access `pokemon.HasItem(ItemId.HeavyDutyBoots)`.

## Root Cause
The `EventHandlerAdapter.ResolveParameter` method had a critical flaw in how it handled parameter resolution and nullability validation:

1. **Early Return Without Validation**: When resolving a `Pokemon` parameter (line 155-159), the method returned `context.TargetPokemon ?? context.SourcePokemon` directly without checking if the result was null
2. **Unreachable Null Check**: The nullability validation at lines 236-243 was only executed if NONE of the type-specific resolution blocks matched, making it unreachable for Pokemon parameters
3. **Null Propagation**: When `context.TargetPokemon` and `context.SourcePokemon` were both null, the null coalescing operator returned null, which was then passed directly to the handler

This caused handlers with non-nullable Pokemon parameters to receive null values, leading to `NullReferenceException` when the handler tried to access Pokemon properties.

## Solution
Added nullability validation BEFORE returning the Pokemon parameter:

```csharp
// Match by type
if (paramType == typeof(Pokemon) || typeof(Pokemon).IsAssignableFrom(paramType))
{
    // Prefer target over source if ambiguous
    Pokemon? pokemon = context.TargetPokemon ?? context.SourcePokemon;
    
    // Check nullability before returning  ? ADDED
    if (pokemon == null &&
        handlerInfo.ParameterNullability != null &&
        position < handlerInfo.ParameterNullability.Length &&
        !handlerInfo.ParameterNullability[position])
    {
        throw new InvalidOperationException(
            $"Event {handlerInfo.Id}: Parameter {position} ({paramType.Name} {paramName}) is non-nullable " +
            $"but no Pokemon found in context (TargetPokemon={context.TargetPokemon != null}, SourcePokemon={context.SourcePokemon != null})");
    }
    
    return pokemon;
}
```

## Files Modified
- `ApogeeVGC\Sim\Events\EventHandlerAdapter.cs` - Added null validation for Pokemon parameters before returning

## Testing
Ran 1000-battle random vs random simulation:
- **Before fix**: Immediate crash with `NullReferenceException` in Stealth Rock handler
- **After fix**: No SwitchIn-related errors occurred (449 successful battles, 551 failures were Residual-event related issues requiring separate fix)

## Impact
- Ensures non-nullable Pokemon parameters are properly validated before being passed to handlers
- Provides detailed error messages when Pokemon parameters cannot be resolved
- Prevents silent null propagation that leads to `NullReferenceException` inside handlers

## Pattern
This same validation pattern should be applied to other type-specific resolution blocks (Side, Field, Move, etc.) to prevent similar issues.

## Related Issues
- Similar pattern may affect other parameter types (Side, Field, Move) that resolve early and return without null validation
- Discovered new issue with Residual events that requires separate investigation

## Keywords
`EventHandlerAdapter`, `ResolveParameter`, `null validation`, `Pokemon parameter`, `Stealth Rock`, `OnSwitchIn`, `NullReferenceException`, `parameter nullability`, `early return`
