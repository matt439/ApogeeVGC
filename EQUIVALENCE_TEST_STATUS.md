# Equivalence Test Status Report

**Date**: 2026-03-11
**Branch**: `equivalence-test-fixes`
**Changed files**: ~259 code/data files (plus ~168 batch fixture files)

---

## Goal

Verify that the C# battle simulator produces **identical protocol output** to the TypeScript Pokemon Showdown simulator for a given set of RNG seeds and player inputs. This is critical for thesis credibility — the C# sim must faithfully reproduce Showdown's battle logic before it can be trusted for AI training data or live inference.

Both simulators are initialized with the same Gen5RNG seed (4 x 16-bit values), the same teams, and the same player choices. The test filters out cosmetic protocol lines (timestamps, animations, metadata) and compares game-state-affecting lines (damage, moves, statuses, switches, items, abilities) line by line.

---

## What Exists

### Test Infrastructure
- **Single test** (`Driver.EquivalenceTest.cs`): Replays one Showdown fixture through C# sim, compares protocol output line-by-line with context on mismatch.
- **Batch test** (`Driver.EquivalenceBatch.cs`): Generates 50 random Showdown battles via Node.js, replays each through C#, reports pass/fail summary.
- **Node.js scripts** (`Tools/EquivalenceTest/`): `run_showdown_battle.js` generates fixtures; `run_showdown_traced.js` and `trace_rng.js` add RNG call tracing for debugging divergences.
- **RNG tracing**: Both C# (`Gen5Rng.CallCount`, `TraceEnabled`) and Showdown (monkey-patched) can output numbered RNG calls for side-by-side comparison.

### Entry Points
- `Program.cs` defaults to `EquivalenceBatchTest` mode
- `--mode EquivalenceTest` for single fixture debugging

---

## Current Results

**Pass rate: ~14% (7/50 batch tests)**

Passing seeds: 22, 29, 50, 71, 113, 134, 148, 169

### Failure Categories

| Category | Description | Frequency |
|---|---|---|
| **Damage mismatches** | HP values differ by 7-25 HP after an attack | Most common |
| **Move target mismatches** | Spread moves (e.g. Blizzard) target wrong Pokemon | Several |
| **Event sequence divergence** | Different event ordering or missing events | Several |
| **Status effect mismatches** | Status conditions appear at wrong times | Some |
| **Item/ability event mismatches** | Different item/ability trigger sequencing | Some |
| **Early termination** | C# battle ends earlier than Showdown | Some |

### RNG Divergence
In a traced battle (seed `8,15,22,35`), C# consumed 44 RNG calls while Showdown consumed 109 for the same battle. This suggests the simulators diverge early in some battles and then cascade — once one RNG call is consumed differently, all subsequent randomness is wrong.

---

## What Has Been Changed (Code, not fixtures)

### Bug Fixes to Sim Logic (~75 files)

1. **Damage attribution** (`Battle.Combat.cs`): Separated `effectCondition` (game logic) from `effectForLogging` (preserves original IEffect like Move for item handlers like Focus Sash).

2. **Event return values** (`OnTryHitEventInfo.cs`): Empty union now returns `NullRelayVar()` (NOT_FAIL) instead of `BoolRelayVar.False`; fixed parameter order to match Showdown API.

3. **Priority type** (`Pokemon.Core.cs`): Changed `Priority` from `int` to `double` (Showdown uses fractional priorities like 0.5).

4. **Display name / details** (`Pokemon.Core.cs`, `Pokemon.Helpers.cs`): Added `DisplayName` field, fixed `FullName` → `Fullname` casing, added `IllusionLevelMod` rule handling for disguise level display.

5. **Ability fixes** (7 files in `Data/Abilities/`): Removed non-functional OnSwitchIn handlers for "As One" abilities; fixed return statements in Compound Eyes; various event handling fixes.

6. **Condition fixes** (7 files in `Data/Conditions/`): Various condition definition corrections.

7. **Item fixes** (3 files in `Data/Items/`): Item handler corrections.

8. **Move fixes** (7 files in `Data/Moves/`): Move effect corrections.

9. **Species data** (14 files in `Data/SpeciesData/`): Pokemon data corrections.

10. **Action classes** (8 files in `Sim/Actions/`): Variable naming and logic fixes.

11. **Battle classes** (12 files in `Sim/BattleClasses/`): Logging, sorting, modifier, validation fixes.

12. **Event system** (~60 files in `Sim/Events/`): Event handler parameter/signature fixes across many event info types.

13. **RNG instrumentation** (`Gen5Rng.cs`, `Prng.cs`): Added call counting and tracing.

14. **Other sim classes**: Fixes in `Move.Core.cs`, `Side.Conditions.cs`, `Species.cs`, `PokemonSet.cs`, `SparseBoostsTable.cs`, `BoolIntUndefinedUnion.cs`, etc.

### New Infrastructure
- `Driver.EquivalenceBatch.cs` — batch test runner
- `run_showdown_traced.js`, `trace_rng.js` — RNG tracing scripts
- Debug output files (`all_debug.txt`, `dmg_debug.txt`, `residual_debug.txt`)

---

## What Still Needs to Be Done

### Immediate Priority: Find Root Causes of Remaining Divergences

The 86% failure rate suggests there are still **fundamental issues**, not just edge cases. The most productive approach is likely:

1. **Isolate the simplest failing case** — find the battle with the earliest first mismatch (fewest turns before divergence) and debug it step by step.

2. **Fix RNG consumption parity** — the fact that C# and Showdown consume different numbers of RNG calls is the clearest signal. Each place the C# sim calls (or fails to call) the RNG differently from Showdown is a bug. The RNG tracing infrastructure already exists for this.

3. **Damage calculation audit** — damage mismatches are the most common failure. Systematically compare the modifier chain (base power, STAB, type effectiveness, spread penalty, random factor, items, abilities) for a specific failing attack.

4. **Targeting logic audit** — spread move targeting differences suggest the target resolution code differs from Showdown's.

### Categories of Likely Remaining Bugs

- **Missing or extra RNG calls**: Any place C# skips a random call Showdown makes (or vice versa) will cascade into all subsequent randomness being wrong.
- **Modifier ordering/rounding**: Showdown applies damage modifiers in a specific order with specific rounding. Any deviation compounds.
- **Edge case ability/item/move interactions**: The 7 passing seeds may just be simple battles; complex interactions expose bugs.
- **Event handler registration/ordering**: If event listeners fire in a different order, side effects happen differently.

### Longer Term

- Once pass rate is high (>95%), increase batch size to 200+ to catch rare edge cases.
- Consider adding a "first divergence point" diagnostic that stops comparison at the exact RNG call or protocol line where things first go wrong.
- Archive a set of passing fixtures as a regression test suite.

---

## File Inventory

| Location | Count | Contents |
|---|---|---|
| `ApogeeVGC/Data/` | ~38 files | Ability, condition, item, move, species data fixes |
| `ApogeeVGC/Sim/Actions/` | 8 files | Action class fixes |
| `ApogeeVGC/Sim/BattleClasses/` | 12 files | Core battle logic fixes |
| `ApogeeVGC/Sim/Events/` | ~60 files | Event handler signature fixes |
| `ApogeeVGC/Sim/PokemonClasses/` | 6 files | Pokemon class fixes |
| `ApogeeVGC/Sim/Core/` | 3 files | Driver and test infrastructure |
| `ApogeeVGC/Sim/` (other) | ~10 files | Moves, conditions, species, stats, utils |
| `Tools/EquivalenceTest/` | ~110 files | Fixtures, scripts, traces, debug output |
| Root | 2 files | Program.cs, .gitignore |

---

## How to Run

```bash
# Batch test (50 random battles)
dotnet run --project ApogeeVGC -- --mode EquivalenceBatchTest

# Single fixture debug
dotnet run --project ApogeeVGC -- --mode EquivalenceTest
# (uses hardcoded paths in Driver.EquivalenceTest.cs)
```

Requires Node.js and a built `pokemon-showdown` repo at `../../pokemon-showdown` or `../../../pokemon-showdown` relative to the solution root.
