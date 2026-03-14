# Equivalence Testing Workflow

This document describes the iterative process used to verify that the C# battle simulator produces byte-identical protocol output to Pokemon Showdown. It is intended as a handoff document for future sessions continuing this work.

## Goal

The C# sim must produce the exact same protocol output as Showdown for any given battle. This is verified by running random battles through both engines with identical seeds and comparing the output line-by-line. The current milestone is **1,000,000 passing battles** using `gen9randomdoublesbattle` format.

## Architecture

### Components

- **Showdown** (`../pokemon-showdown/`) -- The reference TypeScript/JavaScript implementation. Must be built (`npm run build`) so `dist/sim/` exists.
- **C# Sim** (`ApogeeVGC/`) -- The C# port being verified.
- **Node.js scripts** (`Tools/EquivalenceTest/`) -- Generate Showdown battles.
- **Driver modes** (`ApogeeVGC/Sim/Core/Driver.cs`) -- Two modes: `EquivalenceBatchTest` (bulk) and `EquivalenceTest` (single debug).

### Key Files

| File | Purpose |
|------|---------|
| `Driver.EquivalenceBatch.cs` | Batch runner: reads cached fixtures, replays in C#, compares protocol |
| `Driver.EquivalenceTest.cs` | Single-battle runner with detailed output for debugging a specific seed |
| `Driver.cs` | Mode selection and `--format` passthrough |
| `Program.cs` | CLI argument parsing (`--mode`, `--format`) |
| `Tools/EquivalenceTest/generate_batch_cache.js` | Bulk fixture generator: runs all Showdown battles in a single Node.js process |
| `Tools/EquivalenceTest/run_showdown_battle.js` | Generates a single Showdown battle: takes seed, format, outputs fixture JSON + protocol log |
| `Docs/bugfixes-equivalence/` | Documentation for every bug found and fixed (98 entries) |
| `Docs/bugfixes-equivalence/index.md` | Index of all documented bugs -- **check this first** before debugging a new failure |

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

To find the batch index from a seed: `index = (s1 - 1) / 7`. For example, seed `39775,...` -> index `(39775-1)/7 = 5682`.

## Running the Batch Test

### Basic Usage

```bash
dotnet run --project ApogeeVGC --configuration Release
```

This runs `EquivalenceBatchNumTests` battles (currently 1,000,000) using the default format (`gen9randomdoublesbattle`).

### Specifying a Format

```bash
dotnet run --project ApogeeVGC --configuration Release -- --format gen9vgc2024regh
```

Each format gets its own cache directory under `batch_cache/<format>/`. This means you can test multiple formats without regenerating fixtures.

### Batch Size

The batch count is set in `Driver.EquivalenceBatch.cs`:
```csharp
private const int EquivalenceBatchNumTests = 1000000;
```

## Showdown Fixture Cache

### Why Caching?

The Showdown fixture generation is the slowest part. Spawning one Node.js process per battle incurs ~200ms startup overhead each. The cache eliminates this by generating fixtures once, then reusing them across runs.

### Cache Directory Structure

```
Tools/EquivalenceTest/batch_cache/
  gen9randomdoublesbattle/
    showdown_version.txt          # Git commit ID of the Showdown checkout used to generate these fixtures
    battle_000000.fixture.json
    battle_000000.log
    battle_000001.fixture.json
    battle_000001.log
    ...
  gen9vgc2024regh/                # Another format would go here
    showdown_version.txt
    ...
```

The `batch_cache/` directory is gitignored. The C# batch test reads directly from this cache, falling back to per-process Node.js generation only for cache misses.

### Showdown Version Tracking

Each format's cache directory contains a `showdown_version.txt` file recording the git commit ID of the Showdown checkout that generated the fixtures. On each batch run, the current Showdown commit is compared against this file:

- **Match**: Proceeds normally.
- **Mismatch**: Prints a warning. Cached fixtures may be stale and should be regenerated.
- **First run**: Records the current commit automatically.

### Generating the Cache

The C# batch runner generates fixtures on cache miss automatically. Alternatively, use the bulk generator for faster pre-generation:

```bash
cd Tools/EquivalenceTest
node generate_batch_cache.js --count 1000000 --outdir batch_cache/gen9randomdoublesbattle --concurrency 64
```

Options:
- `--count N` -- Number of battles to generate (default: 100000)
- `--format <id>` -- Battle format (default: gen9randomdoublesbattle)
- `--outdir <dir>` -- Cache directory
- `--start N` -- Resume from index N, skipping already-cached battles (default: 0)
- `--concurrency N` -- Number of concurrent battles in-process (default: 64)

The script is **resumable**: if interrupted, re-run with the same arguments and it will skip battles that already have both `.fixture.json` and `.log` files.

### Cache Performance

- **Without cache**: ~0.5s/test (Node.js process spawn overhead)
- **With cache**: ~0.01s/test (file read only, no Node.js)

### When to Regenerate the Cache

The cache stores **Showdown's** output, which is deterministic for a given seed. You only need to regenerate if:
- The Showdown build is updated (the version check will warn you)
- You change the seed generation formula in the C# batch test
- The cache is deleted or corrupted

To regenerate, delete the format's cache subdirectory and re-run.

## The Iterative Debugging Cycle

### Step 1: Run the Batch Test

```bash
dotnet run --project ApogeeVGC --configuration Release
```

Output shows PASS/FAIL per seed. Failed seeds are listed at the end with context about the first mismatching protocol line and PRNG call counts.

### Step 2: Identify and Triage Failures

**Before debugging, check `Docs/bugfixes-equivalence/index.md`** to see if a similar bug has already been diagnosed and fixed. Many failures share root causes (e.g., missing event handlers, JS falsy/truthy mismatches).

The batch output shows each failure with:
- The seed string (copy this for single-test debugging)
- Match count (e.g., "159/341 match")
- PRNG call counts (C# vs Showdown)
- PRNG final seed comparison (PRNG=OK or PRNG=DIFF)
- First mismatch context (3 lines before + the diverging line)

**Key diagnostics:**
- **PRNG=DIFF with different call counts** -> C# is making extra or fewer PRNG calls (speedSort shuffle, randomFoe, accuracy check, etc.)
- **PRNG=OK but protocol differs** -> A protocol formatting issue (missing tag, wrong message, etc.)
- **Early divergence (low match count)** -> Likely a missing handler or wrong return value
- **Late divergence (high match count)** -> Likely a subtle PRNG desync that cascades

### Step 3: Debug a Single Failure

1. **Calculate the batch index** from the seed: `index = (s1 - 1) / 7`

2. **Point the single-test mode at the cached fixture**: Edit `Driver.cs` line ~98:
   ```csharp
   case DriverMode.EquivalenceTest:
       RunEquivalenceTest(
           "Tools/EquivalenceTest/batch_cache/gen9randomdoublesbattle/battle_XXXXXX.fixture.json",
           "Tools/EquivalenceTest/batch_cache/gen9randomdoublesbattle/battle_XXXXXX.log");
       break;
   ```

3. **Run the single test**:
   ```bash
   dotnet run --project ApogeeVGC -- --mode EquivalenceTest
   ```
   This prints detailed output: every `eachEvent` call, PRNG calls with callers, and a full side-by-side protocol diff.

4. **Enable PRNG tracing** if needed: Set `TRACE_SEED` env var or set `battle.Prng.TraceEnabled = true` in `Driver.EquivalenceTest.cs`.

### Step 4: Diagnose Root Cause

The most common bug categories found:

1. **Missing event handlers** -- A Showdown condition/ability has a handler (e.g., `onModifyMove`, `onDisableMove`) that the C# port omitted. This causes different handler counts in `speedSort`, producing different shuffle PRNG calls.

2. **JS falsy/truthy semantics** -- JavaScript treats `0`, `null`, `undefined`, `false`, and `""` as falsy. C# union types must carefully preserve these distinctions. `FromInt(0)` is NOT the same as `FromFalse()`.

3. **Return value mapping** -- Showdown handlers return `null`, `undefined`, `false`, numbers, or objects. The C# relay variable system (`RelayVar`, `BoolRelayVar`, `IntRelayVar`, `NullRelayVar`, `EmptyRelayVar`) must map these correctly through `CombineResults` and event propagation.

4. **PRNG-consuming code paths** -- `speedSort` shuffles tied handlers, `getRandomTarget`/`randomFoe` resolves targets, and various checks consume PRNG. Any mismatch in handler counts, target resolution, or check ordering causes cascading divergence.

5. **Protocol formatting** -- Missing `[from]` tags, wrong effect names, missing `[silent]` flags, wrong `[of]` attribution.

### Step 5: Fix and Verify

1. Make the code change.
2. Verify the single failing seed: `dotnet run --project ApogeeVGC -- --mode EquivalenceTest`
3. Run the full batch to confirm no regressions: `dotnet run --project ApogeeVGC --configuration Release`
4. Commit with a descriptive message.

### Step 6: Document the Fix

Create a markdown file in `Docs/bugfixes-equivalence/` and add an entry to `Docs/bugfixes-equivalence/index.md`.

## PRNG System

Both engines use an identical Gen5 RNG (Linear Congruential Generator with 4x16-bit state). Every `Random()`, `RandomChance()`, and `Sample()` call consumes exactly 1 `Gen5Rng.Next()` call. `Shuffle(start, end)` consumes `(end - start - 1)` calls.

Key PRNG consumers:
- **speedSort** -- Selection sort with Fisher-Yates shuffle for tied elements. Called inside `RunEvent` and `eachEvent` to order handlers by priority/speed.
- **randomFoe** -- Target resolution for moves. Called via `getTarget -> getRandomTarget -> Side.RandomFoe()`.
- **Accuracy checks** -- `randomChance(accuracy, 100)` for each non-true accuracy move.
- **Damage roll** -- `random(16)` for the 85-100% damage randomizer.
- **Critical hit** -- `randomChance(critRatio, 24)`.
- **Secondary effects** -- `random(100)` chance checks.

When PRNG traces diverge, the first mismatching call number and its caller identify the root cause.

## Current State

- **Branch:** `equivalence-test-fixes`
- **Batch size:** 1,000,000
- **Pass rate:** 1,000,000/1,000,000
- **Documented bugs:** 98 (in `Docs/bugfixes-equivalence/`)
- **Format:** `gen9randomdoublesbattle`

## Continuing This Work

To extend coverage to new formats:

1. Run with a different format: `dotnet run --project ApogeeVGC --configuration Release -- --format <format-id>`
2. Fix any new failures using the cycle above.
3. **Check `Docs/bugfixes-equivalence/index.md` before debugging** -- a similar bug may have already been fixed.
4. Document each fix in `Docs/bugfixes-equivalence/`.
5. Commit frequently -- one commit per fix.
6. Revert `Driver.cs` back to the default fixture path after debugging (don't leave it pointing at a specific battle).

The `batch_cache/` directory is gitignored and persists across runs. Each format gets its own subdirectory with a `showdown_version.txt` for cache validation.
