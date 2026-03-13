# U-turn selfSwitch Not Triggering When Disguise Reduces Damage to 0

**Commit:** `e9a7335b`
**Date:** 2026-03-12

## Problem
U-turn (and other selfSwitch moves) did not trigger the switch-out effect when the target's Disguise ability reduced damage to 0, causing the user to remain on the field when Showdown would switch them out.

## Root Cause
Two issues combined to prevent selfSwitch from working with zero damage. First, the `selfSwitch` logic that sets `didSomething = true` (matching Showdown lines 1307-1313) was missing entirely from the per-target loop. Without it, a move dealing 0 damage had no mechanism to mark itself as "succeeded" for selfSwitch purposes. Second, the `didAnything` update was inside a guard that only ran when `damage[i]` was not an integer. When Disguise reduced damage to integer 0, `didAnything` was never updated, so even if `selfSwitch` had set `didSomething = true`, it would not propagate to `moveSucceeded`.

## Fix
Added the selfSwitch `didSomething = true` block inside the per-target loop (checking `CanSwitch` and `Commanded`), and moved the `didAnything = CombineResults(didAnything, didSomething)` line outside the `damage[i]` integer guard so it always executes.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/BattleActions.MoveEffects.cs` -- Added selfSwitch didSomething logic in per-target loop and moved didAnything update outside damage integer guard

## Pattern
Missing move-success signaling for selfSwitch combined with an overly restrictive guard on result propagation.
