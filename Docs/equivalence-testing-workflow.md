# Equivalence Testing Workflow

This document describes the iterative process used to verify that the C# battle simulator produces byte-identical protocol output to Pokemon Showdown. It is intended as a handoff document for future sessions continuing this work.

## Goal

The C# sim must produce the exact same protocol output as Showdown for any given battle. This is verified by running thousands of random doubles battles through both engines with identical seeds and comparing the output line-by-line. The current target is **10,000 passing battles** using `gen9randomdoublesbattle` format.

## Architecture

### Components

- **Showdown** (`../pokemon-showdown/`) -- The reference TypeScript/JavaScript implementation. Must be built (`npm run build`) so `dist/sim/` exists.
- **C# Sim** (`ApogeeVGC/`) -- The C# port being verified.
- **Node.js scripts** (`Tools/EquivalenceTest/`) -- Generate Showdown battles and optionally trace PRNG/events.
- **Driver modes** (`ApogeeVGC/Sim/Core/Driver.cs`) -- Two modes: `EquivalenceBatchTest` (bulk) and `EquivalenceTest` (single debug).

### Key Files

| File | Purpose |
|------|---------|
| `Driver.EquivalenceBatch.cs` | Batch runner: generates fixtures via Node.js, replays in C#, compares protocol |
| `Driver.EquivalenceTest.cs` | Single-battle runner with detailed output for debugging a specific seed |
| `Driver.cs` | Mode selection; `EquivalenceTest` mode points to a specific fixture file |
| `Tools/EquivalenceTest/run_showdown_battle.js` | Generates a Showdown battle: takes seed, format, outputs fixture JSON + protocol log |
| `Tools/EquivalenceTest/run_showdown_traced.js` | Same as above but with PRNG call tracing to stderr |
| `Tools/EquivalenceTest/trace_rng.js` | Standalone PRNG trace script for detailed debugging |
| `Docs/bugfixes-equivalence/` | Documentation for every bug found and fixed (86 entries) |

### Fixture Format

The Node.js script produces a `.fixture.json` file containing:
```json
{
  "formatid": "gen9randomdoublesbattle",
  "seed": "1,2,3,4",
  "p1Team": [...],
  "p2Team": [...],
  "inputLog": [">start ...", ">player p1 ...", ">p1 move ...", ...],
  "prngFinalSeed": "a,b,c,d",
  "prngCallCount": 195
}
```

And a `.log` file with Showdown's raw protocol output (one line per protocol message).

### Seed Generation

The batch test generates deterministic seeds per test index `i`:
```
s1 = (i * 7 + 1) % 65536
s2 = (i * 13 + 2) % 65536
s3 = (i * 19 + 3) % 65536
s4 = (i * 31 + 4) % 65536
```

To find the batch index for a seed, solve: `i = (s1 - 1) / 7`. For example, seed `39775,...` → index `(39775-1)/7 = 5682`.

## The Iterative Debugging Cycle

### Step 1: Run the Batch Test

```bash
dotnet run --project ApogeeVGC -- --mode EquivalenceBatchTest
```

This runs `EquivalenceBatchNumTests` battles (currently 10,000) in parallel. Output shows PASS/FAIL per seed. Failed seeds are listed at the end with context about the first mismatching protocol line and PRNG call counts.

The batch count is set in `Driver.EquivalenceBatch.cs`:
```csharp
private const int EquivalenceBatchNumTests = 10000;
```

When all tests pass, increase this number and re-run to find more failures. The progression was: 50 → 200 → 500 → 2000 → 4000 → 10000.

### Step 2: Identify and Triage Failures

The batch output shows each failure with:
- The seed string (copy this for single-test debugging)
- Match count (e.g., "159/341 match")
- PRNG call counts (C# vs Showdown)
- PRNG final seed comparison (PRNG=OK or PRNG=DIFF)
- First mismatch context (3 lines before + the diverging line)

**Key diagnostics:**
- **PRNG=DIFF with different call counts** → C# is making extra or fewer PRNG calls (speedSort shuffle, randomFoe, accuracy check, etc.)
- **PRNG=OK but protocol differs** → A protocol formatting issue (missing tag, wrong message, etc.)
- **Early divergence (low match count)** → Likely a missing handler or wrong return value
- **Late divergence (high match count)** → Likely a subtle PRNG desync that cascades

### Step 3: Debug a Single Failure

1. **Calculate the batch index** from the seed: `index = (s1 - 1) / 7`

2. **Point the single-test mode at the fixture**: Edit `Driver.cs` line ~98:
   ```csharp
   case DriverMode.EquivalenceTest:
       RunEquivalenceTest(
           "Tools/EquivalenceTest/batch_temp/battle_XXXX.fixture.json",
           "Tools/EquivalenceTest/batch_temp/battle_XXXX.log");
       break;
   ```

3. **Run the single test**:
   ```bash
   dotnet run --project ApogeeVGC -- --mode EquivalenceTest
   ```
   This prints detailed output: every `eachEvent` call, PRNG calls with callers, and a full side-by-side protocol diff.

4. **Enable PRNG tracing** if needed: Set `TRACE_SEED` env var or set `battle.Prng.TraceEnabled = true` in `Driver.EquivalenceTest.cs`.

5. **Run Showdown with tracing** to compare PRNG call-by-call:
   ```bash
   node Tools/EquivalenceTest/run_showdown_traced.js batch_temp/battle_XXXX.fixture.json 2>sd_trace.txt
   ```

6. **Compare PRNG traces** side-by-side to find the exact call number where they diverge. The caller stack tells you what code path made the extra/missing call.

### Step 4: Diagnose Root Cause

The most common bug categories found:

1. **Missing event handlers** — A Showdown condition/ability has a handler (e.g., `onModifyMove`, `onDisableMove`) that the C# port omitted. This causes different handler counts in `speedSort`, producing different shuffle PRNG calls.

2. **JS falsy/truthy semantics** — JavaScript treats `0`, `null`, `undefined`, `false`, and `""` as falsy. C# union types must carefully preserve these distinctions. `FromInt(0)` is NOT the same as `FromFalse()`.

3. **Return value mapping** — Showdown handlers return `null`, `undefined`, `false`, numbers, or objects. The C# relay variable system (`RelayVar`, `BoolRelayVar`, `IntRelayVar`, `NullRelayVar`, `EmptyRelayVar`) must map these correctly through `CombineResults` and event propagation.

4. **PRNG-consuming code paths** — `speedSort` shuffles tied handlers, `getRandomTarget`/`randomFoe` resolves targets, and various checks consume PRNG. Any mismatch in handler counts, target resolution, or check ordering causes cascading divergence.

5. **Protocol formatting** — Missing `[from]` tags, wrong effect names, missing `[silent]` flags, wrong `[of]` attribution.

### Step 5: Fix and Verify

1. Make the code change.
2. Build: `dotnet build ApogeeVGC/ApogeeVGC.csproj -c Debug`
3. Verify the single failing seed: `dotnet run --project ApogeeVGC -- --mode EquivalenceTest`
4. Run the full batch to confirm no regressions: `dotnet run --project ApogeeVGC -- --mode EquivalenceBatchTest`
5. Commit with a descriptive message.

### Step 6: Document the Fix

Create a markdown file in `Docs/bugfixes-equivalence/` following this template:

```markdown
# Title

**Commit:** `<hash>`
**Date:** YYYY-MM-DD

## Problem
1-2 sentences.

## Root Cause
2-3 sentences.

## Fix
1-2 sentences.

## Files Changed
- `path/to/file` -- description

## Pattern
1 sentence categorizing the bug type.
```

Then add an entry to `Docs/bugfixes-equivalence/index.md`.

### Step 7: Increase Batch Size

When all tests pass at the current count, increase `EquivalenceBatchNumTests` in `Driver.EquivalenceBatch.cs` and repeat from Step 1.

## PRNG System

Both engines use an identical Gen5 RNG (Linear Congruential Generator with 4x16-bit state). Every `Random()`, `RandomChance()`, and `Sample()` call consumes exactly 1 `Gen5Rng.Next()` call. `Shuffle(start, end)` consumes `(end - start - 1)` calls.

Key PRNG consumers:
- **speedSort** — Selection sort with Fisher-Yates shuffle for tied elements. Called inside `RunEvent` and `eachEvent` to order handlers by priority/speed.
- **randomFoe** — Target resolution for moves. Called via `getTarget → getRandomTarget → Side.RandomFoe()`.
- **Accuracy checks** — `randomChance(accuracy, 100)` for each non-true accuracy move.
- **Damage roll** — `random(16)` for the 85-100% damage randomizer.
- **Critical hit** — `randomChance(critRatio, 24)`.
- **Secondary effects** — `random(100)` chance checks.

When PRNG traces diverge, the first mismatching call number and its caller identify the root cause.

## Current State

- **Branch:** `equivalence-test-fixes`
- **Batch size:** 10,000
- **Pass rate:** 10,000/10,000
- **Documented bugs:** 86 (in `Docs/bugfixes-equivalence/`)
- **Total commits on branch:** 106

## Continuing This Work

To extend coverage:

1. Increase `EquivalenceBatchNumTests` (e.g., to 20,000 or 50,000).
2. Run the batch test and fix any new failures using the cycle above.
3. Document each fix in `Docs/bugfixes-equivalence/`.
4. Commit frequently — one commit per fix.
5. Revert `Driver.cs` back to the default fixture path after debugging (don't leave it pointing at a specific battle).
6. Clean up any temporary trace files before committing.

The `batch_temp/` directory is gitignored and auto-cleaned on a fully passing batch run. Fixture files are only preserved when failures exist (for debugging).
