# Bug Fixes Index

This index provides summaries of all documented bug fixes in the ApogeeVGC project. Use this as a first reference to determine which detailed documentation is relevant to your current issue.

---

## Quick Reference by Category

### Event System Issues
- [Hadron Engine Bug Fix](#hadron-engine-bug-fix) - Ability OnStart handlers not executing during switch-in
- [Trick Room Bug Fix](#trick-room-bug-fix) - Field event handlers not mapped/invoked
- [Protect Stalling Mechanic Issue](#protect-stalling-mechanic-issue) - Move event handlers not mapped

### Union Type Handling
- [Protect Bug Fix](#protect-bug-fix) - IsZero() logic error treating false as zero
- [Leech Seed Bug Fix](#leech-seed-bug-fix) - Undefined check occurring after .ToInt() conversion
- [Tailwind OnModifySpe Fix](#tailwind-onmodifyspe-fix) - VoidReturn causing IntRelayVar type mismatch
- [Union Type Handling Guide](#union-type-handling-guide) - Comprehensive guide for preventing union type issues

### Move Mechanics
- [Volt Switch Damage Type Bug](#volt-switch-damage-type-bug) - Type priority overwriting damage values
- [Headlong Rush Self-Stat Drops Fix](#headlong-rush-self-stat-drops-fix) - HitEffect not being stored for self-targeting effects
- [Complete Draco Meteor Bug Fix](#complete-draco-meteor-bug-fix) - Multiple interconnected issues with self-targeting moves
- [Self-Drops Infinite Recursion Fix](#self-drops-infinite-recursion-fix) - Infinite loop in damage calculation for self-stat changes
- [Spirit Break Secondary Effect Fix](#spirit-break-secondary-effect-fix) - Secondary property not converted to Secondaries array

### Status Conditions
- [TrySetStatus Logic Error](#trysetstatus-logic-error) - Incorrect conditional logic when applying status
- [Wild Charge Recoil Fix](#wild-charge-recoil-fix) - Missing Recoil condition dictionary entry

### UI and Display
- [Reflect Side Condition Display Fix](#reflect-side-condition-display-fix) - Side conditions not visible in console UI

---

## Detailed Summaries

### Hadron Engine Bug Fix
**File**: `HadronEngineBugFix.md`  
**Severity**: High  
**Systems Affected**: Ability/Item OnStart handlers during switch-in (Gen 5+)

**Problem**: Miraidon's Hadron Engine ability (and all abilities/items with OnStart handlers) were not activating during switch-in in Gen 5+. The handler was discovered correctly but never invoked.

**Root Causes**:
1. EventHandlerInfo ID mismatch (handler had `EventId.Start` but needed `EventId.SwitchIn`)
2. FieldEvent performing indirect invocation causing second lookup to fail
3. State verification using reference equality incorrectly skipping handlers

**Solution**: 
- Modified handler ID using C# record's `with` expression
- Changed to direct handler invocation instead of indirect lookup
- Skip state verification for abilities/items on SwitchIn

**Keywords**: `ability`, `item`, `switch-in`, `OnStart`, `SwitchIn`, `event handler`, `Gen 5`

---

### Trick Room Bug Fix
**File**: `TrickRoomBugFix.md`  
**Severity**: High  
**Systems Affected**: Field conditions, pseudo-weather effects

**Problem**: Trick Room didn't end when used while already active, instead continued counting down duration normally. This affected all field conditions with OnFieldRestart handlers.

**Root Causes**:
1. Field event handlers (`OnFieldStart`, `OnFieldRestart`, etc.) not mapped in `EventHandlerInfoMapper`
2. `EventHandlerAdapter` couldn't resolve `Field` and `Side` parameters

**Solution**:
- Added field/side event handler mappings to `EventHandlerInfoMapper`
- Extended `EventHandlerAdapter.ResolveParameter` to support Field and Side types

**Keywords**: `field condition`, `pseudo-weather`, `Trick Room`, `event mapping`, `parameter resolution`

---

### Protect Bug Fix
**File**: `ProtectBugFix.md`  
**Severity**: High  
**Systems Affected**: Protective moves (Protect, Detect, King's Shield, etc.)

**Problem**: Protect executed but didn't prevent damage from incoming attacks. The move would display but opponents' attacks still dealt full damage.

**Root Cause**: `IsZero()` method in `BoolBoolIntEmptyUndefinedUnion` was implemented as `!Value`, treating `false` (blocked move) as "zero damage" instead of as a failure that filters out the target.

**Solution**: Changed `IsZero()` to always return `false` for boolean variants, since boolean values represent pass/fail states, not damage amounts.

**Semantic Distinction**:
- Integer 0 = Move dealt 0 damage but still "hit" (should keep target)
- Boolean false = Move was blocked/failed (should filter out target)

**Keywords**: `Protect`, `union type`, `IsZero`, `target filtering`, `blocking moves`

---

### Protect Stalling Mechanic Issue
**File**: `ProtectStallingMechanicIssue.md`  
**Severity**: Medium  
**Systems Affected**: Stalling moves (Protect, Detect, Endure, etc.)

**Problem**: Protect always succeeded at 100% rate even on consecutive uses, instead of decreasing success rates (33% on 2nd use, 11% on 3rd, etc.).

**Root Cause**: `IMoveEventMethods` interface and its handlers (`OnPrepareHit`, `OnHit`) weren't mapped in `EventHandlerInfoMapper`, preventing move event handlers from being invoked.

**Solution**:
- Added `IMoveEventMethods` support to `EventHandlerInfoMapper`
- Implemented `OnPrepareHit` and `OnHit` handlers for Protect
- Implemented complete Stall condition with counter-based success calculation
- Added `BoolEmptyVoidUnion` conversion support in `EventHandlerAdapter`

**Keywords**: `stalling`, `Protect`, `consecutive usage`, `move events`, `Stall condition`

---

### Leech Seed Bug Fix
**File**: `LeechSeedBugFix.md`  
**Severity**: High  
**Systems Affected**: Non-damaging status moves

**Problem**: Battle ended prematurely as a tie when Leech Seed was used on Turn 1. Exception thrown: "Cannot convert Undefined to int."

**Root Cause**: `SpreadDamage` was calling `.ToInt()` on damage values **before** checking if they were `Undefined`. Non-damaging moves like Leech Seed have `Undefined` damage (not 0), and attempting to convert caused an exception.

**Solution**: 
- Moved `Undefined` check to occur BEFORE calling `.ToInt()`
- Fixed two additional locations in `BattleActions.HitSteps.cs` that unsafely called `.ToInt()` on `move.TotalDamage`

**Semantic Distinction**:
- `0` = Move dealt 0 damage (immunity/resistance)
- `Undefined` = Move doesn't deal damage at all (status moves)

**Keywords**: `Leech Seed`, `Undefined`, `status move`, `damage calculation`, `type conversion`

---

### Volt Switch Damage Type Bug
**File**: `volt-switch-damage-type-bug.md`  
**Severity**: High  
**Systems Affected**: Self-switching moves (Volt Switch, U-turn, Flip Turn, etc.)

**Problem**: Volt Switch dealt damage successfully but didn't trigger the switch prompt. Game would immediately request another move instead.

**Root Cause**: Type priority system in `CombineResults` was overwriting integer damage values with boolean success indicators. Boolean has higher priority than integer, so combining them resulted in the boolean winning and the damage value being discarded.

**Solution**:
- Preserve integer damage values when they exist (don't overwrite with boolean)
- Only combine `didSomething` when damage isn't already an integer
- Check for non-zero integer to determine success (zero = immunity/failure)

**Key Insight**: When a move deals damage, that integer is more meaningful than a generic boolean "something happened" flag.

**Keywords**: `Volt Switch`, `self-switch`, `type priority`, `result combining`, `damage preservation`

---

### Headlong Rush Self-Stat Drops Fix
**File**: `HeadlongRushSelfStatDropsFix.md`  
**Severity**: High  
**Systems Affected**: Moves with self-targeting stat changes

**Problem**: Moves with self-targeting stat changes (Headlong Rush, Draco Meteor, Overheat, Close Combat, Superpower) weren't applying stat drops to the user after execution.

**Root Cause**: Type system mismatch - `hitEffect` parameter wasn't being stored in `move.HitEffect`, so `RunMoveEffects` couldn't find the boosts to apply. The `ActiveMove.HitEffect` property exists specifically to bridge this gap but wasn't being populated.

**Solution**: Store the `hitEffect` parameter in `move.HitEffect` when `SpreadMoveHit` is called with one, allowing `RunMoveEffects` to access it via `moveData.HitEffect.Boosts`.

**TypeScript vs C#**: TypeScript uses duck typing (`hitEffect as ActiveMove`), C# requires explicit storage in a bridge property.

**Keywords**: `self-stat drop`, `HitEffect`, `type mismatch`, `Headlong Rush`, `secondary effect`

---

### Complete Draco Meteor Bug Fix
**File**: `CompleteDracoMeteorBugFix.md`  
**Severity**: Critical  
**Systems Affected**: Moves with self-targeting stat changes, Choice items, terrain-boosting abilities

**Problem**: Draco Meteor on Miraidon with Choice Specs and Electric Terrain caused multiple interconnected bugs preventing move execution.

**Three Root Causes**:
1. **TryAddVolatile Parameter Mismatch**: EventHandlerAdapter expected `Condition` but was resolving from `SourceEffect` (ActiveMove) instead of `RelayVar`
2. **Hadron Engine Terrain Check**: Calling `battle.Field.IsTerrain()` triggered event system, potentially causing re-entrancy
3. **Self-Targeting Stat Changes Infinite Recursion**: Self-drops went through full move hit pipeline including damage calculation, creating infinite loop

**Solutions**:
1. Modified `EventHandlerAdapter.ResolveParameter()` to unwrap `IEffect` from `EffectRelayVar`
2. Changed Hadron Engine to use direct `Field.Terrain` property access
3. Implemented TypeScript-matching behavior where `isSelf=true` skips damage calculation

**Keywords**: `Draco Meteor`, `Choice Specs`, `infinite recursion`, `event adapter`, `self-targeting`

---

### Self-Drops Infinite Recursion Fix
**File**: `SelfDropsInfiniteRecursionFix.md`  
**Severity**: Critical  
**Systems Affected**: Moves with self-targeting stat changes

**Problem**: Stack overflow from infinite recursion when moves with self-targeting stat changes were executed. The loop: `SelfDrops ? MoveHit ? SpreadMoveHit ? GetSpreadDamage ? GetDamage ? RunImmunity ? [loop back]`

**Root Cause**: Self-targeting stat changes were going through the full move hit pipeline including damage calculation and immunity checks, creating an infinite loop when event handlers were triggered.

**Solution**: Ensure `isSelf=true` flag properly prevents damage calculation throughout the pipeline by explicitly skipping `GetDamage()` call when flag is set.

**Key Difference from TypeScript**: 
- TypeScript: Sets damage to `true` early and lets it propagate (calculation still runs but result is overwritten)
- C#: Explicitly skips `GetDamage()` call for better performance and clearer intent

**Keywords**: `infinite recursion`, `self-targeting`, `damage calculation`, `isSelf flag`, `pipeline optimization`

---

### TrySetStatus Logic Error
**File**: `trysetstatus-logic-error.md`  
**Severity**: High  
**Systems Affected**: Status condition application (Burn, Paralysis, Sleep, etc.)

**Problem**: `KeyNotFoundException` when attempting to apply status conditions from items like Flame Orb. Battle would crash completely.

**Root Cause**: Incorrect ternary operator logic in `TrySetStatus` - when Pokémon already had a status, method passed `ConditionId.None` to `SetStatus` instead of the current status, causing dictionary lookup failure.

**Solution**: Changed from `Status == ConditionId.None ? status : ConditionId.None` to `Status == ConditionId.None ? status : Status`. This allows the duplicate check in `SetStatus` to properly handle the case.

**Additional Improvements**: Added defensive dictionary checks with detailed error messages to diagnose similar issues faster.

**Keywords**: `status condition`, `TrySetStatus`, `logic error`, `ternary operator`, `Flame Orb`

---

### Wild Charge Recoil Fix
**File**: `WildChargeRecoilFix.md`  
**Severity**: High  
**Systems Affected**: Recoil damage moves, move execution pipeline

**Problem**: When any Pokémon used a recoil move (e.g., Wild Charge, Double-Edge, Brave Bird), the move would deal damage correctly but immediately after applying recoil damage, the battle would end prematurely as a tie with `KeyNotFoundException: The given key 'Recoil' was not present in the dictionary.`

**Root Cause**: The `ConditionId.Recoil` enum value existed in the `ConditionId` enum, but there was no corresponding entry in the `Conditions` dictionary in `Conditions.cs`. When `BattleActions.HitSteps.cs` tried to apply recoil damage using `Library.Conditions[ConditionId.Recoil]`, the dictionary lookup threw a `KeyNotFoundException`.

**Solution**: Added the missing `ConditionId.Recoil` entry to the `Conditions` dictionary with a minimal definition (`Id`, `Name`, `EffectType`). Recoil damage doesn't require event handlers since the actual damage calculation is handled by `CalcRecoilDamage()`.

**Affected Moves**: All moves with recoil damage - Wild Charge, Volt Tackle, Take Down, Double-Edge, Head Smash, Brave Bird, Submission, High Jump Kick, Wood Hammer, Chloroblast, and others.

**Keywords**: `recoil`, `Wild Charge`, `KeyNotFoundException`, `Conditions`, `dictionary`, `missing entry`, `enum mismatch`, `damage`, `recoil moves`

---

### Union Type Handling Guide
**File**: `UnionTypeHandlingGuide.md`  
**Type**: Reference Guide  
**Purpose**: Comprehensive guide for preventing recurring union type issues

**Contents**:
- 6 major issue categories with detection patterns
- Complete audit checklist for codebase review
- Code search queries for finding potential issues
- Prevention guidelines for new code
- Testing scenarios for verification
- Quick reference for Union ? RelayVar mapping

**Key Topics**:
- Missing union type conversions in EventHandlerAdapter
- Undefined/Empty handling in success checks
- Event handler return type mismatches
- Missing null checks before union access
- Inconsistent NOT_FAIL representation

**Use Cases**:
- Adding new event handlers
- Adding new union types
- Processing event results
- Determining move/action success
- Code reviews and audits

**Keywords**: `union types`, `Undefined`, `Empty`, `NOT_FAIL`, `EventHandlerAdapter`, `reference guide`

---

### Spirit Break Secondary Effect Fix
**File**: `SpiritBreakSecondaryEffectFix.md`  
**Severity**: High  
**Systems Affected**: Moves with secondary effects using `Secondary` property

**Problem**: Spirit Break and other moves using `Secondary` (singular) instead of `Secondaries` (plural) were not applying their secondary effects. After initial fix, infinite recursion caused stack overflow.

**Root Causes**:
1. **Missing Property Conversion**: TypeScript automatically converts `secondary` to `secondaries` array in constructor; C# was missing this logic
2. **Infinite Recursion**: Secondary effect processing re-triggered secondary processing in a loop

**Solutions**:
1. Added conversion logic in `Move.ToActiveMove()` to wrap `Secondary` in array as `Secondaries`
2. Added `!isSecondary` check in `SpreadMoveHit` step 5 to prevent recursive secondary processing

**Keywords**: `Spirit Break`, `Secondary`, `Secondaries`, `secondary effects`, `stat drops`, `infinite recursion`, `property conversion`, `Struggle Bug`

---

### Tailwind OnModifySpe Fix
**File**: `TailwindOnModifySpeFix.md`  
**Severity**: Critical  
**Systems Affected**: Side conditions, stat modification, speed calculation

**Problem**: When Volcarona used Tailwind, the battle immediately ended in a tie with `InvalidOperationException: stat must be an IntRelayVar`. The Tailwind side condition's `OnModifySpe` handler was returning `VoidReturn()` after calling `ChainModify(2)`, which got converted to `VoidReturnRelayVar` instead of the expected `IntRelayVar`.

**Root Cause**: `OnModifySpe` handlers that use `ChainModify()` were returning `VoidReturn()` instead of applying the modifier and returning the result. The stat calculation code in `Pokemon.GetStat()` strictly checks for `IntRelayVar` and throws an exception when it receives `VoidReturnRelayVar`.

**Solution**:
- Changed Tailwind's `OnModifySpe` to return `battle.FinalModify(spe)` after calling `battle.ChainModify(2)`
- Changed QuarkDrive's `OnModifySpe` to return `battle.FinalModify(spe)` after calling `battle.ChainModify(1.5)`
- Added documentation on the correct pattern for stat modification handlers

**Key Pattern**: When using `ChainModify()` in stat modification handlers, always call `FinalModify(value)` and return the result. Only return `VoidReturn()` for early exits when no modification should occur.

**Keywords**: `Tailwind`, `QuarkDrive`, `OnModifySpe`, `VoidReturn`, `ChainModify`, `FinalModify`, `IntRelayVar`, `speed modification`, `stat events`

---

### Reflect Side Condition Display Fix
**File**: `ReflectSideConditionDisplayFix.md`  
**Severity**: Medium  
**Systems Affected**: Side conditions, UI display, console player

**Problem**: When moves like Reflect were used, the side condition was applied correctly internally, but no visual feedback was shown to players. The move execution message appeared ("Grimmsnarl used Reflect!") but the confirmation message and field status display section were missing.

**Root Causes**:
1. **Missing Message Parsing**: `ParseLogToMessages()` in `Battle.Logging.cs` didn't have cases for `-sidestart` or `-sideend` messages. The log entries were being added correctly but couldn't be parsed and displayed.
2. **Missing Perspective Data**: Side conditions weren't included in `BattlePerspective` data sent to players, so they couldn't appear in the field status section like weather/terrain do.

**Solution**:
- Added `-sidestart` and `-sideend` parsing cases with helper methods `GetSideName()` and `GetStatNameForCondition()`
- Extended `SidePlayerPerspective` and `SideOpponentPerspective` with `SideConditionsWithDuration` property
- Updated `Side.Core.cs` to populate side conditions in perspectives
- Modified `PlayerConsole.RenderBattleState()` to display side conditions in a separate section with durations

**Result**: Side conditions now display in their own section (e.g., "Your Team: Reflect:8") showing which side has them active and how many turns remain.

**Keywords**: `side condition`, `Reflect`, `Light Screen`, `Tailwind`, `log parsing`, `battle perspective`, `console UI`, `visual feedback`, `message display`

---

## Search Guide

### By Symptom

**Battle crashes/exceptions**:
- [Leech Seed Bug Fix](#leech-seed-bug-fix) - Undefined to int conversion
- [TrySetStatus Logic Error](#trysetstatus-logic-error) - Dictionary key not found
- [Wild Charge Recoil Fix](#wild-charge-recoil-fix) - Dictionary key not found for Recoil condition

**Feature not working**:
- [Hadron Engine Bug Fix](#hadron-engine-bug-fix) - Abilities not activating
- [Protect Bug Fix](#protect-bug-fix) - Blocking not working
- [Trick Room Bug Fix](#trick-room-bug-fix) - Toggle effect not working
- [Protect Stalling Mechanic](#protect-stalling-mechanic-issue) - Success rate always 100%

**Visual/UI issues**:
- [Reflect Side Condition Display Fix](#reflect-side-condition-display-fix) - Side conditions not visible

**Wrong behavior**:
- [Volt Switch Damage Type Bug](#volt-switch-damage-type-bug) - No switch prompt after damage
- [Headlong Rush Self-Stat Drops Fix](#headlong-rush-self-stat-drops-fix) - Stat changes not applied

**Infinite loops**:
- [Self-Drops Infinite Recursion Fix](#self-drops-infinite-recursion-fix) - Stack overflow
- [Complete Draco Meteor Bug Fix](#complete-draco-meteor-bug-fix) - Multiple issues including recursion
- [Spirit Break Secondary Effect Fix](#spirit-break-secondary-effect-fix) - Stack overflow from recursive secondary processing

### By Component

**Event System**:
- [Hadron Engine Bug Fix](#hadron-engine-bug-fix)
- [Trick Room Bug Fix](#trick-room-bug-fix)
- [Protect Stalling Mechanic Issue](#protect-stalling-mechanic-issue)
- [Complete Draco Meteor Bug Fix](#complete-draco-meteor-bug-fix) (partial)

**Union Types**:
- [Protect Bug Fix](#protect-bug-fix)
- [Leech Seed Bug Fix](#leech-seed-bug-fix)
- [Union Type Handling Guide](#union-type-handling-guide)
- [Volt Switch Damage Type Bug](#volt-switch-damage-type-bug)

**Move Execution Pipeline**:
- [Volt Switch Damage Type Bug](#volt-switch-damage-type-bug)
- [Headlong Rush Self-Stat Drops Fix](#headlong-rush-self-stat-drops-fix)
- [Complete Draco Meteor Bug Fix](#complete-draco-meteor-bug-fix)
- [Self-Drops Infinite Recursion Fix](#self-drops-infinite-recursion-fix)

**Status/Conditions**:
- [TrySetStatus Logic Error](#trysetstatus-logic-error)
- [Leech Seed Bug Fix](#leech-seed-bug-fix)
- [Wild Charge Recoil Fix](#wild-charge-recoil-fix)

**UI/Display**:
- [Reflect Side Condition Display Fix](#reflect-side-condition-display-fix)

### By File Modified

**EventHandlerAdapter.cs**:
- [Complete Draco Meteor Bug Fix](#complete-draco-meteor-bug-fix)
- [Protect Bug Fix](#protect-bug-fix)
- [Protect Stalling Mechanic Issue](#protect-stalling-mechanic-issue)
- [Trick Room Bug Fix](#trick-room-bug-fix)

**EventHandlerInfoMapper.cs**:
- [Trick Room Bug Fix](#trick-room-bug-fix)
- [Protect Stalling Mechanic Issue](#protect-stalling-mechanic-issue)

**BattleActions.MoveEffects.cs**:
- [Volt Switch Damage Type Bug](#volt-switch-damage-type-bug)
- [Self-Drops Infinite Recursion Fix](#self-drops-infinite-recursion-fix)

**BattleActions.MoveHit.cs**:
- [Headlong Rush Self-Stat Drops Fix](#headlong-rush-self-stat-drops-fix)
- [Self-Drops Infinite Recursion Fix](#self-drops-infinite-recursion-fix)
- [Spirit Break Secondary Effect Fix](#spirit-break-secondary-effect-fix)

**Pokemon.Status.cs**:
- [TrySetStatus Logic Error](#trysetstatus-logic-error)

**Battle.Combat.cs**:
- [Leech Seed Bug Fix](#leech-seed-bug-fix)

**BoolIntEmptyUndefinedUnion.cs**:
- [Protect Bug Fix](#protect-bug-fix)

**Battle.Logging.cs**:
- [Reflect Side Condition Display Fix](#reflect-side-condition-display-fix)

**SidePlayerPerspective.cs / SideOpponentPerspective.cs**:
- [Reflect Side Condition Display Fix](#reflect-side-condition-display-fix)

**Side.Core.cs**:
- [Reflect Side Condition Display Fix](#reflect-side-condition-display-fix)

**PlayerConsole.cs**:
- [Reflect Side Condition Display Fix](#reflect-side-condition-display-fix)

**Conditions.cs**:
- [Wild Charge Recoil Fix](#wild-charge-recoil-fix)

---

## Common Patterns

### Pattern: Event Handler Not Executing
**See**: [Hadron Engine Bug Fix](#hadron-engine-bug-fix), [Trick Room Bug Fix](#trick-room-bug-fix), [Protect Stalling Mechanic](#protect-stalling-mechanic-issue)

**Checklist**:
1. Is the event mapped in `EventHandlerInfoMapper`?
2. Can `EventHandlerAdapter` resolve all parameters?
3. Is the handler being called with correct event ID?
4. Is state verification blocking execution?

### Pattern: Union Type Conversion Missing
**See**: [Protect Bug Fix](#protect-bug-fix), [Leech Seed Bug Fix](#leech-seed-bug-fix), [Union Type Handling Guide](#union-type-handling-guide)

**Checklist**:
1. Does `EventHandlerAdapter.ConvertReturnValue` handle this union variant?
2. Are you checking variant type before accessing `.Value`?
3. Are you treating `Undefined`/`Empty` as NOT_FAIL (success)?
4. Are you distinguishing `0` (zero damage) from `false` (failure)?

### Pattern: Type Priority Issues
**See**: [Volt Switch Damage Type Bug](#volt-switch-damage-type-bug)

**Checklist**:
1. Are you combining results of different types?
2. Is integer damage being overwritten by boolean success?
3. Should you preserve the integer instead of combining?
4. Are you checking for integer first in success determination?

### Pattern: Self-Targeting Move Issues
**See**: [Headlong Rush Self-Stat Drops Fix](#headlong-rush-self-stat-drops-fix), [Complete Draco Meteor Bug Fix](#complete-draco-meteor-bug-fix), [Self-Drops Infinite Recursion Fix](#self-drops-infinite-recursion-fix)

**Checklist**:
1. Is `move.HitEffect` being populated for recursive calls?
2. Is `isSelf=true` flag properly skipping damage calculation?
3. Are self-targeting effects going through the correct pipeline?
4. Are you avoiding infinite recursion in event handlers?

---

## Contributing

When documenting a new bug fix:
1. Create a detailed markdown file in `docs/bugfixes/`
2. Add a summary entry to this INDEX.md
3. Update the appropriate category sections
4. Add relevant keywords for searchability
5. Include the issue in the "Search Guide" if it fits a common pattern

### Template for New Bug Fix Documentation

```markdown
# [Bug Name] Bug Fix

## Problem Summary
[Brief description of the symptom]

## Root Cause
[What was actually wrong in the code]

## Solution
[How it was fixed]

## Files Modified
[List of files and changes]

## Testing
[How to verify the fix]

## Keywords
[Searchable terms]
```

---

## Version History

- **2025-01-XX**: Initial index created with 11 bug fix summaries
- Includes guides for event system, union types, and move mechanics
- Organized by category, symptom, component, and file
- **2025-01-XX**: Added Reflect Side Condition Display Fix (UI/perspective issue)
- **2025-01-XX**: Added Tailwind OnModifySpe Fix (VoidReturn causing IntRelayVar type mismatch)

### Fake Out Flinch Fix
**File**: `FakeOutFlinchFix.md`  
**Severity**: High  
**Systems Affected**: Secondary effects, volatile status application

**Problem**: When Fake Out was used, the flinch volatile status was not applied to the target, so the target could still move normally on the same turn.

**Root Cause**: In `RunMoveEffects`, when processing secondary effects, the code checked `moveData.VolatileStatus` for the volatile status to apply. However, for secondary effects, the `SecondaryEffect` object is stored in `move.HitEffect`, not in `moveData`. Since `moveData` is the `ActiveMove` itself (which doesn't have the secondary effect's properties set), the volatile status check failed and the flinch was never applied.

**Solution**:
- Modified `RunMoveEffects` to check both `moveData` properties AND `move.HitEffect` properties when applying effects
- Applied the same pattern to volatile status, boosts, and status condition application
- Used null-coalescing operator: `moveData.VolatileStatus ?? (move.HitEffect as HitEffect)?.VolatileStatus`

**Keywords**: `Fake Out`, `flinch`, `secondary effect`, `volatile status`, `RunMoveEffects`, `HitEffect`, `SecondaryEffect`, `move.HitEffect`

---

*Last Updated*: 2025-01-XX  
*Total Bug Fixes Documented*: 16  
*Reference Guides*: 1
