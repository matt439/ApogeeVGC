# Trick Room Bug Fix

## Issue Description
**Symptom**: When Trick Room is used while already active, it doesn't remove the existing Trick Room effect. Instead, the effect continues counting down its duration, leading to Trick Room lasting much longer than intended.

**Expected Behavior**: When Trick Room is used while already active, it should immediately end the existing Trick Room effect (similar to how it works in official Pokémon games).

**Observed Behavior**: 
- Turn 1: Trick Room starts with duration 5
- Turn 2: Trick Room used again ? duration decreases to 4 (should end instead)
- Turn 3: Duration decreases to 3
- Turn 4: Duration decreases to 2
- Turn 5: Duration decreases to 1
- Turn 6: Trick Room finally ends naturally

## Root Cause
The bug had **two distinct root causes** in the event system infrastructure:

### 1. Missing Event Handler Mappings
Field event handlers (`OnFieldStart`, `OnFieldRestart`, `OnFieldResidual`, `OnFieldEnd`) were not registered in the `EventHandlerInfoMapper`. The event system uses a frozen dictionary to map `EventId` values to property accessors, but field events were completely absent from this mapping.

**Impact**: The event system couldn't find the `OnFieldRestart` handler, so it was never invoked when Trick Room was used while already active.

### 2. Missing Parameter Resolution in Event Adapter
The `EventHandlerAdapter` is responsible for converting legacy parameter-based event handlers to context-based handlers. It resolves parameters by matching parameter types and names to values in the `EventContext`.

The adapter had resolution logic for:
- `Battle` parameters ?
- `Pokemon` parameters ?
- `Side` parameters ? (missing)
- `Field` parameters ? (missing)
- `IEffect` parameters ?
- `Move` parameters ?

Field event handlers have the signature `(Battle, Field, Pokemon, IEffect)`, but the adapter couldn't resolve the `Field` parameter, causing the handler invocation to fail.

**Impact**: Even after adding the event mappings, the adapter would throw an `InvalidOperationException` when trying to invoke the field event handler because it couldn't resolve the `Field` parameter.

## Debug Evidence
Initial state showed the handler existed but was never called:
```
[AddPseudoWeather] Called for TrickRoom, already exists: True
[AddPseudoWeather] TrickRoom already exists, has restart handler: True
[AddPseudoWeather] Calling OnFieldRestart for TrickRoom
[AddPseudoWeather] After OnFieldRestart, TrickRoom still exists: True
```

Notice: No handler execution logs between "Calling" and "After", indicating the handler wasn't being invoked.

After the first fix (adding event mappings), the battle would end in a tie with:
```
[Simulator.ProcessChoiceResponsesAsync] ERROR: InvalidOperationException: Event FieldStart adapted handler failed on effect Trick Room (Condition)
```

This revealed the adapter couldn't invoke the handler due to missing parameter resolution.

After the complete fix, proper execution:
```
[AddPseudoWeather] Called for TrickRoom, already exists: True
[AddPseudoWeather] TrickRoom already exists, has restart handler: True
[AddPseudoWeather] Calling OnFieldRestart for TrickRoom
[TrickRoom.OnFieldRestart] Handler called!
[RemovePseudoWeather] Called for TrickRoom, exists: True
[RemovePseudoWeather] Calling OnFieldEnd for TrickRoom
[RemovePseudoWeather] Removed TrickRoom, still exists: False
[TrickRoom.OnFieldRestart] After RemovePseudoWeather call
[AddPseudoWeather] After OnFieldRestart, TrickRoom still exists: False
```

## Solution

### Part 1: Add Event Handler Mappings
Updated `EventHandlerInfoMapper.cs` to include field and side event handlers in the `EventMethodsMap`:

```csharp
// Field event handlers
[EventId.FieldStart] = e => e is IFieldEventMethods f ? f.OnFieldStart : null,
[EventId.FieldRestart] = e => e is IFieldEventMethods f ? f.OnFieldRestart : null,
[EventId.FieldResidual] = e => e is IFieldEventMethods f ? f.OnFieldResidual : null,
[EventId.FieldEnd] = e => e is IFieldEventMethods f ? f.OnFieldEnd : null,

// Side event handlers
[EventId.SideStart] = e => e is ISideEventMethods s ? s.OnSideStart : null,
[EventId.SideRestart] = e => e is ISideEventMethods s ? s.OnSideRestart : null,
[EventId.SideResidual] = e => e is ISideEventMethods s ? s.OnSideResidual : null,
[EventId.SideEnd] = e => e is ISideEventMethods s ? s.OnSideEnd : null,
```

**Why This Works**: 
- The event system now properly maps `EventId.FieldRestart` to the `OnFieldRestart` property
- When `AddPseudoWeather` finds an existing pseudo-weather and calls `SingleEvent(EventId.FieldRestart, ...)`, it can now find the handler

### Part 2: Add Parameter Resolution
Updated `EventHandlerAdapter.cs` to resolve `Field` and `Side` parameters:

```csharp
// Match by parameter name
if (paramName.Contains("side"))
{
    return context.TargetSide;
}

// Add support for Field parameters
if (paramName.Contains("field"))
{
    return context.TargetField;
}

// Match by type
if (paramType == typeof(Pokemon) || typeof(Pokemon).IsAssignableFrom(paramType))
{
    // Prefer target over source if ambiguous
    return context.TargetPokemon ?? context.SourcePokemon;
}

// Add support for Field type
if (paramType == typeof(ApogeeVGC.Sim.FieldClasses.Field) || 
    typeof(ApogeeVGC.Sim.FieldClasses.Field).IsAssignableFrom(paramType))
{
    return context.TargetField;
}

// Add support for Side type
if (paramType == typeof(ApogeeVGC.Sim.SideClasses.Side) || 
    typeof(ApogeeVGC.Sim.SideClasses.Side).IsAssignableFrom(paramType))
{
    return context.TargetSide;
}
```

**Why This Works**:
- The adapter can now resolve parameters by both name (`field`, `side`) and type (`Field`, `Side`)
- Field event handlers with signature `(Battle, Field, Pokemon, IEffect)` can be properly invoked
- Side event handlers with similar signatures also work correctly

## Files Modified

### 1. `ApogeeVGC\Sim\Events\EventHandlerInfoMapper.cs`
**Changes**:
- Added field event handler mappings (`FieldStart`, `FieldRestart`, `FieldResidual`, `FieldEnd`)
- Added side event handler mappings (`SideStart`, `SideRestart`, `SideResidual`, `SideEnd`)

**Lines Modified**: Added 8 new entries to the `EventMethodsMap` frozen dictionary (around line 100)

### 2. `ApogeeVGC\Sim\Events\EventHandlerAdapter.cs`
**Changes**:
- Added parameter name resolution for `field` parameters
- Added parameter type resolution for `Field` type
- Added parameter type resolution for `Side` type

**Lines Modified**: Added 15 lines to the `ResolveParameter` method (around line 100-130)

### 3. `ApogeeVGC\Data\Conditions.cs` (Already Correct)
**Verification**: The `OnFieldRestart` handler for Trick Room was already correctly implemented:
```csharp
OnFieldRestart = new OnFieldRestartEventInfo((battle, _, _, _) =>
{
  if (battle.DisplayUi)
    {
        battle.Debug("[TrickRoom.OnFieldRestart] Handler called!");
    }

    // When Trick Room is used while already active, it should end instead of restart
    battle.Field.RemovePseudoWeather(ConditionId.TrickRoom);

    if (battle.DisplayUi)
    {
        battle.Debug("[TrickRoom.OnFieldRestart] After RemovePseudoWeather call");
    }
}),
```

### 4. `ApogeeVGC\Sim\FieldClasses\Field.cs` (Debug Logging)
**Changes**:
- Added debug logging to `AddPseudoWeather` method
- Added debug logging to `RemovePseudoWeather` method

**Purpose**: Help diagnose similar issues in the future by showing the complete flow of pseudo-weather operations

## Testing Results
After the complete fix, Trick Room works correctly:
- ? Turn 1: Trick Room starts with duration 5, displays `-fieldstart` message
- ? Turn 2: Using Trick Room again calls `OnFieldRestart` handler
- ? Turn 2: Handler calls `RemovePseudoWeather(TrickRoom)`
- ? Turn 2: Displays `-fieldend` message when removing Trick Room
- ? Turn 2: Trick Room is completely removed from field
- ? Turn 3: No Trick Room field condition active
- ? Speed ordering returns to normal (no longer reversed)

## Debugging Process Summary
The bug was discovered and fixed through systematic investigation:

1. **Initial Observation**: Debug logs showed `OnFieldRestart` was being called but the handler wasn't executing
2. **First Hypothesis**: Handler implementation was wrong ? Ruled out by reviewing `Conditions.cs`
3. **Second Hypothesis**: Event system can't find the handler ? **Confirmed** by checking `EventHandlerInfoMapper`
4. **First Fix**: Added event mappings ? Battle ended in a tie with adapter error
5. **Third Hypothesis**: Event adapter can't invoke the handler ? **Confirmed** by checking `EventHandlerAdapter`
6. **Second Fix**: Added parameter resolution ? **Complete fix achieved**
7. **Verification**: Debug logs showed full execution flow working correctly

## Related Context

### Field Event Handler Pattern
All field-specific pseudo-weather effects (Trick Room, Gravity, Magic Room, Wonder Room, etc.) should follow this pattern:

```csharp
[ConditionId.TrickRoom] = new()
{
    Id = ConditionId.TrickRoom,
  Name = "Trick Room",
    EffectType = EffectType.Condition,
    Duration = 5,
    
  // Called when the field condition starts
    OnFieldStart = new OnFieldStartEventInfo((battle, field, source, effect) =>
    {
        if (battle.DisplayUi)
        {
  battle.Add("-fieldstart", "move: Trick Room", $"[of] {source}");
        }
    }),
    
    // Called when the field condition is used while already active
OnFieldRestart = new OnFieldRestartEventInfo((battle, field, source, effect) =>
    {
        // For toggle effects like Trick Room, remove the existing effect
   battle.Field.RemovePseudoWeather(ConditionId.TrickRoom);
    }),
    
    // Called at the end of each turn for duration countdown
    OnFieldResidual = new OnFieldResidualEventInfo((battle, field, source, effect) =>
    {
        // Typically empty for most field conditions
        // The duration countdown is handled automatically
    }),
    
    // Called when the field condition ends
    OnFieldEnd = new OnFieldEndEventInfo((battle, field) =>
    {
    if (battle.DisplayUi)
        {
          battle.Add("-fieldend", "move: Trick Room");
 }
    }),
},
```

### Event System Architecture
The event system has three key components that all must work together:

1. **Event Handler Definition** (`IFieldEventMethods` interface)
   - Defines the contract for what event handlers a condition can have
   - Example: `OnFieldRestart`, `OnFieldStart`, etc.

2. **Event Handler Mapping** (`EventHandlerInfoMapper` class)
   - Maps `EventId` enum values to the actual property accessors
   - Uses frozen dictionaries for O(1) lookup performance
   - **Critical**: If an event isn't in the map, it can't be found!

3. **Event Handler Adapter** (`EventHandlerAdapter` class)
   - Converts legacy parameter-based handlers to context-based handlers
   - Resolves parameters from the `EventContext` by name and type
   - **Critical**: If a parameter type isn't supported, invocation fails!

### Similar Bugs to Watch For
When implementing new field conditions or debugging field-related issues:

1. **Missing Event Mappings**
   - Symptom: Handler exists but never executes
   - Check: Is the event in `EventHandlerInfoMapper.EventMethodsMap`?
   - Fix: Add the appropriate mapping

2. **Missing Parameter Resolution**
   - Symptom: `InvalidOperationException: Event [X] adapted handler failed`
   - Check: Does the adapter support all parameter types used in the handler?
   - Fix: Add parameter resolution logic to `EventHandlerAdapter.ResolveParameter`

3. **Wrong Handler Signature**
   - Symptom: Handler executes but parameters are null or wrong type
 - Check: Does the handler signature match the expected pattern for that event?
   - Fix: Update handler signature to match event expectations

4. **Toggle vs. Stackable Effects**
   - Trick Room, Gravity, etc. are **toggle effects** ? `OnFieldRestart` should remove the effect
   - Multiple hazards, multiple screens, etc. might be **stackable** ? `OnFieldRestart` could increase counters
- Always check official game behavior to determine the correct pattern

## Future Reference

### When Adding New Field Conditions
1. ? Define the condition in `Conditions.cs` with all required handlers
2. ? Ensure event is in `EventHandlerInfoMapper` (should already be there after this fix)
3. ? Ensure adapter supports all parameter types (should already be there after this fix)
4. ? Test with debug logging to verify handler execution
5. ? Test restart behavior if applicable

### When Adding New Side Conditions
Same process as field conditions, but use:
- `ISideEventMethods` interface
- `OnSideStart`, `OnSideRestart`, `OnSideResidual`, `OnSideEnd` handlers
- Already mapped in `EventHandlerInfoMapper` after this fix

### When Debugging Event Handler Issues
1. Add debug logging at handler entry/exit points
2. Check if event is in `EventHandlerInfoMapper`
3. Check if adapter supports all parameter types
4. Verify handler signature matches expected pattern
5. Check for exceptions in `EventHandlerAdapter.ResolveParameter`

## Date Fixed
2025-01-23 (AI assistant session)

## Related Issues
- **ProtectBugFix.md** - Similar pattern: event system infrastructure issue blocking proper functionality
- **LeechSeedBugFix.md** - Union type handling issue (different category)
- **UnionTypeHandlingGuide.md** - General guidance on union type semantics

## Key Takeaways
1. **Event system requires complete infrastructure**: Mappings, adapters, and implementations must all be present
2. **Missing mappings are silent failures**: No error, handler just doesn't execute
3. **Missing parameter resolution causes hard failures**: Clear exception with diagnostic info
4. **Debug logging is essential**: Shows execution flow and reveals missing pieces
5. **Field/Side events are different from Pokemon events**: Need special handling in mapper and adapter
6. **Pattern matching with similar bugs is valuable**: The ProtectBugFix pattern helped identify the solution faster
