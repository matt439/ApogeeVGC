# Infinite Battle: CheckWin Skip & Random Player Struggle Fix

## Problem Summary

Battles running in parallel evaluation (`Parallel.For` with 20,000 battles) intermittently exceeded the 5000-turn limit and threw `BattleTurnLimitException`. Two separate bugs were identified, both causing battles to never end.

**Symptoms**:
- `BattleTurnLimitException` thrown at turn 5000 during `RunRndVsRndVgcRegIEvaluation`
- Diagnostic snapshots showed: all Pokemon on one side fainted with `PokemonLeft=0` but `Ended=False`
- Diagnostic snapshots showed: length-1 cycles detected (identical battle state repeating every turn)
- Fainted Pokemon had `HP=MaxHp` (this is normal — `ClearVolatile` → `SetSpecie` resets HP before `Fainted=true` is set)

**Error Output**:
```
ERROR: Battle exceeded turn limit (likely infinite loop!)
-----------------------------------------------------------
Turn: 5000 (Max: 5000)
Team 1 Seed:   82822
Team 2 Seed:   96392
Player 1 Seed: 40848
Player 2 Seed: 30322
Battle Seed:   38381
```

## Root Cause 1: CheckWin Skipped After Target Faints

### Location
`BattleActions.HitSteps.cs` line 949 (not modified — fix applied defensively in `Battle.Fainting.cs`)

### Problem
In the multi-hit move loop (`HitStepMoveHitLoop`), `FaintMessages` is called with:

```csharp
Battle.FaintMessages(false, false, pokemon.Hp <= 0);
```

Here `pokemon` is the **attacker**. The third parameter `checkWin` is only `true` when the attacker's HP drops to 0. If the attacker's move KOs the last opposing Pokemon but the attacker itself survives, `checkWin` is `false`. The faint queue is drained — `PokemonLeft` is decremented to 0 — but `CheckWin()` is never called. The battle enters a state where one side has no Pokemon left but the battle doesn't end.

### Fix
Added a safety net in `Battle.Fainting.cs` `FaintMessages()` method, after the faint queue processing loop:

```csharp
// Safety net: if any side has no Pokemon left, always check for a winner
// regardless of the checkWin parameter. This guards against callers that pass
// checkWin=false based only on the attacker's HP, missing target-side faints
// (e.g., HitStepMoveHitLoop passing checkWin = attacker.Hp <= 0).
if (!checkWin && Sides.Any(side => side.PokemonLeft <= 0))
{
    checkWin = true;
}
```

This ensures `CheckWin` is always called when any side has lost all Pokemon, regardless of what the caller passed for `checkWin`.

## Root Cause 2: Random Player Infinite Switching When Out of PP

### Location
`PlayerRandom.cs` `GetRandomMoveChoice()` method

### Problem
When all of a Pokemon's moves reach 0 PP, they are all marked as disabled in the move request. The random player builds its available choices by collecting non-disabled moves plus a switch option. When all moves are disabled:

1. No move options are added to `availableChoices` (all disabled)
2. If bench Pokemon are available, the switch option IS added
3. The random player's only choice is "switch" — it never selects a move
4. Struggle only fires when a player **chooses** a move and has no PP — but the player never chooses a move

This creates an infinite loop: both sides endlessly swap their 4 Pokemon (2 active, 2 bench) back and forth. No moves are used, no Struggle fires, no recoil damage occurs, no PP is consumed, no game progress happens.

**Cycle**: Both active Pokemon switch out → bench Pokemon come in → they also have 0 PP → they switch out → original Pokemon come back → repeat forever (length-1 cycle in diagnostics).

### Fix
Added a Struggle fallback option in `PlayerRandom.cs` before the switch option is added:

```csharp
// When all moves are disabled (e.g., 0 PP), add a "use move" option so the
// battle engine converts it to Struggle. Without this, the random player would
// always switch when out of PP, creating infinite switching loops where no
// damage or recoil ever occurs.
bool allMovesDisabled = !availableChoices.Any(c => c.isMove);
if (allMovesDisabled && pokemonRequest.Moves.Count > 0)
{
    availableChoices.Add((true, 0, false));
}
```

Now when all moves are disabled, the random player can choose between switching (if available) and "attacking" (which the battle engine converts to Struggle with 25% max HP recoil). This ensures battles make progress and eventually end.

## Modified Files

| File | Change |
|------|--------|
| `Sim/BattleClasses/Battle.Fainting.cs` | Added `checkWin` safety net after faint queue loop |
| `Sim/Player/PlayerRandom.cs` | Added Struggle fallback when all moves are disabled |

## Investigation Notes

### Why the `List<PokemonType>` → `PokemonType[]` optimization was not the cause
The original suspicion was that converting `List<PokemonType>` to `PokemonType[]` across the codebase introduced shared mutable state between concurrent battles. Investigation confirmed this is not the case:
- `GetTypes()` always returns a new `PokemonType[]` array
- `SetType()` makes defensive copies: `Types = [..types]`
- All library objects (`Species`, `Move`, `Ability`, `Item`) are immutable with proper lazy-init caching via `Volatile.Read`/`Interlocked.CompareExchange`
- Each battle has completely independent `Pokemon`, `Side`, `Battle` instances
- The `ActiveMove` template cache on shared `Move` objects is safe: all external callers use `ToActiveMove()` (clone via `with { }`), never `AsActiveMove()` (shared template)

The optimization merely changed execution timing (reduced GC pressure), exposing these latent game logic bugs more frequently during parallel evaluation.

### Why fainted Pokemon have HP=MaxHp
This is **normal behavior**, not corruption. The faint processing sequence in `FaintMessages` is:
1. `ClearVolatile(false)` → calls `SetSpecie(BaseSpecies, ...)` → resets `Hp = MaxHp`
2. `Fainted = true` is set after `ClearVolatile` returns

So fainted Pokemon always have full HP in their final state.

### The Endless Battle Clause doesn't cover these scenarios
The existing `MaybeTriggerEndlessBattleClause` only detects restorative berry cycling (Harvest/Pickup + Leppa Berry). It does not detect:
- The CheckWin skip bug (a code bug, not a game mechanic issue)
- The random player switching loop (a player behavior issue, not detectable by staleness tracking)

**Keywords**: `BattleTurnLimitException`, `infinite loop`, `CheckWin`, `FaintMessages`, `checkWin parameter`, `Struggle`, `PlayerRandom`, `switching loop`, `PP depletion`, `disabled moves`, `HitStepMoveHitLoop`, `parallel battles`, `race condition`, `PokemonType array optimization`, `ClearVolatile`, `faint processing`
