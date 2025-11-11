# Event Handler Specialized Classes

## Directory Structure

```
ApogeeVGC/Sim/Events/
??? EventHandlerInfo.cs           (abstract base class)
??? IEventMethodsV2.cs     (interface - properties return EventHandlerInfo)
??? EventEnums.cs         (EventId enum)
??? Handlers/         (specialized event handler classes)
  ??? OnDamagingHitEventInfo.cs      ? Created
    ??? OnBasePowerEventInfo.cs        ? Created
    ??? OnResidualEventInfo.cs         ? Created
    ??? OnBeforeMoveEventInfo.cs       ? Created
    ??? OnAfterSetStatusEventInfo.cs   ? Created
 ??? OnSetStatusEventInfo.cs      ? Created
    ??? OnDamageEventInfo.cs           ? Created
    ??? OnEmergencyExitEventInfo.cs  ? Created
    ??? ... (~192 more to create)
```

## Naming Convention

**Class Name:** `On{EventId}EventInfo`
- EventId: `DamagingHit` ? Class: `OnDamagingHitEventInfo`
- EventId: `BasePower` ? Class: `OnBasePowerEventInfo`
- EventId: `Residual` ? Class: `OnResidualEventInfo`

**File Name:** Same as class name
- `OnDamagingHitEventInfo.cs`
- `OnBasePowerEventInfo.cs`

## Template for Creating New Event Classes

```csharp
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
// Add other necessary usings based on signature

namespace ApogeeVGC.Sim.Events.Handlers;

/// <summary>
/// Event handler info for On{EventId} event.
/// {Brief description of when this event is triggered}
/// Signature: {Full method signature}
/// </summary>
public sealed record On{EventId}EventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new On{EventId} event handler.
    /// </summary>
  /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first) - OPTIONAL</param>
    /// <param name="order">Execution order (lower executes first) - OPTIONAL</param>
    /// <param name="subOrder">Sub-order for fine-grained ordering - OPTIONAL</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public On{EventId}EventInfo(
        {HandlerType} handler,
  // Add optional parameters as needed:
        // int? priority = null,
        // int? order = null,
        // int? subOrder = null,
        bool usesSpeed = {true|false})
    {
        Id = EventId.{EventId};
        Handler = handler;
     // Set optional properties:
        // Priority = priority;
    // Order = order;
    // SubOrder = subOrder;
   UsesSpeed = usesSpeed;
        ExpectedParameterTypes = new[] 
     { 
          typeof({Param1Type}),
typeof({Param2Type}),
            // ... all parameter types
    };
        ExpectedReturnType = typeof({ReturnType});
}
}
```

## How to Determine Signature

Look at `IEventMethods.cs` for the signature. For example:

```csharp
// From IEventMethods.cs:
Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnDamagingHit { get; }

// Becomes:
public OnDamagingHitEventInfo(
    Action<Battle, int, Pokemon, Pokemon, ActiveMove> handler,
    int? order = null,
    bool usesSpeed = true)
{
    Id = EventId.DamagingHit;
    Handler = handler;
    Order = order;
    UsesSpeed = usesSpeed;
    ExpectedParameterTypes = new[] 
    { 
     typeof(Battle), 
    typeof(int), 
        typeof(Pokemon), 
        typeof(Pokemon), 
   typeof(ActiveMove) 
    };
    ExpectedReturnType = typeof(void);
}
```

## Common Handler Types

| IEventMethods Type | Constructor Parameter Type | Return Type |
|-------------------|---------------------------|-------------|
| `Action<Battle, Pokemon>` | `Action<Battle, Pokemon>` | `typeof(void)` |
| `Action<Battle, int, Pokemon, Pokemon, ActiveMove>` | `Action<Battle, int, Pokemon, Pokemon, ActiveMove>` | `typeof(void)` |
| `VoidSourceMoveHandler` | `VoidSourceMoveHandler` | `typeof(BoolVoidUnion)` |
| `ModifierSourceMoveHandler` | `ModifierSourceMoveHandler` | `typeof(DoubleVoidUnion)` |
| `Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?>` | `Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?>` | `typeof(IntBoolVoidUnion)` |

## Optional Parameters

Based on `IEventMethods.cs` priority/order properties:

- **Priority:** Used for events that need execution priority (higher = earlier)
  - Example: `OnModifyAtkPriority`, `OnBasePowerPriority`
  
- **Order:** Used for events with explicit execution order (lower = earlier)
  - Example: `OnResidualOrder`, `OnDamagingHitOrder`
  
- **SubOrder:** Used for fine-grained ordering within same Order
  - Example: `OnResidualSubOrder`

## Checklist for Each Class

- [ ] Class name matches pattern: `On{EventId}EventInfo`
- [ ] Inherits from `EventHandlerInfo`
- [ ] Marked as `sealed record`
- [ ] XML documentation describes when event triggers
- [ ] Handler parameter type matches `IEventMethods` signature
- [ ] Optional parameters (priority/order/subOrder) added if needed
- [ ] `Id` set correctly
- [ ] `ExpectedParameterTypes` array matches handler signature exactly
- [ ] `ExpectedReturnType` matches return type (void for Action<>)
- [ ] `UsesSpeed` default matches event behavior

## Example Patterns

### Simple Action Event (2 parameters)
```csharp
public OnEmergencyExitEventInfo(
    Action<Battle, Pokemon> handler,
    bool usesSpeed = true)
{
    Id = EventId.EmergencyExit;
    Handler = handler;
    UsesSpeed = usesSpeed;
 ExpectedParameterTypes = new[] { typeof(Battle), typeof(Pokemon) };
    ExpectedReturnType = typeof(void);
}
```

### Modifier Event (returns value)
```csharp
public OnBasePowerEventInfo(
    ModifierSourceMoveHandler handler,
    int? priority = null,
    bool usesSpeed = true)
{
    Id = EventId.BasePower;
    Handler = handler;
    Priority = priority;
 UsesSpeed = usesSpeed;
    ExpectedParameterTypes = new[] 
    { 
     typeof(Battle), 
        typeof(int), 
        typeof(Pokemon), 
      typeof(Pokemon), 
        typeof(ActiveMove) 
    };
    ExpectedReturnType = typeof(DoubleVoidUnion);
}
```

### Residual Event (priority + order + subOrder)
```csharp
public OnResidualEventInfo(
    Action<Battle, Pokemon, Pokemon, IEffect> handler,
    int? priority = null,
    int? order = null,
int? subOrder = null,
    bool usesSpeed = true)
{
    Id = EventId.Residual;
    Handler = handler;
    Priority = priority;
    Order = order;
    SubOrder = subOrder;
    UsesSpeed = usesSpeed;
    ExpectedParameterTypes = new[] 
    { 
        typeof(Battle), 
        typeof(Pokemon), 
typeof(Pokemon), 
   typeof(IEffect) 
    };
    ExpectedReturnType = typeof(void);
}
```

## Next Steps

1. **For each EventId in EventEnums.cs:**
   - Find corresponding property in `IEventMethods.cs`
 - Copy template
   - Fill in signature details
   - Create file in `Handlers/` directory

2. **Update IEventMethodsV2.cs:**
   - Change methods to properties
   - Return type is `EventHandlerInfo?`

3. **Update Ability.cs:**
   - Change property types from `EventHandlerInfo?` to specific types
   - Example: `OnDamagingHitEventInfo? OnDamagingHitInfo { get; init; }`

4. **Update Abilities.cs (data):**
   - Use new specialized classes
   - Example: `OnDamagingHitInfo = new OnDamagingHitEventInfo(...)`
