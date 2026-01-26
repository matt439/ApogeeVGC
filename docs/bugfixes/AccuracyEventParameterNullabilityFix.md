# Lock-On Accuracy Event Parameter Fix

**Date**: 2026-01-26  
**Severity**: High  
**Systems Affected**: Accuracy event handlers (all prefixes: Base, Source, Foe, Ally, Any)

---

## Problem

When Lock-On condition's `OnSourceAccuracy` handler was invoked for an always-hit move, the battle crashed with the following exception:

```
System.InvalidOperationException: Event Accuracy adapted handler failed on effect Lock-On (Condition)

Inner Exception:
InvalidOperationException: Event Accuracy: Parameter 1 (Int32 _) is non-nullable but no matching value found in context
```

**Symptoms**:
- Battle crashes when Accuracy event is triggered for always-hit moves with Lock-On active
- Stack trace shows failure in `EventHandlerAdapter.ResolveParameter`
- Error indicates `int` parameter type cannot handle `BoolRelayVar(true)` for always-hit accuracy

**Affected Code Flow**:
1. Move uses accuracy check during `BattleActions.HitStepAccuracy`
2. Move has `IntTrueUnion` accuracy (can be numeric or always-hit)
3. `Battle.RunEvent(EventId.Accuracy, ...)` is called with `RelayVar.FromIntTrueUnion(accuracy)`
4. For always-hit moves, this creates `BoolRelayVar(true)`
5. Lock-On's `OnSourceAccuracy` handler is triggered
6. `EventHandlerAdapter.ResolveParameter` fails to convert `BoolRelayVar(true)` to `int` parameter

---

## Root Cause

The `Accuracy` event handler signatures (all variants) declared the `accuracy` parameter as `int` (non-nullable), but moves with true accuracy (always-hit moves) pass `BoolRelayVar(true)` instead of `IntRelayVar`. This is identical to the ModifyAccuracy event issue documented in `ModifyAccuracyEventParameterNullabilityFix.md`.

**Code Context**:

In `BattleActions.HitSteps.cs` line 425-426:
```csharp
RelayVar? finalAccuracyEvent = Battle.RunEvent(EventId.Accuracy, target, pokemon,
    move, RelayVar.FromIntTrueUnion(accuracy));
```

The `accuracy` parameter (type `IntTrueUnion`) is converted to `RelayVar`:
- `IntIntTrueUnion` ? `IntRelayVar` (numeric accuracy)
- `TrueIntTrueUnion` ? `BoolRelayVar(true)` (always hits)

**Event Handler Signatures**: All Accuracy event handler info classes expected:
```csharp
Func<Battle, int, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?> handler
```

Where `int accuracy` (non-nullable) cannot represent the "always-hit" case.

**The Gap**: Same as ModifyAccuracy - the `EventHandlerAdapter` could unwrap `IntRelayVar` to `int`, but not `BoolRelayVar(true)` to a non-nullable `int`.

---

## Solution

### 1. Updated Event Handler Signatures
Changed all `Accuracy` event handler signatures to use `int?` (nullable int):

**Files Updated**:
- `OnAccuracyEventInfo.cs`
- `OnSourceAccuracyEventInfo.cs`
- `OnFoeAccuracyEventInfo.cs`
- `OnAllyAccuracyEventInfo.cs`
- `OnAnyAccuracyEventInfo.cs`

**Before**:
```csharp
Func<Battle, int, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?> handler
ExpectedParameterTypes = [typeof(Battle), typeof(int), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
ParameterNullability = [false, false, false, false, false];
```

**After**:
```csharp
Func<Battle, int?, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?> handler
ExpectedParameterTypes = [typeof(Battle), typeof(int?), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
ParameterNullability = [false, true, false, false, false];
```

### 2. EventHandlerAdapter Already Supported
The `EventHandlerAdapter.TryUnwrapRelayVar` method already had support for converting `BoolRelayVar(true)` to `null` when the target type is `int?` (added in the ModifyAccuracy fix):

```csharp
// Handle BoolRelayVar(true) -> null for nullable int (for accuracy events)
// In TypeScript, moves with true accuracy pass `true`, which maps to null in C#
if (relayVar is BoolRelayVar { Value: true } && targetType == typeof(int?))
{
    value = null;
    return true;
}
```

### 3. Updated Handler Implementations
Updated handler implementations that use the `accuracy` parameter:

**Affected Conditions**:
- Micle Berry (`OnSourceAccuracy` in `ConditionsMNO.cs`)
- Minimize (`OnAccuracy` in `ConditionsMNO.cs`)

**Lock-On** (`OnSourceAccuracy` in `ConditionsJKL.cs`) - No changes needed, uses discard parameter `_`

**Example - Micle Berry**:

**Before**:
```csharp
OnSourceAccuracy = new OnSourceAccuracyEventInfo((battle, accuracy, _, source, move) =>
{
    if (move.Ohko != null) return null;
    
    battle.Add("-enditem", source, "Micle Berry");
    source.RemoveVolatile(_library.Conditions[ConditionId.MicleBerry]);
    
    int modifiedAccuracy = (int)Math.Floor(accuracy * 4915.0 / 4096.0);  // Error: accuracy is int?
    return IntBoolVoidUnion.FromInt(modifiedAccuracy);
}),
```

**After**:
```csharp
OnSourceAccuracy = new OnSourceAccuracyEventInfo((battle, accuracy, _, source, move) =>
{
    if (move.Ohko != null) return null;
    
    // Only modify numeric accuracy (TypeScript: typeof accuracy === 'number')
    // Always-hit moves (accuracy == null) pass through unchanged
    if (!accuracy.HasValue) return null;
    
    battle.Add("-enditem", source, "Micle Berry");
    source.RemoveVolatile(_library.Conditions[ConditionId.MicleBerry]);
    
    int modifiedAccuracy = (int)Math.Floor(accuracy.Value * 4915.0 / 4096.0);
    return IntBoolVoidUnion.FromInt(modifiedAccuracy);
}),
```

**Example - Minimize**:

**Before**:
```csharp
OnAccuracy = new OnAccuracyEventInfo((_, accuracy, _, _, move) =>
{
    MoveId[] stompingMoves = [...];
    if (stompingMoves.Contains(move.Id))
    {
        return IntBoolVoidUnion.FromBool(true);
    }
    return IntBoolVoidUnion.FromInt(accuracy);  // Error: accuracy is int?
}),
```

**After**:
```csharp
OnAccuracy = new OnAccuracyEventInfo((_, accuracy, _, _, move) =>
{
    MoveId[] stompingMoves = [...];
    if (stompingMoves.Contains(move.Id))
    {
        return IntBoolVoidUnion.FromBool(true);
    }
    // Pass through accuracy unchanged (null for always-hit, value for numeric)
    return accuracy.HasValue 
        ? IntBoolVoidUnion.FromInt(accuracy.Value) 
        : null;
}),
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

### Test Case 1: Lock-On with Always-Hit Move
**Before**: Exception thrown when Pokemon with Lock-On active uses always-hit move  
**After**: Handler correctly receives `null` for accuracy parameter

### Test Case 2: Micle Berry with Normal Move
**Before**: Exception if accuracy is null  
**After**: Handler correctly modifies accuracy when `accuracy.HasValue` is true, passes through null unchanged

### Test Case 3: Minimize with Stomping Move
**Before**: Exception if accuracy is null  
**After**: Handler correctly returns true for stomping moves, passes through null for always-hit moves

---

## Related Issues

This fix is identical in pattern to:
- **ModifyAccuracyEventParameterNullabilityFix**: Same issue with `ModifyAccuracy` event
- **ImmunityEventParameterConversionFix**: `ConditionIdRelayVar` not converted to `PokemonTypeConditionIdUnion`

All three follow the same pattern:
1. TypeScript signature uses union type (`number | true`)
2. C# signature initially used non-nullable primitive (`int`)
3. Fix: Change to nullable type (`int?`)
4. Update implementations to check `HasValue` before using `.Value`

---

## Lessons Learned

1. **Event parameter types must match RelayVar types**: When an event can receive different RelayVar types (e.g., `IntRelayVar` or `BoolRelayVar`), the parameter type must support both (e.g., `int?`).

2. **TypeScript union types map to C# nullable types**: `number | true` in TypeScript maps to `int?` in C#, where `true` = `null`.

3. **Check all event variants**: When fixing an event handler signature, check for all prefix variants (Source, Foe, Ally, Any).

4. **Existing adapter logic helps**: The `EventHandlerAdapter.TryUnwrapRelayVar` method added for ModifyAccuracy automatically worked for Accuracy events too.

5. **Handler implementations must check nullability**: When changing a parameter from `T` to `T?`, all implementations must check `HasValue` before accessing `.Value`.

---

## Keywords

`Accuracy`, `accuracy`, `nullable`, `int?`, `Lock-On`, `event parameter`, `type mismatch`, `BoolRelayVar`, `IntRelayVar`, `always-hit moves`, `Micle Berry`, `Minimize`
