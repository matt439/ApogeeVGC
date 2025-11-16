# Protect Stalling Mechanic Issue - INCOMPLETE

## Status: IN PROGRESS - NOT RESOLVED

## Problem Summary
The Protect move's stalling mechanic is not working correctly. Protect always succeeds (100% success rate) even on consecutive uses, instead of having decreasing success rates (33% on 2nd use, 11% on 3rd use, etc.).

## Expected Behavior
According to Pokémon Showdown mechanics:
1. **First use**: 100% success rate (no Stall volatile exists yet)
2. **Second use**: 33% success rate (Stall volatile has counter=3)
3. **Third use**: 11% success rate (counter=9)
4. **Fourth use**: 3.7% success rate (counter=27)
5. And so on, with counter multiplying by 3 each time

## Current Behavior
- Protect succeeds 100% of the time on every use
- The Stall volatile Counter is **always null/empty**
- Duration is set correctly (2), but Counter is never initialized

## Root Cause Analysis

### Investigation Summary
The issue is in how the `EffectState` is managed during volatile creation:

1. When `AddVolatile(ConditionId.Stall)` is called, it should trigger `Stall.OnStart`
2. `OnStart` sets `battle.EffectState.Counter = 3`
3. **BUG**: This Counter value is NOT persisting to the volatile's stored state
4. When accessing `pokemon.Volatiles[ConditionId.Stall].Counter`, it's always null

### Evidence from Logs
```
[Protect.OnHit] BEFORE AddVolatile: calyrex-ice has Stall volatile = False
[Protect.OnHit] AFTER AddVolatile: calyrex-ice has Stall volatile = True
[Protect.OnHit] Stall volatile state: Counter=, Duration=2  // <-- Counter is EMPTY!
```

The Duration is correctly set to 2, but Counter remains null despite `OnStart` setting it to 3.

## Changes Made (Current Implementation)

### 1. EventHandlerInfoMapper.cs
**Added IMoveEventMethods support** - This was missing and preventing ALL move event handlers from being invoked.

```csharp
// Added this mapping to support move event handlers
private static readonly FrozenDictionary<EventId, Func<IMoveEventMethods, EventHandlerInfo?>>
    MoveEventMethodsMap = new Dictionary<EventId, Func<IMoveEventMethods, EventHandlerInfo?>>
    {
        [EventId.PrepareHit] = e => e.OnPrepareHit,
        [EventId.Hit] = e => e.OnHit,
        // ... other move events
    }.ToFrozenDictionary();

// Modified GetEventHandlerInfo to check IMoveEventMethods
public static EventHandlerInfo? GetEventHandlerInfo(IEffect effect, EventId id, ...)
{
    // Try move-specific events first (if applicable)
    if (effect is IMoveEventMethods moveMethods &&
 MoveEventMethodsMap.TryGetValue(id, out var moveAccessor))
    {
        EventHandlerInfo? info = moveAccessor(moveMethods);
      if (info != null && MatchesPrefixAndSuffix(info, prefix, suffix))
     return info;
    }
    // ... rest of method
}
```

### 2. Moves.cs - Protect Move Definition
**Added OnPrepareHit and OnHit handlers** to implement the stalling mechanic.

```csharp
[MoveId.Protect] = new()
{
    // ... existing properties
    StallingMove = true,
    VolatileStatus = ConditionId.Protect,
    
    OnPrepareHit = new OnPrepareHitEventInfo((battle, target, source, move) =>
    {
  // source is the Pokemon using Protect
    // Always run both checks, let Stall condition handle the logic
        bool willAct = battle.Queue.WillAct() is not null;
        RelayVar? stallResult = battle.RunEvent(EventId.StallMove, source);
        bool stallSuccess = stallResult is BoolRelayVar { Value: true };
        bool result = willAct && stallSuccess;

 // Return BoolEmptyVoidUnion explicitly
        return result ? (BoolEmptyVoidUnion)true : (BoolEmptyVoidUnion)false;
    }),
    
    OnHit = new OnHitEventInfo((battle, target, source, move) =>
  {
        // source is the Pokemon using Protect
        source.AddVolatile(ConditionId.Stall);
        return new VoidReturn();
    }),
    
    // ... rest of move definition
}
```

### 3. Conditions.cs - Stall Condition
**Implemented the stalling counter logic** with OnStart, OnStallMove, and OnRestart.

```csharp
[ConditionId.Stall] = new()
{
    Id = ConditionId.Stall,
    Name = "Stall",
    Duration = 2,
    CounterMax = 729,
    EffectType = EffectType.Condition,
    
    OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
    {
        // During OnStart, battle.EffectState IS the volatile's state being created
        battle.EffectState.Counter = 3;
        battle.Debug($"[Stall.OnStart] {pokemon.Name}: Initialized counter to 3");
      return new VoidReturn();
    }),
    
    OnStallMove = new OnStallMoveEventInfo((battle, pokemon) =>
    {
   // Get the counter from the Pokemon's Stall volatile state
        int counter = 1; // Default for first use
        
        if (pokemon.Volatiles.TryGetValue(ConditionId.Stall, out var stallState))
        {
   counter = stallState.Counter ?? 1;
        }
        
   battle.Debug($"[Stall.OnStallMove] {pokemon.Name}: Checking with counter={counter}, Success chance: {Math.Round(100.0 / counter, 2)}%");
        
 bool success = battle.RandomChance(1, counter);
        
        if (!success)
    {
       battle.Debug($"[Stall.OnStallMove] {pokemon.Name}: FAILED! Deleting Stall volatile");
    pokemon.DeleteVolatile(ConditionId.Stall);
        }
        else
        {
            battle.Debug($"[Stall.OnStallMove] {pokemon.Name}: SUCCESS!");
        }

        return success;
    }),
    
    OnRestart = new OnRestartEventInfo((battle, pokemon, _, _) =>
    {
        // During OnRestart, battle.EffectState IS the volatile's state being restarted
   int oldCounter = battle.EffectState.Counter ?? 1;
        
// Update the counter in the volatile's state
 if (battle.EffectState.Counter < 729) // CounterMax
        {
  battle.EffectState.Counter *= 3;
    }
   
        battle.EffectState.Duration = 2;
        
        battle.Debug($"[Stall.OnRestart] {pokemon.Name}: Counter increased from {oldCounter} to {battle.EffectState.Counter}, Duration reset to 2");
    
        return new VoidReturn();
}),
}
```

### 4. EventHandlerAdapter.cs
**Added BoolEmptyVoidUnion conversion support** to handle Protect's return type.

```csharp
// Added these cases to ConvertReturnValue
return returnValue switch
{
    // BoolEmptyVoidUnion -> bool, Empty, or VoidReturn
    BoolBoolEmptyVoidUnion boolEmptyVoid => new BoolRelayVar(boolEmptyVoid.Value),
    EmptyBoolEmptyVoidUnion => new BoolRelayVar(false), // Empty means blocked
    VoidUnionBoolEmptyVoidUnion => new VoidReturnRelayVar(),
    
  // ... other cases
};
```

## What Still Needs to Be Fixed

### The Core Issue: EffectState Not Persisting
The problem is in the **`AddVolatile` implementation** (location unknown - not yet examined).

**Theory**: When `OnStart` executes and sets `battle.EffectState.Counter = 3`, the `battle.EffectState` object is either:
1. A temporary object that doesn't get stored in the volatile
2. Gets overwritten/recreated after `OnStart` completes
3. Not properly assigned to `pokemon.Volatiles[conditionId]`

### Files That Need Investigation
1. **Pokemon.Conditions.cs** or similar - Contains `AddVolatile` implementation
2. **Pokemon.Core.cs** - May contain volatile management
3. **Battle.cs** - May contain EffectState management during event execution

### What to Look For
In the `AddVolatile` method, check:
```csharp
public bool AddVolatile(ConditionId id)
{
    // 1. How is the EffectState created?
    var effectState = new EffectState();  // or similar
    
    // 2. How is battle.EffectState set before calling OnStart?
    battle.EffectState = effectState;  // Does this happen?
  
    // 3. Is OnStart called?
    condition.OnStart(battle, pokemon, source, sourceEffect);
    
    // 4. Is the SAME effectState object stored?
    pokemon.Volatiles[id] = effectState;  // Does this use the same reference?
    
    // The bug is likely in steps 2 or 4!
}
```

## Showdown Reference Implementation

### TypeScript (pokemon-showdown/data/moves.ts)
```typescript
protect: {
    num: 182,
    accuracy: true,
    basePower: 0,
    category: "Status",
    name: "Protect",
    pp: 10,
priority: 4,
    flags: { noassist: 1, failcopycat: 1 },
    stallingMove: true,
    volatileStatus: 'protect',
    onPrepareHit(pokemon) {
   return !!this.queue.willAct() && this.runEvent('StallMove', pokemon);
    },
    onHit(pokemon) {
      pokemon.addVolatile('stall');
    },
    // ...
}
```

### TypeScript (pokemon-showdown/data/conditions.ts)
```typescript
stall: {
    // Protect, Detect, Endure counter
    name: 'stall',
    duration: 2,
    counterMax: 729,
    onStart() {
        this.effectState.counter = 3;
    },
    onStallMove(pokemon) {
      const counter = this.effectState.counter || 1;
        this.debug(`Success chance: ${Math.round(100 / counter)}%`);
     const success = this.randomChance(1, counter);
        if (!success) delete pokemon.volatiles['stall'];
        return success;
    },
    onRestart() {
    if (this.effectState.counter < (this.effect as Condition).counterMax!) {
    this.effectState.counter *= 3;
      }
 this.effectState.duration = 2;
    },
}
```

## Testing Evidence
Logs show the stalling mechanic is partially working:
- ? `OnPrepareHit` is being called
- ? `OnHit` is being called
- ? `AddVolatile` is adding the volatile (shows True after call)
- ? Duration is set correctly (2)
- ? **Counter is always null/empty**
- ? `OnStallMove` always sees counter=1 (default fallback)
- ? Protect never fails

## Related Files Modified
1. `ApogeeVGC/Sim/Events/EventHandlerInfoMapper.cs`
2. `ApogeeVGC/Data/Moves.cs`
3. `ApogeeVGC/Data/Conditions.cs`
4. `ApogeeVGC/Sim/Events/EventHandlerAdapter.cs`
5. `ApogeeVGC/Sim/BattleClasses/BattleActions.MoveHit.cs` (debug logging only)

## Next Steps for Resolution

1. **Find the AddVolatile implementation**
   - Search for `public bool AddVolatile` in Pokemon classes
   - Look in `Pokemon.Conditions.cs`, `Pokemon.Core.cs`, or similar

2. **Trace EffectState management**
   - How is `battle.EffectState` set before calling `OnStart`?
   - Is the same `EffectState` object stored in `Volatiles[id]`?

3. **Fix the state persistence**
   - Ensure `OnStart` modifies the actual `EffectState` that gets stored
   - The fix will likely be 1-2 lines ensuring object reference consistency

4. **Test the fix**
   - Use Protect 3+ times consecutively
   - Verify Counter increments: 3 ? 9 ? 27
   - Verify success rates decrease accordingly
   - Check logs show proper Counter values

## Additional Context

### Similar Working Conditions
Other conditions (Confusion, Sleep, Toxic) successfully set their own state in `OnStart`:
- Confusion sets `Time`
- Sleep sets `StartTime` and `Time`
- Toxic sets `Stage`

All of these use `battle.EffectState.PropertyName = value` just like Stall does, but they work. This suggests the issue might be specific to volatiles vs status conditions, or there's something unique about how the Stall volatile is being created/stored.

### Key Insight
The bug was originally reported in `ProtectBugFix.md` as solved, but that document describes fixing the **blocking** mechanic (Protect blocking moves via `OnTryHit`), NOT the **stalling** mechanic (decreasing success rate). The stalling mechanic was never implemented until this session.

## Date
2025-01-16

## Contributors
- Session focused on implementing Protect stalling mechanic
- Issue remains unresolved due to inability to locate `AddVolatile` implementation
