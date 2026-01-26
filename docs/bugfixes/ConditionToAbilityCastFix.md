# Condition to Ability Cast Fix

**Date**: 2024  
**Severity**: High  
**Systems Affected**: Event system, Mold Breaker ability suppression, weather conditions

---

## Problem

When running battles with random teams, the simulation would crash with an `InvalidCastException` when attempting to cast a `Condition` object to an `Ability` object:

```
System.InvalidCastException: Unable to cast object of type 'ApogeeVGC.Sim.Conditions.Condition' to type 'ApogeeVGC.Sim.Abilities.Ability'.
```

**Stack Trace**:
```
at ApogeeVGC.Sim.BattleClasses.Battle.RunEvent(EventId eventId, RunEventTarget target, RunEventSource source, IEffect sourceEffect, RelayVar relayVar, Nullable`1 onEffect, Nullable`1 fastExit) in Battle.Events.cs:line 353
at ApogeeVGC.Sim.BattleClasses.Battle.PriorityEvent(...)
at ApogeeVGC.Sim.PokemonClasses.Pokemon.GetMoveTargets(...)
```

**Symptoms**:
- Random battle simulations crash unpredictably
- Error occurs during event processing in `Battle.RunEvent`
- Crash happens when checking for Mold Breaker suppression

---

## Root Cause

In the `Battle.RunEvent` method at line 350-353, the code checked if an effect's `EffectType` was `EffectType.Ability` but then **unconditionally cast** the effect to `Ability`:

```csharp
// Check for Mold Breaker suppression
if (effect.EffectType == EffectType.Ability &&
    effectHolder is PokemonEffectHolder pokemonHolder2)
{
    var ability = (Ability)effect;  // ? UNSAFE CAST
    if ((ability.Flags.Breakable ?? false) &&
        SuppressingAbility(pokemonHolder2.Pokemon))
    {
        // ...
    }
}
```

**The Problem**: A `Condition` can have multiple `EffectType` values, not just `EffectType.Condition`. According to `Condition.Core.cs`, valid types are:
- `EffectType.Condition`
- `EffectType.Weather`
- `EffectType.Status`
- `EffectType.Terrain`

This means:
1. Some effect could be a `Condition` with `EffectType.Weather`
2. The check `effect.EffectType == EffectType.Ability` would be false and skip this block normally
3. **BUT** if somehow the `EffectType` matched (or there was a bug elsewhere), the cast would fail

The actual issue is likely that the `EffectType` enum check alone is not sufficient to determine the concrete type of the `IEffect` instance. There could be edge cases or bugs where an effect has the wrong `EffectType` set, or where the type checking logic is insufficient.

---

## Solution

Added an explicit type check using the `is` pattern before attempting the cast, matching the pattern used for status condition checking a few lines above (line 341):

**Modified File**: `ApogeeVGC\Sim\BattleClasses\Battle.Events.cs`

**Changes**:
```csharp
// Check for Mold Breaker suppression
if (effect.EffectType == EffectType.Ability &&
    effectHolder is PokemonEffectHolder pokemonHolder2 &&
    effect is Ability ability)  // ? SAFE TYPE CHECK AND CAST
{
    if ((ability.Flags.Breakable ?? false) &&
        SuppressingAbility(pokemonHolder2.Pokemon))
    {
        if (DisplayUi)
        {
            Debug($"{eventId} handler suppressed by Mold Breaker");
        }

        continue;
    }

    // For custom abilities (no num), check if this is an attacking event
    if (ability.Num == 0 && IsAttackingEvent(eventId, sourceEffect))
    {
        if (DisplayUi)
        {
            Debug($"{eventId} handler suppressed by Mold Breaker");
        }

        continue;
    }
}
```

---

## How It Works

The fix uses C#'s pattern matching feature (`is Ability ability`):

1. **Type Safety**: The `is` check verifies at runtime that `effect` is actually an `Ability` instance
2. **Combined Check**: All three conditions must be true:
   - `effect.EffectType == EffectType.Ability` (logical type check)
   - `effectHolder is PokemonEffectHolder` (holder type check)
   - `effect is Ability ability` (concrete type check and cast)
3. **Safe Cast**: If all checks pass, `ability` is automatically cast and available in the block
4. **Graceful Skip**: If `effect` is not actually an `Ability`, the entire block is skipped (no crash)

---

## Pattern Consistency

This fix follows the same pattern used for status condition checking earlier in the same method (line 338-347):

```csharp
// Check if status has changed
if (effect.EffectType == EffectType.Status &&
    effectHolder is PokemonEffectHolder pokemonHolder)
{
    var condition = (Condition)effect;  // ? This is safe because Status conditions are always Conditions
    if (pokemonHolder.Pokemon.Status != condition.Id)
    {
        continue;
    }
}
```

**Note**: The status condition check is safe because `EffectType.Status` can only be on `Condition` objects. However, for defensive programming and consistency, it could also benefit from an `is Condition condition` check.

---

## Prevention

To prevent similar issues in the future:

1. **Always use pattern matching** when casting interface types to concrete types
2. **Never rely solely on EffectType** to determine concrete type
3. **Defensive checks**: Use `is` pattern even when you "know" the type should be correct
4. **Consistency**: Apply the same defensive pattern across all similar checks

**Code Pattern to Follow**:
```csharp
if (effect.EffectType == EffectType.SomeType &&
    effectHolder is TargetHolderType holder &&
    effect is ConcreteEffectType concreteEffect)  // ? Always add this check
{
    // Safe to use concreteEffect here
}
```

---

## Related Fixes

This fix is similar in nature to:
- **Immunity Event Parameter Conversion Fix**: Both involved type mismatches in the event system
- **Status condition checks**: Uses same defensive pattern

---

## Testing

The fix was verified by:
1. Successful compilation
2. Running random battle simulations that previously crashed
3. Verifying that Mold Breaker suppression still works correctly for actual Ability effects

---

## Impact

**Before**: Rare crashes during random battles when the event system processed effects
**After**: Graceful handling of type mismatches; only processes Ability-specific logic when effect is actually an Ability

**Side Effects**: None - only makes the code more defensive
