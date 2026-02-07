# Fainted Pokemon Switch Loop Fix

## Problem Summary

Random vs Random battles were entering infinite loops and timing out when `PlayerRandom` tried to switch to fainted Pokemon or select moves for trapped Pokemon with all moves disabled.

**Symptoms**:
- `BattleTimeoutException` thrown consistently
- Random battles failing at various points (e.g., 496/500, 499/500)
- Exception: "No available choices for random player at Pokemon index X"

## Root Causes

### Issue 1: Fainted Pokemon Not Properly Identified in Switch Requests

**Problem**: `PokemonSwitchRequestData.Condition` was not set to `ConditionId.Fainted` for fainted Pokemon.

When a Pokemon is fainted:
- `GetHealth().Secret` returns `"0 fnt"` (a `SecretString`)
- The pattern matching `is SecretConditionId` fails because it's a string
- Code defaulted to `ConditionId.None`
- Both healthy and fainted Pokemon had `Condition == ConditionId.None`

**In `Pokemon.Requests.cs`**:
```csharp
// OLD CODE (BUGGY)
if (GetHealth().Secret is not SecretConditionId secretCondition)
{
    secretCondition = new SecretConditionId(ConditionId.None);
}
// ...
Condition = secretCondition.Value,  // Always None for fainted Pokemon!
```

**In `PlayerRandom.cs`**:
```csharp
// OLD CODE (BUGGY)
private bool IsPokemonFainted(PokemonSwitchRequestData pokemon)
{
    return pokemon.Reviving;  // Only checked Revival Blessing!
}
```

This caused:
1. Random player to consider fainted Pokemon as valid switch targets
2. Infinite loop: select fainted Pokemon ? choice rejected ? retry ? select same fainted Pokemon

### Issue 2: No Fallback When All Moves Disabled and Trapped

**Problem**: `GetRandomMoveChoice` threw an exception when a Pokemon was trapped with all moves disabled.

**In `PlayerRandom.cs`**:
```csharp
// OLD CODE (BUGGY)
// Add switch option (checking if any switches are available)
var availableSwitches = request.Side.Pokemon
    .Where(x => !x.PokemonData.Active && !IsPokemonFainted(x.PokemonData))
    .ToList();

bool canSwitch = availableSwitches.Count > 0;
if (canSwitch)
{
    availableChoices.Add((false, -1, false));
}

// Pick a random choice
if (availableChoices.Count == 0)
{
    throw new InvalidOperationException("No available choices...");  // ? BUG
}
```

Issues:
1. Didn't check if Pokemon was trapped before adding switch option
2. Threw exception instead of using Struggle when no choices available

## The Fixes

### Fix 1: Set Condition=Fainted for Fainted Pokemon

**File**: `ApogeeVGC/Sim/PokemonClasses/Pokemon.Requests.cs`

```csharp
// Determine the condition - prioritize Fainted status
// When fainted, GetHealth().Secret returns "0 fnt" (a SecretString), not SecretConditionId,
// so we need to explicitly check the Fainted property
ConditionId condition = Fainted
    ? ConditionId.Fainted
    : (GetHealth().Secret is SecretConditionId secretCondition
        ? secretCondition.Value
        : ConditionId.None);

// Create the base entry
var entry = new PokemonSwitchRequestData
{
    // ...
    Condition = condition,  // ? Now correctly set to Fainted
    // ...
};
```

### Fix 2: Check Condition in IsPokemonFainted

**File**: `ApogeeVGC/Sim/Player/PlayerRandom.cs`

```csharp
private bool IsPokemonFainted(PokemonSwitchRequestData pokemon)
{
    // Check if the Pokemon is fainted (Condition == Fainted)
    // or being revived by Revival Blessing (Reviving flag)
    return pokemon.Condition == ConditionId.Fainted || pokemon.Reviving;
}
```

### Fix 3: Handle Trapped State and Provide Fallback

**File**: `ApogeeVGC/Sim/Player/PlayerRandom.cs`

```csharp
// Add switch option only if:
// 1. The Pokemon is not trapped
// 2. There are bench Pokemon available to switch in
bool isTrapped = pokemonRequest.Trapped == true;
var availableSwitches = request.Side.Pokemon
    .Select((p, index) => new { PokemonData = p, Index = index })
    .Where(x => !x.PokemonData.Active && !IsPokemonFainted(x.PokemonData))
    .ToList();

bool canSwitch = !isTrapped && availableSwitches.Count > 0;
if (canSwitch)
{
    availableChoices.Add((false, -1, false));
}

// If no choices available (all moves disabled and can't switch), 
// use the first move anyway - the battle engine will force Struggle
if (availableChoices.Count == 0)
{
    if (pokemonRequest.Moves.Count > 0)
    {
        availableChoices.Add((true, 0, false));
    }
    else
    {
        // No moves at all - pass
        actions.Add(new ChosenAction
        {
            Choice = ChoiceType.Pass,
            Pokemon = null,
            MoveId = MoveId.None,
        });
        continue;
    }
}
```

## Impact

**Before Fixes**:
- Random battles timeout consistently when fainted Pokemon exist
- "No available choices" exception when Pokemon trapped with no moves
- Incremental test runner cannot progress past certain moves

**After Fixes**:
- Fainted Pokemon correctly excluded from switch options
- Trapped Pokemon with disabled moves use Struggle as fallback
- Random battles complete normally

## New Debug Feature: Single Battle Mode

To help debug timeout issues, a new `SingleBattleDebug` mode has been added:

**Usage**:
1. Edit `Program.cs` to use `DriverMode.SingleBattleDebug`
2. Edit seed values in `Driver.RunSingleBattleDebug()`:
   ```csharp
   const int debugPlayer1Seed = 1453;
   const int debugPlayer2Seed = 1454;
   const int debugBattleSeed = 1455;
   ```
3. Run the program

This will:
- Run a single battle with the specified seeds
- Print detailed debug output
- Show exception details if the battle fails
- Allow stepping through in debugger

## Testing

Verified fix resolves timeouts with seeds that previously failed:
- P1=1453, P2=1454, Battle=1455 (previously timed out)
- Various other seed combinations from incremental test failures

## Files Modified

1. **ApogeeVGC/Sim/PokemonClasses/Pokemon.Requests.cs**
   - Set `Condition = ConditionId.Fainted` when Pokemon is fainted

2. **ApogeeVGC/Sim/Player/PlayerRandom.cs**
   - Check `Condition == ConditionId.Fainted` in `IsPokemonFainted`
   - Check `Trapped` state before adding switch options
   - Provide Struggle fallback when no choices available

3. **ApogeeVGC/Sim/Core/DriverMode.cs**
   - Added `SingleBattleDebug` mode

4. **ApogeeVGC/Sim/Core/Driver.cs**
   - Added `RunSingleBattleDebug()` method

## Prevention Guidelines

### Pattern: Verify Union Type Extraction

When extracting values from union types:
1. Check all possible union variants
2. Provide explicit fallbacks for unhandled cases
3. Don't assume a failed pattern match means "default to X"

Example:
```csharp
// ? BAD: Assumes failed match = None
if (GetHealth().Secret is not SecretConditionId condition)
{
    condition = new SecretConditionId(ConditionId.None);
}

// ? GOOD: Explicitly check state first
ConditionId condition = Fainted
    ? ConditionId.Fainted
    : (GetHealth().Secret is SecretConditionId secretCondition
        ? secretCondition.Value
        : ConditionId.None);
```

### Pattern: Random Player Must Handle All Edge Cases

When implementing random player choice logic:
1. Check state constraints (trapped, fainted, disabled)
2. Provide fallbacks for "no valid choices" scenarios
3. Never throw on edge cases - battle engine will handle Struggle

### Pattern: Test with Fainted Pokemon

When testing battle mechanics:
1. Include scenarios with fainted Pokemon
2. Test switch requests with mixed fainted/active Pokemon
3. Verify request data correctly reflects Pokemon state

## Keywords

`endless loop`, `infinite loop`, `battle timeout`, `BattleTimeoutException`, `fainted`, `Condition`, `ConditionId.Fainted`, `IsPokemonFainted`, `PokemonSwitchRequestData`, `PlayerRandom`, `GetSwitchRequestData`, `trapped`, `disabled moves`, `Struggle`, `no available choices`, `single battle debug`, `seed reproduction`
