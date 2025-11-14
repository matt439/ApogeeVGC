# Union Type Conversion Issues: VoidReturn, Undefined, and false

## Overview
This document catalogs incorrect conversions between union types that include `VoidReturn`, `Undefined`, or `false`. In Pokemon Showdown's TypeScript source, these values have distinct semantic meanings:

- **`null`**: NOT_FAIL / continue processing / no specific result
- **`undefined`**: No value / move continues without dealing damage
- **`false`**: Explicit failure
- **`void`**: Explicit absence of return value (different from null)

Mixing these up causes bugs in battle logic.

---

## Issues Found and Fixed

### 1. EventHandlerAdapter: VoidReturn ? null Conversion ? FIXED
**Location**: `ApogeeVGC\Sim\Events\EventHandlerAdapter.cs`, `ConvertReturnValue` method

**Issue**:
```csharp
// OLD - VoidReturn means no value
VoidReturn => null,
```

**Problem**: Converts `VoidReturn` to `null`, losing the semantic distinction between:
- `VoidReturn`: Explicit absence/void return
- `null`: NOT_FAIL / continue processing

**Fix Applied**:
- Added `VoidReturnRelayVar` type to `RelayVar` union
- Updated `ConvertReturnValue` to preserve VoidReturn:
```csharp
VoidReturn => new VoidReturnRelayVar(),
VoidBoolVoidUnion => new VoidReturnRelayVar(),
VoidDoubleVoidUnion => new VoidReturnRelayVar(),
VoidIntVoidUnion => new VoidReturnRelayVar(),
VoidIntBoolVoidUnion => new VoidReturnRelayVar(),
```

---

### 2. BattleActions.TryPrimaryHitEvent: null ? true Conversion ? FIXED
**Location**: `ApogeeVGC\Sim\BattleClasses\BattleActions.MoveHit.cs`, `TryPrimaryHitEvent` method

**Issue**:
```csharp
// OLD
null => BoolIntUndefinedUnion.FromBool(true), // loses distinction
```

**Problem**: Converts `null` (meaning NOT_FAIL/continue) to `true` (explicit success), losing semantic distinction.

**Fix Applied**:
- Added explicit `VoidReturnRelayVar` case
- Added clarifying comments:
```csharp
null => BoolIntUndefinedUnion.FromBool(true), // null means NOT_FAIL/continue
VoidReturnRelayVar => BoolIntUndefinedUnion.FromBool(true), // VoidReturn also means continue
```

**Note**: In this specific context, both null and VoidReturn semantically mean "allow move to continue", so converting to `true` is correct behavior. The fix adds the VoidReturn case and clarifies intent.

---

### 3. UndefinedBoolIntUndefinedUnion.ToInt(): Undefined ? 0 ? FIXED
**Location**: `ApogeeVGC\Sim\Utils\Unions\GenericUnion.cs`, `UndefinedBoolIntUndefinedUnion` record

**Issue**:
```csharp
// OLD
public override int ToInt() => 0;  // conflates undefined with zero!
```

**Problem**: Converts `undefined` to integer `0`, conflating undefined with zero.

**Fix Applied**:
```csharp
public override int ToInt() =>
    throw new InvalidOperationException(
        "Cannot convert Undefined to int. Undefined and 0 are semantically different. " +
        "Check for Undefined before calling ToInt().");
```

**Impact**: Code must now explicitly check for Undefined before calling ToInt(), preventing silent bugs.

---

### 4. EmptyBoolIntEmptyUndefinedUnion.IsZero(): Empty ? true ? FIXED
**Location**: `ApogeeVGC\Sim\Utils\Unions\GenericUnion.cs`, `EmptyBoolIntEmptyUndefinedUnion` record

**Issue**:
```csharp
// OLD
public override bool IsZero() => true;  // WRONG!
```

**Problem**: Treats `Empty` (Battle.NOT_FAIL / "") as equivalent to zero.

**Fix Applied**:
```csharp
public override bool IsZero() => false; // Empty (NOT_FAIL) is not zero
```

**Impact**: Empty now correctly distinguished from 0:
- `Empty`: Move didn't fail, continue processing
- `0`: Move dealt 0 damage (still counts as a hit for abilities like Static)

---

### 5. BattleActions.CombineResults: null + null ? Undefined ? FIXED
**Location**: `ApogeeVGC\Sim\BattleClasses\BattleActions.ResultCombining.cs`, `CombineResults` method

**Issue**:
```csharp
// OLD
case null when right == null:
    return BoolIntEmptyUndefinedUnion.FromUndefined();  // WRONG!
```

**Problem**: When both operands are null, returns Undefined instead of preserving null/NOT_FAIL meaning.

**Fix Applied**:
```csharp
// When both are null (NOT_FAIL/continue), return Empty to represent NOT_FAIL
// This preserves the semantic meaning that neither result failed
case null when right == null:
    return BoolIntEmptyUndefinedUnion.FromEmpty();
```

**Impact**: Combining two null results (both meaning "continue/NOT_FAIL") now produces Empty (NOT_FAIL), preserving semantic meaning.

---

## Summary of Changes

### Files Modified:
1. `ApogeeVGC\Sim\Utils\Unions\SpecificUnion.cs` - Added `VoidReturnRelayVar`
2. `ApogeeVGC\Sim\Events\EventHandlerAdapter.cs` - Fixed VoidReturn handling
3. `ApogeeVGC\Sim\BattleClasses\BattleActions.MoveHit.cs` - Added VoidReturn case
4. `ApogeeVGC\Sim\Utils\Unions\GenericUnion.cs` - Fixed ToInt() and IsZero()
5. `ApogeeVGC\Sim\BattleClasses\BattleActions.ResultCombining.cs` - Fixed null combining

### Build Status:
? All changes compile successfully
? No compilation errors

---

## Semantic Meaning Reference

For clarity, here are the intended meanings of these special values in Pokemon Showdown:

| Value | Meaning | Example Use Case |
|-------|---------|------------------|
| `null` | NOT_FAIL / continue processing | Event handler allows action to continue |
| `undefined` | No value / move continues without damage | Move doesn't calculate damage |
| `false` | Explicit failure | Move blocked by Protect |
| `void` | Explicit void return | Event handler has no return value |
| `Empty` ("") | NOT_FAIL / not a failure | Move succeeded without specific result |
| `0` | Zero value (but still valid) | 0 damage dealt but move still hit |
| `true` | Explicit success | Move successfully hit target |

---

## Recommendations for Future Work

1. **Add Comprehensive Unit Tests**:
   - Test that null ? undefined ? false ? 0
   - Test that VoidReturn is preserved through event chains
   - Test that Empty (NOT_FAIL) has correct semantics
   - Test CombineResults with all combinations

2. **Add Code Analysis Rules**:
   - Warn when calling ToInt() on types that can be Undefined
   - Warn when comparing Undefined/Empty/null without explicit checks

3. **Documentation**:
   - Add XML documentation to union types explaining semantics
   - Add examples of correct usage patterns

4. **Consider Adding Helper Methods**:
   - `IsNotFail()` method to check for null/Empty/VoidReturn
   - `IsActualZero()` to distinguish 0 from Empty/Undefined

---

## Priority

**HIGH** - These issues have been fixed and will prevent incorrect battle simulation results.

## Status

? **ALL CRITICAL ISSUES FIXED** - Build successful, ready for testing
