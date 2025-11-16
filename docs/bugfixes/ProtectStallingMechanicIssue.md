# Protect Stalling Mechanic Issue - RESOLVED ?

## Status: RESOLVED

## Problem Summary
The Protect move's stalling mechanic was not working correctly. Protect always succeeded (100% success rate) even on consecutive uses, instead of having decreasing success rates (33% on 2nd use, 11% on 3rd use, etc.).

## Expected Behavior
According to Pokémon Showdown mechanics:
1. **First use**: 100% success rate (no Stall volatile exists yet)
2. **Second use**: 33% success rate (Stall volatile has counter=3)
3. **Third use**: 11% success rate (counter=9)
4. **Fourth use**: 3.7% success rate (counter=27)
5. And so on, with counter multiplying by 3 each time

## Resolution
The stalling mechanic is now working correctly! Test logs confirm:

```
Turn 2: [Stall.OnStallMove] calyrex-ice: Checking with counter=3, Success chance: 33.33%
Turn 2: [Stall.OnStallMove] calyrex-ice: FAILED! Deleting Stall volatile
```

The second use of Protect correctly:
- Checked the Stall volatile (counter=3)
- Calculated 33.33% success rate (1/3 chance)
- Failed the random roll and deleted the Stall volatile

## Root Cause
The issue was in the event handler system. The `IMoveEventMethods` interface and its event handlers were not being properly mapped in `EventHandlerInfoMapper.cs`, which prevented move event handlers like `OnPrepareHit` and `OnHit` from being invoked.

## Changes Made

### 1. EventHandlerInfoMapper.cs
**Added IMoveEventMethods support** to enable move event handler invocation:

```csharp
private static readonly FrozenDictionary<EventId, Func<IMoveEventMethods, EventHandlerInfo?>>
    MoveEventMethodsMap = new Dictionary<EventId, Func<IMoveEventMethods, EventHandlerInfo?>>
    {
        [EventId.PrepareHit] = e => e.OnPrepareHit,
        [EventId.Hit] = e => e.OnHit,
        [EventId.BasePower] = e => e.OnBasePower,
   // ... other move events
    }.ToFrozenDictionary();
```

### 2. Moves.cs - Protect Move Definition
**Implemented OnPrepareHit and OnHit handlers**:

```csharp
[MoveId.Protect] = new()
{
    StallingMove = true,
    VolatileStatus = ConditionId.Protect,
    
    OnPrepareHit = new OnPrepareHitEventInfo((battle, target, source, move) =>
    {
        // Check if queue will act AND stalling mechanic succeeds
        bool willAct = battle.Queue.WillAct() is not null;
        RelayVar? stallResult = battle.RunEvent(EventId.StallMove, source);
        bool stallSuccess = stallResult is BoolRelayVar { Value: true };
 bool result = willAct && stallSuccess;
        return result ? (BoolEmptyVoidUnion)true : (BoolEmptyVoidUnion)false;
    }),
    
    OnHit = new OnHitEventInfo((battle, target, source, move) =>
    {
        // Add/restart Stall volatile to track consecutive usage
        source.AddVolatile(ConditionId.Stall);
        return new VoidReturn();
    }),
}
```

### 3. Conditions.cs - Stall Condition
**Implemented complete stalling logic**:

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
        battle.EffectState.Counter = 3;
        battle.Debug($"[Stall.OnStart] {pokemon.Name}: Initialized counter to 3");
        return new VoidReturn();
    }),
    
    OnStallMove = new OnStallMoveEventInfo((battle, pokemon) =>
    {
      int counter = 1;
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
        
        return success;
    }),
  
    OnRestart = new OnRestartEventInfo((battle, pokemon, _, _) =>
 {
     int oldCounter = battle.EffectState.Counter ?? 1;
        
        if (battle.EffectState.Counter < 729)
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
**Added BoolEmptyVoidUnion conversion support**:

```csharp
return returnValue switch
{
    BoolBoolEmptyVoidUnion boolEmptyVoid => new BoolRelayVar(boolEmptyVoid.Value),
    EmptyBoolEmptyVoidUnion => new BoolRelayVar(false),
    VoidUnionBoolEmptyVoidUnion => new VoidReturnRelayVar(),
    // ... other cases
};
```

## Testing Evidence
The logs from Turn 2 show the stalling mechanic working correctly:

```
[Stall.OnStallMove] calyrex-ice: Checking with counter=3, Success chance: 33.33%
[Stall.OnStallMove] calyrex-ice: FAILED! Deleting Stall volatile
[UseMoveInner] TrySpreadMoveHit returned: False
```

This confirms:
- ? Stall volatile exists with counter=3 on 2nd use
- ? Success chance correctly calculated as 33.33% (1/3)
- ? Random roll executed and failed
- ? Stall volatile deleted on failure
- ? Protect move failed (returned False)

## Related Files Modified
1. `ApogeeVGC/Sim/Events/EventHandlerInfoMapper.cs` - Added IMoveEventMethods mapping
2. `ApogeeVGC/Data/Moves.cs` - Implemented Protect stalling handlers
3. `ApogeeVGC/Data/Conditions.cs` - Implemented Stall condition logic
4. `ApogeeVGC/Sim/Events/EventHandlerAdapter.cs` - Added union type conversion
5. `ApogeeVGC/Sim/PokemonClasses/Pokemon.Status.cs` - Added debug logging

## Date
2025-01-16 - Issue Resolved

## Contributors
- Debugging session successfully identified and fixed the event handler mapping issue
- Stalling mechanic now matches Pokémon Showdown behavior
