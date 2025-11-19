# Stat Modification Handler VoidReturn Bug Fix

## Problem Summary

Battle was ending immediately in a tie on Turn 1 with error: `InvalidOperationException: stat must be an IntRelayVar, but got VoidReturnRelayVar for Ironhands's Spe (Event: ModifySpe)`

The battle crashed during `CommitChoices` when calculating Pokemon stats for move order determination.

## Root Cause

Stat modification handlers (`OnModifyAtk`, `OnModifyDef`, `OnModifySpA`, `OnModifySpD`, `OnModifySpe`) were incorrectly returning `VoidReturn()` in two scenarios:

1. **When using `ChainModify()` to apply boosts** - Handlers called `ChainModify()` but then returned `VoidReturn()` instead of `FinalModify(stat)`
2. **When conditions didn't apply** - Handlers returned `VoidReturn()` instead of returning the unmodified stat value

The `Pokemon.GetStat()` method expects all stat modification event handlers to return an `IntRelayVar`. When handlers returned `VoidReturnRelayVar` instead, the type check failed and threw an exception.

## Affected Components

### Abilities (Abilities.cs)
- **Hadron Engine** - `OnModifySpA` handler
- **Guts** - `OnModifyAtk` handler

### Conditions (Conditions.cs)
- **QuarkDrive** volatile condition - All 5 stat modification handlers:
  - `OnModifyAtk`
  - `OnModifyDef`
  - `OnModifySpA`
  - `OnModifySpD`
  - `OnModifySpe`

### Items (Items.cs)
- **Choice Specs** - `OnModifySpA` handler
- **Assault Vest** - `OnModifySpD` handler

## Solution

Fixed all stat modification handlers to follow the correct pattern:

### Pattern 1: Using ChainModify
When applying stat modifications using `ChainModify()`, always return `FinalModify(stat)`:

```csharp
OnModifySpA = new OnModifySpAEventInfo((battle, spa, _, _, _) =>
{
    if (!conditionApplies)
        return spa;  // ? Return unmodified stat
    
    battle.ChainModify(multiplier);
    return battle.FinalModify(spa);  // ? Return FinalModify result
})
```

### Pattern 2: Direct Modification
When modifying stats directly without `ChainModify()`, always return the integer value:

```csharp
OnModifySpe = new OnModifySpeEventInfo((battle, spe, pokemon) =>
{
    spe = battle.FinalModify(spe);  // Finalize pending modifications
    if (someCondition)
    {
        spe = (int)Math.Floor(spe * 0.5);  // Direct calculation
    }
    return spe;  // ? Always return int
}, -101)
```

## Key Rule

**Stat modification handlers MUST ALWAYS return an integer value, NEVER `VoidReturn()`.**

There are only two valid return patterns:
1. `return stat;` - When returning unmodified stat (early exit)
2. `return battle.FinalModify(stat);` - When applying modifications via `ChainModify()`

## Files Modified

### ApogeeVGC/Data/Abilities.cs

**Hadron Engine - OnModifySpA:**
```csharp
// Before (WRONG):
OnModifySpA = new OnModifySpAEventInfo((battle, _, _, _, _) =>
{
    if (!battle.Field.IsTerrain(ConditionId.ElectricTerrain, null))
        return new VoidReturn();  // ?
    battle.ChainModify([5461, 4096]);
    return new VoidReturn();  // ?
}, 5)

// After (CORRECT):
OnModifySpA = new OnModifySpAEventInfo((battle, spa, _, _, _) =>
{
    if (!battle.Field.IsTerrain(ConditionId.ElectricTerrain, null))
        return spa;  // ?
    battle.ChainModify([5461, 4096]);
    return battle.FinalModify(spa);  // ?
}, 5)
```

**Guts - OnModifyAtk:**
```csharp
// Before (WRONG):
OnModifyAtk = new OnModifyAtkEventInfo((battle, _, pokemon, _, _) =>
{
    if (pokemon.Status is not ConditionId.None)
    {
        battle.ChainModify(1.5);
        return new VoidReturn();  // ?
    }
    return new VoidReturn();  // ?
}, 5)

// After (CORRECT):
OnModifyAtk = new OnModifyAtkEventInfo((battle, atk, pokemon, _, _) =>
{
    if (pokemon.Status is not ConditionId.None)
    {
        battle.ChainModify(1.5);
        return battle.FinalModify(atk);  // ?
    }
    return atk;  // ?
}, 5)
```

### ApogeeVGC/Data/Conditions.cs

**QuarkDrive - OnModifyAtk/Def/SpA/SpD/Spe:**
```csharp
// Before (WRONG):
OnModifyAtk = new OnModifyAtkEventInfo((battle, _, pokemon, _, _) =>
{
    if (battle.EffectState.BestStat != StatIdExceptHp.Atk ||
        pokemon.IgnoringAbility())
    {
        return new VoidReturn();  // ?
    }
    battle.ChainModify([5325, 4096]);
    return new VoidReturn();  // ?
}, 5)

// After (CORRECT):
OnModifyAtk = new OnModifyAtkEventInfo((battle, atk, pokemon, _, _) =>
{
    if (battle.EffectState.BestStat != StatIdExceptHp.Atk ||
        pokemon.IgnoringAbility())
    {
        return atk;  // ?
    }
    battle.ChainModify([5325, 4096]);
    return battle.FinalModify(atk);  // ?
}, 5)
```

Applied same fix to `OnModifyDef`, `OnModifySpA`, `OnModifySpD`, and `OnModifySpe`.

### ApogeeVGC/Data/Items.cs

**Choice Specs - OnModifySpA:**
```csharp
// Before (WRONG):
OnModifySpA = new OnModifySpAEventInfo((battle, _, _, _, _) =>
{
    battle.ChainModify(1.5);
    return new VoidReturn();  // ?
}, 1)

// After (CORRECT):
OnModifySpA = new OnModifySpAEventInfo((battle, spa, _, _, _) =>
{
    battle.ChainModify(1.5);
    return battle.FinalModify(spa);  // ?
}, 1)
```

**Assault Vest - OnModifySpD:**
```csharp
// Before (WRONG):
OnModifySpD = new OnModifySpDEventInfo((battle, _, _, _, _) =>
{
    battle.ChainModify(1.5);
    return new VoidReturn();  // ?
}, 1)

// After (CORRECT):
OnModifySpD = new OnModifySpDEventInfo((battle, spd, _, _, _) =>
{
    battle.ChainModify(1.5);
    return battle.FinalModify(spd);  // ?
}, 1)
```

### ApogeeVGC/Sim/PokemonClasses/Pokemon.Stats.cs

Enhanced error diagnostics in `GetStat()` method:

```csharp
// Added detailed error message to help diagnose future issues:
else
{
    string relayVarType = relayVar?.GetType().Name ?? "null";
    throw new InvalidOperationException(
        $"stat must be an IntRelayVar, but got {relayVarType} for {Name}'s {statName} (Event: {eventId})");
}
```

## Testing

After fixes, the test battle between Ironhands (Assault Vest, Quark Drive) and Miraidon (Choice Specs, Hadron Engine) on Electric Terrain runs successfully without crashes. All stat calculations complete correctly during move order determination.

## Related Bugs

This bug is similar to the **Tailwind OnModifySpe Fix** (documented in `TailwindOnModifySpeFix.md`), which also involved stat modification handlers incorrectly returning `VoidReturn()`.

## Prevention Guidelines

When implementing new stat modification handlers:

1. **Always return an integer value**, never `VoidReturn()`
2. **Use the `stat` parameter name** (not `_`) to ensure you return it
3. **When using `ChainModify()`**, always call and return `battle.FinalModify(stat)`
4. **When not applying modifications**, return the unmodified `stat` value
5. **Test with different Pokemon/items/abilities** that trigger the handlers
6. **Look for existing patterns** in `Paralysis.OnModifySpe` for correct implementation examples

## Keywords

`stat modification`, `OnModifyAtk`, `OnModifyDef`, `OnModifySpA`, `OnModifySpD`, `OnModifySpe`, `VoidReturn`, `IntRelayVar`, `ChainModify`, `FinalModify`, `Hadron Engine`, `Quark Drive`, `Choice Specs`, `Assault Vest`, `Guts`, `type mismatch`, `event handlers`
