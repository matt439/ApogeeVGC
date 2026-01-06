# Fake Out Flinch Bug Fix

## Problem Summary
When Iron Hands used Fake Out on Volcarona on turn 1, Volcarona did not flinch and was able to use Struggle Bug normally. The flinch volatile status was not being applied despite the secondary effect being processed.

## Symptoms
- Fake Out's secondary effect was being processed (debug showed `[Secondaries] Secondary effect: VolatileStatus=Flinch, Chance=100`)
- The chance check passed successfully 
- But when Volcarona attempted to move, it had 0 volatile conditions (`[RunMove] Volcarona has 0 volatile conditions`)
- Volcarona's move was not prevented by the flinch

## Root Cause
The issue was in `BattleActions.MoveEffects.cs` in the `RunMoveEffects` method. When processing secondary effects:

1. `Secondaries()` calls `MoveHit(target, source, move, secondary, true, isSelf)` where `secondary` is a `SecondaryEffect`
2. `MoveHit` calls `ExecuteMoveHit` which calls `SpreadMoveHit`  
3. `SpreadMoveHit` stores the `hitEffect` (the `SecondaryEffect`) in `move.HitEffect`
4. `SpreadMoveHit` then calls `RunMoveEffects(damage, targets, pokemon, move, move, isSecondary, isSelf)`
   - Note: It passes `move` twice - once as the `move` parameter and once as the `moveData` parameter
5. In `RunMoveEffects`, the code checked `moveData.VolatileStatus` to apply volatile statuses
6. But `moveData` was the `ActiveMove` itself, which didn't have the secondary effect's `VolatileStatus` set
7. The actual `VolatileStatus` was in `move.HitEffect` (the `SecondaryEffect` that was stored earlier)

So the volatile status check was looking in the wrong place and never found the `Flinch` volatile status to apply.

## Solution
Modified `RunMoveEffects` in `BattleActions.MoveEffects.cs` to check both `moveData` properties AND `move.HitEffect` properties when applying effects. This ensures that secondary effects (which are stored in `move.HitEffect`) are properly applied.

### Changes Made

**File**: `ApogeeVGC\Sim\BattleClasses\BattleActions.MoveEffects.cs`

1. **Apply volatile status** (lines ~116-131):
```csharp
// OLD CODE:
if (moveData.VolatileStatus != null)
{
    RelayVar volatileResult = target.AddVolatile(moveData.VolatileStatus.Value, source, move);
    // ...
}

// NEW CODE:
// Check both moveData.VolatileStatus and move.HitEffect?.VolatileStatus (for secondary effects)
ConditionId? volatileToApply = moveData.VolatileStatus ?? (move.HitEffect as HitEffect)?.VolatileStatus;
if (volatileToApply != null)
{
    RelayVar volatileResult = target.AddVolatile(volatileToApply.Value, source, move);
    // ...
}
```

2. **Apply boosts** (lines ~28-43):
```csharp
// OLD CODE:
if (moveData.HitEffect?.Boosts != null && !target.Fainted)
{
    BoolZeroUnion? boostResult = Battle.Boost(moveData.HitEffect.Boosts, target, source, move, isSecondary, isSelf);
    // ...
}

// NEW CODE:
// Check both moveData.HitEffect?.Boosts and move.HitEffect?.Boosts (for secondary effects)
SparseBoostsTable? boostsToApply = moveData.HitEffect?.Boosts ?? (move.HitEffect as HitEffect)?.Boosts;
if (boostsToApply != null && !target.Fainted)
{
    BoolZeroUnion? boostResult = Battle.Boost(boostsToApply, target, source, move, isSecondary, isSelf);
    // ...
}
```

3. **Apply status** (lines ~80-96):
```csharp
// OLD CODE:
if (moveData.Status != null)
{
    bool statusResult = target.TrySetStatus(moveData.Status.Value, source, ...);
    // ...
}

// NEW CODE:
// Check both moveData.Status and move.HitEffect?.Status (for secondary effects)
ConditionId? statusToApply = moveData.Status ?? (move.HitEffect as HitEffect)?.Status;
if (statusToApply != null)
{
    bool statusResult = target.TrySetStatus(statusToApply.Value, source, ...);
    // ...
}
```

4. **Added using directive**:
```csharp
using ApogeeVGC.Sim.Stats;  // Added for SparseBoostsTable
```

## Testing
After the fix:
1. Iron Hands uses Fake Out on Volcarona
2. Debug logs show: `[RunMoveEffects] Attempting to apply volatile status Flinch to Volcarona`
3. `[RunMoveEffects] Volatile application result: True`
4. When Volcarona's turn comes: `[RunMove] Volcarona has 1 volatile conditions: Flinch`
5. `[RunMove] BeforeMove returned bool: False`  
6. `[RunMove] Move prevented by BeforeMove event`
7. Volcarona flinches and cannot move

### Part 2: Secondary Effects Calculating Damage

**Problem**: After fixing the flinch application, Volcarona was taking damage TWICE - once from the move itself (23 HP) and once from the secondary effect processing (26 HP, for a total of 49 HP).

**Debug Output**:
```
[HitStepMoveHitLoop] Target hit count: 1
[SpreadDamage] About to apply 23 damage to Volcarona (current HP: 192)
[SpreadDamage] Applied 23 damage to Volcarona (new HP: 169)
[Secondaries] Applying secondary effect, calling MoveHit
[SpreadDamage] About to apply 26 damage to Volcarona (current HP: 169)
[SpreadDamage] Applied 26 damage to Volcarona (new HP: 143)
```

**Root Cause**: In `GetSpreadDamage`, the code checks `if (!isSelf)` to skip damage calculation for self-targeting effects, but there was no check for `isSecondary`. When `Secondaries()` called `MoveHit` with the secondary effect, it passed `isSecondary=true`, but `GetSpreadDamage` still calculated and applied damage. Secondary effects should ONLY apply their effects (volatile status, boosts, status conditions), not damage.

**Solution**: Modified `GetSpreadDamage` in `BattleActions.MoveHit.cs` to skip damage calculation when either `isSelf=true` OR `isSecondary=true`:

```csharp
// OLD CODE:
if (!isSelf)
{
    IntUndefinedFalseUnion? curDamage = GetDamage(source, target, moveData);
    // ...
}

// NEW CODE:
// Skip damage calculation for self-targeting effects and secondary effects
// This matches TypeScript behavior and prevents infinite recursion
// Secondary effects should only apply status/volatile/boosts, not damage
if (!isSelf && !isSecondary)
{
    IntUndefinedFalseUnion? curDamage = GetDamage(source, target, moveData);
    // ...
}
```

This ensures that when processing secondary effects, only the effect properties (VolatileStatus, Boosts, Status, etc.) are applied, and no additional damage is calculated or dealt.

## Keywords
`Fake Out`, `flinch`, `secondary effect`, `volatile status`, `RunMoveEffects`, `HitEffect`, `SecondaryEffect`, `move.HitEffect`, `GetSpreadDamage`, `isSecondary`, `double damage`

## Related Fixes
- [Spirit Break Secondary Effect Fix](SpiritBreakSecondaryEffectFix.md) - Similar issue with secondary effects not being applied
- [Self-Drops Infinite Recursion Fix](SelfDropsInfiniteRecursionFix.md) - Similar pattern of using `isSelf` flag to skip damage calculation
