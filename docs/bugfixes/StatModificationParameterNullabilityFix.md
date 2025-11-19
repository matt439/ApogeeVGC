# Stat Modification Parameter Nullability and Type Conversion Fix

## Problem Summary
When Miraidon (with Choice Specs and Hadron Engine) used Dazzling Gleam against Ironhands, the battle ended in a tie with the error:
```
Event ModifySpA adapted handler failed on effect Choice Specs (Item)
Inner Exception: Event ModifySpA: Parameter 1 (Int32 spa) is non-nullable but no matching value found in context
```

This occurred when multiple stat modification handlers chained together during damage calculation.

## Root Causes

### Primary Issue: Missing DecimalRelayVar ? int Conversion
When multiple stat modification handlers chain together (e.g., Hadron Engine + Choice Specs both modifying SpA):
1. **First handler** (Hadron Engine) returns `DoubleVoidUnion`, which gets converted to `DecimalRelayVar`
2. **Second handler** (Choice Specs) expects an `int` parameter for the stat value
3. `EventHandlerAdapter.TryUnwrapRelayVar` only handled `IntRelayVar` ? `int`, not `DecimalRelayVar` ? `int`
4. Parameter resolution failed with "Parameter 1 (Int32 spa) is non-nullable but no matching value found in context"

The issue occurred because:
- Stat modification handlers return `DoubleVoidUnion` (decimal values for fractional modifiers like 1.5x)
- Return value gets converted to `DecimalRelayVar` 
- When chaining handlers, the second handler receives the first handler's output as its input
- But `TryUnwrapRelayVar` couldn't convert `DecimalRelayVar` ? `int`, causing parameter resolution to fail

### Secondary Issue: Parameter Nullability Constraints
The stat modification event handlers had all their parameters marked as non-nullable, but during damage calculation, some parameters are legitimately `null`:
- **Source Pokemon** (parameter 4): Can be null when calculating defensive stats
- **Move** (parameter 5): Can be null in non-combat stat calculation contexts

## Sequence of Events
1. Miraidon uses Dazzling Gleam
2. System calculates Miraidon's Special Attack
   - **Hadron Engine** (ability) modifies SpA ? Returns DecimalRelayVar ?
   - **Choice Specs** (item) tries to modify SpA ? **FAILS** ?
     - Receives `DecimalRelayVar` from Hadron Engine's output
     - Expects `int` parameter but can't unwrap `DecimalRelayVar` ? `int`
     - Throws `InvalidOperationException` during parameter resolution
3. Exception caught in DEBUG mode and logged
4. Battle processing stops but doesn't properly end
5. `DetermineWinner()` called on unfinished battle ? Tie

## Solution

### Fix 1: Add DecimalRelayVar ? int Conversion
Added support for converting `DecimalRelayVar` to `int` in `EventHandlerAdapter.TryUnwrapRelayVar`:

```csharp
// Handle decimal -> int conversion (for stat modifications that return decimal but parameter expects int)
if (relayVar is DecimalRelayVar decVarToInt && (targetType == typeof(int) || targetType == typeof(Int32)))
{
    value = (int)decVarToInt.Value;
    return true;
}
```

This allows stat modification handlers to chain correctly, with the output of one handler (DecimalRelayVar) being converted to the input type expected by the next handler (int).

### Fix 2: Update Parameter Nullability
Updated the `ParameterNullability` arrays in the stat modification event handler info classes to correctly reflect which parameters can be null:

### Files Modified

#### EventHandlerAdapter.cs
Added `DecimalRelayVar` ? `int` conversion in `TryUnwrapRelayVar` method to support chaining stat modification handlers.

#### OnModifyAtkEventInfo.cs
```csharp
// OLD
ParameterNullability = [false, false, false, false, false];

// NEW  
// Nullability: Battle, int, and source Pokemon are non-nullable
// Target Pokemon and Move can be null (e.g., when calculating base stats)
ParameterNullability = [false, false, false, true, true];
```

#### OnModifyDefEventInfo.cs
```csharp
// OLD
ParameterNullability = [false, false, false, false, false];

// NEW
// Nullability: Battle, int, and target Pokemon are non-nullable
// Source Pokemon and Move can be null (e.g., when calculating base stats)
ParameterNullability = [false, false, false, true, true];
```

#### OnModifySpAEventInfo.cs
```csharp
// OLD
ParameterNullability = [false, true, true, true, true]; // Incorrectly marked int as nullable

// NEW
// Nullability: Battle, int, and source Pokemon are non-nullable
// Target Pokemon and Move can be null (e.g., when calculating base stats)
ParameterNullability = [false, false, false, true, true];
```

#### OnModifySpDEventInfo.cs
```csharp
// OLD
ParameterNullability = [false, false, false, false, false];

// NEW
// Nullability: Battle, int, and target Pokemon are non-nullable
// Source Pokemon and Move can be null (e.g., when calculating base stats)
ParameterNullability = [false, false, false, true, true];
```

#### OnModifySpeEventInfo.cs
No changes needed - this handler only has 3 parameters (Battle, int, Pokemon) and all are correctly non-nullable.

## Parameter Breakdown

For offensive stat modifications (`OnModifyAtk`, `OnModifySpA`):
1. `Battle` - ? Always present (non-nullable)
2. `int` (stat value) - ? Always present (non-nullable)
3. `Pokemon` (source) - ? The attacking Pokemon (non-nullable)
4. `Pokemon` (target) - ? Can be null when calculating base stats (nullable)
5. `ActiveMove` - ? Can be null in non-combat contexts (nullable)

For defensive stat modifications (`OnModifyDef`, `OnModifySpD`):
1. `Battle` - ? Always present (non-nullable)
2. `int` (stat value) - ? Always present (non-nullable)
3. `Pokemon` (target) - ? The defending Pokemon (non-nullable)
4. `Pokemon` (source) - ? Can be null when calculating base stats (nullable)
5. `ActiveMove` - ? Can be null in non-combat contexts (nullable)

## Additional Improvements
Also added detailed error logging to help diagnose similar issues in the future:

### Battle.Events.cs
Added comprehensive error details including inner exception information and stack traces when event handlers fail.

### EventHandlerAdapter.cs
Enhanced exception messages to include:
- Exception type and message
- Up to 10 lines of stack trace
- Context about which handler and parameters were involved

## Testing
After the fix:
1. Miraidon successfully uses Dazzling Gleam
2. **Hadron Engine's** OnModifySpA executes correctly ? Returns DecimalRelayVar ?
3. **Choice Specs'** OnModifySpA executes correctly ? Receives DecimalRelayVar, converts to int, applies modifier ?
4. Damage calculation completes successfully with both modifiers applied
5. Battle continues normally instead of ending in a tie

## Impact
This fix enables **multiple stat modification handlers to chain together** correctly:
- Abilities + Items (e.g., Hadron Engine + Choice Specs)
- Items + Conditions (e.g., Assault Vest + QuarkDrive)
- Multiple abilities in double battles
- Any combination of stat-modifying effects

Without this fix, only the **first** handler in the chain would execute, and subsequent handlers would fail with parameter resolution errors.

## Related Issues
This is similar to but distinct from:
- **"Stat Modification Handler VoidReturn Fix"**: Addressed handlers returning `VoidReturn()` instead of integer values
- This fix addresses the **parameter conversion** side, enabling handlers to chain by converting between `DecimalRelayVar` and `int`
- Also fixes **parameter nullability** to allow handlers to work in contexts where source Pokemon or move might be null

## Keywords
`stat modification`, `OnModifyAtk`, `OnModifyDef`, `OnModifySpA`, `OnModifySpD`, `parameter nullability`, `EventHandlerAdapter`, `DecimalRelayVar`, `type conversion`, `handler chaining`, `Assault Vest`, `Hadron Engine`, `Choice Specs`, `damage calculation`, `InvalidOperationException`, `adapted handler failed`, `event parameters`, `null parameters`, `TryUnwrapRelayVar`
