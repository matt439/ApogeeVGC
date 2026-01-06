# Tailwind OnModifySpe Fix

## Problem Summary
When Volcarona used Tailwind, the battle immediately ended in a tie with an `InvalidOperationException: stat must be an IntRelayVar` error. The move execution started but crashed during speed calculation when the Tailwind side condition's `OnModifySpe` handler was triggered.

## Root Cause
The Tailwind condition's `OnModifySpe` event handler was returning `new VoidReturn()` after calling `battle.ChainModify(2)`. This caused the following issue chain:

1. `OnModifySpe` handler signature expects `IntVoidUnion` return type (either `int` or `VoidReturn`)
2. When `VoidReturn()` is returned, `EventHandlerAdapter.ConvertReturnValue()` converts it to `VoidReturnRelayVar`
3. `Pokemon.GetStat()` calls `Battle.RunEvent(EventId.ModifySpe, ...)` expecting `IntRelayVar` or `null`
4. `GetStat` checks `if (relayVar is IntRelayVar irv)` and throws exception when it gets `VoidReturnRelayVar` instead

```csharp
// In Pokemon.GetStat() - line 95-99
RelayVar? relayVar = Battle.RunEvent(eventId, this, null, null, new IntRelayVar(stat));
if (relayVar is IntRelayVar irv)
{
    stat = irv.Value;
}
else
{
    throw new InvalidOperationException("stat must be an IntRelayVar");
}
```

## Solution
Modified the `OnModifySpe` handler in both **Tailwind** and **QuarkDrive** conditions to:
1. Accept the `spe` parameter (instead of discarding it with `_`)
2. Call `battle.ChainModify()` to accumulate the speed modifier
3. Return `battle.FinalModify(spe)` which applies the accumulated modifier and returns the result as an `int`

### Before (Tailwind):
```csharp
OnModifySpe = new OnModifySpeEventInfo((battle, _, _) =>
{
    battle.ChainModify(2);
    return new VoidReturn();  // WRONG - causes VoidReturnRelayVar
}),
```

### After (Tailwind):
```csharp
OnModifySpe = new OnModifySpeEventInfo((battle, spe, _) =>
{
    // Tailwind doubles speed using chain modification
    battle.ChainModify(2);
    // Apply the accumulated modifier to the speed value
    return battle.FinalModify(spe);
}),
```

### Before (QuarkDrive):
```csharp
OnModifySpe = new OnModifySpeEventInfo((battle, _, pokemon) =>
{
    // ... checks ...
    battle.ChainModify(1.5);
    return new VoidReturn();  // WRONG - causes VoidReturnRelayVar
}),
```

### After (QuarkDrive):
```csharp
OnModifySpe = new OnModifySpeEventInfo((battle, spe, pokemon) =>
{
    // ... checks ...
    battle.ChainModify(1.5);
    return battle.FinalModify(spe);
}),
```

## Files Modified
- `ApogeeVGC/Data/Conditions.cs`
  - `Tailwind` condition: Fixed `OnModifySpe` handler to return `FinalModify(spe)` instead of `VoidReturn()`
  - `QuarkDrive` condition: Fixed `OnModifySpe` handler to return `FinalModify(spe)` instead of `VoidReturn()`

## Pattern for OnModifySpe Handlers

When implementing `OnModifySpe` (and similar stat modification events), follow this pattern:

### Pattern 1: Direct Modification (e.g., Paralysis)
For handlers that need to apply modifiers after all chain modifications:
```csharp
OnModifySpe = new OnModifySpeEventInfo((battle, spe, pokemon) =>
{
    // Apply all accumulated chain modifiers first
    spe = battle.FinalModify(spe);
    
    // Then apply this handler's logic
    if (!pokemon.HasAbility(AbilityId.QuickFeet))
    {
        spe = (int)Math.Floor(spe * 50.0 / 100);
    }
    
    return spe;
}, priority),
```

### Pattern 2: Chain Modification (e.g., Tailwind, QuarkDrive)
For handlers that accumulate modifiers:
```csharp
OnModifySpe = new OnModifySpeEventInfo((battle, spe, pokemon) =>
{
    // Accumulate modifier (doesn't apply it yet)
    battle.ChainModify(2);  // or 1.5, etc.
    
    // Apply all accumulated modifiers and return
    return battle.FinalModify(spe);
}, priority),
```

### Pattern 3: Conditional Return (QuarkDrive with early exit)
When returning early without modification:
```csharp
OnModifySpe = new OnModifySpeEventInfo((battle, spe, pokemon) =>
{
    // Early exit conditions - return VoidReturn to leave stat unchanged
    if (someCondition)
    {
        return new VoidReturn();
    }
    
    // Apply modification
    battle.ChainModify(1.5);
    return battle.FinalModify(spe);
}, priority),
```

## Key Insights

1. **ChainModify vs FinalModify**:
   - `ChainModify(modifier)` accumulates modifiers in `Event.Modifier` using fixed-point arithmetic
   - `FinalModify(value)` applies `Event.Modifier` to the value and resets `Event.Modifier` to 1.0
   - These are used together for proper modifier stacking (e.g., Tailwind + Paralysis)

2. **VoidReturn vs void**:
   - `VoidReturn` is a union type variant indicating "no modification, use original value"
   - It should ONLY be returned when you want to leave the value unchanged (early exit)
   - After calling `ChainModify`, you MUST call `FinalModify` and return the result
   - Returning `VoidReturn` after `ChainModify` causes the modifier to be lost

3. **Type Safety**:
   - `IntVoidUnion` can be either `int` (explicit value) or `VoidReturn` (no change)
   - `EventHandlerAdapter` converts return values to `RelayVar` types
   - `GetStat` strictly requires `IntRelayVar` (not `VoidReturnRelayVar`) for modified stats

## Testing
The fix resolves the crash when Tailwind is used. The battle should proceed normally with speed doubled for Pokemon on the side with Tailwind active.

## Related Issues
This issue is similar to previous event handler return type problems documented in:
- [Union Type Handling Guide](UnionTypeHandlingGuide.md) - General patterns for union type usage
- [Hadron Engine Bug Fix](HadronEngineBugFix.md) - Event handler invocation issues
- [Complete Draco Meteor Bug Fix](CompleteDracoMeteorBugFix.md) - Multiple event system issues including parameter resolution

## Keywords
`Tailwind`, `QuarkDrive`, `OnModifySpe`, `VoidReturn`, `ChainModify`, `FinalModify`, `IntRelayVar`, `speed modification`, `stat events`, `event handler return types`, `union types`
