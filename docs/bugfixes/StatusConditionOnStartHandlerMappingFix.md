# Status Condition OnStart Handler Mapping Fix

## Problem Summary

Battle exceeded the 1000-turn limit (`BattleTurnLimitException`) due to an endless loop where both Pokémon had Sleep status that **never ended**. Every move was blocked by the `BeforeMove` event handler indefinitely, causing the battle to loop for 1000+ turns until the turn limit was reached.

### Symptom

```
System.InvalidOperationException: Battle exceeded turn limit of 1000 turns (current turn: 1000). 
This likely indicates an infinite loop or stalemate.
```

Debug output showed:
- Both Pokémon (Cleffa and Garganacl) had Sleep status
- Every turn, `BeforeMove` returned `false`, blocking all moves
- Sleep's `OnBeforeMove` handler showed `StatusState.Time=null` (never initialized)
- Sleep's `OnStart` handler **never executed** despite `SetStatus` being called

## Root Cause

**Missing event mappings in `EventHandlerInfoMapper`**. The `EventId.Start`, `EventId.End`, and `EventId.Restart` events were only mapped in the `AbilityEventMethodsMap` (for `IAbilityEventMethodsV2`), but were **completely missing** from the base `EventMethodsMap` (for `IEventMethods`).

### Why This Broke Sleep

1. When `Pokemon.SetStatus(ConditionId.Sleep, ...)` was called, it invoked `battle.SingleEvent(EventId.Start, sleepCondition, ...)`
2. `SingleEvent` called `effect.GetEventHandlerInfo(EventId.Start)`
3. `EventHandlerInfoMapper.GetEventHandlerInfo` checked the `EventMethodsMap` for `EventId.Start`
4. **The mapping didn't exist**, so the method returned `null`
5. `SingleEvent` saw `handlerInfo == null` and returned early without invoking the handler
6. Sleep's `OnStart` never ran, so `EffectState.Time` was never initialized (stayed `null`)
7. In `OnBeforeMove`, the code did:
   ```csharp
   pokemon.StatusState.Time--;  // null-- stays null in C# (lifted nullable arithmetic)
   if (pokemon.StatusState.Time <= 0)  // null <= 0 is false
   {
       pokemon.CureStatus();  // Never reached
   }
   ```
8. Since `Time` stayed `null` and `null <= 0` is always `false`, Sleep never cured itself

### Interface Hierarchy

```
IEffect (base)
├─ IEventMethods (base events: OnDamage, OnHit, OnResidual, etc.)
│  ├─ Condition : IEventMethods, ISideEventMethods, IFieldEventMethods, IPokemonEventMethods
│  ├─ Item : IEventMethods, IPokemonEventMethods  
│  └─ Format : IEventMethods
└─ IAbilityEventMethodsV2 (ability-specific events: OnStart, OnEnd, OnCheckShow)
   └─ Ability : IAbilityEventMethodsV2, IEventMethods
```

**Key Insight**: Abilities use a completely separate interface (`IAbilityEventMethodsV2`) for lifecycle events, so they had their own mapping. Conditions and Items use the base `IEventMethods` interface, which was missing the lifecycle event mappings.

## Solution

**File**: `ApogeeVGC/Sim/Events/EventHandlerInfoMapper.cs`

Added `EventId.Start`, `EventId.End`, and `EventId.Restart` to the base `EventMethodsMap`, following the same type-checking pattern used for field/side events:

```csharp
// Condition/Item-specific lifecycle event handlers
// Abilities use AbilityEventMethodsMap (checked first), so these only match Conditions and Items
[EventId.Start] = e => e switch
{
    Condition c => c.OnStart,
    Item i => i.OnStart,
    _ => null,
},
[EventId.End] = e => e switch
{
    Condition c => c.OnEnd,
    Item i => i.OnEnd,
    _ => null,
},
[EventId.Restart] = e => (e as Condition)?.OnRestart,

// Field event handlers
[EventId.FieldStart] = e => e is IFieldEventMethods f ? f.OnFieldStart : null,
// ... etc
```

Also added necessary using statements:
```csharp
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Items;
```

### Why This Works

1. **Type-based routing**: Uses C# switch expressions to check the concrete type and access the appropriate property
2. **Abilities unaffected**: `GetEventHandlerInfo` checks `AbilityEventMethodsMap` first, so abilities still find their handlers correctly
3. **Restart is Condition-only**: Items don't have `OnRestart`, so we use a simple cast for that mapping
4. **Follows existing pattern**: Matches the approach used for field/side events (type-checking lambdas in the map)

## Files Modified

1. **`ApogeeVGC/Sim/Events/EventHandlerInfoMapper.cs`**
   - Added using statements for `Condition` and `Item`
   - Added `EventId.Start`, `EventId.End`, `EventId.Restart` mappings to `EventMethodsMap`

2. **`ApogeeVGC/Data/Conditions/ConditionsSTU.cs`** (cleanup only)
   - Removed debug logging from Sleep `OnStart` and `OnBeforeMove` handlers

3. **`ApogeeVGC/Sim/BattleClasses/Battle.Events.cs`** (cleanup only)
   - Removed debug logging from `SingleEvent` and `RunEvent` methods

## Impact

This fix affects **all conditions and items** that have `OnStart`, `OnEnd`, or `OnRestart` handlers:

### Status Conditions
- **Sleep** (`OnStart` sets duration counter) - **Primary fix**
- **Freeze** (`OnStart` display message)
- **Burn** (`OnStart` display message)
- **Paralysis** (`OnStart` display message)

### Volatile Conditions
- **Confusion** (`OnStart` sets duration counter)
- **Disable** (`OnStart` validation and message)
- **LockedMove** (`OnStart` for Outrage, Thrash, etc.)
- **TwoTurnMove** (`OnStart` for Fly, Dig, etc.)
- **Charge** (`OnStart`, `OnRestart` messages)
- All other volatile conditions with lifecycle handlers

### Items
- Any items with `OnStart` or `OnEnd` handlers (e.g., Choice items, consumables)

## Testing

### Verification
Battle with debug seeds (Team1=123782, Team2=137352, P1=81808, P2=71282, Battle=79341):
- **Before fix**: 1000-turn `BattleTurnLimitException`
- **After fix**: Completed in **35 turns** with Player1Win

### Debug Output Analysis
**Before fix**:
```
[SingleEvent.Start.Status] effect=Sleep, handler=NULL, target=..., state=...
[Sleep.OnBeforeMove] StatusState.Time=null, StatusState.StartTime=null
```

**After fix**:
- No debug output (removed after verification)
- Sleep duration properly decrements each turn
- Sleep cures itself after 2-4 turns (as designed)

## Pattern for Similar Issues

### Symptom Checklist
If you see an effect's event handler not executing:
1. ✅ Check if event is mapped in `EventHandlerInfoMapper`
2. ✅ Verify handler discovery: Add debug logging to `SingleEvent`
3. ✅ Check interface hierarchy: Does the effect implement the expected interface?
4. ✅ Verify event ID: Is the correct `EventId` being used?

### Prevention Guidelines
When adding new lifecycle events:
1. Map them in **both** `EventMethodsMap` (for Conditions/Items/Format) AND `AbilityEventMethodsMap` (for Abilities)
2. Use type-based switching for maps that need to support multiple effect types
3. Test with at least one Condition, one Item, and one Ability to verify all paths

## Related Fixes

This issue is similar to:
- [Trick Room Bug Fix](TrickRoomBugFix.md) - Field event handlers not mapped
- [Protect Stalling Mechanic Issue](ProtectStallingMechanicIssue.md) - Move event handlers not mapped
- [Hadron Engine Bug Fix](HadronEngineBugFix.md) - Ability OnStart handlers with ID mismatch

All involve event handler discovery/mapping issues in the event system.

## Keywords

`status condition`, `Sleep`, `Freeze`, `Burn`, `Paralysis`, `OnStart`, `OnEnd`, `OnRestart`, `EventHandlerInfoMapper`, `event mapping`, `handler discovery`, `EventId.Start`, `null Time`, `endless loop`, `turn limit`, `BattleTurnLimitException`, `Condition`, `Item`, `IEventMethods`, `lifecycle events`, `event system`, `handler not executing`
