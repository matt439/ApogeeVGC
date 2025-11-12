# V2 EventHandlerInfo Invocation Pattern

## Problem
In the V2 event system, callback properties like `DurationCallback`, `BeforeMoveCallback`, etc., are now `EventHandlerInfo` objects (e.g., `DurationCallbackEventInfo`) instead of raw delegates.

## Solution Pattern

### Old V1 Code (Direct Invocation):
```csharp
// V1: DurationCallback was Func<Battle, Pokemon, Pokemon, IEffect?, int>
if (status.DurationCallback != null)
{
    effectState.Duration = status.DurationCallback(Battle, firstActive, source, sourceEffect);
}
```

### New V2 Code (Handler Property):
```csharp
// V2: DurationCallback is DurationCallbackEventInfo (inherits EventHandlerInfo)
if (status.DurationCallback != null)
{
    var durationHandler = (Func<Battle, Pokemon, Pokemon, IEffect?, int>)status.DurationCallback.Handler!;
    effectState.Duration = durationHandler(Battle, firstActive, source, sourceEffect);
}
```

## General Pattern

```csharp
// Step 1: Check if EventHandlerInfo exists
if (effect.SomeCallback != null)
{
    // Step 2: Extract and cast the Handler property
    var handler = (TExpectedDelegateType)effect.SomeCallback.Handler!;
    
    // Step 3: Invoke the handler
    var result = handler(arg1, arg2, ...);
}
```

## EventHandlerInfo Structure

```csharp
public abstract record EventHandlerInfo
{
    public Delegate? Handler { get; init; }  // ? The actual delegate
    public int? Priority { get; init; }
    public int? Order { get; init; }
    public int? SubOrder { get; init; }
    // ... other metadata
}
```

## Why This Change?

The V2 system wraps delegates in `EventHandlerInfo` to provide:
1. **Type safety** at compile-time
2. **Metadata** (Priority, Order, SubOrder)
3. **Validation** via `Validate()` method
4. **Consistent structure** across all event types

## Fixed Files

### `ApogeeVGC\Sim\SideClasses\Side.Conditions.cs`

**Line 51** (AddSideCondition):
```csharp
// Step 5: Duration callback
if (status.DurationCallback != null)
{
    Pokemon? firstActive = Active.FirstOrDefault(p => p != null);
    if (firstActive is null)
    {
        throw new InvalidOperationException("Side.Active has no non-null Pokemon.");
    }
    var durationHandler = (Func<Battle, Pokemon, Pokemon, IEffect?, int>)status.DurationCallback.Handler!;
    effectState.Duration = durationHandler(Battle, firstActive, source, sourceEffect);
}
```

**Line 163** (AddSlotCondition):
```csharp
// Step 6: Duration callback
if (status.DurationCallback != null)
{
    Pokemon? firstActive = Active.FirstOrDefault(p => p != null);
    if (firstActive is null)
    {
        throw new InvalidOperationException("Side.Active has no non-null Pokemon.");
    }
    var durationHandler = (Func<Battle, Pokemon, Pokemon, IEffect?, int>)status.DurationCallback.Handler!;
    conditionState.Duration = durationHandler(Battle, firstActive, source, sourceEffect);
}
```

## Similar Issues to Look For

Search for these patterns that need fixing:

### 1. DurationCallback (Condition)
```csharp
// ? Old:
status.DurationCallback(battle, pokemon, source, effect)

// ? New:
var handler = (Func<Battle, Pokemon, Pokemon, IEffect?, int>)status.DurationCallback.Handler!;
handler(battle, pokemon, source, effect)
```

### 2. BeforeTurnCallback (Move)
```csharp
// ? Old:
move.BeforeTurnCallback(battle, pokemon, target, move)

// ? New:
var handler = (Func<Battle, Pokemon, Pokemon?, ActiveMove, VoidReturn>)move.BeforeTurnCallback.Handler!;
handler(battle, pokemon, target, move)
```

### 3. PriorityChargeCallback (Move)
```csharp
// ? Old:
move.PriorityChargeCallback(battle, pokemon)

// ? New:
var handler = (Action<Battle, Pokemon>)move.PriorityChargeCallback.Handler!;
handler(battle, pokemon)
```

### 4. DamageCallback (Move)
```csharp
// ? Old:
move.DamageCallback(battle, source, target, move)

// ? New:
var handler = (Func<Battle, Pokemon, Pokemon, ActiveMove, int>)move.DamageCallback.Handler!;
handler(battle, source, target, move)
```

### 5. BasePowerCallback (Move)
```csharp
// ? Old:
move.BasePowerCallback(battle, source, target, move)

// ? New:
var handler = (Func<Battle, Pokemon, Pokemon, ActiveMove, int>)move.BasePowerCallback.Handler!;
handler(battle, source, target, move)
```

### 6. BeforeMoveCallback (Move)
```csharp
// ? Old:
move.BeforeMoveCallback(battle, pokemon, target, move)

// ? New:
var handler = (Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion>)move.BeforeMoveCallback.Handler!;
handler(battle, pokemon, target, move)
```

## Files That May Need Similar Fixes

Based on the build errors, check these files:

1. ? `ApogeeVGC\Sim\SideClasses\Side.Conditions.cs` - **FIXED**
2. ? `ApogeeVGC\Sim\BattleClasses\Battle.Lifecycle.cs` (lines 571, 589)
3. ? `ApogeeVGC\Sim\PokemonClasses\Pokemon.Status.cs` (lines 110, 314)
4. ? `ApogeeVGC\Sim\FieldClasses\Field.cs` (lines 111, 236, 358)
5. ? `ApogeeVGC\Sim\BattleClasses\BattleActions.Damage.cs` (lines 40, 60)
6. ? `ApogeeVGC\Sim\BattleClasses\BattleActions.Moves.cs` (line 111)

## Alternative: Extension Method (Optional)

To reduce boilerplate, you could create an extension method:

```csharp
public static class EventHandlerInfoExtensions
{
    public static TDelegate? GetHandler<TDelegate>(this EventHandlerInfo? info) 
        where TDelegate : Delegate
    {
        return info?.Handler as TDelegate;
    }
}

// Usage:
var handler = status.DurationCallback.GetHandler<Func<Battle, Pokemon, Pokemon, IEffect?, int>>();
if (handler != null)
{
    effectState.Duration = handler(Battle, firstActive, source, sourceEffect);
}
```

## Status

? **`Side.Conditions.cs` Fixed!**
- Line 51: `AddSideCondition` - DurationCallback invocation fixed
- Line 163: `AddSlotCondition` - DurationCallback invocation fixed
- Build errors for this file: **0**

---

**Next Steps:** Apply the same pattern to the other files listed above that have similar callback invocation errors.
