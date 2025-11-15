# Union Type Handling Guide - Recurring Issues Prevention

## Overview
This guide documents recurring issues with union type handling in the ApogeeVGC battle simulator, particularly around `Empty`, `Undefined`, and various union return types from event handlers. Use this as a checklist to audit the codebase and prevent these issues from occurring.

---

## Issue Categories

### 1. Missing Union Type Conversions in EventHandlerAdapter

**Location**: `ApogeeVGC\Sim\Events\EventHandlerAdapter.cs`

**Problem**: The `ConvertReturnValue` method may not handle all union type variants that event handlers can return.

**Known Union Types to Check**:
```csharp
// BoolVoidUnion variants
BoolBoolVoidUnion
VoidBoolVoidUnion

// DoubleVoidUnion variants
DoubleDoubleVoidUnion
VoidDoubleVoidUnion

// IntVoidUnion variants
IntIntVoidUnion
VoidIntVoidUnion

// IntBoolVoidUnion variants
IntIntBoolVoidUnion
BoolIntBoolVoidUnion
VoidIntBoolVoidUnion

// BoolIntEmptyVoidUnion variants (CRITICAL - Often missed)
BoolBoolIntEmptyVoidUnion
IntBoolIntEmptyVoidUnion
EmptyBoolIntEmptyVoidUnion  ?? FIXED - Returns Empty (NOT_FAIL)
VoidUnionBoolIntEmptyVoidUnion

// BoolIntUndefinedUnion variants
IntBoolIntUndefinedUnion
BoolBoolIntUndefinedUnion
UndefinedBoolIntUndefinedUnion  ?? Check this

// IntFalseUndefinedUnion variants
IntIntFalseUndefined
FalseIntFalseUndefined
UndefinedIntFalseUndefined

// Other union types to investigate
BoolStringUnion
IntTrueUnion
StringNumberDelegateObjectUnion
```

**Search Strategy**:
```bash
# Search for all union type files
*.Union.cs in ApogeeVGC\Sim\Utils\Unions\

# Search for union type usages in event handlers
Search for: "return new.*Union" in *.cs
Search for: ": .*Union$" in event handler files
```

**Fix Template**:
```csharp
// In EventHandlerAdapter.ConvertReturnValue
return returnValue switch
{
    // Add missing union variant conversions
    NewUnionTypeVariant value => new AppropriateRelayVar(value.Value),
  VoidVariant => new VoidReturnRelayVar(),
    UndefinedVariant => new UndefinedRelayVar(),
    EmptyVariant => new UndefinedRelayVar(), // Empty = NOT_FAIL
    // ... existing cases
};
```

---

### 2. Undefined/Empty Handling in Success Checks

**Locations to Check**:
- `ApogeeVGC\Sim\BattleClasses\BattleActions.MoveEffects.cs` ? FIXED
- `ApogeeVGC\Sim\BattleClasses\BattleActions.MoveHit.cs`
- `ApogeeVGC\Sim\BattleClasses\BattleActions.HitSteps.cs`
- `ApogeeVGC\Sim\BattleClasses\BattleActions.Damage.cs`
- Any file that checks move/action success

**Problem**: Code treating `Undefined` or `Empty` as failure when they mean NOT_FAIL (success without quantifiable result).

**Search Strategy**:
```csharp
// Search patterns
"switch.*didAnything"
"if.*== false"
"!= true"
"== 0" // In context of damage/result checking
"moveSucceeded"
"success"
```

**Common Anti-Patterns**:
```csharp
// ? BAD - Treats Undefined as failure
bool success = result switch
{
    IntResult intResult => intResult.Value > 0,
    BoolResult boolResult => boolResult.Value,
  _ => false  // This catches Undefined/Empty and treats as failure!
};

// ? GOOD - Explicitly handles Undefined
bool success = result switch
{
    IntResult intResult => intResult.Value > 0,
    BoolResult boolResult => boolResult.Value,
    UndefinedResult => true,  // NOT_FAIL = success
    EmptyResult => true,   // NOT_FAIL = success
 _ => false  // Now only null/false are failures
};
```

**Critical Contexts**:
1. Move execution success determination
2. Hit effect application
3. Status condition application
4. Damage calculation results
5. Volatile condition addition
6. Event handler return processing

---

### 3. Event Handler Return Type Mismatches

**Locations**: All event handler definitions in `ApogeeVGC\Sim\Events\Handlers\`

**Problem**: Event handlers returning types that don't match their declared signature or that aren't handled by the adapter.

**Files to Audit**:
```
ApogeeVGC\Sim\Events\Handlers\EventMethods\*.cs
ApogeeVGC\Sim\Events\Handlers\ConditionSpecific\*.cs
ApogeeVGC\Sim\Events\Handlers\ItemSpecific\*.cs
ApogeeVGC\Sim\Events\Handlers\MoveEventMethods\*.cs
ApogeeVGC\Data\Conditions.cs
ApogeeVGC\Data\Moves.cs
ApogeeVGC\Data\Abilities.cs
ApogeeVGC\Data\Items.cs
```

**Search Strategy**:
```csharp
// Look for event handler definitions
"OnTryHit.*new OnTryHitEventInfo"
"OnStart.*new OnStartEventInfo"
"OnEnd.*new OnEndEventInfo"
"OnResidual.*new OnResidualEventInfo"
// etc. for all event types

// Look for returns in lambdas
"=> {" followed by "return new"
```

**Red Flags**:
- Returning `new Empty()` without verifying it's in a union type that includes Empty
- Returning bare `Undefined` vs `UndefinedRelayVar`
- Returning `0` when `Undefined` would be more appropriate
- Mixing `VoidReturn` and `void` returns

---

### 4. Missing Null Checks Before Union Access

**Problem**: Accessing `.Value` on union types without checking which variant it is.

**Search Strategy**:
```csharp
// Search patterns
"\.Value" // Look for direct .Value access
"as.*Union" // Look for union type casts
"is.*Union.*\." // Look for is-pattern with member access
```

**Anti-Pattern**:
```csharp
// ? BAD - Doesn't check union variant
var result = someEvent.Invoke();
int value = result.Value; // Will fail if result is Undefined/Empty/Void variant

// ? GOOD - Check variant first
var result = someEvent.Invoke();
int value = result switch
{
    IntVariant intValue => intValue.Value,
    UndefinedVariant => 0,  // or appropriate default
    _ => 0
};
```

---

### 5. Inconsistent NOT_FAIL Representation

**Problem**: Different parts of the code use different representations for "success with no quantifiable result":
- `Empty` (from Pokemon Showdown)
- `Undefined`
- `null`
- `0`
- Empty string `""`

**Search Strategy**:
```csharp
// Look for patterns that might indicate NOT_FAIL
"return 0" // In event handlers
"return null" // In event handlers
"return new Empty()"
"return new Undefined()"
'return ""' // Empty string
"Battle.NOT_FAIL" // Legacy constant
```

**Standardization**:
- Event handlers should return `Empty` (wrapped in appropriate union)
- Damage calculations should return `Undefined` (wrapped in appropriate union)
- Never use `null` to mean NOT_FAIL (null should mean no result/no action)
- Never use `0` to mean NOT_FAIL (0 means "did 0 damage" which is different)

---

## Audit Checklist

### Phase 1: Union Type Inventory
- [ ] List all union types in `ApogeeVGC\Sim\Utils\Unions\`
- [ ] For each union type, document all variants
- [ ] Check `EventHandlerAdapter.ConvertReturnValue` has cases for all variants
- [ ] Add missing conversion cases

### Phase 2: Event Handler Audit
- [ ] Audit all event handlers in `Data\Conditions.cs`
- [ ] Audit all event handlers in `Data\Moves.cs`
- [ ] Audit all event handlers in `Data\Abilities.cs`
- [ ] Audit all event handlers in `Data\Items.cs`
- [ ] Check each handler's return type matches expected union type
- [ ] Verify `Empty` returns are used correctly (for NOT_FAIL)

### Phase 3: Success Check Audit
- [ ] Audit `BattleActions.MoveEffects.cs` for Undefined handling ? DONE
- [ ] Audit `BattleActions.MoveHit.cs` for Undefined handling
- [ ] Audit `BattleActions.HitSteps.cs` for Undefined handling
- [ ] Audit `BattleActions.Damage.cs` for Undefined handling
- [ ] Audit `BattleActions.ResultCombining.cs` for union handling
- [ ] Search codebase for `switch.*didAnything` patterns
- [ ] Search codebase for `_ => false` in success checks

### Phase 4: Union Value Access Audit
- [ ] Search for `\.Value` access patterns
- [ ] Verify all union value access checks variant type first
- [ ] Add defensive checks where missing

### Phase 5: Status/Volatile Condition Application
- [ ] Check `Pokemon.AddVolatile` handling
- [ ] Check `Pokemon.SetStatus` handling  
- [ ] Check `Side.AddSideCondition` handling
- [ ] Check `Field.AddPseudoWeather` handling
- [ ] Verify all return Undefined appropriately for success

### Phase 6: Testing
- [ ] Test moves that return Empty (Protect, Detect, etc.)
- [ ] Test moves that return Undefined (status moves)
- [ ] Test moves that deal 0 damage vs fail
- [ ] Test event handlers with each union variant
- [ ] Test edge cases (null targets, fainted Pokemon, etc.)

---

## Example Fixes Completed

### Fix 1: Protect Bug (EmptyBoolIntEmptyVoidUnion)
**File**: `EventHandlerAdapter.cs`
**Issue**: Missing conversion for `EmptyBoolIntEmptyVoidUnion`
**Fix**: Added all BoolIntEmptyVoidUnion variants to switch statement
**Status**: ? FIXED

### Fix 2: Leech Seed Success Check
**File**: `BattleActions.MoveEffects.cs`  
**Issue**: Treating Undefined as failure in move success determination
**Fix**: Added explicit `UndefinedBoolIntUndefinedUnion => true` case
**Status**: ? FIXED

### Fix 3: Move Success Determination
**File**: `BattleActions.MoveEffects.cs`
**Issue**: `_ => false` catching Undefined and treating as failure
**Fix**: Changed to explicit Undefined handling before default case
**Status**: ? FIXED

---

## Code Search Queries for New Issues

Run these searches to find potential issues:

```bash
# 1. Find all union type definitions
# Location: ApogeeVGC\Sim\Utils\Unions\
# Pattern: public.*record.*Union

# 2. Find event handlers returning Empty
# Pattern: return new Empty\(\)

# 3. Find success checks that might miss Undefined
# Pattern: (bool|var).*=.*switch.*\{.*_ => false

# 4. Find direct Value access on unions
# Pattern: \w+\.Value(?!\s*[;:)])

# 5. Find event handler returns without union wrapping
# Pattern: return \d+;|return (true|false);
# Context: Inside event handler lambdas

# 6. Find potential NOT_FAIL representations
# Pattern: return 0;|return null;|return "";
# Context: In event handler files

# 7. Find union type usages in battle actions
# Pattern: (Int|Bool|Empty|Undefined).*Union
# Location: BattleActions*.cs files
```

---

## Prevention Guidelines

### For Adding New Event Handlers:
1. Check what union type the event expects to return
2. Return the appropriate union variant for your case:
   - Success with value: `IntVariant(value)` or `BoolVariant(true)`
   - Success without value: `EmptyVariant(new Empty())`
   - No action taken: `VoidVariant(new VoidReturn())`
   - Unknown/undefined: `UndefinedVariant(new Undefined())`
3. Verify `EventHandlerAdapter.ConvertReturnValue` handles your return type
4. Test the event handler fires and returns correctly

### For Adding New Union Types:
1. Define all variants in the union type file
2. Add conversion cases to `EventHandlerAdapter.ConvertReturnValue`
3. Document which RelayVar each variant should map to
4. Add to this guide's union type list

### For Processing Event Results:
1. Always use pattern matching to check variant type
2. Handle Undefined/Empty explicitly before default case
3. Remember: Undefined/Empty = NOT_FAIL = success
4. Never assume a union is a specific variant without checking

### For Success Determination:
1. Check if the value is quantifiable (int > 0, bool true)
2. Check if the value is NOT_FAIL (Undefined, Empty)
3. Only then treat as failure (false, null, etc.)
4. Document what "success" means in the context

---

## Testing Scenarios

Test these specific scenarios to verify union handling:

1. **Protect/Detect moves** - Return Empty, should succeed
2. **Status moves on immune targets** - Return false, should fail  
3. **Status moves on already-statused** - Return false, should fail
4. **Leech Seed on Grass-type** - Return false, should fail
5. **Moves dealing 0 damage** - Return 0, should be "did nothing" not "failed"
6. **Moves failing accuracy check** - Return false, should fail
7. **Self-targeting status moves** - Often return Undefined, should succeed
8. **Field condition moves** - Return Undefined when already active
9. **Weather moves** - Return Undefined when weather already set
10. **Transform/Mimic fails** - Check failure vs success with no effect

---

## Related Documentation

- `LeechSeedBugFix.md` - Original Leech Seed Undefined handling fix
- Pokemon Showdown source - NOT_FAIL constant usage pattern
- Event system architecture docs (if they exist)

---

## Quick Reference: Union ? RelayVar Mapping

| Union Variant | Maps To | Meaning |
|--------------|---------|---------|
| `Int*Union` with value | `IntRelayVar(value)` | Quantifiable result |
| `Bool*Union` with true | `BoolRelayVar(true)` | Success |
| `Bool*Union` with false | `BoolRelayVar(false)` | Failure |
| `Empty*Union` | `UndefinedRelayVar()` | NOT_FAIL (success, no value) |
| `Undefined*Union` | `UndefinedRelayVar()` | NOT_FAIL (success, no value) |
| `Void*Union` | `VoidReturnRelayVar()` | No action taken |
| `null` | `null` | No result |

---

## Version History

- **v1.0** - Initial guide created after fixing Protect and Leech Seed bugs
- Covers union type conversion, Undefined handling, and success determination
- Documents EmptyBoolIntEmptyVoidUnion fix specifically

---

## Contributing

When you find and fix a new union-related issue:
1. Document it in "Example Fixes Completed" section
2. Add the pattern to relevant search queries
3. Update the audit checklist if needed
4. Add test scenario if applicable
