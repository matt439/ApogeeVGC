# Disguise Ability Null Effect Fix

**Date**: 2025-01-XX  
**Severity**: High  
**Systems Affected**: Disguise ability `OnDamage` event handler

## Problem

The Disguise ability crashed with a `NullReferenceException` when trying to access `effect.EffectType` in its `OnDamage` handler at line 308 of `AbilitiesDEF.cs`. The error occurred during random battle testing when Mimikyu took damage from sources that don't have an associated `IEffect`.

### Error Stack Trace
```
System.NullReferenceException: Object reference not set to an instance of an object.
   at ApogeeVGC.Data.Abilities.Abilities.<>c.<CreateAbilitiesDef>b__9_15(Battle battle, Int32 damage, Pokemon target, Pokemon _, IEffect effect) in C:\VSProjects\ApogeeVGC\ApogeeVGC\Data\Abilities\AbilitiesDEF.cs:line 308
```

## Root Cause

The handler did not check if the `effect` parameter was null before accessing its `EffectType` property. Damage can come from various sources that don't have an associated effect:
- Confusion self-damage
- Recoil damage
- Life Orb recoil
- Weather damage (sandstorm, hail)
- Status conditions (burn, poison)

The TypeScript reference implementation uses optional chaining (`effect?.effectType`) which safely handles null values, but the C# port was missing this null check.

### TypeScript Reference
```typescript
onDamage(damage, target, source, effect) {
    if (effect?.effectType === 'Move' && ['mimikyu', 'mimikyutotem'].includes(target.species.id)) {
        this.add('-activate', target, 'ability: Disguise');
        this.effectState.busted = true;
        return 0;
    }
},
```

The `?.` operator in TypeScript means "if effect is null/undefined, the whole expression evaluates to undefined and the condition is false."

## Solution

Added a null guard before checking the effect type:

```csharp
OnDamage = new OnDamageEventInfo((battle, damage, target, _, effect) =>
{
    if (effect != null &&
        effect.EffectType == EffectType.Move &&
        target.Species.Id is SpecieId.Mimikyu or SpecieId.MimikyuTotem)
    {
        battle.Add("-activate", target, "ability: Disguise");
        battle.EffectState.Busted = true;
        return 0;
    }

    return damage;
}, 1),
```

### Behavior

- **When effect is null**: Disguise doesn't activate (damage proceeds normally) - this is correct for non-move damage sources
- **When effect is a Move**: Disguise activates and blocks the damage (returns 0)
- **When effect is not a Move**: Damage proceeds normally

This ensures Disguise only blocks the first **direct attack** from a move, not damage from other sources like confusion, recoil, or weather.

## Impact

When `effect` is null, the damage proceeds unchanged without activating Disguise's protection, which matches the correct TypeScript behavior. This prevents crashes while maintaining the correct game mechanics where Disguise only protects against direct move attacks.

## Related Fixes

This is similar to the [Ripen Ability Null Effect Fix](RipenNullEffectFix.md), which also required a null check for the `effect` parameter in an event handler. This pattern of missing null checks when porting from TypeScript's optional chaining (`?.`) to C# appears to be a common issue.

## Files Modified

- `ApogeeVGC/Data/Abilities/AbilitiesDEF.cs` - Added null check to Disguise ability's OnDamage handler (line 306-317)

## Testing

- Verified compilation with no errors
- Tested with random battle simulations
- No longer crashes when Mimikyu takes damage from non-move sources

## Keywords

`disguise`, `ability`, `OnDamage`, `null reference`, `effect parameter`, `Mimikyu`, `damage`, `null check`, `optional chaining`, `TypeScript porting`
