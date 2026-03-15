# effectOrder Counter Divergence from Double-Increment and Stall/Protect Ordering

**Commit:** `23646e13`
**Date:** 2026-03-13

## Problem
The `effectOrder` counter diverged from Showdown (producing 0,2,4,6... instead of 0,1,2,3...), and protect-like moves created their volatiles in the wrong order relative to Showdown, both causing downstream PRNG and event-ordering differences.

## Root Cause
Two independent issues contributed to the divergence. First, `BattleActions.Switch.cs` manually assigned `EffectOrder = Battle.EffectOrder++` after `InitEffectState` had already auto-incremented the counter, resulting in a double-increment per switch-in. Second, all protect-like moves (Protect, Detect, Endure, Silk Trap, Spiky Shield, Baneful Bunker, Burning Bulwark) had `VolatileStatus = Stall` with `OnHit` adding the specific protect volatile, which is the opposite of Showdown where the protect volatile is the `volatileStatus` and Stall is added in `OnHit`. This reversed volatile creation order changed `effectOrder` assignment and downstream event handler sorting.

## Fix
Removed the manual `EffectOrder` assignments in switch-in (relying on `InitEffectState`'s auto-increment), and swapped the `VolatileStatus`/`OnHit` volatile assignments on all seven protect-like moves to match Showdown's ordering.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/BattleActions.Switch.cs` -- Removed manual `EffectOrder` override after `InitEffectState`
- `ApogeeVGC/Data/Moves/MovesABC.cs` -- Swapped Baneful Bunker and Burning Bulwark volatile ordering
- `ApogeeVGC/Data/Moves/MovesDEF.cs` -- Swapped Detect and Endure volatile ordering
- `ApogeeVGC/Data/Moves/MovesPQR.cs` -- Swapped Protect volatile ordering
- `ApogeeVGC/Data/Moves/MovesSTU.cs` -- Swapped Silk Trap and Spiky Shield volatile ordering
- `ApogeeVGC/Sim/Core/Driver.EquivalenceBatch.cs` -- Increased batch test count to 4000
- `ApogeeVGC/Sim/Core/Driver.cs` -- Fixed inputlog file extension in equivalence test path

## Pattern
Counter double-increment and inverted volatile creation order causing effectOrder divergence from the reference implementation.
