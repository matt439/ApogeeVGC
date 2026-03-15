# Pressure PP Deduction Skipped Due to Format SourceEffect

**Date:** 2026-03-14

## Problem
Pressure ability's extra PP deduction was never applied. Moves targeting Pokemon with Pressure should cost 2 PP instead of 1, but C# always deducted only 1 PP. This caused PP-dependent mechanics (like Encore's early termination on PP exhaustion) to fire at the wrong time, and any battle involving Pressure to potentially diverge.

## Root Cause
In `UseMoveInner`, the code captures `Battle.Effect` as `sourceEffect` when no explicit source effect is provided:
```csharp
if (sourceEffect == null && Battle.Effect.EffectStateId != EffectStateId.FromEmpty())
    sourceEffect = Battle.Effect;
```

During normal move execution, `Battle.Effect` is the Format/rules effect (type=Format). The Pressure PP block is guarded by:
```csharp
if (sourceEffect == null || callerMoveForPressure != null)
```

Since `sourceEffect` was set to the Format effect (non-null) and `callerMoveForPressure` was null (Format is not an ActiveMove with PP), the entire Pressure PP block was skipped for **every move in every battle**.

Showdown handles this by clearing the sourceEffect when it's a Format-type:
```javascript
if (sourceEffect && (sourceEffect as Format).effectType) sourceEffect = undefined;
```

The C# code only cleared sourceEffect for Instruct and Custap Berry, missing the Format clearing.

## Fix
Added `sourceEffect.EffectType == EffectType.Format` to the sourceEffect clearing condition, matching Showdown's behavior.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/BattleActions.Moves.cs` — Clear sourceEffect for Format-type effects

## Impact
This fix affects every battle where a Pokemon with Pressure is on the field. It also indirectly fixes the Encore end timing bug (3 seeds) where Encore appeared to end one turn late — the real issue was PP not being exhausted at the correct time due to missing Pressure deduction.

## Pattern
When `Battle.Effect` is captured as a default sourceEffect, it must be cleared for Format-type effects to match Showdown's `(sourceEffect as Format).effectType` guard. Other code paths that check sourceEffect for null may also need this consideration.
