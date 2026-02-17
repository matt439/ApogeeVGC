# Disguise CriticalHit Parameter Type Mismatch Fix

## Problem Summary

When Braviary used Shadow Claw targeting Mimikyu (with Disguise ability), the battle crashed with:
```
InvalidOperationException: EventHandlerInfo validation failed for event CriticalHit on effect Disguise (Ability): 
Event CriticalHit: Parameter 2 (_) type mismatch. Expected: Pokemon, Got: Object
```

The error occurred during the critical hit calculation in `GetDamage()` when the `CriticalHit` event was invoked on line 115 of `BattleActions.Damage.cs`.

## Root Cause

The `Validate()` method in `EventHandlerInfo.cs` (and its `new` override in `UnionEventHandlerInfo.cs`) was checking delegate parameter type compatibility in the **wrong direction** for delegate parameter contravariance.

### The Issue

For delegate parameters, contravariance applies: a delegate with a **more general** parameter type (like `object`) can accept **more specific** arguments (like `Pokemon`). The validation was checking:

```csharp
// WRONG: Checks if Pokemon can accept object (false!)
if (!expectedBase.IsAssignableFrom(actualBase))
```

But the correct check for delegates is:

```csharp
// CORRECT: Checks if object can accept Pokemon (true!)
if (!actualBase.IsAssignableFrom(expectedBase))
```

### Why This Happened

The Disguise ability's `OnCriticalHit` handler uses this delegate signature:
```csharp
Func<Battle, Pokemon, object?, Move, BoolVoidUnion>
//                     ^^^^^^^ Parameter 2: object? (from union type definition)
```

But `OnCriticalHitEventInfo` declares `ExpectedParameterTypes` as:
```csharp
[typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)]
//                                ^^^^^^^^^^^^^^^^ Parameter 2: Pokemon
```

The union type `OnCriticalHit` uses `object?` for the source parameter because:
1. The TypeScript reference has `source: null` (can be any type)
2. C# lambdas with discard parameters (`_`) infer nullable types
3. Using `object?` allows the delegate to accept any Pokemon type argument (contravariance)

The validation was incorrectly rejecting this valid delegate signature.

### Call Stack

```
GetDamage (line 115)
  → Battle.RunEvent(EventId.CriticalHit, ...)
    → Battle.InvokeEventHandlerInfo (line 1009)
      → handlerInfo.Validate()
        → EventHandlerInfo.Validate() (line 201) ❌ WRONG DIRECTION CHECK
```

## Solution

Fixed the parameter type validation direction in **both** `EventHandlerInfo.Validate()` and `UnionEventHandlerInfo.Validate()` to correctly check delegate parameter contravariance:

### EventHandlerInfo.cs (Line 197-207)
```csharp
// For reference types, check if either is nullable reference type
if (!expectedBase.IsValueType && !actualBase.IsValueType)
{
    // Reference type comparison - delegate parameter contravariance:
    // The actual delegate parameter type must be assignable from the expected type
    // (e.g., delegate taking 'object' can accept 'Pokemon' arguments)
    if (!actualBase.IsAssignableFrom(expectedBase))  // ← FIXED DIRECTION
    {
        throw new InvalidOperationException(
            $"Event {Id}: Parameter {i} ({actualParams[i].Name}) type mismatch. " +
            $"Expected: {expectedType.Name}, Got: {actualType.Name}");
    }
}
```

### UnionEventHandlerInfo.cs (Line 87-97)
Applied the same fix to the `new` override of `Validate()`:

```csharp
if (!expectedBase.IsValueType && !actualBase.IsValueType)
{
    // Reference type comparison - delegate parameter contravariance:
    // The actual delegate parameter type must be assignable from the expected type
    // (e.g., delegate taking 'object' can accept 'Pokemon' arguments)
    if (!actualBase.IsAssignableFrom(expectedBase))  // ← FIXED DIRECTION
    {
        throw new InvalidOperationException(
            $"Event {Id}: Parameter {i} ({actualParams[i].Name}) type mismatch. " +
            $"Expected: {expectedType.Name}, Got: {actualType.Name}");
    }
}
```

### OnCriticalHitEventInfo.cs (Line 40-41)
Also updated the source Pokemon parameter nullability since `BattleActions.Damage.cs` line 116 passes `null`:

```csharp
// Nullability: Battle (non-null), target (non-null), source (nullable - can be null from RunEvent), move (non-null)
ParameterNullability = [false, false, true, false];  // ← source is now nullable
```

## Key Concepts

### Delegate Variance

- **Covariance (return types)**: Delegate can return a **more specific** type than declared
  - Check: `expectedBase.IsAssignableFrom(actualBase)` ✓ (unchanged, correct)
  
- **Contravariance (parameter types)**: Delegate can accept **more general** parameters than declared
  - Check: `actualBase.IsAssignableFrom(expectedBase)` ✓ (fixed in this PR)

### Why `object?` Works for Source Parameter

A delegate with `object?` parameter can safely accept `Pokemon` arguments because:
1. `object` is assignable from `Pokemon` (all reference types derive from object)
2. The nullable annotation (`?`) allows null values
3. The handler discards the parameter (`_`) so it never actually uses it
4. Contravariance ensures type safety

## Files Modified

| File | Change |
|------|--------|
| `EventHandlerInfo.cs` | Fixed parameter type validation direction (line 201) |
| `UnionEventHandlerInfo.cs` | Fixed parameter type validation direction (line 91) |
| `OnCriticalHitEventInfo.cs` | Updated source parameter nullability to `true` (line 41) |

## Testing

Verified with the original failing seeds:
```csharp
debugTeam1Seed = 59087
debugTeam2Seed = 72657
debugPlayer1Seed = 17113
debugPlayer2Seed = 6587
debugBattleSeed = 14646
```

Battle now completes successfully when Shadow Claw targets Mimikyu, and the Disguise ability's `OnCriticalHit` handler executes correctly.

## Pattern Recognition

This fix applies to **all event handlers** where:
1. The delegate uses a base/interface type for a parameter (e.g., `object`, `IEffect`, `Move`)
2. The `ExpectedParameterTypes` declares a derived/concrete type (e.g., `Pokemon`, `Ability`, `ActiveMove`)
3. The actual arguments passed are the derived/concrete type

The contravariance fix ensures these valid delegate signatures are accepted by validation.

## Related Issues

- This is the **base class fix** for the same issue discovered in `UnionEventHandlerInfo<T>` 
- The earlier fix only updated `UnionEventHandlerInfo.Validate()` which uses `new` (method hiding)
- Since `InvokeEventHandlerInfo` calls through a base `EventHandlerInfo` reference, the base class's `Validate()` was being invoked
- This fix ensures both code paths (direct and polymorphic) use the correct validation logic

## Keywords

`EventHandlerInfo`, `UnionEventHandlerInfo`, `Validate`, `delegate contravariance`, `parameter type`, `IsAssignableFrom`, `Disguise`, `OnCriticalHit`, `object`, `Pokemon`, `type mismatch`, `validation`, `Shadow Claw`, `Mimikyu`, `event system`, `type safety`
