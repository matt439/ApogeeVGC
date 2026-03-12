# ChooseMove Rejecting Locked Moves with Target Location (Recharge)

**Commit:** `95bde91e`, `daef8eef`
**Date:** 2026-03-12

## Problem
When a Pokemon was locked into a move like Recharge, `ChooseMove` rejected the move choice because target validation failed. Recharge has no target in the request data, and the target validation step ran before the locked-move handler could bypass it.

## Root Cause
Two issues combined: (1) When request move data had no target (e.g., Recharge), the code did not default to `MoveTarget.Normal` as Showdown does via `move2.target || "normal"`. (2) Target validation ran before the locked-move check, so locked moves without valid targets were rejected before they could be handled. In Showdown, locked moves are processed before target validation.

## Fix
First commit (`95bde91e`) added the `?? MoveTarget.Normal` fallback for missing request targets. Second commit (`daef8eef`) restructured `ChooseMove` to handle locked moves before target validation, matching Showdown's ordering where locked moves bypass target checks entirely.

## Files Changed
- `ApogeeVGC/Sim/SideClasses/Side.Choices.cs` -- reordered locked-move handling before target validation; added MoveTarget.Normal default

## Pattern
Control flow ordering mismatch: Showdown processes locked moves before target validation. When porting, the order of validation steps matters -- early-exit paths must appear at the same point as in the source.
