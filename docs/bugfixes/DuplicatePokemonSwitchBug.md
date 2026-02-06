# [RESOLVED] Duplicate Pokemon Bug - Roster Position Conflation

## Status: ? FIXED

**Last Updated**: 2025-01-20  
**Priority**: High (Resolved)  
**Systems Affected**: BattleActions.Switch, Pokemon.Core, Side.Conditions, doubles battles  
**Git Branch**: `debug-vgc-reg-i`

---

## Summary

**Problem**: Duplicate Pokemon appeared in `Side.Pokemon` roster during doubles battles, causing "DUPLICATE POKEMON DETECTED" errors that prevented battles from completing.

**Root Cause**: `BattleActions.Switch.SwitchIn()` was conflating Active slot indices (0-1 for doubles) with roster positions (0-3 for VGC), causing roster corruption during switches.

**Solution**: Separated Active slot tracking from roster positions by:
1. Adding `Pokemon.GetActiveSlotIndex()` helper method to find which Active slot (0-1) a Pokemon occupies
2. Removing roster modification from `SwitchIn()` - Pokemon roster stays fixed after team preview
3. Updating all slot condition code to use Active slot indices instead of roster Position

**Result**: 
- **Before**: 0/1000 battles successful, duplicate Pokemon error on every battle
- **After**: 185/1000 battles successful (18.5%), zero duplicate Pokemon errors
- Remaining failures are **unrelated pre-existing bugs** (win condition detection, move targeting)

---

## Root Cause: Position Conflation

The bug occurred because `BattleActions.Switch.SwitchIn()` treated Active slot indices (0-1) as if they were roster positions (0-3), overwriting the roster during switches.

### The Buggy Code (Removed)

```csharp
// Update Pokemon states and positions
if (oldActive != null)
{
    oldActive.IsActive = false;
    oldActive.Position = pokemon.Position;      // Swap positions
    side.Pokemon[oldActive.Position] = oldActive; // Modify roster ?
}

// Swap positions in the side's Pokemon list
pokemon.Position = pos;  // pos = Active slot (0 or 1)
side.Pokemon[pokemon.Position] = pokemon;  // OVERWRITES roster! ?
```

### What Went Wrong

**After team preview**, `Side.Pokemon` contains 4 Pokemon at roster indices 0-3:
```
Side.Pokemon (roster):
  [0] Poliwrath   (Position=0)
  [1] Regice      (Position=1)
  [2] Tyrogue     (Position=2)
  [3] Yungoos     (Position=3)
```

**During a switch** of Yungoos into Active slot 0:
1. `pos = 0` (Active slot to fill)
2. `pokemon.Position = 0` (sets Yungoos.Position to 0)
3. `side.Pokemon[0] = Yungoos` (**overwrites Poliwrath!**)

**Result - Corrupted Roster**:
```
Side.Pokemon:
  [0] Yungoos  <- DUPLICATE! (overwritten)
  [1] Regice
  [2] Tyrogue
  [3] Yungoos  <- DUPLICATE! (original)
```

Now `Side.Pokemon` has the same Pokemon (Yungoos) at two indices, creating the "DUPLICATE POKEMON DETECTED" error.

---

## The Fix: Separate Roster from Active Slots

### Key Principle

**Roster positions are fixed after team preview** - only `Side.Active` references should change during switches.

- **`Side.Pokemon`** = Battle roster (4 Pokemon, indices 0-3) - **immutable order**
- **`Side.Active`** = Active slots (2 slots, indices 0-1) - **references change during switches**
- **`Pokemon.Position`** = Roster index in `Side.Pokemon` - **stays constant**
- **`Pokemon.GetActiveSlotIndex()`** = Active slot index in `Side.Active` - **changes when switching**

### Primary Fix: BattleActions.Switch.cs

**Removed all roster modification**:

```csharp
// AFTER (FIXED):
// Update Pokemon states (but NOT roster positions)
if (oldActive != null)
{
    oldActive.IsActive = false;
    oldActive.IsStarted = false;
    oldActive.UsedItemThisTurn = false;
    oldActive.StatsRaisedThisTurn = false;
    oldActive.StatsLoweredThisTurn = false;
    // ? Do NOT modify oldActive.Position or side.Pokemon
    
    // Clear status if fainted
    if (oldActive.Fainted)
    {
        oldActive.Status = ConditionId.None;
    }
    
    // Gen 4 and earlier: transfer last item
    if (Battle.Gen <= 4)
    {
        pokemon.LastItem = oldActive.LastItem;
        oldActive.LastItem = ItemId.None;
    }
}

// ? Do NOT modify pokemon.Position or side.Pokemon
// Only update the Active slot reference

// Activate the new Pokemon
pokemon.IsActive = true;
side.Active[pos] = pokemon;  // ? Only change Active reference
pokemon.ActiveTurns = 0;
pokemon.ActiveMoveActions = 0;
```

**What was removed**:
- ? `oldActive.Position = pokemon.Position`
- ? `side.Pokemon[oldActive.Position] = oldActive`
- ? `pokemon.Position = pos`
- ? `side.Pokemon[pokemon.Position] = pokemon`

**What changed**:
- ? Only `side.Active[pos]` is updated
- ? Roster (`side.Pokemon`) remains untouched
- ? `Pokemon.Position` stays at roster index

---

## Supporting Changes

### 1. Pokemon.Core.cs - GetActiveSlotIndex() Helper

Added method to find which Active slot (0-1) a Pokemon currently occupies:

```csharp
/// <summary>
/// Gets the Active slot index (0-based) for this Pokemon, or -1 if not active.
/// In doubles, this returns 0 or 1 for active Pokemon.
/// Note: This is different from Position, which is the roster index in Side.Pokemon.
/// </summary>
public int GetActiveSlotIndex()
{
    if (!IsActive) return -1;
    return Side.Active.IndexOf(this);
}
```

**Why**: Because `Pokemon.Position` is now roster index (0-3), we need a separate way to find Active slot (0-1).

### 2. Side.Conditions.cs - Use Active Slot Indices

Updated slot condition methods to use `GetActiveSlotIndex()` instead of `Position`:

```csharp
// AddSlotCondition, GetSlotCondition, RemoveSlotCondition:
int targetSlot = target switch
{
    PokemonPokemonIntUnion pokemon => pokemon.Pokemon.GetActiveSlotIndex(), // ?
    IntPokemonIntUnion intValue => intValue.Value,
    _ => throw new InvalidOperationException("Invalid target type"),
};
```

**Why**: `SlotConditions` has `Active.Count` entries (2 for doubles), so must be indexed by Active slot (0-1), not roster Position (0-3).

### 3. Side.Choices.cs - Check IsActive Instead of Position

```csharp
// BEFORE:
else if (slot < Active.Count && pokemon != null && ...)

// AFTER:
else if (Pokemon[slot].IsActive && pokemon != null && ...)
```

**Why**: After removing position swapping, `slot < Active.Count` no longer indicates if a Pokemon is active.

### 4. Other Files Updated

Similar changes to use `GetActiveSlotIndex()` for slot conditions:
- `Battle.EventHandlers.cs` - Event handler slot condition lookup
- `Battle.Requests.cs` - Revival Blessing checks
- `Battle.Lifecycle.cs` - Revival Blessing in switch logic
- `Pokemon.Requests.cs` - Reviving status check
- `MovesDEF.cs` - Doom Desire and Future Sight slot conditions

And fixes for other position-related issues:
- `Battle.FieldControl.cs` - `SwapPosition()` only modifies Active array
- `Pokemon.Positioning.cs` - `GetAtLoc()` bounds checking for doubles

---

## Files Modified (12 Total)

1. ? **ApogeeVGC/Sim/BattleClasses/BattleActions.Switch.cs** - Primary fix
2. **ApogeeVGC/Sim/PokemonClasses/Pokemon.Core.cs** - GetActiveSlotIndex() helper
3. **ApogeeVGC/Sim/SideClasses/Side.Conditions.cs** - Slot condition methods
4. **ApogeeVGC/Sim/SideClasses/Side.Choices.cs** - Active Pokemon validation
5. **ApogeeVGC/Sim/BattleClasses/Battle.EventHandlers.cs** - Event handler slot conditions
6. **ApogeeVGC/Sim/BattleClasses/Battle.FieldControl.cs** - SwapPosition()
7. **ApogeeVGC/Sim/BattleClasses/Battle.Requests.cs** - Revival Blessing checks
8. **ApogeeVGC/Sim/BattleClasses/Battle.Lifecycle.cs** - Revival Blessing in switches
9. **ApogeeVGC/Sim/PokemonClasses/Pokemon.Requests.cs** - Reviving status
10. **ApogeeVGC/Sim/PokemonClasses/Pokemon.Positioning.cs** - GetAtLoc() bounds
11. **ApogeeVGC/Data/Moves/MovesDEF.cs** - Doom Desire and Future Sight
12. **Logging Cleanup**: Battle.Lifecycle.cs, PlayerRandom.cs, RandomTeamGenerator.cs, Side.Choices.cs

---

## Diagnostic Process

### How We Found It

1. **Added hash code logging** to detect if duplicates were same object or different objects:
```
Copy 1: Hash=598734685, Ident=p1: Yungoos
Copy 2: Hash=1086528457, Ident=p1: Yungoos
Different objects with same species ?
```

2. **Conclusion**: Two different Pokemon objects existed with same species ? roster corruption, not double-add

3. **Traced SwitchIn()**: Found it was using Active slot index to modify roster

4. **Identified pervasive Position usage**: Found many places using `Pokemon.Position` to index into `SlotConditions`

---

## Test Results

### Before Fix
```
Successful Battles: 0/1000
Failed Battles: 1000
Error: DUPLICATE POKEMON DETECTED on every battle
```

### After Fix
```
Successful Battles: 185/1000 (18.5%)
Failed Battles: 815
- Zero duplicate Pokemon errors ?
- Remaining failures are unrelated bugs:
  - Win condition detection (470 battles)
  - Move targeting issues
  - Turn limit exceeded (21 battles)
```

### Win Rate (Among Successful Battles)
```
Player 1 Wins: 98 (52.9%)
Player 2 Wins: 87 (47.1%)
Balanced ?
```

---

## Related Bugs (Previously Fixed)

These bugs were fixed during the investigation leading up to the duplicate Pokemon bug:

1. ? **ForceSwitch Table Generation** - Null Active slots marked as needing switches
2. ? **SwitchIn Request Validation** - Null slots recognized
3. ? **ForcedSwitchesLeft Calculation** - Null slots counted
4. ? **Null Active Slots in ChooseSwitch()** - Null handling added
5. ? **Fainted Pokemon Detection** - Proper condition checking
6. ? **Team Preview Pokemon Count** - Respects MaxChosenTeamSize (4 for VGC)
7. ? **Duplicate Pokemon in Switches** - Tracks used Pokemon
8. ? **GenderRatio Constructor** - Added missing cases

See earlier sections of this file for full details on these bugs.

---

## Remaining Unrelated Bugs

After fixing the duplicate Pokemon bug, **815/1000 battles still fail** due to different pre-existing bugs:

### Most Common (470/815): Win Condition Detection
**Error**: "Player has no Pokemon available to switch but battle is requesting switches"

**Example**:
```
Pokemon status:
  [0] Groudon: Fainted
  [1] Marshtomp: Fainted
  [2] Rowlet: Active=True, NOT fainted ?
  [3] Vibrava: Fainted
Switches needed: 1 ?
```

Player has only 1 alive Pokemon (already active) but battle requests a switch. Should end the game instead.

### Other Failures
- **Invalid target errors** - Move targeting validation
- **Turn limit exceeded** (21 battles) - Stalemate detection
- **Not enough Pokemon to switch** - Related to win condition

**These require separate investigation.**

---

## Reproduction Seeds (Original Bug)

To verify the fix works, use these seeds that originally triggered the duplicate bug:

```
Team 1 Seed:   56202
Team 2 Seed:   69772  
Player 1 Seed: 14228
Player 2 Seed: 3702
Battle Seed:   11761
```

1. Set `DriverMode.DebugSingleBattle` in `Program.cs`
2. Set above seeds in `Driver.cs` method `RunDebugSingleBattle()`
3. Run battle - should complete without duplicate Pokemon error ?

---

## Key Takeaways

1. **Position had dual meaning** - Both roster index AND Active slot index
2. **Arrays must use correct indices** - `SlotConditions[i]` needs Active slot, not roster position
3. **Roster should be immutable** - After team preview, roster order doesn't change
4. **Hash codes reveal object identity** - Diagnostic logging was critical
5. **Small change, big impact** - Removing 4 lines fixed the entire bug

---

## Conclusion

The duplicate Pokemon bug was caused by conflating Active slot indices with roster positions. The fix maintains clear separation:
- **Roster** (`Side.Pokemon`) = Fixed list of battle Pokemon (set at team preview)
- **Active** (`Side.Active`) = Current Pokemon on field (changes during switches)
- **Position** = Roster index (constant)
- **GetActiveSlotIndex()** = Active slot (dynamic)

**Status**: ? Bug resolved completely
- 185/1000 battles now succeed (up from 0)
- Zero duplicate Pokemon errors
- Remaining failures are unrelated bugs

---

## Git Information
- **Branch**: `debug-vgc-reg-i`
- **Repository**: https://github.com/matt439/ApogeeVGC
- **Suggested Commit**: "Fix duplicate Pokemon bug: separate Active slot from roster Position"
