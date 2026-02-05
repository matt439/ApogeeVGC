# PokemonFalseVoidUnion Return Conversion Fix

## Problem Summary

When a Pokémon with the Sweet Veil ability was on the field and another Pokémon tried to apply a sleep status to an ally, the battle crashed with:
```
InvalidOperationException: Event SetStatus: Unable to convert return value of type 'VoidPokemonFalseVoidUnion' to RelayVar
```

## Root Cause

The `EventHandlerAdapter.ConvertReturnValue` method was missing cases for the `PokemonFalseVoidUnion` union type and its three variants:
- `PokemonPokemonFalseVoidUnion` - Contains a Pokemon (e.g., the blocker)
- `FalsePokemonFalseVoidUnion` - Contains false (blocking signal)
- `VoidPokemonFalseVoidUnion` - Contains VoidReturn (no effect, allow status)

The `OnAllySetStatus` event handlers (used by Sweet Veil, Flower Veil, and similar abilities) return `PokemonFalseVoidUnion?` where:
- `null` = Block the status (returning null signals the status was blocked)
- `VoidReturn` = Allow the status (no interference)
- `Pokemon` = Block and identify the blocking Pokémon
- `false` = Explicit block signal

## Solution

Added conversion cases for all three `PokemonFalseVoidUnion` variants in `EventHandlerAdapter.ConvertReturnValue`:

```csharp
// PokemonFalseVoidUnion -> Pokemon, false, or VoidReturn
PokemonPokemonFalseVoidUnion pokemonFalseVoid => new PokemonRelayVar(pokemonFalseVoid.Pokemon),
FalsePokemonFalseVoidUnion => new BoolRelayVar(false),
VoidPokemonFalseVoidUnion => new VoidReturnRelayVar(),
```

## Files Modified

- `ApogeeVGC\Sim\Events\EventHandlerAdapter.cs` - Added union type conversion cases

## Affected Abilities

Abilities that use `OnAllySetStatus` or similar status-blocking handlers:
- Sweet Veil (blocks Sleep for allies)
- Flower Veil (blocks stat reduction and status for Grass-type allies)
- Aroma Veil (blocks moves like Taunt, Disable, etc.)
- Pastel Veil (blocks Poison for allies)

## Pattern

This follows the same pattern as other union type conversion fixes documented in:
- MoveIdVoidUnion Return Conversion Fix
- VoidFalseUnion Return Conversion Fix
- IntBoolUnion Return Conversion Fix
- SparseBoostsTableVoidUnion Return Conversion Fix

When adding new event handlers that return union types, ensure all variants have explicit conversion cases in `EventHandlerAdapter.ConvertReturnValue`.

## Keywords

`Sweet Veil`, `PokemonFalseVoidUnion`, `VoidPokemonFalseVoidUnion`, `SetStatus event`, `OnAllySetStatus`, `event handler return`, `union type conversion`, `status blocking`, `Flower Veil`, `EventHandlerAdapter`
