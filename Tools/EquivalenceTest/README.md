# Equivalence Testing: C# Sim vs Pokemon Showdown

Deterministic equivalence tests that verify the C# battle simulator produces
identical results to the TypeScript Showdown simulator.

## Prerequisites

1. Build Showdown:
   ```
   cd pokemon-showdown
   npm install
   node build
   ```

2. Build the C# project (it includes Gen5RNG, the same PRNG Showdown uses).

## How it works

1. Both sims use the same PRNG algorithm (Gen5RNG LCG) with the same seed
2. Same teams and choices are fed to both sims
3. Protocol output is compared line-by-line
4. Any divergence pinpoints the exact mechanic that differs

## Files

- `run_showdown_battle.js` — Node.js script that drives a Showdown battle via BattleStream
- `gen5rng_test.js` — Standalone test to verify Gen5RNG port produces identical sequences
- Fixtures go in `fixtures/` (JSON files with teams + seed + choices)

## Quick PRNG verification

```
node gen5rng_test.js
```

This generates 1000 random numbers from Gen5RNG with a known seed and outputs
them for comparison with the C# Gen5Rng class.
