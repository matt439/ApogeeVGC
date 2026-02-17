# BoostId To String Evasion/Accuracy Conversion Fix

**Date**: December 2024  
**Severity**: High  
**Systems Affected**: Stat boost display system, all moves/abilities/items that modify accuracy or evasion

---

## Problem

When attempting to display stat boost changes for Evasion or Accuracy, the application threw an `ArgumentOutOfRangeException`:

```
System.ArgumentOutOfRangeException: Cannot convert Evasion to StatId. (Parameter 'stat')
  at ApogeeVGC.Sim.Utils.Extensions.StatIdTools.ConvertToStatId(BoostId stat)
  at ApogeeVGC.Sim.Utils.Extensions.StatIdTools.ConvertToString(BoostId boost, Boolean leadingCapital)
  at ApogeeVGC.Sim.BattleClasses.Battle.Boost(...)
```

This occurred when any move attempted to modify evasion (e.g., Defog lowering evasion, Double Team raising evasion) or accuracy.

---

## Root Cause

The `ConvertToString(BoostId)` extension method was implemented incorrectly:

```csharp
public static string ConvertToString(this BoostId boost, bool leadingCapital = false)
{
    return boost.ConvertToStatId().ConvertToString();  // ? Wrong!
}
```

This implementation attempted to convert the `BoostId` to a `StatId` first, then convert that to a string. However:

- **`BoostId`** includes: Atk, Def, SpA, SpD, Spe, **Accuracy**, **Evasion**
- **`StatId`** includes: HP, Atk, Def, SpA, SpD, Spe

Accuracy and Evasion are **boost-only values**—they can be modified by stat boosts but are not actual stats that Pokemon possess. They don't appear in `StatId`, so `ConvertToStatId()` correctly threw an exception when encountering them.

The error manifests when:
1. A move uses `battle.Boost()` with Accuracy or Evasion changes (e.g., Defog's `-1 evasion`)
2. `Battle.Boost()` calls `boostId.ConvertToString()` to format the battle message
3. `ConvertToString()` tries to convert to `StatId` first, which fails for Accuracy/Evasion

---

## Solution

Modified `ConvertToString(BoostId)` to handle all `BoostId` values directly, including Accuracy and Evasion:

```csharp
public static string ConvertToString(this BoostId boost, bool leadingCapital = false)
{
    return (boost, leadingCapital) switch
    {
        (BoostId.Atk, true) => "Attack",
        (BoostId.Atk, false) => "attack",
        (BoostId.Def, true) => "Defense",
        (BoostId.Def, false) => "defense",
        (BoostId.SpA, true) => "Special Attack",
        (BoostId.SpA, false) => "special attack",
        (BoostId.SpD, true) => "Special Defense",
        (BoostId.SpD, false) => "special defense",
        (BoostId.Spe, true) => "Speed",
        (BoostId.Spe, false) => "speed",
        (BoostId.Accuracy, true) => "Accuracy",      // ? Added
        (BoostId.Accuracy, false) => "accuracy",     // ? Added
        (BoostId.Evasion, true) => "Evasion",        // ? Added
        (BoostId.Evasion, false) => "evasion",       // ? Added
        _ => throw new ArgumentOutOfRangeException(nameof(boost), "Invalid boost ID."),
    };
}
```

This matches the TypeScript implementation, which uses boost IDs directly as strings without conversion.

---

## Files Changed

- **`ApogeeVGC\Sim\Utils\Extensions\StatIdTools.cs`**: Rewrote `ConvertToString(BoostId)` method

---

## Testing

After the fix:
- Defog correctly displays "lowered [target]'s evasion by 1"
- Double Team correctly displays "raised [user]'s evasion by 1"
- Moves affecting accuracy (e.g., Sand Attack, Flash) work correctly
- All other stat boosts continue to work as before

---

## Related Issues

This fix follows the pattern established in previous bug fixes where C# enum conversion logic needed to account for all valid enum values, similar to:
- **Union Type Handling Guide** - Proper handling of discriminated unions
- **Stat Modification Parameter Nullability Fix** - Ensuring stat modification methods handle all valid inputs

---

## Prevention

**Key Insight**: When working with Pokemon mechanics:
- `StatId` = actual Pokemon stats (HP, Attack, Defense, Sp. Attack, Sp. Defense, Speed)
- `BoostId` = stats that can be boosted (all of the above except HP, **plus** Accuracy and Evasion)

Accuracy and Evasion are **modifiers** applied during battle calculations, not base stats.

When creating conversion methods:
1. Always check that the target type can represent all values of the source type
2. If not, handle the conversion directly rather than through an intermediate type
3. Reference the TypeScript source to understand which values are valid in each context
