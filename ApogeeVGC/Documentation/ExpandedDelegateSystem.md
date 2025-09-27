# Expanded Library Delegate System

## Overview

The Library delegate system has been expanded to support all move event handlers from the original `IMoveEventMethods` interface. The system cleverly reuses `CommonHandlers` delegates where possible and adds move-specific delegates only when needed.

## Key Components

### 1. EventId Enum (`ApogeeVGC\Sim\Events\EventEnums.cs`)

**CommonHandlers Delegates (Reusable):**
- `ModifierEffectHandler` - Modifies values with effect parameter
- `ModifierMoveHandler` - Modifies values with move parameter  
- `ResultMoveHandler` - Returns bool results
- `ExtResultMoveHandler` - Returns IntBoolUnion results
- `VoidEffectHandler` - Performs actions with effect parameter
- `VoidMoveHandler` - Performs actions with move parameter
- `ModifierSourceEffectHandler` - Source-first modifier with effect
- `ModifierSourceMoveHandler` - Source-first modifier with move
- `ResultSourceMoveHandler` - Source-first result handler
- `ExtResultSourceMoveHandler` - Source-first extended result handler
- `VoidSourceEffectHandler` - Source-first action with effect
- `VoidSourceMoveHandler` - Source-first action with move

**Move-Specific Delegates (When CommonHandlers Don't Fit):**
- `BasePowerCallback` - Move-specific base power calculation
- `BeforeMoveCallback` - Pre-move execution check
- `DamageCallback` - Move-specific damage calculation
- `OnEffectiveness` - Type effectiveness modification
- `OnModifyMove` - Move modification before execution
- And many more...

### 2. Delegate Type Mapping

The system maps multiple `IMoveEventMethods` handlers to single `CommonHandlers` delegates:

```csharp
// These ALL use VoidSourceMoveHandler:
OnAfterHit -> EventId.VoidSourceMoveHandler
OnAfterMove -> EventId.VoidSourceMoveHandler  
OnAfterMoveSecondarySelf -> EventId.VoidSourceMoveHandler
OnUseMoveMessage -> EventId.VoidSourceMoveHandler

// These ALL use ModifierSourceMoveHandler:
OnBasePower -> EventId.ModifierSourceMoveHandler
OnModifyPriority -> EventId.ModifierSourceMoveHandler

// These ALL use ResultMoveHandler:
OnHit -> EventId.ResultMoveHandler
OnHitField -> EventId.ResultMoveHandler
OnPrepareHit -> EventId.ResultMoveHandler
OnTryImmunity -> EventId.ResultMoveHandler
```

### 3. Registration Examples

```csharp
var library = new Library();

// Register a CommonHandlers delegate that works for multiple events
library.RegisterMoveDelegate<VoidSourceMoveHandler>(
    EventId.VoidSourceMoveHandler,
    MoveId.Tackle,
    (battleContext, source, target, move) => {
        // This handler works for OnAfterHit, OnAfterMove, OnUseMoveMessage, etc.
        Console.WriteLine($"{source.Name} made contact!");
    });

// Register a move-specific delegate
library.RegisterMoveDelegate<BasePowerCallbackHandler>(
    EventId.BasePowerCallback,
    MoveId.WeatherBall,
    (battleContext, pokemon, target, move) => {
        // Weather Ball has special base power calculation
        return IntFalseUnion.FromInt(100); // Weather-modified power
    });
```

### 4. Usage Examples

```csharp
// Get and use CommonHandlers delegates
var afterHitHandler = move.GetVoidSourceMoveDelegate(library);
afterHitHandler?.Invoke(battleContext, source, target, move);

// Get and use move-specific delegates
var basePowerCallback = move.GetBasePowerCallback(library);
var modifiedPower = basePowerCallback?.Invoke(battleContext, source, target, move);
```

## Benefits

### ? **No Duplication**
- Single `VoidSourceMoveHandler` serves 4+ different event types
- Reuses existing `CommonHandlers` delegate signatures
- Only creates new delegates when absolutely necessary

### ? **Type Safety** 
- Enum-based `EventId` prevents string typos
- Compile-time validation of delegate types
- Generic methods ensure correct casting

### ? **Centralized Management**
- All delegates stored in `Library` like other data
- Consistent API with `Moves`, `Abilities`, `Items`
- Extension methods provide clean access

### ? **Flexible Architecture**
- Can register the same handler for multiple event types
- Easy to add new event types without breaking changes
- Supports both generic and specific use cases

## Real-World Usage

### Move Database Loading
```csharp
public static void LoadMoveEffects(Library library)
{
    // Contact moves - single handler for multiple events
    foreach (var contactMove in GetContactMoves())
    {
        library.RegisterMoveDelegate<VoidSourceMoveHandler>(
            EventId.VoidSourceMoveHandler,
            contactMove.Id,
            HandleContactMove);
    }
    
    // Priority moves - single handler type, different values
    library.RegisterMoveDelegate<ModifierSourceMoveHandler>(
        EventId.ModifierSourceMoveHandler,
        MoveId.QuickAttack,
        (ctx, priority, src, tgt, mv) => priority + 1);
        
    library.RegisterMoveDelegate<ModifierSourceMoveHandler>(
        EventId.ModifierSourceMoveHandler,
        MoveId.ExtremeSpeed,
        (ctx, priority, src, tgt, mv) => priority + 2);
}
```

### Battle Engine Integration
```csharp
public void ExecuteMoveAfterHit(Pokemon source, Pokemon target, Move move)
{
    // All these events can use the same registered handler
    var handler = move.GetVoidSourceMoveDelegate(library);
    
    // Call for different event contexts
    handler?.Invoke(battleContext, source, target, move); // OnAfterHit
    handler?.Invoke(battleContext, source, target, move); // OnAfterMove
    handler?.Invoke(battleContext, source, target, move); // OnUseMoveMessage
}
```

## Migration from IMoveEventMethods

The system provides a clean migration path from the old interface-based approach:

1. **Keep existing logic** - Just change how you register/access handlers
2. **Reduce code duplication** - Multiple events can share handlers
3. **Improve type safety** - Enums instead of strings
4. **Better performance** - Direct delegate calls instead of interface dispatch

This expansion transforms the delegate system into a comprehensive, efficient solution for handling all Pokemon Showdown move events while maintaining the clean architecture you established.