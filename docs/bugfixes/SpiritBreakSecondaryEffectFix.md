# Spirit Break Secondary Effect Bug Fix

## Problem Summary
Spirit Break (and other moves using `Secondary` instead of `Secondaries`) was not applying its 100% chance secondary effect to lower the target's Special Attack stat by 1 stage. This affected all moves that use the `Secondary` property instead of `Secondaries` array.

## Symptoms
- Spirit Break hits and deals damage successfully
- No stat drop message appears
- Opponent's Special Attack stat remains unchanged
- Debug output showed step 5 (secondary effects) was being skipped

## Root Causes

### Cause 1: Missing Secondary-to-Secondaries Conversion
**File**: `ApogeeVGC\Sim\Moves\Move.Core.cs`
**Method**: `ToActiveMove()`

**Problem**: The TypeScript pokemon-showdown source automatically converts a single `secondary` effect into a `secondaries` array during move initialization:

```typescript
this.secondaries = data.secondaries || (this.secondary && [this.secondary]) || null;
```

The C# implementation was missing this conversion logic. When `Secondaries` was null but `Secondary` existed, the move definition had the effect data but it wasn't being accessed by the battle engine.

**Impact**: Moves like Spirit Break and Struggle Bug that define `Secondary` (singular) didn't have their secondary effects processed.

### Cause 2: Infinite Recursion in Secondary Effect Processing
**File**: `ApogeeVGC\Sim\BattleClasses\BattleActions.MoveHit.cs`
**Method**: `SpreadMoveHit()`

**Problem**: After fixing Cause 1, applying secondary effects caused a stack overflow due to infinite recursion:

1. `Secondaries` method calls `MoveHit` with `isSecondary = true`
2. `MoveHit` calls `ExecuteMoveHit`
3. `ExecuteMoveHit` calls `SpreadMoveHit` with `isSecondary = true`
4. `SpreadMoveHit` step 5 checks `if (move.Secondaries != null)` and calls `Secondaries` again
5. Loop back to step 1

The TypeScript implementation has an implicit guard against this because the `secondary` property is passed as `moveData`, not the full move object, preventing re-processing of secondaries.

**Impact**: Stack overflow exception when any move with secondary effects was used.

## Solutions

### Solution 1: Convert Secondary to Secondaries Array
**File**: `ApogeeVGC\Sim\Moves\Move.Core.cs`

Added logic to `ToActiveMove()` method to match TypeScript behavior:

```csharp
public ActiveMove ToActiveMove()
{
    // Match TypeScript behavior: if secondaries is null but secondary exists,
    // wrap secondary in an array to populate secondaries
    SecondaryEffect[]? secondaries = Secondaries ?? (Secondary != null ? [Secondary] : null);
    
    return new ActiveMove
    {
        // ... other properties ...
        Secondary = Secondary,
        Secondaries = secondaries,
        // ... other properties ...
    };
}
```

**Why This Works**: When a `Move` is converted to an `ActiveMove`, if `Secondaries` is null but `Secondary` exists, it automatically creates a `Secondaries` array containing the single `Secondary` effect. This matches the TypeScript constructor behavior and ensures the battle engine can find and process the secondary effects.

### Solution 2: Prevent Secondary Effect Recursion
**File**: `ApogeeVGC\Sim\BattleClasses\BattleActions.MoveHit.cs`

Added `isSecondary` check to step 5 in `SpreadMoveHit`:

```csharp
// 5. secondary effects
// Only process secondaries if this is not already a secondary effect (prevents infinite recursion)
if (move.Secondaries != null && !isSecondary)
{
    Secondaries(targets, pokemon, move, move, isSelf);
}
```

**Why This Works**: The `isSecondary` flag indicates we're already processing a secondary effect. By checking this flag before processing secondaries again, we prevent the infinite recursion. When `Secondaries` calls `MoveHit` with `isSecondary=true`, the subsequent `SpreadMoveHit` call will skip step 5, breaking the loop.

## TypeScript Reference

### Secondary-to-Secondaries Conversion
From `pokemon-showdown/sim/dex-moves.ts` (line 490-491):
```typescript
this.secondary = data.secondary || null;
this.secondaries = data.secondaries || (this.secondary && [this.secondary]) || null;
```

### Secondary Effect Processing
From `pokemon-showdown/sim/battle-actions.ts` (line 1119):
```typescript
// 5. secondary effects
if (moveData.secondaries) this.secondaries(targets, pokemon, move, moveData, isSelf);
```

Note that TypeScript passes `moveData` (the secondary effect object) rather than the full move, which implicitly prevents re-processing since `moveData.secondaries` would be undefined for a secondary effect.

## Files Modified

1. **ApogeeVGC\Sim\Moves\Move.Core.cs**
   - Modified `ToActiveMove()` method to convert `Secondary` to `Secondaries` array

2. **ApogeeVGC\Sim\BattleClasses\BattleActions.MoveHit.cs**
   - Added `!isSecondary` check in step 5 of `SpreadMoveHit()` method

## Testing

### Verification Steps
1. Run a battle with Grimmsnarl using Spirit Break against Volcarona
2. Observe that Spirit Break deals damage
3. Confirm message appears: "Volcarona's Special Attack fell!"
4. Check opponent's stat changes show SpA: -1
5. Verify no stack overflow exception occurs
6. Test with other moves using `Secondary`: Crunch, Fake Out, Struggle Bug

### Expected Behavior After Fix
- Spirit Break applies its 100% chance to lower target's Special Attack by 1 stage
- All moves with `Secondary` property work correctly
- No infinite recursion or stack overflow
- Battle continues normally after secondary effects apply

## Related Patterns

### Pattern: Property Naming Mismatches Between TypeScript and C#
This bug demonstrates a subtle difference in how TypeScript and C# handle property initialization. TypeScript's constructor can perform transformations during initialization, while C# typically requires explicit conversion logic.

**Checklist when porting TypeScript to C#**:
1. Check if TypeScript constructor performs any property transformations
2. Look for expressions like `prop1 || (prop2 && [prop2]) || null`
3. Implement equivalent logic in C# conversion methods
4. Consider using init-only properties or computed properties for derived values

### Pattern: Preventing Infinite Recursion in Effect Processing
**See also**: [Self-Drops Infinite Recursion Fix](SelfDropsInfiniteRecursionFix.md)

**Checklist**:
1. Identify recursive call paths in effect processing
2. Add flag parameters (`isSecondary`, `isSelf`) to track recursion depth
3. Check flags before re-entering recursive paths
4. Match TypeScript's implicit guards (like passing partial data structures)

## Common Gotchas

### Gotcha 1: Singular vs. Plural Property Names
The Pokemon Showdown data uses both `secondary` (singular) and `secondaries` (plural). Most moves use `secondary`, but the engine processes `secondaries`. This mismatch is resolved in the TypeScript constructor but must be explicitly handled in C#.

### Gotcha 2: Secondary Effects on Secondary Effects
Some moves can have secondary effects that themselves have secondary effects (e.g., `secondary.self`). The `isSecondary` flag prevents infinite loops but still allows nested processing within a single level.

### Gotcha 3: Build Succeeds But Runtime Fails
Both issues in this bug passed compilation but failed at runtime:
- Issue 1: No compilation error, but behavior was wrong
- Issue 2: Compilation succeeded, but stack overflow at runtime

**Lesson**: Always test moves in actual battle simulation, not just unit tests of individual methods.

## Keywords
`Spirit Break`, `Secondary`, `Secondaries`, `secondary effects`, `stat drops`, `infinite recursion`, `stack overflow`, `ToActiveMove`, `property conversion`, `Struggle Bug`, `Crunch`, `Fake Out`

## Version History
- **2025-01-XX**: Initial fix for Spirit Break secondary effect not applying
- Fixed both property conversion and infinite recursion issues

---

*Last Updated*: 2025-01-XX
*Related Fixes*: [Headlong Rush Self-Stat Drops Fix](HeadlongRushSelfStatDropsFix.md), [Self-Drops Infinite Recursion Fix](SelfDropsInfiniteRecursionFix.md)
