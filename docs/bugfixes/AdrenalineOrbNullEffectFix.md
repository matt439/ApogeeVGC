# Adrenaline Orb Null Effect Fix

**Date**: 2025-01-XX  
**Severity**: High  
**Systems Affected**: Adrenaline Orb item `OnAfterBoost` event handler

## Problem

The Adrenaline Orb item crashed with a `NullReferenceException` when trying to access `effect.EffectStateId` in its `OnAfterBoost` handler at line 139 of `ItemsABC.cs`. The error occurred during random battle testing when boost events were triggered without an associated effect.

### Error Stack Trace
```
System.NullReferenceException
  HResult=0x80004003
  Message=Object reference not set to an instance of an object.
  Source=ApogeeVGC
  StackTrace:
   at ApogeeVGC.Data.Items.Items.<>c.<CreateItemsAbc>b__8_5(Battle _, SparseBoostsTable boost, Pokemon target, Pokemon _, IEffect effect) in C:\VSProjects\ApogeeVGC\ApogeeVGC\Data\Items\ItemsABC.cs:line 139
```

## Root Cause

The handler did not check if the `effect` parameter was null before accessing its `EffectStateId` property. Stat boosts can come from various sources that don't have an associated effect:
- Weather effects
- Terrain effects
- Field conditions
- Internal battle mechanics

While the TypeScript reference implementation directly accesses `effect.name` without a null check, C# requires explicit null handling. The TypeScript code may rely on runtime behavior where `effect` is always guaranteed to be present in this specific event context, but the C# port encounters scenarios where `effect` is null.

### TypeScript Reference
```typescript
// pokemon-showdown/data/items.ts:117-128
onAfterBoost(boost, target, source, effect) {
    // Adrenaline Orb activates if Intimidate is blocked by an ability like Hyper Cutter,
    // which deletes boost.atk,
    // but not if the holder's attack is already at -6 (or +6 if it has Contrary),
    // which sets boost.atk to 0
    if (target.boosts['spe'] === 6 || boost.atk === 0) {
        return;
    }
    if (effect.name === 'Intimidate') {
        target.useItem();
    }
},
```

## Solution

Added a null guard before checking the effect's identity:

```csharp
OnAfterBoost = new OnAfterBoostEventInfo((_, boost, target, _, effect) =>
{
    // Adrenaline Orb activates if Intimidate is blocked by an ability like Hyper Cutter,
    // which deletes boost.atk,
    // but not if the holder's attack is already at -6 (or +6 if it has Contrary),
    // which sets boost.atk to 0
    if (target.Boosts.GetBoost(BoostId.Spe) == 6 ||
        boost.GetBoost(BoostId.Atk) == 0)
    {
        return;
    }

    if (effect != null && effect.EffectStateId == AbilityId.Intimidate)
    {
        target.UseItem();
    }
}),
```

### Behavior

- **When effect is null**: Adrenaline Orb doesn't activate (no speed boost) - correct because the boost wasn't from Intimidate
- **When effect is Intimidate**: Adrenaline Orb activates and the holder gets a speed boost
- **When effect is not Intimidate**: Item doesn't activate

This ensures Adrenaline Orb only activates when the boost event was specifically caused by the Intimidate ability, not from other boost sources.

## Impact

When `effect` is null, the Adrenaline Orb simply doesn't activate, which matches the correct behavior since the item is designed to only respond to Intimidate. This prevents crashes while maintaining the correct game mechanics where Adrenaline Orb only triggers when the holder is affected by Intimidate.

## Related Fixes

This follows the same pattern as:
- [Ripen Ability Null Effect Fix](RipenNullEffectFix.md) - `OnTryHeal` handler
- [Disguise Ability Null Effect Fix](DisguiseNullEffectFix.md) - `OnDamage` handler

All three cases involve missing null checks when porting from TypeScript to C#. TypeScript's looser typing and optional chaining (`?.`) operator masks these potential null issues, while C# requires explicit null handling.

## Prevention Pattern

**When implementing event handlers that accept `IEffect` parameters:**

1. **Always add a null check** before accessing effect properties
2. **Consider the context**: Events can be triggered from many sources, not all of which have associated effects
3. **Check the TypeScript reference** but don't assume it handles all edge cases

**Standard pattern to follow:**

```csharp
OnEventName = new OnEventNameEventInfo((battle, ..., effect) =>
{
    // If effect-specific logic is needed, guard it
    if (effect != null && effect.EffectStateId == SomeValue)
    {
        // Safe to access effect properties
        battle.DoSomething();
    }
    
    // General logic that doesn't depend on effect
    // ...
});
```

## Files Modified

- `ApogeeVGC/Data/Items/ItemsABC.cs` - Added null check to Adrenaline Orb's OnAfterBoost handler (line 127-143)

## Testing

- Verified compilation with no errors
- Tested with random battle simulations
- No longer crashes when boost events occur without an associated effect
- Item correctly activates only when Intimidate is the boost source

## Keywords

`adrenaline orb`, `item`, `OnAfterBoost`, `null reference`, `effect parameter`, `intimidate`, `stat boost`, `null check`, `TypeScript porting`
