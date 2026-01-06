# Migration Guide: Adding Context Support to Event Handler Classes

## Overview

This guide shows how to enhance existing `EventHandlerInfo` subclasses to support both legacy and context-based handlers while maintaining strong typing.

## Pattern to Follow

Every concrete event handler class should support **THREE constructor patterns**:

### 1. Legacy Constructor (Keep As-Is)
The existing strongly-typed constructor with specific parameters.

### 2. Context Constructor (Add New)
A new constructor that accepts `EventHandlerDelegate`.

### 3. Create Helper (Add New - Recommended)
A static factory method that provides strongly-typed parameters but uses context internally.

## Template

```csharp
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Utils.Unions;
// ... other using statements ...

namespace ApogeeVGC.Sim.Events.Handlers.XXX;

/// <summary>
/// Event handler info for OnXXX event.
/// Description of when this event triggers.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Param1, Param2, ...) => ReturnType
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnXXXEventInfo : EventHandlerInfo
{
    // === LEGACY CONSTRUCTOR (KEEP AS-IS) ===
  
    /// <summary>
    /// Creates event handler using legacy strongly-typed pattern.
    /// </summary>
    public OnXXXEventInfo(
        Func<Param1Type, Param2Type, ..., ReturnType> handler,
    int? priority = null,
        ... other params ...)
    {
     Id = EventId.XXX;
        Handler = handler;
      Priority = priority;
        // ... existing initialization ...
        
        ExpectedParameterTypes = [typeof(Param1Type), typeof(Param2Type), ...];
        ExpectedReturnType = typeof(ReturnType);
      ParameterNullability = [false, false, ...]; // Adjust as needed
        ReturnTypeNullable = false; // Adjust as needed
      
        ValidateConfiguration();
    }
    
    // === CONTEXT CONSTRUCTOR (ADD NEW) ===
    
    /// <summary>
/// Creates event handler using context-based pattern.
    /// Context provides: Battle, TargetPokemon, SourcePokemon, Move, etc.
  /// </summary>
    public OnXXXEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
   ... other params ...)
    {
   Id = EventId.XXX;
        ContextHandler = contextHandler;
        Priority = priority;
        // ... copy other initialization ...
        
        // No ExpectedParameterTypes needed for context handlers
    }
    
    // === CREATE HELPER (ADD NEW - RECOMMENDED) ===
    
    /// <summary>
  /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnXXXEventInfo Create(
        Func<Param1Type, Param2Type, ..., RelayVar?> handler,
        int? priority = null,
    ... other params ...)
  {
        return new OnXXXEventInfo(
    context => handler(
        context.GetParam1(), // Use appropriate accessor
  context.GetParam2(),
   // ...
            ),
       priority,
       // ... other params ...
        );
  }
}
```

## Parameter Mapping Guide

Map legacy parameters to `EventContext` accessors:

| Legacy Parameter Type | EventContext Accessor |
|----------------------|----------------------|
| `Battle battle` | `context.Battle` |
| `Pokemon target` | `context.GetTargetPokemon()` |
| `Pokemon source` | `context.GetSourcePokemon()` |
| `ActiveMove move` | `context.GetMove()` |
| `IEffect effect` / `IEffect sourceEffect` | `context.GetSourceEffect<IEffect>()` |
| `Side side` | `context.GetTargetSide()` |
| `Field field` | `context.Battle.Field` |
| `int relayValue` | `context.GetRelayVar<IntRelayVar>().Value` |
| `decimal relayValue` | `context.GetRelayVar<DecimalRelayVar>().Value` |

## Return Type Mapping

Map legacy return types to `RelayVar?`:

| Legacy Return Type | Context Return Type |
|-------------------|---------------------|
| `void` / `VoidReturn` | `return null;` |
| `bool` | `return new BoolRelayVar(value);` |
| `int` | `return new IntRelayVar(value);` |
| `BoolVoidUnion` | `return value ? new BoolRelayVar(value) : null;` |
| `IntBoolVoidUnion` (bool) | `return new BoolRelayVar(value);` |
| `IntBoolVoidUnion` (int) | `return new IntRelayVar(value);` |

## Example 1: OnBeforeMoveEventInfo (Already Updated)

```csharp
public sealed record OnBeforeMoveEventInfo : EventHandlerInfo
{
    // Legacy constructor - keep as-is
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
     ParameterNullability = [false, false, false, false];
        ReturnTypeNullable = false;
        ValidateConfiguration();
    }
    
    // Context constructor - NEW
 public OnBeforeMoveEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.BeforeMove;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    
    // Create helper - NEW
    public static OnBeforeMoveEventInfo Create(
        Func<Battle, Pokemon, Pokemon, ActiveMove, RelayVar?> handler,
     int? priority = null,
  bool usesSpeed = true)
    {
   return new OnBeforeMoveEventInfo(
            context => handler(
         context.Battle,
       context.GetTargetPokemon(),
                context.GetSourcePokemon(),
context.GetMove()
   ),
       priority,
            usesSpeed
        );
    }
}
```

## Example 2: OnResidualEventInfo

**Before:**
```csharp
public sealed record OnResidualEventInfo : EventHandlerInfo
{
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
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(IEffect)];
     ExpectedReturnType = typeof(void);
        ParameterNullability = [false, false, false, false];
     ReturnTypeNullable = false;
        ValidateConfiguration();
    }
}
```

**After (Add These):**
```csharp
public sealed record OnResidualEventInfo : EventHandlerInfo
{
    // Keep existing constructor
    public OnResidualEventInfo(
     Action<Battle, Pokemon, Pokemon, IEffect> handler,
   int? priority = null,
        int? order = null,
      int? subOrder = null,
        bool usesSpeed = true)
    {
        // ... existing code ...
    }
  
    // ADD: Context constructor
    public OnResidualEventInfo(
        EventHandlerDelegate contextHandler,
int? priority = null,
        int? order = null,
        int? subOrder = null,
        bool usesSpeed = true)
    {
        Id = EventId.Residual;
        ContextHandler = contextHandler;
        Priority = priority;
  Order = order;
      SubOrder = subOrder;
        UsesSpeed = usesSpeed;
    }
    
    // ADD: Create helper
    public static OnResidualEventInfo Create(
        Action<Battle, Pokemon, Pokemon, IEffect> handler,
     int? priority = null,
        int? order = null,
  int? subOrder = null,
        bool usesSpeed = true)
    {
        return new OnResidualEventInfo(
        context =>
 {
          handler(
         context.Battle,
             context.GetTargetPokemon(),
        context.GetSourcePokemon(),
                 context.GetSourceEffect<IEffect>()
        );
      return null; // void return
       },
 priority,
            order,
 subOrder,
     usesSpeed
      );
    }
}
```

## Example 3: OnModifySpeEventInfo

**Before:**
```csharp
public sealed record OnModifySpeEventInfo : EventHandlerInfo
{
    public OnModifySpeEventInfo(
        Func<Battle, int, Pokemon, int> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifySpe;
        Handler = handler;
        // ... initialization ...
  }
}
```

**After:**
```csharp
public sealed record OnModifySpeEventInfo : EventHandlerInfo
{
    // Keep existing
    public OnModifySpeEventInfo(
      Func<Battle, int, Pokemon, int> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        // ... existing code ...
    }
    
    // ADD: Context constructor
    public OnModifySpeEventInfo(
        EventHandlerDelegate contextHandler,
     int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifySpe;
     ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    
    // ADD: Create helper
    public static OnModifySpeEventInfo Create(
    Func<Battle, int, Pokemon, int> handler,
        int? priority = null,
     bool usesSpeed = true)
    {
        return new OnModifySpeEventInfo(
          context =>
       {
                int spe = context.GetRelayVar<IntRelayVar>().Value;
    int result = handler(
          context.Battle,
  spe,
    context.GetTargetPokemon()
   );
      return new IntRelayVar(result);
            },
  priority,
   usesSpeed
    );
    }
}
```

## Migration Strategy

### Phase 1: High-Priority Events (Do First)
Update these frequently-used events first:
- `OnBeforeMoveEventInfo` ? (already done)
- `OnDamagingHitEventInfo`
- `OnBasePowerEventInfo`
- `OnModifySpeEventInfo`
- `OnModifyAtkEventInfo`
- `OnModifyDefEventInfo`
- `OnResidualEventInfo`
- `OnStartEventInfo` (all variants)

### Phase 2: Medium-Priority Events (Do Next)
- All `OnModifyXXX` events
- All damage calculation events
- Move preparation events

### Phase 3: Low-Priority Events (Do Later)
- Rarely-used events
- Special case events
- Field/Side events

### Phase 4: Cleanup (Eventually)
- Once all events migrated
- Remove legacy adapter code
- Remove obsolete methods from Battle.Delegates.cs

## Benefits of Migration

### Before Migration
```csharp
// Only one way to use it
new OnResidualEventInfo((battle, target, source, effect) =>
{
    battle.Damage(target.BaseMaxHp / 16);
}, order: 10)
```

### After Migration
```csharp
// THREE ways to use it:

// 1. Legacy (still works)
new OnResidualEventInfo((battle, target, source, effect) =>
{
    battle.Damage(target.BaseMaxHp / 16);
}, order: 10)

// 2. Context (new, flexible)
new OnResidualEventInfo(context =>
{
    context.Battle.Damage(context.GetTargetPokemon().BaseMaxHp / 16);
    return null;
}, order: 10)

// 3. Create (best of both!)
OnResidualEventInfo.Create((battle, target, source, effect) =>
{
    battle.Damage(target.BaseMaxHp / 16);
}, order: 10)
```

## Quick Reference Checklist

When updating an event handler class:

- [ ] Keep existing constructor unchanged
- [ ] Add context constructor accepting `EventHandlerDelegate`
- [ ] Add static `Create` method with same signature as legacy
- [ ] Map parameters to appropriate context accessors
- [ ] Convert return type (void ? null, bool ? BoolRelayVar, etc.)
- [ ] Copy all optional parameters to both new constructors
- [ ] Update XML docs to mention context support
- [ ] Test with existing code (should still compile)

## Testing Your Changes

```csharp
// All three patterns should work:

// Legacy
var legacy = new OnXXXEventInfo((p1, p2) => result);

// Context
var context = new OnXXXEventInfo(ctx => new BoolRelayVar(true));

// Create
var create = OnXXXEventInfo.Create((p1, p2) => new BoolRelayVar(true));

// Verify they all compile and run
```

## Need Help?

Refer to:
- `OnBeforeMoveEventInfo.cs` - Complete working example
- `EVENT_CONTEXT_REFACTORING.md` - Full migration guide
- `EventContext.cs` - See all available properties
- `EventHandlerHelpers.cs` - Helper methods for common patterns
