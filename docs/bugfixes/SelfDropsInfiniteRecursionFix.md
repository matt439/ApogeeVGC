# Self-Targeting Stat Changes Fix (Draco Meteor Bug)

## Summary

Fixed infinite recursion bug when moves with self-targeting stat changes (like Draco Meteor's -2 SpA) were executed. The solution implements TypeScript-matching behavior where the `isSelf=true` flag properly skips damage calculation throughout the pipeline.

## Original Problem

### Stack Overflow from Infinite Recursion
```
SelfDrops ? MoveHit ? SpreadMoveHit ? GetSpreadDamage ? GetDamage ? RunImmunity ? Event System ? [loop back]
```

**Root Cause**: Self-targeting stat changes were going through the full move hit pipeline including damage calculation and immunity checks, creating an infinite loop when event handlers were triggered.

## Solutions Compared

### Initial Solution (Direct Boost Application)
The first fix applied stat boosts directly using `Battle.Boost()`, bypassing the move hit pipeline entirely:

```csharp
// Direct approach - DEVIATES from TypeScript
Battle.Boost(moveData.Self.Boosts, source, source, move, isSecondary, true);
```

**Problem**: This altered the logic from the TypeScript source code, which always calls `moveHit()` for self-targeting effects.

### Final Solution (TypeScript-Matching Flag Propagation)
The correct fix ensures `isSelf=true` properly prevents damage calculation throughout the pipeline:

#### 1. Skip Damage Calculation in `GetSpreadDamage`
```csharp
public SpreadMoveDamage GetSpreadDamage(...)
{
    for (int i = 0; i < targets.Count; i++)
    {
        // ...
        
        // Skip damage calculation for self-targeting effects
        // This matches TypeScript behavior and prevents infinite recursion
        if (!isSelf)
        {
            IntUndefinedFalseUnion? curDamage = GetDamage(source, target, moveData);
            // ... process damage ...
        }
    }
}
```

#### 2. Restore Original `SelfDrops` Logic
```csharp
public void SelfDrops(...)
{
    // ...
    if (!isSecondary && moveData.Self.Boosts != null)
    {
        int secondaryRoll = Battle.Random(100);
        if (moveData.Self.Chance == null || secondaryRoll < moveData.Self.Chance)
        {
            // isSelf=true prevents damage calculation (matching TypeScript behavior)
            MoveHit(source, source, move, moveData.Self, isSecondary, true);
        }
        if (move.MultiHit == null)
        {
            move.SelfDropped = true;
        }
    }
    else
    {
        // isSelf=true prevents damage calculation (matching TypeScript behavior)
        MoveHit(source, source, move, moveData.Self, isSecondary, true);
    }
}
```

## TypeScript Reference

From `pokemon-showdown/sim/battle-actions.ts`:

### TypeScript `selfDrops` (line 2027)
```typescript
selfDrops(
    targets: SpreadMoveTargets, source: Pokemon,
    move: ActiveMove, moveData: ActiveMove, isSecondary?: boolean
) {
    for (const target of targets) {
        if (target === false) continue;
        if (moveData.self && !move.selfDropped) {
            if (!isSecondary && moveData.self.boosts) {
                const secondaryRoll = this.battle.random(100);
                if (typeof moveData.self.chance === 'undefined' || secondaryRoll < moveData.self.chance) {
                    this.moveHit(source, source, move, moveData.self, isSecondary, true);
                }
                if (!move.multihit) move.selfDropped = true;
            } else {
                this.moveHit(source, source, move, moveData.self, isSecondary, true);
            }
        }
    }
}
```

### TypeScript `spreadMoveHit` (line 1835)
```typescript
// Skip damage for secondary effects without self-targeting
if (targets[i] && isSecondary && !moveData.self) {
    damage[i] = true;
}
```

### TypeScript `getSpreadDamage` (line 1920)
The TypeScript version doesn't explicitly skip `getDamage` when `isSelf=true`, but the damage array has already been set to `true` earlier in `spreadMoveHit`, so the damage calculation result is overwritten.

## Key Differences from TypeScript

While our C# implementation achieves the same functional result, there is one architectural difference:

**TypeScript Approach**: Sets damage to `true` early in the pipeline and lets it propagate, with the actual damage calculation still occurring but being ignored.

**C# Approach**: Explicitly skips the `GetDamage()` call entirely when `isSelf=true` for better performance and clearer intent.

Both approaches are valid - the C# version is arguably more explicit about the intent to skip damage calculation.

## Testing

The fix was tested with:
- **Move**: Draco Meteor (130 BP Dragon-type special move with -2 SpA self-drop)
- **Pokemon**: Miraidon with Choice Specs in Electric Terrain
- **Target**: Calyrex-Ice
- **Result**: Move executes successfully, stat drop applies, no infinite recursion

### Expected Behavior
1. Draco Meteor hits the target for damage
2. Self stat drop (-2 SpA) is applied to the user
3. No damage calculation occurs for the self-targeting effect
4. No infinite recursion

## Files Modified

1. **ApogeeVGC\Sim\BattleClasses\BattleActions.MoveHit.cs**
   - Modified `GetSpreadDamage()` to skip `GetDamage()` call when `isSelf=true`

2. **ApogeeVGC\Sim\BattleClasses\BattleActions.MoveEffects.cs**
   - Restored `SelfDrops()` to match TypeScript by calling `MoveHit()` with `isSelf=true`

3. **ApogeeVGC\Sim\Events\EventHandlerAdapter.cs** (from earlier fix)
   - Added unwrapping of `IEffect` from `EffectRelayVar` for `TryAddVolatile` event

## Related Fixes

This fix works in conjunction with:
- **TryAddVolatile Parameter Fix**: Properly unwraps `Condition` from `EffectRelayVar` in event handlers
- **Hadron Engine Terrain Check Fix**: Uses direct `Field.Terrain` access instead of `IsTerrain()` to avoid event triggers

## Conclusion

The final solution properly matches the TypeScript behavior by ensuring the `isSelf=true` flag prevents damage calculation while still going through the move hit pipeline for proper stat application. This approach:

1. ? Prevents infinite recursion
2. ? Matches TypeScript logic structure
3. ? Maintains proper event handling
4. ? Applies stat changes correctly
5. ? More explicit and performant than TypeScript's approach
