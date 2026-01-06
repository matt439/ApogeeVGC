# Event Handler Type Mismatch Investigation and Fixes

## Problem Summary

The event system had two sources of truth for delegate parameter information:
1. **`ParameterInfoCache`** - A concurrent dictionary caching `ParameterInfo[]` from reflection for performance
2. **`EventHandlerInfo.ExpectedParameterTypes`** - Compile-time type specifications for each event handler

These could become misaligned, leading to type mismatch exceptions during runtime that were difficult to diagnose.

## Root Causes Identified

1. **No Runtime Validation**: The `EventHandlerInfo.Validate()` method existed but was never called to verify that actual delegate signatures matched the expected types.

2. **Separated Validation Logic**: `ValidateConfiguration()` was called in constructors to validate internal consistency, but `Validate()` (which checks the actual delegate) was never invoked.

3. **Parameter Matching by Convention**: `BuildSingleArg()` used parameter names and types from reflection to dynamically match arguments, which could fail if naming conventions weren't followed.

4. **Limited Error Messages**: When type mismatches occurred, error messages didn't include enough context about which event, which parameter, or what the mismatch was.

## Changes Made

### 1. Enhanced Diagnostic Logging (`Battle.Delegates.cs`)

Added comprehensive error handling with detailed diagnostics for three types of exceptions:

- **`TargetParameterCountException`**: Logs delegate name, parameter count, expected types, and EventHandlerInfo specifications
- **`TargetInvocationException`**: Logs inner exception details and attempts to build each argument to show which one failed
- **`ArgumentException`**: Logs type mismatches between expected and actual parameter types

### 2. Added EventHandlerInfo Validation (`Battle.Events.cs`)

Modified `InvokeEventHandlerInfo()` to call `handlerInfo.Validate()` before invoking the delegate:

```csharp
try
{
    handlerInfo.Validate();
}
catch (InvalidOperationException ex)
{
    throw new InvalidOperationException(
 $"EventHandlerInfo validation failed for event {handlerInfo.Id} " +
        $"on effect {Effect?.Name ?? "unknown"} ({Effect?.EffectType}): {ex.Message}",
        ex);
}
```

This catches signature mismatches **before** attempting invocation, providing clearer error messages.

### 3. Integrated EventHandlerInfo into Invocation

Modified `InvokeDelegateEffectDelegate()` to accept `EventHandlerInfo?` as a parameter:

```csharp
private RelayVar? InvokeDelegateEffectDelegate(Delegate del, 
    ApogeeVGC.Sim.Events.EventHandlerInfo? handlerInfo,
    bool hasRelayVar,
    RelayVar relayVar,
    SingleEventTarget? target, SingleEventSource? source, IEffect? sourceEffect)
```

This enables:
- Parameter count validation against `EventHandlerInfo.ExpectedParameterTypes`
- Better error messages that include event IDs
- Future optimization potential (could use EventHandlerInfo types instead of reflection)

### 4. Parameter-Level Type Validation (`BuildSingleArg`)

Added validation in `BuildSingleArg()` to verify that each parameter matches the EventHandlerInfo specification:

```csharp
if (handlerInfo?.ExpectedParameterTypes != null && 
    position < handlerInfo.ExpectedParameterTypes.Length)
{
    Type expectedType = handlerInfo.ExpectedParameterTypes[position];
    
    // Strip nullability and validate assignability
    // Throws detailed exception if mismatch detected
}
```

Also added check for non-nullable parameters that receive null values:

```csharp
if (handlerInfo?.ParameterNullability != null &&
    position < handlerInfo.ParameterNullability.Length &&
    !handlerInfo.ParameterNullability[position])
{
    throw new InvalidOperationException(
     $"Event {handlerInfo.Id}: Parameter {position} is non-nullable but no matching value was found...");
}
```

### 5. Added Using Directive

Added `using ApogeeVGC.Sim.Events;` to `Battle.Delegates.cs` to access `EventHandlerInfo`.

## Benefits

1. **Early Detection**: Type mismatches are now caught during validation before invocation attempts
2. **Better Diagnostics**: Error messages include event IDs, parameter positions, expected vs actual types, and EventHandlerInfo specifications
3. **Dual Validation**: Both compile-time (EventHandlerInfo) and runtime (ParameterInfo cache) information are cross-checked
4. **Null Safety**: Non-nullable parameters that receive null values are explicitly detected and reported
5. **Maintainability**: Future changes to event signatures will be caught immediately with clear error messages

## Future Considerations

### Potential Optimizations

1. **Replace ParameterInfoCache with EventHandlerInfo**: Since EventHandlerInfo already contains parameter types, we could eliminate the reflection cache entirely and build arguments directly from EventHandlerInfo specifications.

2. **Compile-Time Parameter Binding**: Instead of dynamic matching by name/type, use EventHandlerInfo to create strongly-typed parameter builders.

3. **Caching Built Arguments**: For handlers that always receive the same argument types, cache the argument building logic.

### Migration Path

The current implementation maintains backward compatibility while adding validation. The ParameterInfoCache still provides performance benefits for reflection operations. To fully migrate:

1. Audit all EventHandlerInfo constructors to ensure ExpectedParameterTypes is always set correctly
2. Add unit tests that validate every event handler's signature
3. Consider generating EventHandlerInfo classes from metadata to ensure consistency
4. Gradually replace DynamicInvoke with strongly-typed delegates where possible

## Testing Recommendations

1. **Run Existing Tests**: All existing battle simulations should run to verify no regressions
2. **Intentional Mismatches**: Create test cases with intentionally wrong signatures to verify error messages
3. **Null Parameter Tests**: Verify that null parameters are caught when they shouldn't be null
4. **Performance Testing**: Measure any performance impact from the additional validation

## Notes

- The ParameterInfoCache is still useful for performance as it avoids repeated reflection calls
- EventHandlerInfo.ExpectedParameterTypes provides the single source of truth for what parameters SHOULD be
- The validation happens at every invocation but only adds overhead when EventHandlerInfo is provided (which it now always is)
- The fully-qualified namespace (`ApogeeVGC.Sim.Events.EventHandlerInfo`) was necessary to avoid conflicts with potential local types
