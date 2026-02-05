# Bug Fixes Index

This index provides summaries of all documented bug fixes in the ApogeeVGC project. Use this as a first reference to determine which detailed documentation is relevant to your current issue.

---

## Quick Reference by Category

### Event System Issues
- [Hadron Engine Bug Fix](#hadron-engine-bug-fix) - Ability OnStart handlers not executing during switch-in
- [Trick Room Bug Fix](#trick-room-bug-fix) - Field event handlers not mapped/invoked
- [Protect Stalling Mechanic Issue](#protect-stalling-mechanic-issue) - Move event handlers not mapped
- [Facade BasePower Event Parameter Fix](#facade-basepower-event-parameter-fix) - Int passed to RunEvent instead of IntRelayVar
- [Immunity Event Parameter Conversion Fix](#immunity-event-parameter-conversion-fix) - ConditionIdRelayVar not converted to PokemonTypeConditionIdUnion
- [Condition to Ability Cast Fix](#condition-to-ability-cast-fix) - InvalidCastException when trying to cast Condition to Ability
- [ModifyAccuracy Event Parameter Nullability Fix](#modifyaccuracy-event-parameter-nullability-fix) - Int accuracy parameter cannot handle always-hit moves
- [Accuracy Event Parameter Nullability Fix](#accuracy-event-parameter-nullability-fix) - Int accuracy parameter cannot handle always-hit moves (Lock-On)
- [Effectiveness Event PokemonType Parameter Fix](#effectiveness-event-pokemontype-parameter-fix) - PokemonType parameter not resolved in event context
- [Ripen Ability Null Effect Fix](#ripen-ability-null-effect-fix) - NullReferenceException when effect parameter is null in OnTryHeal handler
- [Disguise Ability Null Effect Fix](#disguise-ability-null-effect-fix) - NullReferenceException when effect parameter is null in OnDamage handler
- [Adrenaline Orb Null Effect Fix](#adrenaline-orb-null-effect-fix) - NullReferenceException when effect parameter is null in OnAfterBoost handler
- [Berserk Ability Null Effect Fix](#berserk-ability-null-effect-fix) - NullReferenceException when effect parameter is null in OnDamage handler
- [Grassy Glide ModifyPriority Source Fix](#grassy-glide-modifypriority-source-fix) - NullReferenceException when source parameter is null in OnModifyPriority handler
- [Wind Rider Null SideCondition Fix](#wind-rider-null-sidecondition-fix) - NullReferenceException when sideCondition parameter is null in OnSideConditionStart handler
- [Big Root Null Effect Fix](#big-root-null-effect-fix) - NullReferenceException when effect parameter is null in OnTryHeal handler

### Union Type Handling
- [Protect Bug Fix](#protect-bug-fix) - IsZero() logic error treating false as zero
- [Leech Seed Bug Fix](#leech-seed-bug-fix) - Undefined check occurring after .ToInt() conversion
- [Tailwind OnModifySpe Fix](#tailwind-onmodifyspe-fix) - VoidReturn causing IntRelayVar type mismatch
- [Stat Modification Handler VoidReturn Fix](#stat-modification-handler-voidreturn-fix) - Multiple stat handlers returning VoidReturn instead of int
- [Stat Modification Parameter Nullability Fix](#stat-modification-parameter-nullability-fix) - Incorrect nullability constraints on stat modification parameters
- [MoveIdVoidUnion Return Conversion Fix](#moveidvoidunion-return-conversion-fix) - VoidMoveIdVoidUnion cannot be converted to RelayVar
- [VoidFalseUnion Return Conversion Fix](#voidfalseunion-return-conversion-fix) - VoidVoidFalseUnion cannot be converted to RelayVar
- [IntBoolUnion Return Conversion Fix](#intboolunion-return-conversion-fix) - IntIntBoolUnion cannot be converted to RelayVar
- [SparseBoostsTableVoidUnion Return Conversion Fix](#sparsebooststablevoidunion-return-conversion-fix) - VoidSparseBoostsTableVoidUnion cannot be converted to RelayVar
- [Union Type Handling Guide](#union-type-handling-guide) - Comprehensive guide for preventing union type issues

### Move Mechanics
- [Volt Switch Damage Type Bug](#volt-switch-damage-type-bug) - Type priority overwriting damage values
- [Headlong Rush Self-Stat Drops Fix](#headlong-rush-self-stat-drops-fix) - HitEffect not being stored for self-targeting effects
- [Complete Draco Meteor Bug Fix](#complete-draco-meteor-bug-fix) - Multiple interconnected issues with self-targeting moves
- [Self-Drops Infinite Recursion Fix](#self-drops-infinite-recursion-fix) - Infinite loop in damage calculation for self-stat changes
- [Spirit Break Secondary Effect Fix](#spirit-break-secondary-effect-fix) - Secondary property not converted to Secondaries array
- [Steel Beam & Mind Blown Recoil Fix](#steel-beam--mind-blown-recoil-fix) - MoveId does not have corresponding ConditionId for recoil damage effect

### Status Conditions
- [TrySetStatus Logic Error](#trysetstatus-logic-error) - Incorrect conditional logic when applying status
- [Wild Charge Recoil Fix](#wild-charge-recoil-fix) - Missing Recoil condition dictionary entry

### Type Conversion & Stat System
- [BoostId To String Evasion/Accuracy Conversion Fix](#boostid-to-string-evasionaccuracy-conversion-fix) - ArgumentOutOfRangeException when converting Evasion/Accuracy BoostId to string

### UI and Display
- [Reflect Side Condition Display Fix](#reflect-side-condition-display-fix) - Side conditions not visible in console UI
- [GUI Team Preview Fix](#gui-team-preview-fix) - No Pokémon displayed during GUI team preview


### Battle Lifecycle
- [Collection Modified During Enumeration Fix](#collection-modified-during-enumeration-fix) - InvalidOperationException when side.Active is modified during foreach iteration
- [Battle End Condition Null Request Fix](#battle-end-condition-null-request-fix) - Null request passed to player after battle ends
- [Endless Battle Loop Fix](#endless-battle-loop-fix) - Infinite loop when active Pokemon fainted without proper switch handling
- [Sync Simulator Request After Battle End Fix](#sync-simulator-request-after-battle-end-fix) - RequestPlayerChoices called after battle ended during request generation
- [Player 2 Always Wins Bug Fix](#player-2-always-wins-bug-fix) - Winner detection comparing player names against side IDs incorrectly
- [Player Random Doubles Targeting Fix](#player-random-doubles-targeting-fix) - Random player always returning invalid target location 0 for targeting moves in doubles

### Choice System
- [Pokemon Position Index Mismatch Fix](#pokemon-position-index-mismatch-fix) - ArgumentOutOfRangeException when using pokemon.Position to index into Active-sized arrays

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

### MoveIdVoidUnion Return Conversion Fix
**File**: `MoveIdVoidUnionReturnConversionFix.md`  
**Severity**: High  
**Systems Affected**: Two-turn moves, LockMove event handlers

**Problem**: When a Pokemon used a two-turn move (Fly, Dig, Dive, Skull Bash, Solar Beam, Sky Drop), the battle crashed with `InvalidOperationException: Event LockMove: Unable to convert return value of type 'VoidMoveIdVoidUnion' to RelayVar`.

**Root Cause**: The `TwoTurnMove` condition's `OnLockMove` handler returns `MoveIdVoidUnion`, which can be either `MoveIdMoveIdVoidUnion` (containing a MoveId) or `VoidMoveIdVoidUnion` (containing VoidReturn). The `EventHandlerAdapter.ConvertReturnValue` method had cases for many union types but was missing the case for `MoveIdVoidUnion`.

**Solution**: Added conversion cases for both variants of `MoveIdVoidUnion`:
- `MoveIdMoveIdVoidUnion` ? `MoveIdRelayVar`
- `VoidMoveIdVoidUnion` ? `VoidReturnRelayVar`

**Pattern**: Event handlers that return union types need explicit conversion cases in `EventHandlerAdapter.ConvertReturnValue` to unwrap the variant and convert it to the appropriate `RelayVar` type.

**Keywords**: `two-turn move`, `LockMove event`, `MoveIdVoidUnion`, `event handler return`, `union type conversion`, `Fly`, `Dig`, `Dive`, `Skull Bash`, `Solar Beam`, `Sky Drop`

---

### VoidFalseUnion Return Conversion Fix
**File**: `VoidFalseUnionReturnConversionFix.md`  
**Severity**: High  
**Systems Affected**: Move-blocking conditions (Throat Chop, Taunt, etc.), ModifyMove event handlers

**Problem**: When a Pokemon with the Throat Chop condition tried to use a sound-based move, the battle crashed with `InvalidOperationException: Event ModifyMove: Unable to convert return value of type 'VoidVoidFalseUnion' to RelayVar`.

**Root Cause**: The `ThroatChop` condition's `OnModifyMove` handler returns `VoidFalseUnion`, which can be either `VoidVoidFalseUnion` (VoidReturn, allow move) or `FalseVoidFalseUnion` (false, block move). The `EventHandlerAdapter.ConvertReturnValue` method had cases for many union types but was missing the case for `VoidFalseUnion`.

**Solution**: Added conversion cases for both variants of `VoidFalseUnion`:
- `VoidVoidFalseUnion` ? `VoidReturnRelayVar` (no effect, allow move)
- `FalseVoidFalseUnion` ? `BoolRelayVar(false)` (block move)

**Semantic Meaning**: Following copilot instructions - `VoidReturn` = "no effect" (move proceeds), `false` = explicit block signal.

**Keywords**: `Throat Chop`, `sound move`, `move blocking`, `VoidFalseUnion`, `ModifyMove event`, `event handler return`, `union type conversion`, `Taunt`, `Torment`, `Disable`

---

### IntBoolUnion Return Conversion Fix
**File**: `IntBoolUnionReturnConversionFix.md`  
**Severity**: High  
**Systems Affected**: Healing events, berry interactions, Ripen ability

**Problem**: When a Pokemon with the Ripen ability had Berry Juice (or any berry) trigger healing, the battle crashed with `InvalidOperationException: Event TryHeal: Unable to convert return value of type 'IntIntBoolUnion' to RelayVar`.

**Root Cause**: The Ripen ability's `OnTryHeal` handler returns `IntBoolUnion`, which can be either `IntIntBoolUnion` (containing an int heal amount) or `BoolIntBoolUnion` (containing a bool success/failure). The `EventHandlerAdapter.ConvertReturnValue` method had cases for many union types but was missing the case for `IntBoolUnion`.

**Solution**: Added conversion cases for both variants of `IntBoolUnion`:
- `IntIntBoolUnion` ? `IntRelayVar` (preserves the integer heal amount)
- `BoolIntBoolUnion` ? `BoolRelayVar` (preserves the boolean value)

**Pattern**: Event handlers that return union types need explicit conversion cases in `EventHandlerAdapter.ConvertReturnValue` to unwrap the variant and convert it to the appropriate `RelayVar` type.

**Keywords**: `Ripen`, `IntBoolUnion`, `IntIntBoolUnion`, `BoolIntBoolUnion`, `TryHeal event`, `event handler return`, `union type conversion`, `berry`, `healing`, `Berry Juice`, `Leftovers`

---

### SparseBoostsTableVoidUnion Return Conversion Fix
**File**: `SparseBoostsTableVoidUnionReturnConversionFix.md`  
**Severity**: High  
**Systems Affected**: ModifyBoost events, stat boost modifications, Unaware ability

**Problem**: When a Pokémon with the Unaware ability was switching in, the battle crashed with `InvalidOperationException: Event ModifyBoost adapted handler failed on effect Unaware (Ability)` with inner exception `Unable to convert return value of type 'VoidSparseBoostsTableVoidUnion' to RelayVar`.

**Root Cause**: The Unaware ability's `OnAnyModifyBoost` handler returns `SparseBoostsTableVoidUnion`, which can be either `SparseBoostsTableSparseBoostsTableVoidUnion` (containing a `SparseBoostsTable` with modified boosts) or `VoidSparseBoostsTableVoidUnion` (indicating no modification). The `EventHandlerAdapter.ConvertReturnValue` method had cases for many union types but was missing the case for `SparseBoostsTableVoidUnion`.

**Solution**: Added conversion cases for both variants of `SparseBoostsTableVoidUnion`:
- `SparseBoostsTableSparseBoostsTableVoidUnion` ? `SparseBoostsTableRelayVar` (preserves the modified boost table)
- `VoidSparseBoostsTableVoidUnion` ? `VoidReturnRelayVar` (indicates no modification)

**Pattern**: Event handlers that return union types need explicit conversion cases in `EventHandlerAdapter.ConvertReturnValue` to unwrap the variant and convert it to the appropriate `RelayVar` type. The `SparseBoostsTableRelayVar` type already existed in the codebase.

**Keywords**: `Unaware`, `SparseBoostsTableVoidUnion`, `SparseBoostsTableSparseBoostsTableVoidUnion`, `VoidSparseBoostsTableVoidUnion`, `ModifyBoost event`, `OnAnyModifyBoost`, `event handler return`, `union type conversion`, `stat boosts`, `EventHandlerAdapter`, `SparseBoostsTableRelayVar`, `switch-in`

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
- [MoveIdVoidUnion Return Conversion Fix](#moveidvoidunion-return-conversion-fix) - VoidMoveIdVoidUnion cannot be converted to RelayVar
- [VoidFalseUnion Return Conversion Fix](#voidfalseunion-return-conversion-fix) - VoidVoidFalseUnion cannot be converted to RelayVar
- [IntBoolUnion Return Conversion Fix](#intboolunion-return-conversion-fix) - IntIntBoolUnion cannot be converted to RelayVar
- [Pokemon Position Index Mismatch Fix](#pokemon-position-index-mismatch-fix) - ArgumentOutOfRangeException when using pokemon.Position

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
- [Player Random Doubles Targeting Fix](#player-random-doubles-targeting-fix) - Random player repeatedly failing move validation in doubles

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
- [Immunity Event Parameter Conversion Fix](#immunity-event-parameter-conversion-fix)
- [MoveIdVoidUnion Return Conversion Fix](#moveidvoidunion-return-conversion-fix)
- [VoidFalseUnion Return Conversion Fix](#voidfalseunion-return-conversion-fix)
- [IntBoolUnion Return Conversion Fix](#intboolunion-return-conversion-fix)
- [SparseBoostsTableVoidUnion Return Conversion Fix](#sparsebooststablevoidunion-return-conversion-fix)

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

**ItemsABC.cs**:
- [Big Root Null Effect Fix](#big-root-null-effect-fix)

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

**PlayerRandom.cs**:
- [Player Random Doubles Targeting Fix](#player-random-doubles-targeting-fix)

**Battle.Requests.cs**:
- [GUI Team Preview Fix](#gui-team-preview-fix)

---

### GUI Team Preview Fix
**File**: `GuiTeamPreviewFix.md`  
**Severity**: High  
**Systems Affected**: GUI rendering, battle perspectives, team preview

**Problem**: When running GUI battles, no Pokémon were displayed during the team preview phase. The screen showed an empty battlefield with no Pokémon sprites or information.

**Root Cause**: The `BattleRenderer` was receiving perspectives with the wrong `BattlePerspectiveType`. When `RequestPlayerChoices()` was called during team preview, it called `UpdateAllPlayersUi()` without parameters, which defaulted to `BattlePerspectiveType.InBattle` instead of `TeamPreview`. This caused the renderer to use `RenderInBattle()` (which only shows active Pokémon) instead of `RenderTeamPreview()` (which shows the full team).

**Solution**: Modified `Battle.Requests.RequestPlayerChoices()` to determine the correct perspective type based on `RequestState` and pass it explicitly to `UpdateAllPlayersUi()`.

**Keywords**: `GUI`, `team preview`, `BattleRenderer`, `BattlePerspective`, `perspective type`, `UpdateAllPlayersUi`, `RequestPlayerChoices`, `RenderTeamPreview`, `default parameter`

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

---

### ModifyAccuracy Event Parameter Nullability Fix
**File**: `ModifyAccuracyEventParameterNullabilityFix.md`  
**Severity**: High  
**Systems Affected**: ModifyAccuracy event handlers (abilities like Hustle, Compound Eyes; items like Wide Lens, Zoom Lens)

**Problem**: The `ModifyAccuracy` event handler signature declared the `accuracy` parameter as `int` (non-nullable), but moves with true accuracy (always-hit moves) pass `BoolRelayVar(true)` instead of `IntRelayVar`. This caused parameter resolution to fail when abilities tried to process always-hit moves.

**Root Cause**: TypeScript allows `accuracy: number | true`, but C# signature used `int` (non-nullable). TypeScript handlers check `typeof accuracy === 'number'` before modifying, but C# handlers had no way to distinguish between numeric accuracy and always-hit moves.

**Solution**:
- Changed all `ModifyAccuracy` event handler signatures to use `int?` (nullable int)
- Updated `EventHandlerAdapter.TryUnwrapRelayVar` to convert `BoolRelayVar(true)` ? `null` for `int?` parameters
- Updated all handler implementations (Hustle, Compound Eyes, Sand Veil, Snow Cloak, Tangled Feet, Victory Star, Wide Lens, Zoom Lens, Bright Powder) to check `accuracy.HasValue` before modifying

**Semantic Mapping**:
- TypeScript `typeof accuracy === 'number'` ? C# `accuracy.HasValue`
- TypeScript `true` (always hit) ? C# `null`

**Keywords**: `ModifyAccuracy`, `accuracy`, `nullable`, `int?`, `Hustle`, `event parameter`, `type mismatch`, `BoolRelayVar`, `always-hit moves`

---

### Accuracy Event Parameter Nullability Fix
**File**: `AccuracyEventParameterNullabilityFix.md`  
**Severity**: High  
**Systems Affected**: Accuracy event handlers (all prefixes: Base, Source, Foe, Ally, Any)

**Problem**: The `Accuracy` event handler signatures declared the `accuracy` parameter as `int` (non-nullable), but moves with true accuracy (always-hit moves) pass `BoolRelayVar(true)` instead of `IntRelayVar`. This caused the Lock-On condition's `OnSourceAccuracy` handler to crash when triggered for always-hit moves.

**Root Cause**: Identical to ModifyAccuracy issue - TypeScript allows `accuracy: number | true`, but C# signatures used `int` (non-nullable). The `BattleActions.HitStepAccuracy` method passes `RelayVar.FromIntTrueUnion(accuracy)` which can be either `IntRelayVar` or `BoolRelayVar(true)`.

**Solution**:
- Changed all `Accuracy` event handler signatures (OnAccuracy, OnSourceAccuracy, OnFoeAccuracy, OnAllyAccuracy, OnAnyAccuracy) to use `int?` (nullable int)
- EventHandlerAdapter already supported `BoolRelayVar(true)` ? `null` conversion from ModifyAccuracy fix
- Updated handler implementations (Micle Berry, Minimize) to check `accuracy.HasValue` before using `.Value`
- Lock-On handler unchanged (uses discard parameter `_`)

**Semantic Mapping**:
- TypeScript `typeof accuracy === 'number'` ? C# `accuracy.HasValue`
- TypeScript `true` (always hit) ? C# `null`

**Keywords**: `Accuracy`, `accuracy`, `nullable`, `int?`, `Lock-On`, `event parameter`, `type mismatch`, `BoolRelayVar`, `always-hit moves`, `Micle Berry`, `Minimize`

---

### Effectiveness Event PokemonType Parameter Fix
**File**: `EffectivenessEventPokemonTypeParameterFix.md`  
**Severity**: High  
**Systems Affected**: Effectiveness event handlers (items like Iron Ball, abilities that modify type matchups)

**Problem**: When Iron Ball item's Effectiveness event handler was invoked during type effectiveness calculation, the event system crashed with `InvalidOperationException: Event Effectiveness: Parameter 3 (PokemonType _) is non-nullable but no matching value found in context`.


**Root Cause**: The Effectiveness event is called from `Pokemon.RunEffectiveness` with a `PokemonType` as the event source to represent the type being checked for effectiveness. While `RunEventSource` and `SingleEventSource` both supported `PokemonType` through their respective type source records (`TypeRunEventSource` and `PokemonTypeSingleEventSource`), the event context system wasn't properly handling it:
1. `Battle.RunEvent` wasn't converting `TypeRunEventSource` to `PokemonTypeSingleEventSource`
2. `EventContext` had no field for `SourceType`
3. `EventInvocationContext.ToEventContext` wasn't extracting `PokemonType` from the source
4. `EventHandlerAdapter.ResolveParameter` had no logic to resolve `PokemonType` parameters

**Solution**:
- Added `SourceType` property to `EventContext` to hold `PokemonType` sources
- Updated `Battle.RunEvent` to convert `TypeRunEventSource` ? `PokemonTypeSingleEventSource`
- Updated `EventInvocationContext.ToEventContext` to extract `PokemonType` from `PokemonTypeSingleEventSource`
- Added `PokemonType` resolution logic to `EventHandlerAdapter.ResolveParameter`

**Handler Example** (Iron Ball):
```csharp
OnEffectiveness = new OnEffectivenessEventInfo((battle, typeMod, target, type, move) =>
{
    if (move.Type == MoveType.Ground && target.HasType(PokemonType.Flying))
        return 0; // Ground moves hit Flying-types holding Iron Ball
    return typeMod;
}),
```

**Impact**: Effectiveness event handlers can now properly receive and use the `PokemonType` parameter to modify type matchup calculations.

**Keywords**: `Effectiveness`, `type effectiveness`, `PokemonType`, `Iron Ball`, `event parameter`, `event source`, `TypeRunEventSource`, `parameter resolution`, `type matchup`

---

### Ripen Ability Null Effect Fix
**File**: `RipenNullEffectFix.md`  
**Severity**: High  
**Systems Affected**: Ripen ability `OnTryHeal` event handler

**Problem**: The Ripen ability crashed with a `NullReferenceException` when trying to access `effect.EffectStateId` in its `OnTryHeal` handler. The error occurred when a Pokémon with Ripen was healed from sources that don't have an associated `IEffect` (e.g., natural regeneration, certain abilities).

**Root Cause**: The handler did not check if the `effect` parameter was null before accessing its properties. The TypeScript reference implementation includes an explicit null check (`if (!effect) return;`) that was missing from the C# port.

**Solution**: Added a null guard at the beginning of the `OnTryHeal` handler:
```csharp
if (effect == null)
{
    return IntBoolUnion.FromInt(damage);
}
```

**Impact**: When `effect` is null, the healing proceeds unchanged without activating Ripen's berry-doubling logic, matching the correct TypeScript behavior.

**Keywords**: `ripen`, `ability`, `OnTryHeal`, `null reference`, `effect parameter`, `berry`, `healing`, `item`, `null check`

---

### Disguise Ability Null Effect Fix
**File**: `DisguiseNullEffectFix.md`  
**Severity**: High  
**Systems Affected**: Disguise ability `OnDamage` event handler

**Problem**: The Disguise ability crashed with a `NullReferenceException` when trying to access `effect.EffectType` in its `OnDamage` handler. The error occurred when Mimikyu took damage from sources that don't have an associated `IEffect` (e.g., confusion self-damage, recoil, weather damage, status conditions).

**Root Cause**: The handler did not check if the `effect` parameter was null before accessing its properties. The TypeScript reference implementation uses optional chaining (`effect?.effectType`) that was missing from the C# port.

**Solution**: Added a null guard at the beginning of the handler:
```csharp
if (effect != null &&
    effect.EffectType == EffectType.Move &&
    target.Species.Id is SpecieId.Mimikyu or SpecieId.MimikyuTotem)
{
    // Activate Disguise
}
```

**Impact**: When `effect` is null, the damage proceeds unchanged without activating Disguise's protection, matching the correct TypeScript behavior where Disguise only protects against direct move attacks.

**Keywords**: `disguise`, `ability`, `OnDamage`, `null reference`, `effect parameter`, `Mimikyu`, `damage`, `null check`, `optional chaining`

---

### Adrenaline Orb Null Effect Fix
**File**: `AdrenalineOrbNullEffectFix.md`  
**Severity**: High  
**Systems Affected**: Adrenaline Orb item `OnAfterBoost` event handler

**Problem**: The Adrenaline Orb item crashed with a `NullReferenceException` when trying to access `effect.EffectStateId` in its `OnAfterBoost` handler. The error occurred when boost events were triggered without an associated effect (e.g., weather effects, terrain effects, field conditions).

**Root Cause**: The handler did not check if the `effect` parameter was null before accessing its `EffectStateId` property. Stat boosts can come from various sources that don't have an associated effect. The TypeScript reference directly accesses `effect.name` without a null check, but C# requires explicit null handling.

**Solution**: Added a null guard before checking the effect's identity:
```csharp
if (effect != null && effect.EffectStateId == AbilityId.Intimidate)
{
    target.UseItem();
}
```

**Impact**: When `effect` is null, the Adrenaline Orb doesn't activate, which is correct because the item is designed to only respond to the Intimidate ability. This prevents crashes while maintaining correct game mechanics.

**Keywords**: `adrenaline orb`, `item`, `OnAfterBoost`, `null reference`, `effect parameter`, `intimidate`, `stat boost`, `null check`

---

### Berserk Ability Null Effect Fix
**File**: `BerserkNullEffectFix.md`  
**Severity**: High  
**Systems Affected**: Berserk ability `OnDamage` event handler

**Problem**: The Berserk ability crashed with a `NullReferenceException` when trying to access `effect.EffectType` in its `OnDamage` handler at line 647 of `AbilitiesABC.cs`. The error occurred during random battle testing when a Pokémon with Berserk took damage from sources that don't have an associated `IEffect`.

**Root Cause**: The handler did not check if the `effect` parameter was null before accessing its `EffectType` property. Damage can come from various sources that don't have an associated effect (confusion self-damage, recoil, weather damage, status conditions). The TypeScript reference directly accesses `effect.effectType` without a null check, which works in JavaScript because accessing a property on `undefined` returns `undefined` (no error), but in C# it throws a `NullReferenceException`.

**Solution**: Added a null guard before checking the effect type:
```csharp
if (effect != null &&
    effect.EffectType == EffectType.Move &&
    effect is Move { MultiHit: null } move &&
    !(move.HasSheerForce == true && source != null &&
      source.HasAbility(AbilityId.SheerForce)))
{
    battle.EffectState.CheckedBerserk = false;
}
else
{
    battle.EffectState.CheckedBerserk = true;
}
```

**Impact**: When `effect` is null, `CheckedBerserk` is set to `true`, matching the TypeScript behavior where a null/undefined effect causes the condition to fail and fall into the else branch. This prevents crashes while maintaining correct game mechanics where Berserk tracks whether the current damage should allow berry consumption and trigger the Special Attack boost.

**Keywords**: `berserk`, `ability`, `OnDamage`, `null reference`, `effect parameter`, `damage`, `null check`, `TypeScript porting`, `CheckedBerserk`

---

### Collection Modified During Enumeration Fix
**File**: `CollectionModifiedDuringEnumerationFix.md`  
**Severity**: High  
**Systems Affected**: Battle lifecycle, switching mechanics, phazing

**Problem**: During random battle testing, the simulator threw `System.InvalidOperationException: Collection was modified; enumeration operation may not execute.` in the `RunAction` method at line 719. The error occurred when phazing moves (Roar, Dragon Tail, etc.) triggered forced switches.

**Root Cause**: The `RunAction` method had four `foreach` loops that iterated directly over `side.Active`:
1. Phazing loop (lines 716-729) 
2. Cancel fainted actions loop (lines 737-747)
3. Revival Blessing check loop (lines 788-804)
4. BeforeSwitchOut event loop (lines 821-843)

When `Actions.DragIn()` was called during the phazing loop, it called `BattleActions.SwitchIn()`, which modifies `side.Active[pos]` while the `foreach` loop was still iterating over the collection.

**Solution**: Created snapshot copies of `side.Active` before each foreach loop using C# 12 collection expression syntax `[.. side.Active]`. This ensures that even if the original collection is modified during iteration, the loop continues safely over the snapshot.

**Example Change**:
```csharp
// Before (unsafe):
foreach (Pokemon? pokemon in side.Active) { ... }

// After (safe):
Pokemon?[] activeSnapshot = [.. side.Active];
foreach (Pokemon? pokemon in activeSnapshot) { ... }
```

**Impact**: Prevents concurrent modification exceptions during forced switches, ensuring battle stability during phazing scenarios.

**Keywords**: `collection modified`, `enumeration`, `concurrent modification`, `InvalidOperationException`, `foreach`, `side.Active`, `phazing`, `DragIn`, `SwitchIn`, `Roar`, `Dragon Tail`, `snapshot`, `collection expression`

---


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

### Player 2 Always Wins Bug Fix
**File**: `Player2AlwaysWinsBugFix.md`  
**Severity**: Critical  
**Systems Affected**: Winner detection, battle result reporting, synchronous simulation

**Problem**: When running random vs random battles with identical teams, Player 2 appeared to win 100% of battles. This was misinterpreted as a battle mechanics bias, but was actually a **winner reporting bug**.

**Root Cause**: In `SyncSimulator.DetermineWinner()`, the winner detection compared `Battle.Winner` (which contains the side's **name**, e.g., "Player 1") against the hardcoded string `"p1"`. Since the comparison always failed for Player 1, the code fell through to return `SimulatorResult.Player2Win` even when Player 1 actually won.

**Solution**: Updated winner detection to compare against actual side names from `Battle.P1.Name` and `Battle.P2.Name`, with fallback to side ID strings for backwards compatibility.

**Verification**: After fix, 100 battles showed 49% P1 wins / 51% P2 wins (expected 50/50 with random variance).

**Key Insight**: Apparent systematic biases may be reporting bugs rather than logic bugs. The battle mechanics were working correctly - only the result interpretation was wrong.

**Keywords**: `winner detection`, `Player 2 bias`, `false positive`, `string comparison`, `SyncSimulator`, `DetermineWinner`, `side name`, `side ID`, `battle result`, `reporting bug`, `random battles`

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

### Immunity Event Parameter Conversion Fix
**File**: `ImmunityEventParameterConversionFix.md`  
**Severity**: High  
**Systems Affected**: Weather effects with immunity handlers, condition immunity checks, event parameter resolution

**Problem**: When weather (e.g., Sunny Day) with immunity handlers was active and a move tried to apply a volatile status, the battle crashed with `Event Immunity adapted handler failed on effect SunnyDay (Weather)` and inner exception `Parameter 1 (PokemonTypeConditionIdUnion type) is non-nullable but no matching value found in context`.

**Root Cause**: The `EventHandlerAdapter.ResolveParameter` method didn't know how to convert a `ConditionIdRelayVar` (or `PokemonTypeRelayVar`) into a `PokemonTypeConditionIdUnion` parameter type. When `Pokemon.RunStatusImmunity` called `RunEvent` with a `ConditionId?`, it was implicitly converted to `ConditionIdRelayVar` but the adapter couldn't extract it for the handler's `PokemonTypeConditionIdUnion` parameter.

**Solution**: Added logic to `EventHandlerAdapter.ResolveParameter` to handle the conversion:
```csharp
if (paramType == typeof(PokemonTypeConditionIdUnion))
{
    if (context.HasRelayVar)
    {
        // Handle ConditionIdRelayVar -> PokemonTypeConditionIdUnion
        if (context.RelayVar is ConditionIdRelayVar conditionIdVar && conditionIdVar.Id.HasValue)
        {
            return new PokemonTypeConditionIdUnion(conditionIdVar.Id.Value);
        }
        
        // Handle PokemonTypeRelayVar -> PokemonTypeConditionIdUnion
        if (context.RelayVar is PokemonTypeRelayVar pokemonTypeVar)
        {
            return new PokemonTypeConditionIdUnion(pokemonTypeVar.Type);
        }
    }
}
```

**Union Type Context**: `PokemonTypeConditionIdUnion` is used for immunity checks because immunity can apply to either type immunity (e.g., Ground moves vs Flying) or condition immunity (e.g., Freeze immunity in Sunny Day). The union allows a single `OnImmunity` handler signature to check both.

**Pattern**: Similar parameter conversion issues may exist for other union types. If you see "Parameter X is non-nullable but no matching value found in context" errors, check if the RelayVar type needs explicit conversion logic in `EventHandlerAdapter.ResolveParameter`.

**Keywords**: `immunity`, `weather`, `SunnyDay`, `OnImmunity`, `PokemonTypeConditionIdUnion`, `ConditionIdRelayVar`, `EventHandlerAdapter`, `parameter conversion`, `union type`, `status immunity`, `RelayVar unwrapping`

---

### Condition to Ability Cast Fix
**File**: `ConditionToAbilityCastFix.md`  
**Severity**: High  
**Systems Affected**: Event system, Mold Breaker ability suppression, all effect types

**Problem**: Random battle simulations would crash with `InvalidCastException: Unable to cast object of type 'ApogeeVGC.Sim.Conditions.Condition' to type 'ApogeeVGC.Sim.Abilities.Ability'` during event processing in `Battle.RunEvent` at line 353.

**Root Cause**: The code checked if `effect.EffectType == EffectType.Ability` but then unconditionally cast the effect to `Ability` without verifying the concrete type. A `Condition` can have multiple `EffectType` values (Condition, Weather, Status, Terrain), so relying solely on `EffectType` is insufficient to determine the concrete type. This created a potential crash when an effect had an `EffectType` that didn't match its concrete class.

**Solution**: Added explicit type check using pattern matching before casting:
```csharp
if (effect.EffectType == EffectType.Ability &&
    effectHolder is PokemonEffectHolder pokemonHolder2 &&
    effect is Ability ability)  // ? Added type check
{
    // Safe to use ability here
}
```

**Pattern**: This follows the same defensive pattern used for status condition checking. Always use `is ConcreteType variable` pattern matching when casting interface types to concrete types, even if the `EffectType` check suggests it should be safe.

**Prevention**:
- Never rely solely on `EffectType` to determine concrete type
- Always use pattern matching (`is`) before casting
- Apply defensive checks consistently across all similar code

**Keywords**: `InvalidCastException`, `Condition`, `Ability`, `type safety`, `pattern matching`, `EffectType`, `Mold Breaker`, `event system`, `defensive programming`

---

### Player Random Doubles Targeting Fix
**File**: `PlayerRandomDoublesTargetingFix.md`  
**Severity**: Critical  
**Systems Affected**: Random player AI, doubles battles, move targeting

**Problem**: When running doubles battles with `PlayerRandom`, the battle entered an infinite loop where the random player repeatedly generated invalid move choices. Every move choice was rejected with errors like "Can't move: Heavy Slam needs a target", causing the player to regenerate choices indefinitely.

**Root Cause**: The `PlayerRandom.GetRandomTargetLocation()` method always returned `0` (auto-targeting), regardless of move type or battle format. In doubles battles, moves requiring explicit targeting (Normal, Any, AdjacentFoe, etc.) fail validation when `targetLoc == 0` and `Active.Count >= 2`.

**Move Types Requiring Explicit Targets**:
- `MoveTarget.Normal` - Standard single-target moves
- `MoveTarget.Any` - Can target any Pokemon
- `MoveTarget.AdjacentAlly` - Target an adjacent ally
- `MoveTarget.AdjacentAllyOrSelf` - Target self or adjacent ally
- `MoveTarget.AdjacentFoe` - Target an adjacent opponent

**Solution**: Modified `GetRandomTargetLocation()` to detect moves requiring explicit targeting and return a valid target location (1 or 2) for doubles battles. The random player now picks between opponent slots 1 and 2 for all targeting moves.

**Target Location System**:
- `0` = Auto-targeting (for moves that don't need explicit targets)
- `1` = Left opponent slot
- `2` = Right opponent slot
- `-1` = Left ally slot (rarely used)
- `-2` = Right ally slot (rarely used)

**Impact**: Makes `PlayerRandom` functional in doubles format. The infinite loop is resolved, allowing doubles battles to complete normally.

**Keywords**: `PlayerRandom`, `doubles battle`, `infinite loop`, `target location`, `targeting moves`, `TargetTypeChoices`, `MoveTarget`, `GetRandomTargetLocation`, `Side.ChooseMove`, `AdjacentFoe`, `Normal target`, `doubles format`, `auto-targeting`, `move validation`

---

### Pokemon Position Index Mismatch Fix
**File**: `PokemonPositionIndexMismatchFix.md`  
**Severity**: High  
**Systems Affected**: Choice system, switching, trapped Pokemon handling, move request generation

**Problem**: `ArgumentOutOfRangeException` and `InvalidOperationException` when attempting to switch with a trapped Pokemon. The crash occurred in `UpdateRequestForPokemon` when accessing `moveRequest.Active[index]`.

**Root Cause**: Two related issues:
1. The code assumed `pokemon.Position` always equals the active slot index, but this invariant wasn't always maintained
2. More critically, `moveRequest.Active` was built using `.Where().Select()` which **filtered out** fainted Pokemon and changed the indices. TypeScript uses `.map()` which **preserves** indices with undefined entries

Example: If `Active[0]` is fainted and `Active[1]` is alive:
- C# (old): `moveRequest.Active` = [alive_data] (size 1)
- TypeScript: `moveRequest.Active` = [null, alive_data] (size 2, index preserved)

**Solution**: 
1. Changed request generation to use `.Select()` preserving indices with null for fainted Pokemon
2. Made `MoveRequest.Active` property nullable (`IReadOnlyList<PokemonMoveRequestData?>`)
3. Use `Active.IndexOf(pokemon)` instead of `pokemon.Position` for lookups

**Files Changed**: `MoveRequest.cs`, `Battle.Requests.cs`, `Side.Choices.cs`

**Keywords**: `Position`, `Active`, `SlotConditions`, `ArgumentOutOfRangeException`, `trapped`, `ChooseSwitch`, `UpdateRequestForPokemon`, `index mismatch`, `MoveRequest`, `fainted Pokemon`

---

### BoostId To String Evasion/Accuracy Conversion Fix
**File**: `BoostIdToStringEvasionAccuracyFix.md`  
**Severity**: High  
**Systems Affected**: Stat boost display system, all moves/abilities/items that modify accuracy or evasion

**Problem**: When attempting to display stat boost changes for Evasion or Accuracy, the application threw an `ArgumentOutOfRangeException`:
```
System.ArgumentOutOfRangeException: Cannot convert Evasion to StatId. (Parameter 'stat')
  at StatIdTools.ConvertToStatId(BoostId stat)
  at StatIdTools.ConvertToString(BoostId boost, Boolean leadingCapital)
```

This occurred when any move attempted to modify evasion (e.g., Defog lowering evasion, Double Team raising evasion) or accuracy.

**Root Cause**: The `ConvertToString(BoostId)` extension method incorrectly attempted to convert the `BoostId` to a `StatId` first:
```csharp
return boost.ConvertToStatId().ConvertToString();  // ? Wrong!
```

However:
- **`BoostId`** includes: Atk, Def, SpA, SpD, Spe, **Accuracy**, **Evasion**
- **`StatId`** includes: HP, Atk, Def, SpA, SpD, Spe

Accuracy and Evasion are **boost-only values**—they can be modified by stat boosts but are not actual stats that Pokemon possess. They don't appear in `StatId`, so conversion correctly threw an exception.

**Solution**: Rewrote `ConvertToString(BoostId)` to handle all `BoostId` values directly, including Accuracy and Evasion, using a comprehensive switch expression that mirrors the stat conversion logic but includes the two additional boost-only values.

**Affected Moves/Abilities**:
- Defog (lowers evasion)
- Double Team (raises evasion)
- Minimize (raises evasion)
- Sand Attack, Flash, etc. (lower accuracy)
- Coil, Hone Claws (raise accuracy)
- Hustle, Snow Cloak (abilities affecting accuracy)

**Pattern**: When working with Pokemon mechanics, remember that `BoostId` is a superset of `StatId` (minus HP, plus Accuracy/Evasion). Direct conversions between these types are unsafe for the two boost-only values.

**Keywords**: `BoostId`, `StatId`, `Evasion`, `Accuracy`, `ArgumentOutOfRangeException`, `type conversion`, `stat boost display`, `ConvertToString`, `boost-only values`, `Defog`, `Double Team`, `stat modification`

---

### Steel Beam & Mind Blown Recoil Fix
**File**: `SteelBeamMindBlownRecoilFix.md`  
**Severity**: High  
**Systems Affected**: Move recoil mechanics (MindBlownRecoil property)

**Problem**: Moves with the `MindBlownRecoil` property (Steel Beam and Mind Blown) crashed with `ArgumentException: MoveId 'SteelBeam' does not have a corresponding ConditionId` when attempting to apply recoil damage after hitting.

**Root Cause**: The code tried to look up a `ConditionId` for these moves using `Library.Conditions[move.Id.ToConditionId()]` to use as the damage effect. However, Steel Beam and Mind Blown don't have corresponding `ConditionId` entries because they don't create persistent conditions. In TypeScript, `conditions.get(move.id)` can dynamically handle any move as a condition reference for damage tracking.

**Solution**: Pass the move directly as the damage effect since `ActiveMove` implements `IEffect`:

```csharp
// Changed from:
BattleDamageEffect.FromIEffect(Library.Conditions[move.Id.ToConditionId()])
// To:
BattleDamageEffect.FromIEffect(move)
```

**Pattern**: The `ToConditionId()` method should only be used for moves that have explicit `ConditionId` mappings (e.g., Protect, Trick Room, Leech Seed). For damage tracking purposes, moves can serve as `IEffect` instances directly without needing a separate condition.

**Keywords**: `MindBlownRecoil`, `Steel Beam`, `Mind Blown`, `recoil`, `damage effect`, `ConditionId`, `IEffect`, `move mechanics`, `ArgumentException`, `ToConditionId`

---

### Grassy Glide ModifyPriority Source Fix
**File**: `GrassyGlideModifyPrioritySourceFix.md`  
**Severity**: Medium  
**Systems Affected**: Move-specific OnModifyPriority handlers

**Problem**: Grassy Glide's `OnModifyPriority` handler threw `NullReferenceException` when checking if the Pokemon using the move is grounded in Grassy Terrain. The `source` parameter was `null`.

**Root Cause**: In `GetActionSpeed`, `SingleEvent` was called with `source = null`. The handler expected to access the Pokemon using the move via the `source` parameter (following the `ModifierSourceMove` pattern), but C#'s name-based parameter resolution mapped `source` to `SourcePokemon` which was null.

**Solution**: Pass `moveAction.Pokemon` as both `target` and `source` parameters to `SingleEvent`, ensuring handlers using the `ModifierSourceMove` pattern can access the Pokemon via the `source` parameter name.

**Keywords**: `ModifyPriority`, `Grassy Glide`, `SingleEvent`, `source`, `target`, `NullReferenceException`, `EventHandlerAdapter`, `ModifierSourceMove`

---

### Wind Rider Null SideCondition Fix
**File**: `WindRiderNullSideConditionFix.md`  
**Severity**: High  
**Systems Affected**: OnSideConditionStart event handlers, abilities that react to side conditions

**Problem**: Wind Rider ability crashed with `NullReferenceException` when trying to access `sideCondition.Id` in its `OnSideConditionStart` handler. The error occurred during random battle testing when a Pokémon with Wind Rider was on the field and a side condition was added.

**Root Cause**: The handler did not check if the `sideCondition` parameter was null before accessing its properties. The parameter was marked as non-nullable in the event handler info, but `EventHandlerAdapter.ResolveParameter` returns `context.SourceEffect` even if it's null when resolving `IEffect`-derived parameters.

**Solution**:
- Added null guard in Wind Rider's handler: `if (sideCondition != null && sideCondition.Id == ConditionId.Tailwind)`
- Updated `OnSideConditionStartEventInfo` to mark the `sideCondition` parameter as nullable
- Changed handler signature to `Action<Battle, Side, Pokemon, Condition?>` 
- Updated `ParameterNullability` array to `[false, false, false, true]`

**Pattern**: Follows the same defensive null checking pattern as Ripen, Disguise, Adrenaline Orb, and Berserk fixes. Event parameters that implement `IEffect` or derived types can be null when events are triggered without an associated effect.

**Keywords**: `Wind Rider`, `OnSideConditionStart`, `null reference`, `sideCondition parameter`, `Tailwind`, `side condition`, `null check`, `IEffect`, `Condition`, `parameter nullability`, `defensive programming`

---

### Big Root Null Effect Fix
**File**: `BigRootNullEffectFix.md`  
**Severity**: High  
**Systems Affected**: Big Root item `OnTryHeal` event handler

**Problem**: The Big Root item crashed with `NullReferenceException` when trying to access `effect.EffectStateId` in its `OnTryHeal` handler. The error occurred when a Pokémon holding Big Root was healed from sources that don't have an associated `IEffect` (e.g., natural regeneration, certain abilities).

**Root Cause**: The handler did not check if the `effect` parameter was null before accessing its `EffectStateId` property. The TypeScript reference implementation directly accesses `effect.id` without a null check, which works in JavaScript (accessing properties on `undefined` returns `undefined`), but in C# it throws a `NullReferenceException`.

**Solution**: Added a null guard at the beginning of the `OnTryHeal` handler:
```csharp
if (effect == null)
{
    return null;
}
```

**Impact**: When `effect` is null, the healing proceeds unchanged without activating Big Root's healing boost, matching the correct TypeScript behavior where Big Root only boosts specific healing sources (drain moves, Leech Seed, Ingrain, Aqua Ring, Strength Sap).

**Keywords**: `big root`, `item`, `OnTryHeal`, `null reference`, `effect parameter`, `healing`, `drain`, `null check`, `TypeScript porting`, `Leech Seed`, `Ingrain`, `Aqua Ring`, `Strength Sap`

---

*Last Updated*: 2025-01-20  
*Total Bug Fixes Documented*: 29  
*Reference Guides*: 1
