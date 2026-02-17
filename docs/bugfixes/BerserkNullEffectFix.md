# Berserk Ability Null Effect Fix

**Date**: 2025-01-XX  
**Severity**: High  
**Systems Affected**: Berserk ability `OnDamage` event handler

## Problem

The Berserk ability crashed with a `NullReferenceException` when trying to access `effect.EffectType` in its `OnDamage` handler at line 647 of `AbilitiesABC.cs`. The error occurred during random battle testing when a Pokémon with Berserk took damage from sources that don't have an associated `IEffect`.

### Error Stack Trace
```
System.NullReferenceException: Object reference not set to an instance of an object.
   at ApogeeVGC.Data.Abilities.Abilities.<>c.<CreateAbilitiesAbc>b__8_42(Battle battle, Int32 damage, Pokemon _, Pokemon source, IEffect effect) in C:\VSProjects\ApogeeVGC\ApogeeVGC\Data\Abilities\AbilitiesABC.cs:line 647
   at ApogeeVGC.Sim.Events.EventHandlerAdapter.AdaptLegacyHandler.<>c__DisplayClass0_0.<.ctor>b__0(EventContext context)
   at ApogeeVGC.Sim.BattleClasses.Battle.InvokeEventHandlerInfo(EventHandlerInfo handlerInfo, Boolean hasRelayVar, RelayVar relayVar, SingleEventTarget target, SingleEventSource source, IEffect sourceEffect)
   at ApogeeVGC.Sim.BattleClasses.Battle.RunEvent(EventId eventId, RunEventTarget target, RunEventSource source, IEffect sourceEffect, RelayVar relayVar, Nullable`1 onEffect, Nullable`1 fastExit)
   at ApogeeVGC.Sim.BattleClasses.Battle.SpreadDamage(SpreadMoveDamage damage, SpreadMoveTargets targetArray, Pokemon source, BattleDamageEffect effect, Boolean instaFaint)
```

## Root Cause

The handler did not check if the `effect` parameter was null before accessing its `EffectType` property. Damage can come from various sources that don't have an associated effect:
- Confusion self-damage
- Recoil damage
- Life Orb recoil
- Weather damage (sandstorm, hail)
- Status conditions (burn, poison)

The TypeScript reference implementation accesses `effect.effectType` directly without a null check, which works in JavaScript because accessing a property on `undefined` doesn't throw an error—it just returns `undefined`. However, in C#, accessing a property on a null reference throws a `NullReferenceException`.

### TypeScript Reference
```typescript
onDamage(damage, target, source, effect) {
    if (
        effect.effectType === "Move" &&
        !effect.multihit &&
        !(effect.hasSheerForce && source.hasAbility('sheerforce'))
    ) {
        this.effectState.checkedBerserk = false;
    } else {
        this.effectState.checkedBerserk = true;
    }
},
```

In JavaScript, if `effect` is `undefined`, `effect.effectType` evaluates to `undefined`, which is not equal to `"Move"`, so the condition is false.

## Solution

Added a null guard before checking the effect type:

```csharp
OnDamage = new OnDamageEventInfo((battle, damage, _, source, effect) =>
{
    if (effect != null &&
        effect.EffectType == EffectType.Move &&
        effect is Move { MultiHit: null } move &&
        !(move.HasSheerForce == true && source != null &&
          source.HasAbility(AbilityId.SheerForce)))
    {
        battle.EffectState.CheckedBerserk = false;
    }
    else
    {
        battle.EffectState.CheckedBerserk = true;
    }

    return damage;
}),
```

### Behavior

- **When effect is null**: `CheckedBerserk` is set to `true` (falls into the else branch)
- **When effect is a Move without multihit and not Sheer Force**: `CheckedBerserk` is set to `false`
- **When effect is a Move with multihit or Sheer Force**: `CheckedBerserk` is set to `true`
- **When effect is not a Move**: `CheckedBerserk` is set to `true`

The `CheckedBerserk` state is used by the `OnTryEatItem` handler to prevent consuming healing berries during certain damage calculations and by `OnAfterMoveSecondary` to trigger the Special Attack boost when HP drops below 50%.

## Impact

When `effect` is null, `CheckedBerserk` is set to `true`, which matches the TypeScript behavior where a null/undefined effect causes the condition to fail and fall into the else branch. This prevents crashes while maintaining the correct game mechanics.

## Related Fixes

This is similar to:
- [Disguise Ability Null Effect Fix](DisguiseNullEffectFix.md)
- [Ripen Ability Null Effect Fix](RipenNullEffectFix.md)
- [Adrenaline Orb Null Effect Fix](AdrenalineOrbNullEffectFix.md)

All required null checks for the `effect` parameter in event handlers. This pattern of missing null checks when porting from TypeScript to C# is a common issue because:
1. TypeScript's `?.` optional chaining safely handles null/undefined
2. JavaScript allows property access on undefined (returns undefined) without throwing errors
3. C# throws `NullReferenceException` when accessing properties on null references

## Files Modified

- `ApogeeVGC/Data/Abilities/AbilitiesABC.cs` - Added null check to Berserk ability's OnDamage handler (line 645-660)

## Testing

- Verified compilation with no errors
- Build successful
- Ready for random battle simulation testing

## Keywords

`berserk`, `ability`, `OnDamage`, `null reference`, `effect parameter`, `damage`, `null check`, `TypeScript porting`, `CheckedBerserk`, `effect state`
