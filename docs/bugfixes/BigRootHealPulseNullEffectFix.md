# Big Root Heal Pulse Null Effect Fix

**Date**: 2025  
**Severity**: High  
**Systems Affected**: Big Root item `OnTryHeal` event handler

## Problem

The Big Root item caused a `NullReferenceException` when a Pokémon holding Big Root was healed by Heal Pulse or other moves that don't pass an explicit effect parameter. The error occurred at line 390 in `ItemsABC.cs`:

```
System.NullReferenceException
  Message=Object reference not set to an instance of an object.
  Source=ApogeeVGC
  StackTrace:
   at ApogeeVGC.Data.Items.Items.<>c.<CreateItemsAbc>b__8_23(Battle battle, Int32 damage, Pokemon _, Pokemon _, IEffect effect) in C:\VSProjects\ApogeeVGC\ApogeeVGC\Data\Items\ItemsABC.cs:line 390
```

### Error Context

The exception occurred when:
1. A Pokémon holding Big Root was healed by Heal Pulse
2. The `TryHeal` event was triggered during the healing process
3. The Big Root item's `OnTryHeal` handler executed
4. The handler attempted to access `effect.EffectStateId` when `effect` was `null`

## Root Cause

The `OnTryHeal` handler for Big Root did not check if the `effect` parameter was null before accessing its properties. When moves like Heal Pulse call `battle.Heal(healAmount, target)` with only the damage and target parameters, the `effect` parameter defaults to null.

### Chain of Events

1. **Heal Pulse OnHit** (`MovesGHI.cs:988-1015`):
   ```csharp
   IntFalseUnion healResult = battle.Heal(healAmount, target);
   ```
   - Calls `Battle.Heal()` with only `healAmount` and `target`
   - No explicit `effect` parameter is passed

2. **Battle.Heal Method** (`Battle.Combat.cs:308-330`):
   ```csharp
   public IntFalseUnion Heal(int damage, Pokemon? target = null, Pokemon? source = null,
       BattleHealEffect? effect = null)
   {
       // ...
       Condition? effectCondition = effect switch
       {
           DrainBattleHealEffect => Library.Conditions[ConditionId.Drain],
           EffectBattleHealEffect ebhe => ebhe.Effect as Condition,
           null => Effect as Condition,  // ? Effect is the Move, not a Condition!
           _ => throw new InvalidOperationException("Unknown BattleHealEffect type."),
       };
   ```
   - When `effect` is `null`, it tries `Effect as Condition`
   - `Effect` is currently the Heal Pulse move (a `Move`, not a `Condition`)
   - The cast to `Condition` returns `null`

3. **RunEvent for TryHeal** (`Battle.Combat.cs:343-349`):
   ```csharp
   RelayVar? tryHealResult = RunEvent(
       EventId.TryHeal,
       RunEventTarget.FromNullablePokemon(target),
       RunEventSource.FromNullablePokemon(source),
       effectCondition,  // ? null is passed here
       new IntRelayVar(damage)
   );
   ```
   - Passes the `null` `effectCondition` to the event

4. **Big Root OnTryHeal Handler** (`ItemsABC.cs:390`):
   ```csharp
   bool isBigRootHeal = effect.EffectStateId switch  // ? NullReferenceException!
   ```
   - Attempts to access `effect.EffectStateId` without null check
   - Crashes with `NullReferenceException`

### Why TypeScript Doesn't Crash

In the TypeScript source (`pokemon-showdown/data/items.ts:489-494`):

```typescript
onTryHeal(damage, target, source, effect) {
    const heals = ['drain', 'leechseed', 'ingrain', 'aquaring', 'strengthsap'];
    if (heals.includes(effect.id)) {
        return this.chainModify([5324, 4096]);
    }
},
```

The TypeScript code also accesses `effect.id` without an explicit null check, but:
1. TypeScript's `includes()` method on a falsy value doesn't throw (returns false)
2. The TypeScript battle engine might always pass an effect object
3. JavaScript's looser type system handles null/undefined more gracefully

However, in C#, we need explicit null checking because:
- The type system is stricter
- Accessing properties on null throws `NullReferenceException`
- The `Battle.Heal()` method can legitimately pass null effects

## Affected Code

**Original Code** (`ApogeeVGC\Data\Items\ItemsABC.cs:381-409`):

```csharp
OnTryHeal = new OnTryHealEventInfo(
    (Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolUnion?>)((battle, damage,
        _, _, effect) =>
    {
        // BigRoot boosts healing from: drain moves, Leech Seed, Ingrain, Aqua Ring, Strength Sap
        // TS checks effect.id for: 'drain', 'leechseed', 'ingrain', 'aquaring', 'strengthsap'
        // The effect passed here is always an IEffect (Condition or Move), not BattleHealEffect
        // Drain healing is passed as the Drain condition (ConditionId.Drain)
        // Strength Sap passes MoveId.StrengthSap
        bool isBigRootHeal = effect.EffectStateId switch  // ? No null check!
        {
            ConditionEffectStateId
            {
                ConditionId: ConditionId.Drain or ConditionId.LeechSeed
                or ConditionId.Ingrain or ConditionId.AquaRing
            } => true,
            MoveEffectStateId { MoveId: MoveId.StrengthSap } => true,
            _ => false
        };

        if (isBigRootHeal)
        {
            battle.ChainModify([5324, 4096]);
            return IntBoolUnion.FromInt(battle.FinalModify(damage));
        }

        // Return null to match TS undefined - "don't modify, pass through"
        return null;
    }), 1),
```

## Solution

Added a null check at the beginning of the `OnTryHeal` handler to guard against null effects:

```csharp
OnTryHeal = new OnTryHealEventInfo(
    (Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolUnion?>)((battle, damage,
        _, _, effect) =>
    {
        // Early return if effect is null (e.g., Heal Pulse doesn't pass a Condition)
        if (effect == null)
        {
            return null;
        }

        // BigRoot boosts healing from: drain moves, Leech Seed, Ingrain, Aqua Ring, Strength Sap
        // TS checks effect.id for: 'drain', 'leechseed', 'ingrain', 'aquaring', 'strengthsap'
        // The effect passed here is always an IEffect (Condition or Move), not BattleHealEffect
        // Drain healing is passed as the Drain condition (ConditionId.Drain)
        // Strength Sap passes MoveId.StrengthSap
        bool isBigRootHeal = effect.EffectStateId switch
        {
            ConditionEffectStateId
            {
                ConditionId: ConditionId.Drain or ConditionId.LeechSeed
                or ConditionId.Ingrain or ConditionId.AquaRing
            } => true,
            MoveEffectStateId { MoveId: MoveId.StrengthSap } => true,
            _ => false
        };

        if (isBigRootHeal)
        {
            battle.ChainModify([5324, 4096]);
            return IntBoolUnion.FromInt(battle.FinalModify(damage));
        }

        // Return null to match TS undefined - "don't modify, pass through"
        return null;
    }), 1),
```

### Rationale

When `effect` is null, the healing source is not one of the effects that Big Root boosts (drain moves, Leech Seed, etc.), so:
- Big Root should not boost the healing
- Return `null` to pass through the original heal amount unchanged

This matches the expected behavior: Heal Pulse and similar moves should not be boosted by Big Root.

## Prevention

**When implementing event handlers that accept `IEffect` parameters:**

1. **Always add null guards** for effect parameters in event handlers
2. **Check the parameter flow**: If a method can call the event without passing an explicit effect, the handler must handle null
3. **Follow the established pattern** from similar fixes (Ripen, Disguise, Adrenaline Orb, Berserk, etc.)

**Standard pattern for effect-checking handlers:**

```csharp
OnEventName = new OnEventNameEventInfo((battle, ..., effect) =>
{
    // Guard against null effect first
    if (effect == null)
    {
        return /* appropriate default value */;
    }
    
    // Safe to access effect properties now
    if (effect.EffectStateId == SomeValue) { ... }
});
```

## Related Issues

This is part of a broader pattern of null effect issues:
- **Ripen Ability Null Effect Fix**: Same issue with OnTryHeal handler
- **Disguise Ability Null Effect Fix**: OnDamage handler with null effect
- **Adrenaline Orb Null Effect Fix**: OnAfterBoost handler with null effect
- **Berserk Ability Null Effect Fix**: OnDamage handler with null effect
- **Grassy Glide ModifyPriority Source Fix**: OnModifyPriority with null source

**Pattern**: Event handlers that check effect/source properties must guard against null parameters because:
1. The battle engine may not always pass explicit effects/sources
2. Default parameter handling (`Effect as Condition`) can return null
3. C# requires explicit null handling unlike TypeScript's more permissive approach

## Testing

**Reproduction scenario:**
1. Create a Pokémon holding Big Root
2. Have an opponent use Heal Pulse on the Pokémon
3. The `NullReferenceException` occurred when the `TryHeal` event triggered

**After fix:**
- No `NullReferenceException` when Heal Pulse is used on a Pokémon with Big Root
- Big Root still correctly boosts healing from drain moves, Leech Seed, etc.
- Heal Pulse healing is not boosted by Big Root (correct behavior)

**Verification:**
- Build successful
- Fix follows established pattern from similar null effect fixes
- Code matches defensive programming practices for C# nullable reference handling

## Keywords

`big root`, `item`, `OnTryHeal`, `null reference`, `effect parameter`, `heal pulse`, `healing`, `drain`, `leech seed`, `NullReferenceException`
