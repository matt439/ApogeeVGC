# Bug Fixes Index

This index provides summaries of all documented bug fixes in the ApogeeVGC project. Use this as a first reference to determine which detailed documentation is relevant to your current issue.

---

## Quick Reference by Category

### Event System Issues
- [Hadron Engine Bug Fix](#hadron-engine-bug-fix) - Ability OnStart handlers not executing during switch-in
- [Trick Room Bug Fix](#trick-room-bug-fix) - Field event handlers not mapped/invoked
- [Protect Stalling Mechanic Issue](#protect-stalling-mechanic-issue) - Move event handlers not mapped
- [Facade BasePower Event Parameter Fix](#facade-basepower-event-parameter-fix) - Int passed to RunEvent instead of IntRelayVar

### Union Type Handling
- [Protect Bug Fix](#protect-bug-fix) - IsZero() logic error treating false as zero
- [Leech Seed Bug Fix](#leech-seed-bug-fix) - Undefined check occurring after .ToInt() conversion
- [Tailwind OnModifySpe Fix](#tailwind-onmodifyspe-fix) - VoidReturn causing IntRelayVar type mismatch
- [Stat Modification Handler VoidReturn Fix](#stat-modification-handler-voidreturn-fix) - Multiple stat handlers returning VoidReturn instead of int
- [Stat Modification Parameter Nullability Fix](#stat-modification-parameter-nullability-fix) - Incorrect nullability constraints on stat modification parameters
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

### Battle Lifecycle
- [Battle End Condition Null Request Fix](#battle-end-condition-null-request-fix) - Null request passed to player after battle ends
- [Endless Battle Loop Fix](#endless-battle-loop-fix) - Infinite loop when active Pokemon fainted without proper switch handling
- [Sync Simulator Request After Battle End Fix](#sync-simulator-request-after-battle-end-fix) - RequestPlayerChoices called after battle ended during request generation

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

### Stat Modification Handler VoidReturn Fix
**File**: `StatModificationHandlerVoidReturnFix.md`  
**Severity**: Critical  
**Systems Affected**: Stat modification handlers, abilities, items, conditions

**Problem**: Battle ended immediately in a tie on Turn 1 with `InvalidOperationException: stat must be an IntRelayVar, but got VoidReturnRelayVar for Ironhands's Spe (Event: ModifySpe)`. The battle crashed during `CommitChoices` when calculating Pokemon stats for move order determination.

**Root Cause**: Stat modification handlers (`OnModifyAtk`, `OnModifyDef`, `OnModifySpA`, `OnModifySpD`, `OnModifySpe`) were incorrectly returning `VoidReturn()` in two scenarios:
1. When using `ChainModify()` to apply boosts - returned `VoidReturn()` instead of `FinalModify(stat)`
2. When conditions didn't apply - returned `VoidReturn()` instead of the unmodified stat value

**Affected Components**:
- **Abilities**: Hadron Engine (OnModifySpA), Guts (OnModifyAtk)
- **Conditions**: QuarkDrive volatile (all 5 stat handlers)
- **Items**: Choice Specs (OnModifySpA), Assault Vest (OnModifySpD)

**Solution**: Fixed all stat modification handlers to follow the correct pattern:
- Always return an integer value, never `VoidReturn()`
- Return `stat` when condition doesn't apply (unmodified value)
- Return `battle.FinalModify(stat)` when using `ChainModify()`

**Key Rule**: Stat modification handlers MUST ALWAYS return an integer, never `VoidReturn()`.

**Keywords**: `stat modification`, `OnModifyAtk`, `OnModifyDef`, `OnModifySpA`, `OnModifySpD`, `OnModifySpe`, `VoidReturn`, `IntRelayVar`, `ChainModify`, `FinalModify`, `Hadron Engine`, `Quark Drive`, `Choice Specs`, `Assault Vest`, `Guts`, `type mismatch`

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
- [Battle End Condition Null Request Fix](#battle-end-condition-null-request-fix) - Null request after battle ends
- [Sync Simulator Request After Battle End Fix](#sync-simulator-request-after-battle-end-fix) - RequestPlayerChoices called after battle ended
- [Leech Seed Bug Fix](#leech-seed-bug-fix) - Undefined to int conversion
- [TrySetStatus Logic Error](#trysetstatus-logic-error) - Dictionary key not found
- [Wild Charge Recoil Fix](#wild-charge-recoil-fix) - Dictionary key not found for Recoil condition
- [Stat Modification Handler VoidReturn Fix](#stat-modification-handler-voidreturn-fix) - VoidReturn instead of int in stat handlers
- [Stat Modification Parameter Nullability Fix](#stat-modification-parameter-nullability-fix) - Null parameter when non-nullable expected
- [Facade BasePower Event Parameter Fix](#facade-basepower-event-parameter-fix) - Primitive int instead of IntRelayVar

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
- [Endless Battle Loop Fix](#endless-battle-loop-fix) - Battle runs for 1000+ turns

### By Component

**Event System**:
- [Hadron Engine Bug Fix](#hadron-engine-bug-fix)
- [Trick Room Bug Fix](#trick-room-bug-fix)
- [Protect Stalling Mechanic Issue](#protect-stalling-mechanic-issue)
- [Complete Draco Meteor Bug Fix](#complete-draco-meteor-bug-fix) (partial)
- [Facade BasePower Event Parameter Fix](#facade-basepower-event-parameter-fix)

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

**BattleActions.Damage.cs**:
- [Facade BasePower Event Parameter Fix](#facade-basepower-event-parameter-fix)

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

**Battle.Requests.cs**:
- [Battle End Condition Null Request Fix](#battle-end-condition-null-request-fix)
- [Endless Battle Loop Fix](#endless-battle-loop-fix)

**SidePlayerPerspective.cs / SideOpponentPerspective.cs**:
- [Reflect Side Condition Display Fix](#reflect-side-condition-display-fix)

**Side.Core.cs**:
- [Reflect Side Condition Display Fix](#reflect-side-condition-display-fix)

**PlayerConsole.cs**:
- [Reflect Side Condition Display Fix](#reflect-side-condition-display-fix)

**Conditions.cs**:
- [Wild Charge Recoil Fix](#wild-charge-recoil-fix)
- [Stat Modification Handler VoidReturn Fix](#stat-modification-handler-voidreturn-fix)

**Battle.Fainting.cs**:
- [Endless Battle Loop Fix](#endless-battle-loop-fix)

**Abilities.cs**:
- [Stat Modification Handler VoidReturn Fix](#stat-modification-handler-voidreturn-fix)

**Items.cs**:
- [Stat Modification Handler VoidReturn Fix](#stat-modification-handler-voidreturn-fix)

**Pokemon.Stats.cs**:
- [Stat Modification Handler VoidReturn Fix](#stat-modification-handler-voidreturn-fix)

**OnModify*EventInfo.cs files**:
- [Stat Modification Parameter Nullability Fix](#stat-modification-parameter-nullability-fix)

**Battle.Events.cs**:
- [Stat Modification Parameter Nullability Fix](#stat-modification-parameter-nullability-fix) (enhanced error logging)

**EventHandlerAdapter.cs**:
- [Stat Modification Parameter Nullability Fix](#stat-modification-parameter-nullability-fix) (enhanced error logging)

**SyncSimulator.cs**:
- [Sync Simulator Request After Battle End Fix](#sync-simulator-request-after-battle-end-fix)

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

### Pattern: Stat Modification Handler Issues
**See**: [Tailwind OnModifySpe Fix](#tailwind-onmodifyspe-fix), [Stat Modification Handler VoidReturn Fix](#stat-modification-handler-voidreturn-fix), [Stat Modification Parameter Nullability Fix](#stat-modification-parameter-nullability-fix)

**Checklist**:
1. Does the handler ALWAYS return an integer (never `VoidReturn()`)?
2. When condition doesn't apply, does it return the unmodified stat value?
3. When using `ChainModify()`, does it return `battle.FinalModify(stat)`?
4. Are you using the correct parameter name (`stat`/`atk`/`def`/etc, not `_`)?
5. Are the parameter nullability settings correct in the EventHandlerInfo?
6. Have you tested with different Pokemon/items/abilities that trigger the handler?

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
- **2025-01-XX**: Added Stat Modification Handler VoidReturn Fix (comprehensive fix for all stat handlers returning VoidReturn)
- **2025-01-XX**: Added Battle End Condition Null Request Fix (null request passed after battle ends mid-loop)
- **2025-01-19**: Added Endless Battle Loop Fix (infinite loop when active Pokemon fainted without proper switch handling)
- **2025-01-19**: Added Sync Simulator Request After Battle End Fix (RequestPlayerChoices called after battle ended during request generation)

### Stat Modification Parameter Nullability and Type Conversion Fix
**File**: `StatModificationParameterNullabilityFix.md`  
**Severity**: Critical  
**Systems Affected**: Stat modification handler chaining, damage calculation, event parameter resolution

**Problem**: Battle ended in a tie with `Event ModifySpA adapted handler failed on effect Choice Specs (Item)` when Miraidon (with both Hadron Engine and Choice Specs) used Dazzling Gleam. The error occurred when the second stat modification handler tried to execute after the first one completed.

**Root Causes**:
1. **Primary**: `EventHandlerAdapter.TryUnwrapRelayVar` couldn't convert `DecimalRelayVar` ? `int`. When handlers chain, the first handler returns `DecimalRelayVar`, but the second handler expects `int` parameters.
2. **Secondary**: Parameters were marked non-nullable but could legitimately be null in some contexts.

**Solution**: 
1. Added `DecimalRelayVar` ? `int` conversion support in `EventHandlerAdapter.TryUnwrapRelayVar`
2. Updated `ParameterNullability` arrays to mark source Pokemon and move as nullable

**Files Modified**:
- `EventHandlerAdapter.cs` - Added DecimalRelayVar ? int conversion
- `OnModifyAtkEventInfo.cs` - Allowed null target Pokemon and move
- `OnModifyDefEventInfo.cs` - Allowed null source Pokemon and move  
- `OnModifySpAEventInfo.cs` - Fixed incorrect int nullability and allowed null target/move
- `OnModifySpDEventInfo.cs` - Allowed null source Pokemon and move

**Key Insight**: Stat modification handlers return `DoubleVoidUnion` (converted to `DecimalRelayVar`) but expect `int` parameters. When chaining multiple handlers, type conversion is required between each handler's output and the next handler's input.

**Impact**: Enables multiple stat-modifying effects to stack correctly (abilities + items, multiple abilities, etc.)

**Keywords**: `stat modification`, `handler chaining`, `parameter nullability`, `type conversion`, `DecimalRelayVar`, `OnModifySpA`, `Choice Specs`, `Hadron Engine`, `Assault Vest`, `EventHandlerAdapter`, `TryUnwrapRelayVar`, `damage calculation`, `InvalidOperationException`, `adapted handler failed`, `parameter resolution`

---

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

### Battle End Condition Null Request Fix
**File**: `BattleEndConditionNullRequestFix.md`  
**Severity**: High  
**Systems Affected**: Battle lifecycle, request/choice system, win condition detection

**Problem**: When one side's last Pokémon fainted, the battle attempted to request moves from the losing player (who had no Pokémon left), causing `ArgumentNullException: Choice request cannot be null`. The battle correctly detected the win condition but continued to process choice requests.

**Root Causes**:
1. **Request Assignment Before End Check**: Requests were assigned to all sides (including WaitRequest for loser) before detecting the battle should end
2. **Loop Continuation After Battle End**: `RequestPlayerChoices` was iterating through sides; when processing the first side triggered battle end, the loop continued to the next side
3. **Missing Mid-Loop Exit Check**: No check for `Ended` between processing each side in the request loop

**Solutions**:
1. Clear `RequestState` and all `ActiveRequest`s when `MakeRequest` detects battle end after generating WaitRequests
2. **Critical Fix**: Added `if (Ended)` check inside `RequestPlayerChoices` loop before processing each side
3. Existing early return in `MakeRequest` if battle already ended

**Key Insight**: Even in "synchronous" code, event-driven architecture creates deep call nesting. When `RequestPlayerChoices` calls an event that triggers battle end, control unwinds back to the loop which must check if state changed before continuing iteration.

**Call Flow**: SyncSimulator.Run ? RequestPlayerChoices ? RequestPlayerChoice(P1) ? OnChoiceRequested ? Choose ? CommitChoices ? TurnLoop ? FaintMessages ? CheckWin (sets Ended=true) ? unwind to RequestPlayerChoices loop ? [FIX] check Ended before RequestPlayerChoice(P2)

**Keywords**: `battle end`, `win condition`, `null request`, `ArgumentNullException`, `MakeRequest`, `RequestPlayerChoices`, `ActiveRequest`, `RequestState`, `fainting`, `CheckWin`, `event loop`, `mid-loop exit`, `state consistency`

---

### Endless Battle Loop Fix
**File**: `EndlessBattleLoopFix.md`  
**Severity**: Critical  
**Systems Affected**: Battle lifecycle, request generation, win condition detection

**Problem**: Battle entered an infinite loop when a side had all active Pokémon fainted but still had Pokémon in reserve. The battle continued for 1000+ turns with one player receiving `WaitRequest` while the other switched Pokémon indefinitely, eventually throwing `BattleTurnLimitException`.

**Root Cause**: Active Pokémon fainted but weren't properly processed through `FaintMessages()` and `CheckFainted()`, so `SwitchFlag` was never set. This caused `EndTurn()` to call `MakeRequest(RequestState.Move)` instead of detecting that switches were needed. `GetRequests()` then created a `WaitRequest` for the side with no active Pokémon but didn't check if the battle should end.

**Solution**: 
1. **GetRequests()**: When detecting a side has no active non-fainted Pokémon, force switch flags on fainted Pokémon, check available switches, and call `Lose(side)` if no switches available
2. **MakeRequest()**: Check if battle ended during `GetRequests()` and return early if so
3. **CheckFainted()**: Added validation logging to detect problematic states

**Key Insight**: This is a defensive recovery mechanism that handles cases where earlier battle logic failed to properly process faints. The fix forces the battle into the correct state and ends it properly if a side has truly lost.

**Debug Pattern**: Look for warnings:
- `WARNING: {side} has no active non-fainted Pokemon during move request phase`
- `ERROR: {side} has no active non-fainted Pokemon during move request phase`

**Keywords**: `endless loop`, `infinite battle`, `turn limit`, `BattleTurnLimitException`, `WaitRequest`, `fainted Pokemon`, `switch flag`, `move request`, `battle state`, `win condition`, `defensive programming`, `error recovery`

---

### Sync Simulator Request After Battle End Fix
**File**: `SyncSimulatorRequestAfterBattleEndFix.md`  
**Severity**: High  
**Systems Affected**: Synchronous simulation, battle lifecycle, request/choice system

**Problem**: When running a synchronous battle test (`SyncSimulator`), the battle crashed with `InvalidOperationException: Cannot request choices from players: Some sides have no active request` after one side's last Pokémon fainted. This occurred when `GetRequests()` detected a win condition and called `Lose()`, but the simulator still attempted to request player choices.

**Root Cause**: In `SyncSimulator.Run()`, the main loop checks `if (Battle.RequestState != RequestState.None)` before calling `RequestPlayerChoices()`. When `GetRequests()` calls `Lose()` during request generation, it sets `Ended = true` but doesn't clear `RequestState`. The simulator checks for pending requests before checking if the battle ended during request generation, causing it to attempt requesting choices with no active requests.

**Solution**: Added a battle-ended check in `SyncSimulator.Run()` immediately before calling `RequestPlayerChoices()`. This prevents attempting to request choices when the battle ended during request generation.

**Key Insight**: `RequestState` can be set before the battle ends during request generation. The simulator must check both `RequestState != None` AND `!Ended` before attempting to request choices.

**Impact**: Fixes crashes in all synchronous battle simulations when win conditions are detected during `GetRequests()`.

**Keywords**: `SyncSimulator`, `RequestPlayerChoices`, `InvalidOperationException`, `no active request`, `battle end`, `GetRequests`, `Lose`, `RequestState`, `battle lifecycle`, `synchronous simulation`, `win condition`, `request generation`, `state consistency`, `defensive programming`

---

### Facade BasePower Event Parameter Fix
**File**: `FacadeBasePowerEventParameterFix.md`  
**Severity**: High  
**Systems Affected**: Event system, damage calculation, BasePower events, handler chaining

**Problem**: When Ursaluna (with Burn status) used Facade, the battle crashed with `Event BasePower adapted handler failed` and inner exception `Parameter 1 (Int32 _) is non-nullable but no matching value found in context`. The error occurred on the second handler invocation when multiple handlers existed for the `BasePower` event.

**Root Causes**: 
1. **Primary**: In `Battle.Events.cs`, the `RunEvent` method unconditionally replaced `relayVar` with handler return values. When a handler returned `VoidReturnRelayVar`, it overwrote the `IntRelayVar` containing the base power value, breaking the handler chain.
2. **Secondary**: In `BattleActions.Damage.cs`, the `BasePower` event was called with a primitive `int` instead of wrapping it in an `IntRelayVar`.

**Solution**: 
1. Modified `RunEvent` to check if return value is `VoidReturnRelayVar` and preserve the original `relayVar` in that case (VoidReturn means "no value change, just side effects")
2. Changed to pass `new IntRelayVar(basePower.ToInt())` instead of the raw int value

**Key Patterns**: 
- **RelayVar Passing**: Always wrap primitive values in appropriate RelayVar types when calling `RunEvent`
- **VoidReturn Handling**: Don't replace `relayVar` when handlers return `VoidReturnRelayVar` - it means "I used ChainModify() for side effects but have no replacement value"

**Impact**: Enables all moves with `OnBasePower` handlers to execute correctly and allows multiple handlers to chain properly for any event using the ChainModify() + VoidReturn pattern.

**Keywords**: `Facade`, `BasePower`, `event parameter`, `RelayVar`, `IntRelayVar`, `VoidReturn`, `VoidReturnRelayVar`, `EventHandlerAdapter`, `parameter resolution`, `non-nullable`, `type mismatch`, `GetDamage`, `RunEvent`, `damage calculation`, `handler chaining`, `ChainModify`

---

*Last Updated*: 2025-01-19  
*Total Bug Fixes Documented*: 22  
*Reference Guides*: 1
