# Multi-Accuracy Float Accuracy Skips RunEvent Modifier

**Date:** 2026-03-14

## Problem
Population Bomb with Wide Lens and evasion boosts sometimes got more hits than Showdown. For example, with -1 evasion boost, C# got 5 hits while Showdown got 4.

## Root Cause
Showdown keeps accuracy as a **float** throughout the multi-accuracy code path. After boost calculation, accuracy can be fractional (e.g., `90 / (4/3) = 67.5`). This float is passed to `runEvent('ModifyAccuracy')`.

Showdown's `runEvent` applies `FinalModify` (the accumulated chainModify modifier) **only when the relay variable is an integer**:
```javascript
if (typeof relayVar === 'number' && relayVar === Math.abs(Math.floor(relayVar))) {
    relayVar = this.modify(relayVar, this.event.modifier);
}
```

When accuracy is 67.5 (a float), `67.5 === Math.floor(67.5)` is false, so the Wide Lens modifier (`chainModify([4505, 4096])`) is **silently dropped**. The float 67.5 flows directly to `randomChance(67.5, 100)`.

In C#, the previous fix used `Math.Ceiling(67.5) = 68` to convert to int before `RunEvent`. Since 68 IS an integer, `FinalModify` applied the Wide Lens modifier: `Modify(68, 4505/4096) = 75`. This gave a higher accuracy than Showdown's effective 67.5 (≈68 for integer comparison), causing extra hits.

## Fix
Track whether the boost calculation produced a fractional result (`accIsFloat`). When true:
- Still call `RunEvent(ModifyAccuracy)` for side effects, but **discard the modifier result** (matching Showdown's FinalModify skip for floats)
- Use `Math.Ceiling(accValue)` for `RandomChance` (since `random(100) < 67.5` ≡ `random(100) < 68` for integer random values)

When false (integer accuracy): proceed normally through `RunEvent` with modifier application.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/BattleActions.HitSteps.cs` — Handle float accuracy in multi-accuracy path

## Impact
Affects multi-accuracy moves (Population Bomb, Triple Axel, Triple Kick) when:
1. Accuracy/evasion boosts produce a fractional value, AND
2. An accuracy-modifying item/ability (Wide Lens, Compound Eyes) is present

## Pattern
Showdown's `runEvent` FinalModify has an implicit integer guard. When C# converts floats to ints before `RunEvent`, modifiers that would be skipped in Showdown are incorrectly applied. For any code path that passes potentially-fractional values through `runEvent`, check whether Showdown's float-vs-integer distinction affects the result.
