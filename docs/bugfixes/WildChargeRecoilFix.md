# Wild Charge Recoil Damage Bug Fix

## Problem Summary

When Iron Hands used Wild Charge against Volcarona, the move dealt damage correctly (75 damage to target, 43 recoil damage to user), but immediately after the recoil damage was applied, the battle ended prematurely as a tie with a `KeyNotFoundException: The given key 'Recoil' was not present in the dictionary.`

### Debug Output Showing the Issue

```
[SpreadDamage] About to apply 75 damage to Volcarona (current HP: 192)
[SpreadDamage] Applied 75 damage to Volcarona (new HP: 117)
[SpreadDamage] About to apply 43 damage to Ironhands (current HP: 240)
[SpreadDamage] Applied 43 damage to Ironhands (new HP: 197)
[Simulator.ProcessChoiceResponsesAsync] ERROR: KeyNotFoundException: The given key 'Recoil' was not present in the dictionary.
[Simulator.ProcessChoiceResponsesAsync] Exiting, Battle.Ended=False
```

## Root Cause

The `ConditionId.Recoil` enum value exists in the `ConditionId` enum (in `ApogeeVGC\Sim\Conditions\ConditionId.cs`), but there was no corresponding entry in the `Conditions` dictionary (in `ApogeeVGC\Data\Conditions.cs`).

When recoil damage is applied in `BattleActions.HitSteps.cs` (lines 837-843), the code attempts to retrieve the Recoil condition from the Library:

```csharp
// Recoil damage
if ((move.Recoil != null || move.Id == MoveId.Chloroblast) && 
    move.TotalDamage is IntIntFalseUnion totalDamageInt && totalDamageInt.Value > 0)
{
    int hpBeforeRecoil = pokemon.Hp;

    Battle.Damage(CalcRecoilDamage(totalDamageInt.Value, move, pokemon), pokemon, pokemon,
        BattleDamageEffect.FromIEffect(Library.Conditions[ConditionId.Recoil]));  // <-- KeyNotFoundException here

    if (pokemon.Hp <= pokemon.MaxHp / 2 && hpBeforeRecoil > pokemon.MaxHp / 2)
    {
        Battle.RunEvent(EventId.EmergencyExit, pokemon, pokemon);
    }
}
```

The dictionary lookup `Library.Conditions[ConditionId.Recoil]` throws a `KeyNotFoundException` because the key doesn't exist.

## Solution

Added the missing `ConditionId.Recoil` entry to the `Conditions` dictionary in `ApogeeVGC\Data\Conditions.cs`:

```csharp
[ConditionId.Recoil] = new()
{
    Id = ConditionId.Recoil,
    Name = "Recoil",
    EffectType = EffectType.Condition,
},
```

This is a minimal condition definition - recoil damage doesn't require any event handlers since it's just a label for the damage effect. The actual damage calculation is handled by `CalcRecoilDamage()` in `BattleActions.Damage.cs`.

## Files Modified

1. **ApogeeVGC\Data\Conditions.cs**
   - Added `ConditionId.Recoil` entry to the `CreateConditions()` dictionary
   - Placed after `ConditionId.StruggleRecoil` for logical grouping

## Similar Issues

This is similar to the pattern seen in other bug fixes where enum values exist but their dictionary entries are missing. The `StruggleRecoil` condition was already defined, but `Recoil` (for regular recoil moves) was not.

### Affected Moves

This bug would have affected all moves with recoil damage in Gen 9, including:
- **Electric-type**: Wild Charge, Volt Tackle
- **Normal-type**: Take Down, Double-Edge, Head Smash, Brave Bird
- **Fighting-type**: Submission, High Jump Kick
- **Rock-type**: Head Smash
- **Flying-type**: Brave Bird
- **Grass-type**: Wood Hammer, Chloroblast (special case with 50% max HP recoil)
- And many others

## Testing

The fix can be verified by:
1. Running any battle where a Pokémon uses a recoil move (e.g., Wild Charge, Double-Edge, Brave Bird)
2. Confirming the move deals damage to the target
3. Confirming recoil damage is applied to the user
4. Confirming the battle continues normally without a `KeyNotFoundException`

### Expected Behavior After Fix

When Iron Hands uses Wild Charge:
- Wild Charge deals 75 damage to Volcarona (or calculated damage based on stats)
- Wild Charge deals 1/4 recoil (18-19 damage) to Iron Hands
- Battle continues normally
- No exception is thrown

## Related Code

### Recoil Damage Calculation

From `BattleActions.Damage.cs`:

```csharp
public int CalcRecoilDamage(int damageDealt, Move move, Pokemon pokemon)
{
    // Chloroblast is a special case - returns 50% of max HP as recoil
    if (move.Id == MoveId.Chloroblast)
    {
        return (int)Math.Round(pokemon.MaxHp / 2.0);
    }

    // Standard recoil calculation: damageDealt * recoil[0] / recoil[1]
    // Clamped to minimum of 1
    if (move.Recoil == null) return 0;

    int recoilDamage = (int)Math.Round(damageDealt * move.Recoil.Value.Item1 /
                                       (double)move.Recoil.Value.Item2);
    return Battle.ClampIntRange(recoilDamage, 1, null);
}
```

Wild Charge has `Recoil = (1, 4)` which means 1/4 of damage dealt is taken as recoil.

## Keywords

`recoil`, `Wild Charge`, `KeyNotFoundException`, `Conditions`, `dictionary`, `missing entry`, `enum mismatch`, `damage`, `recoil moves`, `Iron Hands`, `condition definition`

## Related Previous Fixes

This issue follows a similar pattern to:
- **TrySetStatus Logic Error**: Missing/incorrect dictionary handling for conditions
- The pattern where enum values exist but their corresponding data structures are incomplete

## Prevention

When adding new `ConditionId` enum values:
1. Always add a corresponding entry in the `Conditions` dictionary in `Conditions.cs`
2. Define at minimum: `Id`, `Name`, and `EffectType`
3. Add any required event handlers based on the condition's behavior
4. Test with moves/abilities/items that reference the condition

---

**Fixed**: 2025-01-XX  
**Severity**: High (battle-breaking exception)  
**Systems Affected**: Recoil damage moves, move execution pipeline
