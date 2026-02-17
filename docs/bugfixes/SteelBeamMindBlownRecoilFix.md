# Steel Beam & Mind Blown Recoil Effect Fix

**Status**: Fixed  
**Date**: 2025-01-XX  
**Severity**: High  
**Systems Affected**: Move recoil mechanics, specifically MindBlownRecoil property  

---

## Problem

When using moves with the `MindBlownRecoil` property (Steel Beam and Mind Blown), the game crashed with an `ArgumentException`:

```
System.ArgumentException: MoveId 'SteelBeam' does not have a corresponding ConditionId.
  at ApogeeVGC.Sim.Utils.Extensions.MoveIdTools.ToConditionId(MoveId moveId)
  at ApogeeVGC.Sim.BattleClasses.BattleActions.HitStepMoveHitLoop(...)
```

The error occurred when the code tried to apply recoil damage after the move hit.

---

## Root Cause

The C# code was attempting to look up a `ConditionId` for the move to use as the damage effect:

```csharp
// BEFORE (incorrect)
Battle.Damage((int)Math.Round(pokemon.MaxHp / 2.0), pokemon, pokemon,
    BattleDamageEffect.FromIEffect(Library.Conditions[move.Id.ToConditionId()]),
    true);
```

However, Steel Beam and Mind Blown **do not have corresponding `ConditionId` entries** in the enum. These moves don't create persistent conditions; they only need to be referenced as the source of the recoil damage.

The TypeScript source code handles this differently. At `pokemon-showdown/sim/battle-actions.ts:963`:

```typescript
this.battle.damage(Math.round(pokemon.maxhp / 2), pokemon, pokemon, 
    this.dex.conditions.get(move.id), true);
```

In TypeScript, `conditions.get(move.id)` can dynamically retrieve or create a condition reference for any move, even if it's not a "real" condition. The move itself serves as the effect reference for damage tracking purposes.

---

## Solution

Since `ActiveMove` implements `IEffect`, we can pass the move directly as the damage effect instead of trying to look it up:

```csharp
// AFTER (correct)
Battle.Damage((int)Math.Round(pokemon.MaxHp / 2.0), pokemon, pokemon,
    BattleDamageEffect.FromIEffect(move),
    true);
```

This aligns with how TypeScript treats the move as an effect for damage source tracking.

---

## Changes Made

### File: `ApogeeVGC/Sim/BattleClasses/BattleActions.HitSteps.cs`

**Lines ~908-909**: Changed the damage effect parameter from attempting to look up a condition to using the move directly.

```diff
  if (move.MindBlownRecoil == true)
  {
      int hpBeforeRecoil = pokemon.Hp;

      Battle.Damage((int)Math.Round(pokemon.MaxHp / 2.0), pokemon, pokemon,
-         BattleDamageEffect.FromIEffect(Library.Conditions[move.Id.ToConditionId()]),
+         BattleDamageEffect.FromIEffect(move),
          true);

      move.MindBlownRecoil = false;
      if (pokemon.Hp <= pokemon.MaxHp / 2 && hpBeforeRecoil > pokemon.MaxHp / 2)
      {
          Battle.RunEvent(EventId.EmergencyExit, pokemon, pokemon);
      }
  }
```

---

## Testing

- **Reproduced**: Steel Beam caused crash when used in random battles
- **Verified**: After fix, Steel Beam applies recoil damage correctly
- **Side Effects**: None - Mind Blown also benefits from this fix

---

## Notes

- This fix applies to both **Steel Beam** and **Mind Blown** (the only two moves with `MindBlownRecoil = true`)
- The `ToConditionId()` method should only be used for moves that have explicit `ConditionId` mappings (e.g., Protect, Trick Room, Leech Seed)
- For damage tracking purposes, moves can serve as `IEffect` instances directly without needing a separate condition

---

## Related

- **Move Data**: `ApogeeVGC/Data/Moves/MovesSTU.cs` - Steel Beam definition with `MindBlownRecoil = true`
- **TypeScript Reference**: `pokemon-showdown/sim/battle-actions.ts:961-968` - MindBlownRecoil handling
- **TypeScript Move Definitions**: 
  - `pokemon-showdown/data/moves.ts:12306-12330` - Mind Blown
  - `pokemon-showdown/data/moves.ts:18565-18580` - Steel Beam

---

## Keywords

`MindBlownRecoil`, `Steel Beam`, `Mind Blown`, `recoil`, `damage effect`, `ConditionId`, `IEffect`, `move mechanics`
