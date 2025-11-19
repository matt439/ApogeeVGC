# Facade BasePower Event Parameter Fix

## Problem Summary

When Ursaluna (with Burn status and Flame Orb item) used Facade against Miraidon, the battle crashed with:

```
Event BasePower adapted handler failed on effect Facade (Move)
Inner exception: InvalidOperationException: Event BasePower: Parameter 1 (Int32 _) is non-nullable but no matching value found in context
```

The error occurred during damage calculation when the BasePower event was triggered for the Facade move.

## Symptom Details

- **When**: During damage calculation for Facade move (and any move with an `OnBasePower` handler)
- **Stack**: `GetDamage` ? `RunEvent(BasePower)` ? `InvokeEventHandlerInfo` ? `EventHandlerAdapter.ResolveParameter`
- **Error**: Parameter resolution failure for non-nullable `int` parameter on second handler invocation

## Root Causes

### Primary Issue: VoidReturn Overwrites RelayVar

The main problem was in `Battle.Events.cs` in the `RunEvent` method. When multiple handlers exist for an event:

1. First handler executes with `relayVar = IntRelayVar(70)`
2. Handler returns `VoidReturnRelayVar` (indicating "no value change, just side effects")
3. Line 408 unconditionally replaced `relayVar` with the return value: `relayVar = returnVal`
4. Second handler (Facade's `OnBasePower`) tries to execute
5. Parameter resolution attempts to extract `int` from `VoidReturnRelayVar` ? **FAILS**

The issue was that `VoidReturn` means "no change/no opinion" but the code was treating it as "replace the value with nothing", breaking the handler chain.

### Secondary Issue: Wrong Parameter Type Passed

Initially, the code in `BattleActions.Damage.cs` line 120 was passing a primitive `int` instead of wrapping it in `IntRelayVar`:

```csharp
// INCORRECT - passing int directly
RelayVar? basePowerEvent = Battle.RunEvent(EventId.BasePower, source,
    RunEventSource.FromNullablePokemon(target), move, basePower.ToInt(), true);
```

This needed to be fixed to:

```csharp
// CORRECT - wrapping in IntRelayVar
RelayVar? basePowerEvent = Battle.RunEvent(EventId.BasePower, source,
    RunEventSource.FromNullablePokemon(target), move, new IntRelayVar(basePower.ToInt()), true);
```

However, fixing only this wasn't sufficient due to the primary issue above.

## Handler Signature

The `OnBasePowerEventInfo` expects a handler with this signature:

```csharp
Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>
//           ^^^
//           This parameter needs to be provided in the RelayVar
```

All parameters are marked as non-nullable in the `ParameterNullability` array:

```csharp
ParameterNullability = new[] { false, false, false, false, false };
```

So when the `int` base power value isn't found in the context, it throws an exception.

## Solution

### Fix 1: Preserve RelayVar Through Handler Chain (Primary Fix)

Modified `Battle.Events.cs` in the `RunEvent` method to not replace the `relayVar` when a handler returns `VoidReturnRelayVar`:

```csharp
// Process return value
if (returnVal != null)
{
    // Don't replace relayVar if handler returned VoidReturn (meaning "no change")
    if (returnVal is not VoidReturnRelayVar)
    {
        relayVar = returnVal;
    }

    // Check for early exit
    if (!IsRelayVarTruthy(relayVar) || fastExit == true)
    {
        // ... early exit logic
    }
}
```

**Why this works:** Handlers that return `VoidReturn` are indicating "I don't have a value to contribute, but I may have done side effects" (like `ChainModify()`). The original `relayVar` with the base power value is preserved for subsequent handlers to access.

### Fix 2: Pass Base Power as IntRelayVar

Changed `BattleActions.Damage.cs` to wrap the base power value in an `IntRelayVar`:

```csharp
// CORRECT - passing IntRelayVar
Battle.Debug($"[GetDamage] About to call BasePower event with IntRelayVar({basePower.ToInt()})");
RelayVar? basePowerEvent = Battle.RunEvent(EventId.BasePower, source,
    RunEventSource.FromNullablePokemon(target), move, new IntRelayVar(basePower.ToInt()), true);
Battle.Debug($"[GetDamage] BasePower event returned: {(basePowerEvent != null ? basePowerEvent.GetType().Name : "null")}");
```

Now the event system can properly:
1. Pass the base power value through the `RelayVar` parameter
2. Extract it via `EventHandlerAdapter.TryUnwrapRelayVar` to resolve the `int` parameter
3. Preserve it through the handler chain when handlers return `VoidReturn`
4. Invoke all handlers (Facade's `OnBasePower`, etc.) with correct parameters

## Files Modified

### `ApogeeVGC/Sim/BattleClasses/Battle.Events.cs`

**Location**: `RunEvent` method, lines ~406-410  
**Change**: Added check to preserve RelayVar when handler returns VoidReturnRelayVar

```diff
             // Process return value
             if (returnVal != null)
             {
-                relayVar = returnVal;
+                // Don't replace relayVar if handler returned VoidReturn (meaning "no change")
+                if (returnVal is not VoidReturnRelayVar)
+                {
+                    relayVar = returnVal;
+                }

                 // Check for early exit
                 if (!IsRelayVarTruthy(relayVar) || fastExit == true)
                 {
```

### `ApogeeVGC/Sim/BattleClasses/BattleActions.Damage.cs`

**Location**: Line ~120  
**Change**: Wrapped `basePower.ToInt()` in `new IntRelayVar(...)` and added debug logging

```diff
         // Run BasePower event after crit calculation
+        Battle.Debug($"[GetDamage] About to call BasePower event with IntRelayVar({basePower.ToInt()})");
         RelayVar? basePowerEvent = Battle.RunEvent(EventId.BasePower, source,
-            RunEventSource.FromNullablePokemon(target), move, basePower.ToInt(), true);
+            RunEventSource.FromNullablePokemon(target), move, new IntRelayVar(basePower.ToInt()), true);
+        Battle.Debug($"[GetDamage] BasePower event returned: {(basePowerEvent != null ? basePowerEvent.GetType().Name : "null")}");

         if (basePowerEvent is IntRelayVar bpIrv)
         {
             basePower = bpIrv.Value;
         }
```

### `ApogeeVGC/Sim/Events/EventHandlerAdapter.cs`

**Location**: `ResolveParameter` method  
**Change**: Added debug logging to trace parameter resolution

```diff
         // Check for RelayVar types
         if (context.HasRelayVar)
         {
+            context.Battle.Debug($"[ResolveParameter] Event={handlerInfo.Id}, Position={position}, ParamType={paramType.Name}, RelayVar={context.RelayVar?.GetType().Name}");
+            
             if (paramType.IsInstanceOfType(context.RelayVar!))
             {
+                context.Battle.Debug($"[ResolveParameter] Returning RelayVar directly");
                 return context.RelayVar;
             }

             // Check for primitive unwrapping
             if (TryUnwrapRelayVar(context.RelayVar!, paramType, out object? unwrapped))
             {
+                context.Battle.Debug($"[ResolveParameter] Unwrapped to: {unwrapped}");
                 return unwrapped;
             }
+            
+            context.Battle.Debug($"[ResolveParameter] TryUnwrapRelayVar failed");
```

## Why This Works

1. **VoidReturn Semantics**: When a handler returns `VoidReturn` (converted to `VoidReturnRelayVar`), it means "I don't have a value to contribute" not "replace the current value with nothing". Handlers use `ChainModify()` to apply multiplicative changes and return `VoidReturn` to indicate they didn't produce a replacement value.

2. **Handler Chaining**: Multiple handlers for the same event can now execute in sequence, each accessing the original base power value from the `IntRelayVar`:
   - Handler 1 (e.g., ability): Reads base power, calls `ChainModify()`, returns `VoidReturn`
   - Handler 2 (e.g., Facade): Reads same base power value, calls `ChainModify(2)`, returns `VoidReturn`
   - Both modifications are applied via the chain modifier system

3. **Type Safety**: The event system expects `RelayVar` types for passing values through the event chain
4. **Parameter Resolution**: `EventHandlerAdapter.TryUnwrapRelayVar` can extract the `int` value from the `IntRelayVar` to resolve the handler's parameter
5. **Non-Nullable Requirement**: The parameter is marked as non-nullable, so the value must be present in the context (which it now is, and stays present)

## Testing

**Test Case**: Use Facade with a status condition (Burn, Paralysis, or Poison)
- **Expected**: Facade should deal double damage when user has a status (except Sleep)
- **Actual**: Move executes successfully with 2x base power modifier

**Verification Steps**:
1. Start a battle with a Pokémon that has or can get Burn/Paralysis/Poison
2. Use Facade against an opponent
3. Confirm the move executes without crashing
4. Verify damage is doubled (debug output shows "Facade is increasing move damage")

## Related Issues

This is similar to previous parameter type mismatches in the event system:
- **Stat Modification Parameter Nullability Fix** - Similar issue where parameters needed proper type conversion
- **Complete Draco Meteor Bug Fix** - Had parameter resolution issues in event adapter

## Patterns Identified

### Pattern 1: RelayVar Parameter Passing

**General Rule**: When calling `Battle.RunEvent` with a value that needs to be passed to event handlers:
- ? **Don't** pass primitive types directly (int, bool, decimal)
- ? **Do** wrap them in appropriate `RelayVar` types (IntRelayVar, BoolRelayVar, DecimalRelayVar)

The event system is designed to pass values through `RelayVar` wrappers for:
- Type safety
- Nullability tracking  
- Chaining multiple handlers
- Return value conversion

### Pattern 2: VoidReturn Handling in Event Chains

**General Rule**: When processing handler return values in event chains:
- ? **Don't** blindly replace the `relayVar` with any non-null return value
- ? **Do** check if return value is `VoidReturnRelayVar` and preserve original `relayVar`

**Handler Return Value Semantics**:
- `VoidReturn` ? "I performed side effects (e.g., ChainModify) but have no replacement value"
- Actual value (IntRelayVar, BoolRelayVar, etc.) ? "Replace the current value with this"
- `null` ? "No change, no side effects"

**Affected Events**: Any event where handlers use `ChainModify()` instead of returning modified values:
- `BasePower` - Handlers like Facade, Electro Drift use `ChainModify()` and return `VoidReturn`
- `ModifyAtk`, `ModifyDef`, `ModifySpA`, `ModifySpD`, `ModifySpe` - Some handlers modify and return values, others use `ChainModify()`

## Impact

This fix enables:
- All moves with `OnBasePower` handlers to execute correctly (Facade, Electro Drift, etc.)
- Multiple `OnBasePower` handlers to chain properly (ability + move effects)
- Proper handler chaining for any event where handlers use `ChainModify()` + `VoidReturn` pattern

## Keywords

`Facade`, `BasePower`, `event parameter`, `RelayVar`, `IntRelayVar`, `VoidReturn`, `VoidReturnRelayVar`, `EventHandlerAdapter`, `parameter resolution`, `non-nullable`, `type mismatch`, `GetDamage`, `RunEvent`, `damage calculation`, `handler chaining`, `ChainModify`

---

**Date Fixed**: 2025-01-19  
**Severity**: High  
**Component**: Event System / Damage Calculation  
**Fixed By**: 
1. Preserving RelayVar through handler chain when VoidReturn is returned
2. Wrapping base power value in IntRelayVar before passing to RunEvent
