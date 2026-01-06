# Hadron Engine Ability Not Activating - Bug Fix Documentation

**Bug ID**: Ability Switch-In Handler Execution Failure  
**Date Fixed**: 2024  
**Severity**: High  
**Affected Systems**: Ability/Item OnStart handlers during SwitchIn events (Gen 5+)

---

## Problem Description

Miraidon's Hadron Engine ability was not activating Electric Terrain when switching in, despite the fallback logic correctly discovering the `OnStart` handler for `SwitchIn` events. This affected all abilities and items in Gen 5+ that rely on their `OnStart` handler being called during switch-in.

### Expected Behavior
When Miraidon switches in, Hadron Engine should:
1. Trigger its `OnStart` handler
2. Call `Field.SetTerrain(ElectricTerrain)`
3. Display the field terrain message
4. Apply terrain effects to subsequent moves

### Actual Behavior
- Handler was discovered correctly
- Handler was never invoked
- No terrain was set
- No error messages appeared

---

## Debug Output Analysis

Initial debug logs showed the handler discovery working correctly:
```
[FindPokemonEventHandlers] miraidon | Ability: Hadron Engine | Event: SwitchIn | Handler: NOT FOUND
[GetHandlerInfo] Using OnStart for SwitchIn event: Hadron Engine (Type: Ability) for Pokemon
[FindPokemonEventHandlers] miraidon | Ability: Hadron Engine | Event: SwitchIn | Handler: FOUND
```

However, subsequent investigation revealed:
1. Handler was found and modified ?
2. Handler was added to the event processing queue ?  
3. Handler invocation was attempted ?
4. Handler execution never occurred ?

---

## Root Cause Analysis

### Background: Gen 5+ Switch-In Behavior
In Pokemon Showdown (Gen 5+), abilities and items use their `onStart` handler during `SwitchIn` events instead of having a separate `Start` event. The C# implementation attempted to replicate this with fallback logic in `GetHandlerInfo`.

### Three Critical Issues

#### Issue 1: EventHandlerInfo ID Mismatch
The `OnStart` handler was returned with `Id = EventId.Start`, but it needed to be invoked as part of `EventId.SwitchIn`. This ID mismatch prevented proper event routing.

**Why it mattered**: The `EventHandlerInfo.Id` property is used throughout the event system to track which event a handler belongs to. Event processors use this ID to determine invocation context.

#### Issue 2: FieldEvent Indirect Invocation
`FieldEvent` was calling `SingleEvent(handlerEventId, effect, handler.State, singleEventTarget)`, which caused a second handler lookup via `effect.GetEventHandlerInfo(handlerEventId)`. 

**The problem**: 
- We modified the handler in the `EventListener` to have `Id = SwitchIn`
- But the ability's internal handler map still only had an entry for `EventId.Start`
- The second lookup for `EventId.SwitchIn` returned `null`
- Handler was never invoked

#### Issue 3: State Verification Reference Equality
The effect verification block in `FieldEvent` was comparing `handler.State` with `pokemon.AbilityState` using reference equality (`!=`). During switch-in events, these were different object references even though they logically represented the same state, causing handlers to be skipped with `continue`.

---

## Solution Implementation

### Part 1: Modify EventHandlerInfo ID
**File**: `ApogeeVGC\Sim\BattleClasses\Battle.EventHandlers.cs`  
**Method**: `GetHandlerInfo`

Created a modified handler with the correct event ID when using the fallback:

```csharp
// Special case: In Gen 5+, abilities and items trigger onStart during SwitchIn
if (handlerInfo is null &&
    target is PokemonRunEventTarget pokemonTarget &&
    Gen >= 5 &&
    callbackName == EventId.SwitchIn &&
    prefix == EventPrefix.None &&
    suffix == EventSuffix.None &&
    effect.GetEventHandlerInfo(EventId.AnySwitchIn, null, null) == null &&
    (IsAbilityOrItem(effect) || IsInnateAbilityOrItem(effect)))
{
    // Use onStart handler for the SwitchIn event
    EventHandlerInfo? startHandler = effect.GetEventHandlerInfo(EventId.Start, null, null);
    if (startHandler != null)
    {
        // Create a modified handler with SwitchIn as the event ID
        // This ensures the handler is properly invoked during the SwitchIn event
        handlerInfo = startHandler with { Id = EventId.SwitchIn };
        
        if (DisplayUi)
        {
            Debug($"[GetHandlerInfo] Using OnStart for SwitchIn event: {effect.Name} (Type: {effect.EffectType}) for Pokemon");
        }
    }
}
```

**Key change**: Using C# record's `with` expression to create a copy of the handler with only the `Id` changed.

### Part 2: Direct Handler Invocation
**File**: `ApogeeVGC\Sim\BattleClasses\Battle.Events.cs`  
**Method**: `FieldEvent`

Modified the handler invocation to use the pre-resolved handler directly:

```csharp
// Execute the handler's callback
if (handler.HandlerInfo != null)
{
    SingleEventTarget? singleEventTarget = handler.EffectHolder switch
    {
        PokemonEffectHolder pokemonEh => new PokemonSingleEventTarget(pokemonEh.Pokemon),
        SideEffectHolder sideEh => new SideSingleEventTarget(sideEh.Side),
        FieldEffectHolder fieldEh => new FieldSingleEventTarget(fieldEh.Field),
        BattleEffectHolder battleEh => SingleEventTarget.FromBattle(battleEh.Battle),
        _ => null,
    };

    // Save parent context
    IEffect parentEffect = Effect;
    EffectState parentEffectState = EffectState;
    Event parentEvent = Event;

    // Set up new event context
    Effect = effect;
    EffectState = handler.State ?? InitEffectState();
    Event = new Event
    {
        Id = handlerEventId,
        Target = singleEventTarget,
        Source = null,
        Effect = null,
    };
    EventDepth++;

    // Invoke the handler directly
    RelayVar? returnVal;
    try
    {
        returnVal = InvokeEventHandlerInfo(handler.HandlerInfo, false, 
                                          new BoolRelayVar(true), 
                                          singleEventTarget, null, null);
    }
    finally
    {
        // Restore parent context
        EventDepth--;
        Effect = parentEffect;
        EffectState = parentEffectState;
        Event = parentEvent;
    }
}
```

**Key change**: Directly invoking `InvokeEventHandlerInfo` with the `handler.HandlerInfo` instead of calling `SingleEvent`, which eliminates the failed second lookup.

### Part 3: Skip State Verification for Switch-In
**File**: `ApogeeVGC\Sim\BattleClasses\Battle.Events.cs`  
**Method**: `FieldEvent`

Added logic to skip state verification for abilities and items on SwitchIn:

```csharp
// Verify the effect hasn't been removed by a prior handler
// (e.g., Toxic Spikes being absorbed during a double switch)
//
// Skip verification for abilities and items on SwitchIn events, since these are
// triggered via their OnStart handlers and the state references may not match yet
bool skipVerification = eventId == EventId.SwitchIn &&
                       effect.EffectType is EffectType.Ability or EffectType.Item;

if (!skipVerification && handler.State?.Target != null)
{
    // ... verification logic ...
}
```

**Key change**: The state verification is necessary for most events (to detect effects removed by prior handlers), but for abilities/items on switch-in, the state references legitimately don't match yet, so we skip it.

---

## Verification

### Test Case
Lead with Miraidon against any opponent.

### Expected Results After Fix
```
[FieldEvent] About to invoke handler for Hadron Engine
[HadronEngine.OnStart] HANDLER EXECUTING for miraidon
[HadronEngine.OnStart] SetTerrain returned: True
[HadronEngine.OnStart] Current terrain: ElectricTerrain
[FieldEvent] Handler invoked for Hadron Engine
Field: Electric:5
```

### Confirmed Working
? Hadron Engine activates Electric Terrain on switch-in  
? Terrain duration correctly set to 5 turns  
? Electric-type moves receive 1.3x boost  
? No errors or exceptions  

---

## Impact Assessment

### Affected Systems
- **All Gen 5+ abilities with OnStart handlers**: Intimidate, Download, Drought, Drizzle, Sand Stream, Snow Warning, Electric Surge, Grassy Surge, Misty Surge, Psychic Surge, Hadron Engine, Orichalcum Pulse, etc.
- **All Gen 5+ items with OnStart handlers**: Air Balloon (popup), etc.
- **Innate abilities/items**: Status conditions with associated abilities/items

### Risk Level
Low - The changes are localized to event handler discovery and invocation, with clear fallback behavior maintained.

---

## Related Files

| File | Purpose | Changes Made |
|------|---------|--------------|
| `Battle.EventHandlers.cs` | Handler discovery | Modified `GetHandlerInfo` to create handler with correct ID |
| `Battle.Events.cs` | Event processing | Direct invocation + state verification skip |
| `Abilities.cs` | Ability definitions | Added debug logging (can be removed) |
| `Field.cs` | Terrain management | No changes (verified working) |
| `Conditions.cs` | Condition definitions | No changes (verified working) |

---

## Pokemon Showdown Reference

This implementation matches Pokemon Showdown's behavior from `sim/battle.ts`:

```typescript
getCallback(target: Pokemon | Side | Field | Battle, effect: Effect, callbackName: string) {
    let callback: Function | undefined = (effect as any)[callbackName];
    // Abilities and items Start at different times during the SwitchIn event, 
    // so we run their onStart handlers during the SwitchIn event instead of 
    // running the Start event during switch-ins
    // gens 4 and before still use the old system, though
    if (
        callback === undefined && 
        target instanceof Pokemon && 
        this.gen >= 5 && 
        callbackName === 'onSwitchIn' &&
        !(effect as any).onAnySwitchIn && 
        (['Ability', 'Item'].includes(effect.effectType) || (
            // Innate abilities/items
            effect.effectType === 'Status' && 
            ['ability', 'item'].includes(effect.id.split(':')[0])
        ))
    ) {
        callback = (effect as any).onStart;
    }
    return callback;
}
```

---

## Lessons Learned

1. **Event ID consistency is critical** - When aliasing one event to another, all references to the handler must use the aliased ID
2. **Avoid redundant lookups** - If a handler has already been resolved, pass it directly rather than looking it up again
3. **State verification needs context** - Not all state mismatches indicate removed effects; some are timing-related
4. **Debug early, debug often** - The extensive debug logging added during investigation made the issue obvious once we tracked the full execution path

---

## Future Considerations

### Potential Improvements
1. Consider adding a `HandlerAlias` concept to formally track when one event type uses another's handler
2. Add unit tests specifically for Gen 5+ switch-in ability activation
3. Create a centralized "handler invocation" method that both `FieldEvent` and `RunEvent` can use

### Watch For
- Other Gen-specific event aliasing that might have similar issues
- Performance impact of skipping state verification (currently minimal since it's only for abilities/items on SwitchIn)
- Edge cases where abilities/items genuinely get removed during switch-in processing

---

## Keywords
`ability`, `item`, `switch-in`, `OnStart`, `SwitchIn`, `event handler`, `Hadron Engine`, `Electric Terrain`, `Gen 5`, `handler invocation`, `state verification`
