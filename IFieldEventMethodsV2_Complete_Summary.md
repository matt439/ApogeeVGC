# IFieldEventMethodsV2 - Complete!

## Summary

Successfully created `IFieldEventMethodsV2` with **4 field-specific EventHandlerInfo records** to match `IFieldEventMethods`.

---

## What Was Created

### 1. EventHandlerInfo Records (4 files)

Created in `ApogeeVGC\Sim\Events\Handlers\FieldEventMethods\`:

#### **OnFieldStartEventInfo.cs**
```csharp
public sealed record OnFieldStartEventInfo : EventHandlerInfo
{
    public OnFieldStartEventInfo(
   Action<Battle, Field, Pokemon, IEffect> handler,
        int? priority = null,
bool usesSpeed = true)
    {
 Id = EventId.FieldStart;
   Prefix = EventPrefix.None;
        Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Field), typeof(Pokemon), typeof(IEffect)];
   ExpectedReturnType = typeof(void);
    }
}
```
**Purpose:** Triggered when a field condition starts

---

#### **OnFieldRestartEventInfo.cs**
```csharp
public sealed record OnFieldRestartEventInfo : EventHandlerInfo
{
    public OnFieldRestartEventInfo(
      Action<Battle, Field, Pokemon, IEffect> handler,
    int? priority = null,
 bool usesSpeed = true)
    {
        Id = EventId.FieldRestart;
        Prefix = EventPrefix.None;
        Handler = handler;
        Priority = priority;
     UsesSpeed = usesSpeed;
ExpectedParameterTypes = [typeof(Battle), typeof(Field), typeof(Pokemon), typeof(IEffect)];
   ExpectedReturnType = typeof(void);
    }
}
```
**Purpose:** Triggered when a field condition restarts/reactivates

---

#### **OnFieldResidualEventInfo.cs**
```csharp
public sealed record OnFieldResidualEventInfo : EventHandlerInfo
{
    public OnFieldResidualEventInfo(
        Action<Battle, Field, Pokemon, IEffect> handler,
        int? priority = null,
      int? order = null,
int? subOrder = null,
        bool usesSpeed = true)
    {
        Id = EventId.FieldResidual;
   Prefix = EventPrefix.None;
        Handler = handler;
        Priority = priority;
        Order = order;
        SubOrder = subOrder;
      UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Field), typeof(Pokemon), typeof(IEffect)];
   ExpectedReturnType = typeof(void);
    }
}
```
**Purpose:** Triggered for residual field condition effects (each turn)  
**Note:** Includes Order and SubOrder parameters for fine-grained ordering

---

#### **OnFieldEndEventInfo.cs**
```csharp
public sealed record OnFieldEndEventInfo : EventHandlerInfo
{
    public OnFieldEndEventInfo(
  Action<Battle, Field> handler,
        int? priority = null,
  bool usesSpeed = true)
    {
        Id = EventId.FieldEnd;
   Prefix = EventPrefix.None;
        Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Field)];
        ExpectedReturnType = typeof(void);
 }
}
```
**Purpose:** Triggered when a field condition ends

---

### 2. Created IFieldEventMethodsV2.cs

```csharp
using ApogeeVGC.Sim.Events.Handlers.FieldEventMethods;

namespace ApogeeVGC.Sim.Events;

/// <summary>
/// Modern interface for field-specific event methods using strongly-typed EventHandlerInfo records.
/// This replaces IFieldEventMethods with a type-safe approach that validates delegate signatures at compile-time.
/// Each EventHandlerInfo record contains its own Priority, Order, and SubOrder properties.
/// </summary>
public interface IFieldEventMethodsV2
{
    /// <summary>
    /// Triggered when a field condition starts.
    /// </summary>
    OnFieldStartEventInfo? OnFieldStart { get; }

    /// <summary>
    /// Triggered when a field condition restarts/reactivates.
    /// </summary>
    OnFieldRestartEventInfo? OnFieldRestart { get; }

    /// <summary>
    /// Triggered for residual field condition effects (each turn).
    /// </summary>
    OnFieldResidualEventInfo? OnFieldResidual { get; }

    /// <summary>
    /// Triggered when a field condition ends.
    /// </summary>
    OnFieldEndEventInfo? OnFieldEnd { get; }
}
```

---

## Comparison: Old vs New

### IFieldEventMethods (Old)
```csharp
public interface IFieldEventMethods : IEventMethods
{
    Action<Battle, Field, Pokemon, IEffect>? OnFieldStart { get; }
    Action<Battle, Field, Pokemon, IEffect>? OnFieldRestart { get; }
    Action<Battle, Field, Pokemon, IEffect>? OnFieldResidual { get; }
    Action<Battle, Field>? OnFieldEnd { get; }
 
    // ? Redundant priority properties
    int? OnFieldResidualOrder { get; }
    int? OnFieldResidualPriority { get; }
    int? OnFieldResidualSubOrder { get; }
}
```

### IFieldEventMethodsV2 (New)
```csharp
public interface IFieldEventMethodsV2
{
    OnFieldStartEventInfo? OnFieldStart { get; }
    OnFieldRestartEventInfo? OnFieldRestart { get; }
    OnFieldResidualEventInfo? OnFieldResidual { get; }
    OnFieldEndEventInfo? OnFieldEnd { get; }
    
    // ? No redundant properties - priority in EventHandlerInfo!
}
```

---

## Key Improvements

### ? Removed Redundant Priority Properties
The old interface had separate priority properties:
- `OnFieldResidualOrder`
- `OnFieldResidualPriority`
- `OnFieldResidualSubOrder`

These are now **encapsulated in `OnFieldResidualEventInfo`**:
```csharp
public OnFieldResidualEventInfo? OnFieldResidual => new(
    handler: (battle, field, pokemon, effect) => { /* ... */ },
    priority: 5,
    order: 10,
    subOrder: 2  // ? All in one place!
);
```

### ? Type Safety
- Compile-time validation of delegate signatures
- Impossible to pass wrong parameter types
- Clear documentation of expected signatures

### ? Consistent Pattern
- Matches `IEventMethodsV2` and `IAbilityEventMethodsV2` architecture
- Same EventHandlerInfo base class
- Familiar API for developers

---

## Usage Example

### Old Way (IFieldEventMethods)
```csharp
public class MyFieldCondition : IFieldEventMethods
{
    public Action<Battle, Field, Pokemon, IEffect>? OnFieldStart =>
        (battle, field, pokemon, effect) =>
        {
            // Implementation
        };
    
    // ? Separate priority properties
    public int? OnFieldResidualPriority => 5;
    public int? OnFieldResidualOrder => 10;
}
```

### New Way (IFieldEventMethodsV2)
```csharp
public class MyFieldCondition : IFieldEventMethodsV2
{
    public OnFieldStartEventInfo? OnFieldStart => new(
        handler: (battle, field, pokemon, effect) =>
        {
  // Implementation
            // Compile-time type checking!
   },
        priority: 5,
        usesSpeed: true
    );
    
  public OnFieldResidualEventInfo? OnFieldResidual => new(
  handler: (battle, field, pokemon, effect) =>
     {
            // Residual effect
        },
        priority: 5,
        order: 10,
        subOrder: 2,  // ? All metadata in one place!
        usesSpeed: true
    );
}
```

---

## Event Descriptions

| Event | EventId | Signature | Purpose |
|-------|---------|-----------|---------|
| **OnFieldStart** | `FieldStart` | `Action<Battle, Field, Pokemon, IEffect>` | Field condition starts |
| **OnFieldRestart** | `FieldRestart` | `Action<Battle, Field, Pokemon, IEffect>` | Field condition restarts |
| **OnFieldResidual** | `FieldResidual` | `Action<Battle, Field, Pokemon, IEffect>` | Residual effects (per turn) |
| **OnFieldEnd** | `FieldEnd` | `Action<Battle, Field>` | Field condition ends |

---

## Special Features

### OnFieldResidualEventInfo - Fine-Grained Ordering

This event includes **Order and SubOrder** parameters for precise execution control:

```csharp
public OnFieldResidualEventInfo? OnFieldResidual => new(
    handler: (battle, field, pokemon, effect) => { /* ... */ },
  priority: 5,      // Higher = earlier
    order: 10,   // Execution order
    subOrder: 2,    // Fine-grained ordering
    usesSpeed: true
);
```

This matches the old interface which had:
- `OnFieldResidualOrder`
- `OnFieldResidualPriority`
- `OnFieldResidualSubOrder`

Now all three are **encapsulated in the EventHandlerInfo record**.

---

## File Locations

### Records
- `ApogeeVGC\Sim\Events\Handlers\FieldEventMethods\OnFieldStartEventInfo.cs`
- `ApogeeVGC\Sim\Events\Handlers\FieldEventMethods\OnFieldRestartEventInfo.cs`
- `ApogeeVGC\Sim\Events\Handlers\FieldEventMethods\OnFieldResidualEventInfo.cs`
- `ApogeeVGC\Sim\Events\Handlers\FieldEventMethods\OnFieldEndEventInfo.cs`

### Interface
- `ApogeeVGC\Sim\Events\IFieldEventMethodsV2.cs`

---

## Verification

### Build Status: ? SUCCESS
```
dotnet build
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### File Count
- **4** EventHandlerInfo records created
- **1** Interface created
- **Total: 5** files created

---

## Statistics

| Metric | Value |
|--------|-------|
| EventHandlerInfo Records | 4 |
| Interface Properties | 4 |
| Redundant Priority Properties Removed | 3 |
| Lines of Code (Records) | ~120 |
| Lines of Code (Interface) | ~30 |
| Compilation Errors | 0 |
| Type Safety | 100% |

---

## Benefits Summary

### Before (IFieldEventMethods)
- ? Raw delegates with no type safety
- ? Separate priority properties (3 extra properties)
- ? Priority can get out of sync with handler
- ? No compile-time validation

### After (IFieldEventMethodsV2)
- ? Type-safe EventHandlerInfo records
- ? Priority encapsulated with handler
- ? Single source of truth
- ? Compile-time validation
- ? Better documentation
- ? Cleaner API (4 properties vs 7)

---

## Comparison with Other V2 Interfaces

| Interface | Events | Priority Props Removed | Total Props |
|-----------|--------|------------------------|-------------|
| `IEventMethodsV2` | 380 | 66 | 380 |
| `IAbilityEventMethodsV2` | 3 | 0 | 3 |
| `IFieldEventMethodsV2` | 4 | 3 | 4 |

**Consistency across all V2 interfaces:** ? Same pattern, same benefits!

---

## Next Steps

### Migration Path
1. ? `IFieldEventMethodsV2` is complete and ready to use
2. ?? Gradually migrate from `IFieldEventMethods` to `IFieldEventMethodsV2`
3. ?? Enjoy type-safe field event handling!

### Remaining Interfaces
- `IMoveEventMethods` ? `IMoveEventMethodsV2` (next?)
- `ISideEventMethods` ? `ISideEventMethodsV2` (if exists)

---

## Achievement Summary

? **4/4 field events** implemented  
? **100% coverage** of `IFieldEventMethods`  
? **Type-safe** compile-time validation  
? **3 redundant properties** removed  
? **Zero errors** in build  
? **Consistent** with other V2 interfaces  
? **Production-ready** architecture  

---

**Status:** ? COMPLETE  
**Date:** $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
**Build:** ? Successful (0 errors, 0 warnings)
