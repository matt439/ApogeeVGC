# Immunity Event Parameter Conversion Fix

**Date**: 2024
**Severity**: High  
**Systems Affected**: Weather effects with immunity handlers, condition immunity checks

---

## Problem

When Sunny Day weather was active and attempting to apply a volatile status (e.g., during move effects), the battle crashed with the following exception:

```
System.InvalidOperationException: Event Immunity adapted handler failed on effect SunnyDay (Weather)

Inner Exception:
InvalidOperationException: Event Immunity: Parameter 1 (PokemonTypeConditionIdUnion type) is non-nullable but no matching value found in context
```

**Symptoms**:
- Battle crashes when weather with immunity handlers is active and status immunity is checked
- Stack trace shows failure in `EventHandlerAdapter.ResolveParameter`
- Error indicates parameter type mismatch for `PokemonTypeConditionIdUnion`

**Affected Code Flow**:
1. Move effect tries to add a volatile status (`Pokemon.AddVolatile`)
2. `Pokemon.RunStatusImmunity` is called to check immunity
3. `Battle.RunEvent(EventId.Immunity, this, null, null, conditionId)` is invoked
4. Weather's (e.g., SunnyDay) `OnImmunity` handler is triggered
5. `EventHandlerAdapter.ResolveParameter` fails to convert `ConditionIdRelayVar` to `PokemonTypeConditionIdUnion`

---

## Root Cause

The `EventHandlerAdapter.ResolveParameter` method didn't know how to convert a `ConditionIdRelayVar` (or `PokemonTypeRelayVar`) into a `PokemonTypeConditionIdUnion` parameter type.

**Code Context**:

In `Pokemon.Immunity.cs` line 185-186:
```csharp
RelayVar? immunity = Battle.RunEvent(EventId.Immunity, this, null, null, conditionId);
```

The `conditionId` parameter (type `ConditionId?`) is implicitly converted to `ConditionIdRelayVar` via the implicit operator defined in `RelayVar.cs`:
```csharp
public static implicit operator RelayVar(ConditionId? id) => new ConditionIdRelayVar(id);
```

The `OnImmunityEventInfo` handler signature expects:
```csharp
Func<Battle, PokemonTypeConditionIdUnion, Pokemon, BoolVoidUnion> handler
```

Where `PokemonTypeConditionIdUnion` is a union type that can represent either a `PokemonType` or a `ConditionId`.

**The Gap**: The `EventHandlerAdapter` could extract primitive types (int, bool, decimal) from RelayVars, but didn't know how to convert `ConditionIdRelayVar` or `PokemonTypeRelayVar` into the `PokemonTypeConditionIdUnion` parameter type that the handler expected.

---

## Solution

Added logic to `EventHandlerAdapter.ResolveParameter` to handle the conversion from `ConditionIdRelayVar` or `PokemonTypeRelayVar` to `PokemonTypeConditionIdUnion`.

**Modified File**: `ApogeeVGC\Sim\Events\EventHandlerAdapter.cs`

**Changes**:
Added a new parameter type check after the `PokemonSideFieldUnion` check (around line 204):

```csharp
// Check for PokemonTypeConditionIdUnion
if (paramType == typeof(PokemonTypeConditionIdUnion))
{
    if (context.HasRelayVar)
    {
        // Handle ConditionIdRelayVar -> PokemonTypeConditionIdUnion
        if (context.RelayVar is ConditionIdRelayVar conditionIdVar && conditionIdVar.Id.HasValue)
        {
            return new PokemonTypeConditionIdUnion(conditionIdVar.Id.Value);
        }
        
        // Handle PokemonTypeRelayVar -> PokemonTypeConditionIdUnion
        if (context.RelayVar is PokemonTypeRelayVar pokemonTypeVar)
        {
            return new PokemonTypeConditionIdUnion(pokemonTypeVar.Type);
        }
    }
}
```

---

## How It Works

1. When `Pokemon.RunStatusImmunity` calls `RunEvent` with a `ConditionId?`, it's implicitly converted to `ConditionIdRelayVar`
2. The event system invokes weather's (or other effects') `OnImmunity` handler
3. `EventHandlerAdapter.ResolveParameter` is called to resolve each parameter
4. For parameter 1 (`PokemonTypeConditionIdUnion type`), the adapter now:
   - Checks if the parameter type is `PokemonTypeConditionIdUnion`
   - Checks if the context has a RelayVar
   - If it's a `ConditionIdRelayVar`, unwraps the `ConditionId` and creates a new `PokemonTypeConditionIdUnion`
   - If it's a `PokemonTypeRelayVar`, unwraps the `PokemonType` and creates a new `PokemonTypeConditionIdUnion`
5. The handler is invoked successfully with the correct parameter type

---

## Union Type Context

**PokemonTypeConditionIdUnion** is used for immunity checks because immunity can apply to:
- **Type immunity**: e.g., Ground-type moves can't hit Flying-type Pokémon
- **Condition immunity**: e.g., Fire-type Pokémon can't be frozen in Sunny Day

The union allows a single `OnImmunity` handler signature to check both types of immunity.

**Constructor Signatures**:
```csharp
public PokemonTypeConditionIdUnion(PokemonType pokemonType)
public PokemonTypeConditionIdUnion(ConditionId conditionId)
```

**Properties**:
- `IsPokemonType` / `IsConditionId`: Discriminator
- `AsPokemonType` / `AsConditionId`: Accessors

---

## Testing

After the fix:
1. Build successful (verified)
2. Weather effects with immunity handlers (SunnyDay, RainDance, etc.) can now properly check condition immunity
3. Moves that apply volatile statuses work correctly during weather conditions

**Example Scenario**:
- SunnyDay weather is active
- SunnyDay has an `OnImmunity` handler that prevents Freeze status
- When a move tries to apply Freeze (or any volatile status), immunity is properly checked
- The `ConditionId.Freeze` is correctly passed to the handler as a `PokemonTypeConditionIdUnion`

---

## Related Issues

Similar parameter conversion issues may exist for other union types. If you encounter similar "Parameter X is non-nullable but no matching value found in context" errors:

1. Check what `RelayVar` type is being passed (look for implicit conversions in `RelayVar.cs`)
2. Check what parameter type the handler expects
3. Add conversion logic in `EventHandlerAdapter.ResolveParameter` if needed

**Pattern to follow**:
```csharp
// Check for [UnionType]
if (paramType == typeof([UnionType]))
{
    if (context.HasRelayVar)
    {
        // Handle [SourceRelayVar] -> [UnionType]
        if (context.RelayVar is [SourceRelayVar] var && var.[Value].HasValue)
        {
            return new [UnionType](var.[Value].Value);
        }
    }
}
```

---

## Keywords

`immunity`, `weather`, `SunnyDay`, `OnImmunity`, `PokemonTypeConditionIdUnion`, `ConditionIdRelayVar`, `EventHandlerAdapter`, `parameter conversion`, `union type`, `status immunity`
