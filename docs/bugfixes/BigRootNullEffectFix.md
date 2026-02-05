# Big Root Null Effect Fix

## Problem Summary

The Big Root item crashed with a `NullReferenceException` when trying to access `effect.EffectStateId` in its `OnTryHeal` handler at line 390 of `ItemsABC.cs`. The error occurred during random battle testing when a Pokémon holding Big Root was healed from sources that don't have an associated `IEffect`.

## Root Cause

The handler did not check if the `effect` parameter was null before accessing its `EffectStateId` property. Healing can come from various sources that don't have an associated effect (natural regeneration, certain abilities, weather effects, etc.). 

The TypeScript reference implementation directly accesses `effect.id` without a null check (line 491 of `pokemon-showdown/data/items.ts`):

```typescript
onTryHeal(damage, target, source, effect) {
    const heals = ['drain', 'leechseed', 'ingrain', 'aquaring', 'strengthsap'];
    if (heals.includes(effect.id)) {
        return this.chainModify([5324, 4096]);
    }
},
```

In JavaScript, accessing a property on `undefined` returns `undefined` (no error), and the `includes()` check returns `false`. In C#, accessing a property on `null` throws a `NullReferenceException`.

## Solution

Added a null guard at the beginning of the handler to match the TypeScript behavior:

```csharp
if (effect == null)
{
    return null;
}
```

**Impact**: When `effect` is null, the healing proceeds unchanged without activating Big Root's healing boost, matching the correct TypeScript behavior where Big Root only boosts specific healing sources (drain moves, Leech Seed, Ingrain, Aqua Ring, Strength Sap).

## Files Modified

**`ApogeeVGC/Data/Items/ItemsABC.cs`**

- Added null check before accessing `effect.EffectStateId` in Big Root's `OnTryHeal` handler
- Inserted at line 390 (after the comments, before the switch expression)

## Error Details

**Exception Type**: `System.NullReferenceException`  
**Error Message**: "Object reference not set to an instance of an object."  
**Location**: `ItemsABC.cs:line 390`

**Stack Trace Context**:
```
at Items.<>c.<CreateItemsAbc>b__8_23(Battle battle, Int32 damage, Pokemon _, Pokemon _, IEffect effect)
? Battle.Heal(damage, target, source, effect)
? BattleActions.RunMoveEffects(...)
? SpreadMoveHit(...)
```

**Trigger Scenario**: Pokémon with Big Root item was healed from a source without an associated effect (e.g., natural regeneration, certain abilities).

## Related Fixes

This follows the same pattern as:
- [Ripen Ability Null Effect Fix](RipenNullEffectFix.md) - `OnTryHeal` handler
- [Disguise Ability Null Effect Fix](DisguiseNullEffectFix.md) - `OnDamage` handler
- [Adrenaline Orb Null Effect Fix](AdrenalineOrbNullEffectFix.md) - `OnAfterBoost` handler
- [Berserk Ability Null Effect Fix](BerserkNullEffectFix.md) - `OnDamage` handler
- [Wind Rider Null SideCondition Fix](WindRiderNullSideConditionFix.md) - `OnSideConditionStart` handler

**Pattern**: Event handlers that receive `IEffect` or derived type parameters must check for null before accessing properties, because events can be triggered without an associated effect.

## Testing

After applying this fix:
1. Big Root correctly ignores healing from sources without effects (no crash)
2. Big Root correctly boosts healing from drain moves, Leech Seed, Ingrain, Aqua Ring, and Strength Sap
3. Random battle testing continues without `NullReferenceException` errors related to Big Root

## Prevention

When porting TypeScript event handlers to C#:
- Always add null checks for `IEffect` parameters before accessing properties
- Remember: TypeScript `effect.id` without null check ? C# needs `if (effect == null) return;`
- Return appropriate default value when effect is null (typically `null`, `VoidReturn()`, or pass-through value)

## Keywords

`big root`, `item`, `OnTryHeal`, `null reference`, `effect parameter`, `healing`, `drain`, `null check`, `TypeScript porting`, `Leech Seed`, `Ingrain`, `Aqua Ring`, `Strength Sap`
