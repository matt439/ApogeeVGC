# FieldResidual Pokemon Parameter Nullability Fix

## Problem Summary
When a Pokémon battled with weather (e.g., Sunny Day) active and the FieldResidual event was triggered, the battle crashed with:
```
Event FieldResidual adapted handler failed on effect SunnyDay (Weather)
Event FieldResidual: Parameter 2 (Pokemon _) is non-nullable but no Pokemon found in context (TargetPokemon=False, SourcePokemon=False)
```

The error occurred during random battle testing when weather conditions triggered their residual handlers at the end of turn.

## Root Cause
The `OnFieldResidualEventInfo` handler signature declared the `Pokemon` parameter (parameter index 2) as non-nullable in its `ParameterNullability` array, but field-level events (weather, terrain, pseudo-weather) don't have a Pokemon context when invoked.

**Event Flow:**
1. `Battle.RunAction(ActionId.Residual)` calls `FieldEvent(EventId.Residual)` at turn end
2. `FieldEvent` invokes weather condition FieldResidual handlers
3. `InvokeEventHandlerInfo` is called with `FieldSingleEventTarget` (no Pokemon)
4. `EventHandlerAdapter.ResolveParameter` tries to resolve the Pokemon parameter
5. No Pokemon found in context ? exception thrown

**Handler Signature (before fix):**
```csharp
public sealed record OnFieldResidualEventInfo : EventHandlerInfo
{
    public OnFieldResidualEventInfo(
        Action<Battle, Field, Pokemon, IEffect> handler, ...)
    {
        ...
        // Pokemon parameter (index 2) marked as non-nullable
        ParameterNullability = new[] { false, false, false, false };
        ...
    }
}
```

**Actual Event Invocation:**
```csharp
// In Battle.Events.cs, line 904-905
returnVal = InvokeEventHandlerInfo(handler.HandlerInfo, false,
    new BoolRelayVar(true), 
    singleEventTarget,  // FieldSingleEventTarget, not PokemonSingleEventTarget
    null,               // No source
    null);              // No sourceEffect
```

## Solution
Mark the `Pokemon` parameter as nullable in `OnFieldResidualEventInfo.ParameterNullability` array, changing index 2 from `false` to `true`.

**File Modified:** `ApogeeVGC\Sim\Events\Handlers\FieldEventMethods\OnFieldResidualEventInfo.cs`

**Change:**
```csharp
// Before:
ParameterNullability = new[] { false, false, false, false };

// After:
// Pokemon parameter (index 2) can be null for field-level residual events like weather
ParameterNullability = new[] { false, false, true, false };
```

## Verification
All existing weather condition FieldResidual handlers use discard parameters (`_`) for the Pokemon argument, so they correctly handle null Pokemon:

**Example Handlers:**
- **SunnyDay** (ConditionsSTU.cs, line 989):
  ```csharp
  OnFieldResidual = new OnFieldResidualEventInfo((battle, _, _, _) => {...}, 1)
  ```
- **DeltaStream** (ConditionsDEF.cs, line 79):
  ```csharp
  OnFieldResidual = new OnFieldResidualEventInfo((battle, _, _, _) => {...}, 1)
  ```
- **PrimordialSea** (ConditionsPQR.cs, line 367):
  ```csharp
  OnFieldResidual = new OnFieldResidualEventInfo((battle, _, _, _) => {...}, 1)
  ```

## Impact
- Allows all weather/field conditions with `OnFieldResidual` handlers to execute correctly
- Matches the TypeScript behavior where field residual events don't require a Pokemon context
- Prevents crashes during end-of-turn residual processing

## Pattern
This follows the same pattern as previous nullable parameter fixes:
- **WindRiderNullSideConditionFix** - Side condition parameter nullable
- **RipenNullEffectFix** - Effect parameter nullable
- **DisguiseNullEffectFix** - Effect parameter nullable
- **SwitchInNullPokemonFix** - Pokemon parameter nullable

**General Rule:** When an event can be triggered at different scopes (field-level vs Pokemon-level), parameters specific to narrower scopes (e.g., Pokemon for field events) should be marked as nullable.

## Testing
The fix was verified by running random battles where weather conditions are active. The FieldResidual events now execute successfully without throwing parameter resolution exceptions.

---

**Keywords:** `FieldResidual`, `weather`, `SunnyDay`, `Pokemon parameter`, `nullable`, `EventHandlerAdapter`, `parameter nullability`, `OnFieldResidualEventInfo`, `field event`, `residual`, `turn end`, `DeltaStream`, `PrimordialSea`

**Category:** Event System Issues
**Severity:** High
**Systems Affected:** Field-level residual event handlers, weather conditions, pseudo-weather
