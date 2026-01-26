# ModifyAccuracy Event Parameter Nullability Fix

**Date**: 2025-01-XX  
**Severity**: High  
**Systems Affected**: ModifyAccuracy event handlers (abilities, items)

---

## Problem

The `ModifyAccuracy` event handler signature declared the `accuracy` parameter as `int` (non-nullable), but moves with true accuracy (always-hit moves) pass `BoolRelayVar(true)` instead of `IntRelayVar`. This caused a parameter resolution error when abilities like Hustle tried to process always-hit moves.

### Error Message
```
System.InvalidOperationException: Event ModifyAccuracy adapted handler failed on effect Hustle (Ability)
Inner Exception: Event ModifyAccuracy: Parameter 1 (Int32 accuracy) is non-nullable but no matching value found in context
```

### Stack Trace
The error occurred in:
- `EventHandlerAdapter.ResolveParameter` - Failed to unwrap `BoolRelayVar(true)` to `int`
- `Battle.InvokeEventHandlerInfo` - When invoking Hustle's `OnSourceModifyAccuracy` handler
- `BattleActions.HitStepAccuracy` - When running `ModifyAccuracy` event for always-hit moves

---

## Root Cause

1. **TypeScript vs C# Type Mismatch**: In TypeScript, the `onSourceModifyAccuracy` handler receives `accuracy` as `number | true`:
   ```typescript
   onSourceModifyAccuracy(accuracy, target, source, move) {
       if (move.category === 'Physical' && typeof accuracy === 'number') {
           return this.chainModify([3277, 4096]);
       }
   }
   ```
   The handler checks `typeof accuracy === 'number'` because moves with true accuracy (always hit) pass `true` instead of a number.

2. **C# Implementation**: The C# event handler signature was:
   ```csharp
   Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion> handler
   ```
   This expects `int accuracy` (non-nullable), which cannot represent the "true accuracy" case.

3. **RelayVar Conversion**: When a move has `IntTrueUnion` accuracy:
   - `IntIntTrueUnion` ? `IntRelayVar` (numeric accuracy)
   - `TrueIntTrueUnion` ? `BoolRelayVar(true)` (always hits)
   
   The `EventHandlerAdapter.TryUnwrapRelayVar` method could unwrap `IntRelayVar` to `int`, but not `BoolRelayVar(true)` to `int`.

---

## Solution

### 1. Updated Event Handler Signatures
Changed all `ModifyAccuracy` event handler signatures to use `int?` (nullable int):

**Files Updated**:
- `OnModifyAccuracyEventInfo.cs`
- `OnSourceModifyAccuracyEventInfo.cs`
- `OnFoeModifyAccuracyEventInfo.cs`
- `OnAllyModifyAccuracyEventInfo.cs`
- `OnAnyModifyAccuracyEventInfo.cs`

**Before**:
```csharp
Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion> handler
ExpectedParameterTypes = [typeof(Battle), typeof(int), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
ParameterNullability = [false, false, false, false, false];
```

**After**:
```csharp
Func<Battle, int?, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion> handler
ExpectedParameterTypes = [typeof(Battle), typeof(int?), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
ParameterNullability = [false, true, false, false, false];
```

### 2. Updated EventHandlerAdapter
Added support for converting `BoolRelayVar(true)` to `null` when the target type is `int?`:

```csharp
// Handle nullable int (int?)
if (relayVar is IntRelayVar intVarNullable && targetType == typeof(int?))
{
    value = intVarNullable.Value;
    return true;
}

// Handle BoolRelayVar(true) -> null for nullable int (for accuracy events)
// In TypeScript, moves with true accuracy pass `true`, which maps to null in C#
if (relayVar is BoolRelayVar { Value: true } && targetType == typeof(int?))
{
    value = null;
    return true;
}
```

### 3. Updated All Handler Implementations
Updated all abilities and items using `ModifyAccuracy` events to check `accuracy.HasValue` before modifying:

**Affected Abilities**:
- Hustle (`OnSourceModifyAccuracy`)
- Compound Eyes (`OnSourceModifyAccuracy`)
- Sand Veil (`OnModifyAccuracy`)
- Snow Cloak (`OnModifyAccuracy`)
- Tangled Feet (`OnModifyAccuracy`)
- Victory Star (`OnAllyModifyAccuracy`)

**Affected Items**:
- Wide Lens (`OnSourceModifyAccuracy`)
- Zoom Lens (`OnSourceModifyAccuracy`)
- Bright Powder (`OnModifyAccuracy`)

**Example - Hustle**:

**Before**:
```csharp
OnSourceModifyAccuracy = new OnSourceModifyAccuracyEventInfo(
    (battle, accuracy, _, _, move) =>
    {
        if (move.Category == MoveCategory.Physical)
        {
            battle.ChainModify([3277, 4096]);
            return battle.FinalModify(accuracy);  // Error: accuracy is int?
        }
        return accuracy;
    }, -1),
```

**After**:
```csharp
OnSourceModifyAccuracy = new OnSourceModifyAccuracyEventInfo(
    (battle, accuracy, _, _, move) =>
    {
        // Only modify accuracy for physical moves with numeric accuracy
        // (TypeScript checks: move.category === 'Physical' && typeof accuracy === 'number')
        if (move.Category == MoveCategory.Physical && accuracy.HasValue)
        {
            battle.ChainModify([3277, 4096]);
            return battle.FinalModify(accuracy.Value);  // Unwrap nullable
        }
        return accuracy;
    }, -1),
```

---

## Semantic Mapping

| TypeScript | C# |
|------------|-----|
| `accuracy: number \| true` | `accuracy: int?` |
| `typeof accuracy === 'number'` | `accuracy.HasValue` |
| `accuracy` (when number) | `accuracy.Value` |
| `true` (always hit) | `null` |

---

## Testing

### Test Case 1: Hustle with Always-Hit Move
**Before**: Exception thrown when Pokemon with Hustle uses an always-hit move  
**After**: Handler correctly skips modification when `accuracy == null`

### Test Case 2: Hustle with Normal Move
**Before**: N/A (didn't reach this due to exception)  
**After**: Handler correctly modifies accuracy when `accuracy.HasValue` is true

### Test Case 3: Other Abilities
Verified all other `ModifyAccuracy` handlers (Sand Veil, Snow Cloak, Tangled Feet, Victory Star, Compound Eyes) correctly check `accuracy.HasValue` before modification.

---

## Related Issues

This fix is similar to previous parameter type mismatches:
- **ImmunityEventParameterConversionFix**: `ConditionIdRelayVar` not converted to `PokemonTypeConditionIdUnion`
- **Stat Modification Parameter Nullability Fix**: Incorrect nullability constraints on stat modification parameters

---

## Lessons Learned

1. **Always check TypeScript signatures for union types**: When a TypeScript parameter can be multiple types (e.g., `number | true`), the C# equivalent should use nullable types or unions.

2. **Type guards translate to null checks**: TypeScript's `typeof x === 'type'` maps to C#'s `x.HasValue` (for nullables) or `x is Type` (for union types).

3. **Document semantic equivalence**: When translating TypeScript to C#, document how TypeScript type guards map to C# null checks or type checks.

4. **Parameter nullability matters**: Setting `ParameterNullability` correctly in `EventHandlerInfo` is critical for proper parameter resolution.

---

## Keywords

`ModifyAccuracy`, `accuracy`, `nullable`, `int?`, `Hustle`, `event parameter`, `type mismatch`, `BoolRelayVar`, `IntRelayVar`, `always-hit moves`
