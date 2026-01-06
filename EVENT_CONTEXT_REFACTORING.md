# Event System Refactoring: Context-Based Architecture

## Overview

The event system has been refactored to use a **strongly-typed context-based architecture** that eliminates reflection, improves type safety, and makes handlers much easier to write and debug.

## What Changed

### Old Way (Complex & Fragile)
```csharp
// Legacy handler with specific parameters - hard to understand what's available
new OnBeforeMoveEventInfo((Battle battle, Pokemon target, Pokemon source, ActiveMove move) =>
{
    // Magic parameter matching by name and type
    if (battle.RandomChance(1, 4))
    {
    battle.Add("cant", target, "par");
        return false;
    }
    return new VoidReturn();
})
```

**Problems:**
- ? Parameter matching by magic strings ("target", "source")
- ? Complex reflection-based invocation
- ? Two sources of truth (ParameterInfoCache + EventHandlerInfo)
- ? Hard to debug type mismatches
- ? Different signatures for every event type

### New Way (Clean & Type-Safe)
```csharp
// Context-based handler - everything available in one place
new OnBeforeMoveEventInfo(context =>
{
    if (context.Battle.RandomChance(1, 4))
    {
        context.Battle.Add("cant", context.GetTargetPokemon(), "par");
   return new BoolRelayVar(false);
    }
    return null; // void return
})
```

**Benefits:**
- ? One delegate signature for all events
- ? IntelliSense shows everything available
- ? Type-safe accessors with clear error messages
- ? No reflection during invocation
- ? Easy to test and debug
- ? Backward compatible with existing handlers

## Architecture

### Core Components

```
EventContext (public API)
??? Battle, EventId
??? TargetPokemon, TargetSide, TargetField
??? SourcePokemon, SourceEffect
??? Move, RelayVar
??? Type-safe accessors (GetTargetPokemon(), etc.)

EventHandlerDelegate
??? RelayVar? Handler(EventContext context)

EventHandlerInfo
??? Delegate? Handler (legacy)
??? EventHandlerDelegate? ContextHandler (new)

EventHandlerAdapter
??? Converts legacy handlers to context-based automatically
```

### Event Flow

```
1. Event triggered
   ?
2. Build EventInvocationContext (internal)
 ?
3. Convert to EventContext (public)
   ?
4. If handler is context-based:
   ? Invoke directly (fast path)
   ?
5. If handler is legacy:
   ? Adapt using EventHandlerAdapter
   ? Invoke adapted handler
   ?
6. Return RelayVar result
```

## Using EventContext

### Available Properties

```csharp
public sealed class EventContext
{
    // Always available
 public Battle Battle { get; }
    public EventId EventId { get; }
    
    // Available for specific event types
    public Pokemon? TargetPokemon { get; }        // Pokemon-targeted events
    public Pokemon? SourcePokemon { get; }        // Events with a source Pokemon
    public Side? TargetSide { get; }              // Side-targeted events
    public Field? TargetField { get; }     // Field events
    public ActiveMove? Move { get; }       // Move events
    public IEffect? SourceEffect { get; }         // Events triggered by an effect
    public RelayVar? RelayVar { get; }            // Relay variable
}
```

### Type-Safe Accessors

Instead of null checks, use the type-safe accessors:

```csharp
// Throws if not available - use for required properties
Pokemon target = context.GetTargetPokemon();
Pokemon source = context.GetSourcePokemon();
ActiveMove move = context.GetMove();
TEffect effect = context.GetSourceEffect<TEffect>();

// Returns null if not available - use for optional properties
TEffect? effect = context.TryGetSourceEffect<TEffect>();
IntRelayVar? intVar = context.TryGetRelayVar<IntRelayVar>();

// Boolean checks
if (context.HasTargetPokemon) { ... }
if (context.HasMove) { ... }
```

## Migration Guide

### Step 1: Identify Handler Signature

Look at what parameters your handler needs:

```csharp
// Old
(Battle battle, Pokemon target, Pokemon source, IEffect effect) => { ... }

// Identify: needs battle, target pokemon, source pokemon, source effect
```

### Step 2: Convert to Context Access

Replace parameters with context property access:

```csharp
// New
context =>
{
    var battle = context.Battle;
    var target = context.GetTargetPokemon();
 var source = context.GetSourcePokemon();
    var effect = context.SourceEffect;
    
  // ... handler logic ...
}
```

### Step 3: Update Return Value

Return `RelayVar?` instead of union types:

```csharp
// Old return types:
return new VoidReturn();           ? return null;
return true;   ? return new BoolRelayVar(true);
return false;    ? return new BoolRelayVar(false);
return 42;      ? return new IntRelayVar(42);
return BoolVoidUnion.FromBool(...) ? return new BoolRelayVar(...);
```

### Step 4: Use Helper Methods (Optional)

For simple cases, use the helper methods:

```csharp
using static ApogeeVGC.Sim.Events.EventHandlerHelpers;

// Simple void handler
CreateVoidHandler(context => 
{
    context.Battle.Add("-status", context.GetTargetPokemon(), "brn");
});

// Boolean handler
CreateBoolHandler(context => 
    context.Battle.RandomChance(1, 4));

// Conditional handler
CreateConditionalHandler(
    context => context.HasTargetPokemon,
    context => { /* logic */ return null; }
);
```

## Examples

### Example 1: OnBeforeMove (Paralysis)

```csharp
// OLD WAY
new OnBeforeMoveEventInfo((Battle battle, Pokemon target, Pokemon source, ActiveMove move) =>
{
    if (!battle.RandomChance(1, 4))
        return new VoidReturn();
        
    if (battle.DisplayUi)
    {
        battle.Add("cant", target, "par");
    }
    return false;
}, priority: 1)

// NEW WAY
new OnBeforeMoveEventInfo(context =>
{
    if (!context.Battle.RandomChance(1, 4))
        return null;
        
    if (context.Battle.DisplayUi)
    {
        context.Battle.Add("cant", context.GetTargetPokemon(), "par");
 }
    return new BoolRelayVar(false);
}, priority: 1)
```

### Example 2: OnStart (Burn)

```csharp
// OLD WAY
new OnStartEventInfo((Battle battle, Pokemon target, Pokemon source, IEffect sourceEffect) =>
{
    if (sourceEffect is null)
        throw new ArgumentNullException("sourceEffect");
      
    if (!battle.DisplayUi) return new VoidReturn();
    
    if (sourceEffect is Item { Id: ItemId.FlameOrb })
    {
        battle.Add("-status", target, "brn", "[from] item: Flame Orb");
    }
    
    return new VoidReturn();
})

// NEW WAY
new OnStartEventInfo(context =>
{
    var effect = context.GetSourceEffect<IEffect>();
    
    if (!context.Battle.DisplayUi)
   return null;
    
    if (effect is Item { Id: ItemId.FlameOrb })
    {
        context.Battle.Add("-status", context.GetTargetPokemon(), "brn", "[from] item: Flame Orb");
    }
    
    return null;
})
```

### Example 3: OnModifySpe (Paralysis)

```csharp
// OLD WAY
new OnModifySpeEventInfo((Battle battle, int spe, Pokemon pokemon) =>
{
    spe = battle.FinalModify(spe);
    if (!pokemon.HasAbility(AbilityId.QuickFeet))
    {
        spe = (int)Math.Floor(spe * 50.0 / 100);
    }
    return spe;
}, priority: -101)

// NEW WAY  
new OnModifySpeEventInfo(context =>
{
 var battle = context.Battle;
    var pokemon = context.GetTargetPokemon();
  var spe = context.GetRelayVar<IntRelayVar>().Value;
    
    spe = battle.FinalModify(spe);
    if (!pokemon.HasAbility(AbilityId.QuickFeet))
    {
 spe = (int)Math.Floor(spe * 50.0 / 100);
    }
    return new IntRelayVar(spe);
}, priority: -101)
```

### Example 4: Using Helper Methods

```csharp
// Void handler (no return value)
new OnResidualEventInfo(
    CreateVoidHandler(context =>
    {
    var battle = context.Battle;
        var pokemon = context.GetTargetPokemon();
        battle.Damage(pokemon.BaseMaxHp / 16);
    }),
    order: 10
)

// Boolean handler
new OnTryHitEventInfo(
    CreateBoolHandler(context =>
        context.GetMove().Flags.Protect == true
    )
)

// Chained handlers
new OnDamageEventInfo(
    ChainHandlers(
        context => { /* first check */ return context.RelayVar; },
      context => { /* second check */ return context.RelayVar; },
     context => { /* final modification */ return new IntRelayVar(42); }
    )
)
```

## Backward Compatibility

All existing handlers continue to work! The `EventHandlerAdapter` automatically converts legacy handlers to the new format:

```
Legacy Handler
     ?
EventHandlerAdapter.AdaptLegacyHandler()
     ?
Context-Based Handler
     ?
Invoked normally
```

This means you can:
- ? Migrate handlers gradually
- ? Keep old code working while writing new code
- ? Test new handlers alongside old ones
- ? No breaking changes required

## Performance

### Before (Legacy)
- Reflection on every invocation (`DynamicInvoke`)
- Parameter name matching via strings
- Cache lookups for `ParameterInfo`
- Multiple try-catch blocks for type mismatches

### After (Context-Based)
- **Direct delegate invocation** (no reflection)
- Type-safe property access
- No string matching
- Clear exception messages

**Result:** Significantly faster invocation with better error messages.

## Testing

### Test EventContext Creation

```csharp
[Test]
public void EventContext_HasCorrectProperties()
{
    var context = new EventContext
    {
     Battle = battle,
 EventId = EventId.BeforeMove,
        TargetPokemon = targetPokemon,
        SourcePokemon = sourcePokemon,
        Move = activeMove
    };
    
    Assert.That(context.HasTargetPokemon, Is.True);
    Assert.That(context.HasMove, Is.True);
  Assert.That(context.GetTargetPokemon(), Is.EqualTo(targetPokemon));
}
```

### Test Handlers

```csharp
[Test]
public void OnBeforeMove_PreventsMoveWhenParalyzed()
{
    var handler = CreateBoolHandler(context =>
        !context.Battle.RandomChance(1, 4)
    );
    
    var context = CreateTestContext();
    var result = handler(context);
    
    Assert.That(result, Is.InstanceOf<BoolRelayVar>());
}
```

## Troubleshooting

### "Event X does not have a target Pokemon"

Your handler is trying to access a Pokemon that isn't available for this event. Check if the property is optional:

```csharp
// Instead of:
var target = context.GetTargetPokemon(); // throws if null

// Use:
if (context.HasTargetPokemon)
{
    var target = context.TargetPokemon; // nullable access
}
```

### "Cannot convert handler to EventHandlerDelegate"

Your lambda doesn't return `RelayVar?`. Make sure to return:
- `null` for void
- `new BoolRelayVar(value)` for booleans
- `new IntRelayVar(value)` for integers
- etc.

### Legacy Handler Still Using Reflection

If you see `EventHandlerAdapter` in stack traces, your handler is still using the legacy format. Convert it to context-based for better performance:

```csharp
// Legacy (slow)
new OnStartEventInfo((battle, target, source, effect) => { ... })

// Context-based (fast)
new OnStartEventInfo(context => { ... })
```

## Next Steps

1. **Start with new handlers**: Write all new event handlers using `EventContext`
2. **Migrate high-frequency handlers**: Convert handlers that are called often (e.g., damage calculation)
3. **Keep low-frequency legacy**: Leave rarely-called handlers as-is until convenient
4. **Remove adapter eventually**: Once all handlers are migrated, remove `EventHandlerAdapter` and legacy code

## Summary

The context-based architecture makes event handlers:
- ?? **Simpler** - One signature, clear API
- ?? **Faster** - No reflection
- ?? **Safer** - Type-safe with great error messages
- ?? **Testable** - Easy to mock and test
- ?? **Compatible** - Works with existing code

All existing handlers continue to work while you gradually migrate to the cleaner approach!
