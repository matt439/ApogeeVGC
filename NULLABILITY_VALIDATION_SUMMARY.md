# EventHandlerInfo Nullability Validation

## Summary

Added nullability tracking and validation to the `EventHandlerInfo` system to ensure type safety and catch configuration errors at handler registration time.

## Changes Made

### 1. Added to `EventHandlerInfo` base class:

```csharp
/// <summary>
/// Indicates which parameters are nullable (true = nullable, false = non-null).
/// Array indices correspond to ExpectedParameterTypes indices.
/// If null, all parameters are assumed non-nullable.
/// </summary>
public bool[]? ParameterNullability { get; init; }

/// <summary>
/// Indicates whether the return type is nullable.
/// </summary>
public bool ReturnTypeNullable { get; init; }
```

### 2. Added Configuration Validation:

```csharp
/// <summary>
/// Validates configuration consistency (parameter nullability array length, etc.).
/// Should be called in derived class constructors after setting all properties.
/// </summary>
protected void ValidateConfiguration()
{
    // Validate ParameterNullability array length matches ExpectedParameterTypes
    if (ParameterNullability != null && ExpectedParameterTypes != null)
{
        if (ParameterNullability.Length != ExpectedParameterTypes.Length)
        {
     throw new InvalidOperationException(
       $"Event {Id}: ParameterNullability length ({ParameterNullability.Length}) " +
       $"does not match ExpectedParameterTypes length ({ExpectedParameterTypes.Length})");
        }
}
}
```

### 3. Updated `Validate()` Method:

The main `Validate()` method now also checks for ParameterNullability/ExpectedParameterTypes length mismatch.

### 4. Enhanced `GetSignatureDescription()`:

Now includes nullability markers (?) in the signature description:
```
BoolVoidUnion (Battle, Pokemon, Pokemon?, ActiveMove)
                ? nullable marker
```

### 5. Added Runtime Validation:

```csharp
/// <summary>
/// Validates parameter nullability - checks if a parameter value is null when it shouldn't be
/// </summary>
public void ValidateParameterNullability(object?[] args)
```

This is called by `InvokeCallback<TResult>()` to validate arguments at invocation time.

## Usage in Concrete Classes

All concrete `EventHandlerInfo` classes should:

1. **Set ParameterNullability** in their constructor
2. **Set ReturnTypeNullable** in their constructor
3. **Call ValidateConfiguration()** at the end of their constructor

### Example:

```csharp
public sealed record OnBeforeMoveEventInfo : EventHandlerInfo
{
    public OnBeforeMoveEventInfo(
        VoidSourceMoveHandler handler,
        int? priority = null,
  bool usesSpeed = true)
    {
        Id = EventId.BeforeMove;
        Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(BoolVoidUnion);
        
    // NEW: Specify nullability
        ParameterNullability = new[] { false, false, false, false };
 ReturnTypeNullable = false;
        
// NEW: Validate configuration
    ValidateConfiguration();
    }
}
```

## Benefits

? **Compile-time safety**: Catches configuration errors when EventHandlerInfo is created
? **Runtime validation**: Validates null arguments before invocation  
? **Clear documentation**: Signature description shows which parameters can be null  
? **Early error detection**: Fails fast with clear error messages  
? **Type consistency**: Ensures metadata matches actual implementation  

## Validation Points

1. **Construction Time** (via `ValidateConfiguration()`):
   - ParameterNullability length matches ExpectedParameterTypes length

2. **Handler Registration** (via `Validate()`):
   - Handler parameter types match ExpectedParameterTypes
   - Handler return type matches ExpectedReturnType
   - ParameterNullability length matches ExpectedParameterTypes length

3. **Invocation Time** (via `ValidateParameterNullability()`):
   - Non-null parameters aren't passed null values
   - Parameter count matches expected count

## Next Steps

To complete the implementation:

1. ? Update ~3 example EventHandlerInfo classes (done)
2. ? Update remaining ~497 concrete EventHandlerInfo classes
3. ? Update all callback invocation sites to use `InvokeCallback<T>()`
4. ? Add unit tests for validation logic
5. ? Document nullability patterns in developer guide

## Error Messages

The validation provides clear, actionable error messages:

```
Event BeforeMove: ParameterNullability length (3) does not match ExpectedParameterTypes length (4)
```

```
Event CriticalHit: Parameter 2 (Pokemon) is non-nullable but null was provided
```

```
Event DamageCallback: Handler returned null but return type is non-nullable
```
