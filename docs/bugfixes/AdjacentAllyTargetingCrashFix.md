# Adjacent Ally Targeting Crash Fix

## Summary
**Severity**: High  
**Systems Affected**: PlayerRandom targeting, BattleQueue action resolution  
**Root Cause**: Two interrelated bugs — (1) `PlayerRandom.GetRandomTargetLocation` returned positive (foe) target locations for ally-targeting moves, and (2) `BattleQueue.ResolveAction` crashed when `GetRandomTarget` returned null and `targetLoc` remained 0.

## Symptoms
- `ArgumentOutOfRangeException: Active slot index -1 is out of range` during `CommitChoices`
- Crash path: `CommitChoices` → `AddChoice` → `ResolveAction` → `GetAtLoc(0)` → `GetActiveAt(-1)`
- Only occurs in doubles when:
  1. An ally-targeting move (e.g., Dragon Cheer, `MoveTarget.AdjacentAlly`) is used
  2. The only ally is fainted, leaving no valid ally target

## Scenario
1. P2's Ursaluna (slot 0) fainted from Drain Punch
2. P2's Miraidon (slot 1) needed to choose a move
3. PlayerRandom picked Dragon Cheer (`MoveTarget.AdjacentAlly`) with `targetLoc=1` (a foe slot)
4. `Side.Choose` → `ValidTargetLoc` rejected the target (positive value for an ally-only move)
5. Fallback to `AutoChoose` → `ChooseMove()` auto-picked Dragon Cheer with `targetLoc=0`
6. During `ResolveAction`, `GetRandomTarget` returned `null` (no living allies)
7. `targetLoc` stayed 0, then `GetAtLoc(0)` computed `Math.Abs(0) - 1 = -1` → crash

## Bug 1: PlayerRandom.GetRandomTargetLocation

### Before
```csharp
// All targeting moves returned positive values (foe slots)
return _random.Random(1, 3); // Returns 1 or 2
```

### After
```csharp
return targetType switch
{
    MoveTarget.Normal or MoveTarget.Any or MoveTarget.AdjacentFoe =>
        _random.Random(1, 3),          // Positive = foe slots
    MoveTarget.AdjacentAlly =>
        -_random.Random(1, 3),         // Negative = ally slots
    MoveTarget.AdjacentAllyOrSelf =>
        -_random.Random(1, 3),         // Negative = ally slots
    _ => 0,                            // Auto-targeting
};
```

**Target location conventions**: Positive = foe side, Negative = ally side, 0 = auto-resolve.

## Bug 2: BattleQueue.ResolveAction null OriginalTarget

In TypeScript, `getAtLoc(0)` returns `undefined` because `side.active[-1]` is undefined in JS. The C# port throws an `ArgumentOutOfRangeException` instead.

### Before
```csharp
currentAction = moveAct with
{
    OriginalTarget = moveAct.Pokemon.GetAtLoc(moveAct.TargetLoc), // Crashes when targetLoc=0
};
```

### After
```csharp
currentAction = moveAct with
{
    OriginalTarget = moveAct.TargetLoc != 0
        ? moveAct.Pokemon.GetAtLoc(moveAct.TargetLoc)
        : null,
};
```

`MoveAction.OriginalTarget` changed from `required Pokemon` to `Pokemon?` to match TS behavior. All downstream usages (`RunMoveOptions.OriginalTarget`, `GetTarget`) already accept `Pokemon?`.

## Files Changed
- `ApogeeVGC/Sim/Player/PlayerRandom.cs` — Fixed `GetRandomTargetLocation` to use negative locations for ally moves
- `ApogeeVGC/Sim/Actions/MoveAction.cs` — Made `OriginalTarget` nullable (`Pokemon?`)
- `ApogeeVGC/Sim/BattleClasses/BattleQueue.cs` — Guard against `GetAtLoc(0)` when no valid target exists

## Keywords
AdjacentAlly, Dragon Cheer, targetLoc, GetAtLoc, GetRandomTargetLocation, OriginalTarget, ArgumentOutOfRangeException, fainted ally, doubles targeting
