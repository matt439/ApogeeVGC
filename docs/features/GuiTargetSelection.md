# GUI Target Selection Feature

## Overview
Added target selection capability to the GUI for moves with `MoveTarget.Normal` and other choosable targets (like Facade, protecting moves targeting allies, etc.). Previously, the GUI would auto-select target location 0 for all moves, which prevented proper targeting in doubles battles.

## Issue
When selecting moves with `Normal` target type (such as Facade, Crunch, etc.) in the GUI, the system didn't prompt for target selection and defaulted to target location 0. This meant players couldn't choose which opponent or ally to target in doubles battles.

## Solution
Implemented a complete target selection flow matching the console player's behavior:

### 1. Target Selection Detection
Added `MoveRequiresTargetSelection` helper method that checks:
- Move's target type (Normal, AdjacentFoe, Any, AdjacentAlly, AdjacentAllyOrSelf, RandomNormal)
- Whether it's a doubles battle (Active.Count > 1)
- Excludes moves that don't need selection (Self, All, Field, AllAdjacent, etc.)

### 2. State Machine Updates
- Leveraged existing `TargetSelectionFirstPokemon` and `TargetSelectionSecondPokemon` states in `MainBattlePhaseState`
- Added transitions from move selection to target selection when needed
- Added back navigation from target selection to move selection (ESC key)

### 3. UI Implementation
Created `MainBattleUiHelper.CreateTargetSelectionButtons` that:
- Shows all valid opponent targets (red buttons) with HP information
- Shows all valid ally targets (green buttons) with HP information  
- Includes "(Self)" indicator when move can target the user
- Uses positive target locations for opponents (1, 2, ...)
- Uses negative target locations for allies (-1, -2, ...)
- Matches the logic from `PlayerConsole.GetTargetLocationAsync`

### 4. Target Storage and Submission
- `HandleTargetSelection` stores the selected target in `TurnSelection.FirstPokemonTarget` or `TurnSelection.SecondPokemonTarget`
- After target selection, automatically transitions to next Pokemon (doubles) or submits choice (singles)
- Target location is properly passed to `ChosenAction.TargetLoc` in `SubmitMainBattleTurnChoice`

## Move Target Types Requiring Selection
Based on `BattleActions._choosableTargets`:
- `MoveTarget.Normal` - Can hit any adjacent Pokemon (opponent or ally in doubles)
- `MoveTarget.AdjacentFoe` - Can hit adjacent opponent (with selection in doubles)
- `MoveTarget.Any` - Can hit any active Pokemon
- `MoveTarget.AdjacentAlly` - Must select an ally
- `MoveTarget.AdjacentAllyOrSelf` - Can select ally or self
- `MoveTarget.RandomNormal` - Random target but still allows selection in some cases

## Files Modified
1. `ApogeeVGC/Gui/ChoiceUI/ChoiceInputManager.MainBattle.cs`
   - Added `MoveRequiresTargetSelection` helper method
   - Updated `HandleMoveSelection` to check for target selection needs
   - Added `HandleTargetSelection` to handle target selection
   - Updated `HandleEscapeKeyNavigation` for target selection back navigation
   - Added target selection cases to `SetupMainBattleUi`

2. `ApogeeVGC/Gui/ChoiceUI/MainBattleUiHelper.cs`
   - Added `CreateTargetSelectionButtons` method
   - Updated `GetInstructionText` to include target selection states

## Testing
To test this feature:
1. Start a GUI vs Random Doubles battle
2. Select a Pokemon with a Normal-target move (e.g., Facade, Crunch, etc.)
3. After selecting the move, a target selection screen should appear
4. Choose which opponent or ally to target
5. The move should be executed with the correct target

## Behavior Differences from Console
- Console: Prompts for target selection using Spectre.Console dropdown
- GUI: Shows buttons for each valid target with navigation

## Related Code References
- Console implementation: `PlayerConsole.GetTargetLocationAsync` (lines 264-371)
- Move target checking: `BattleActions._choosableTargets` (lines 13-20)
- Target validation: `Side.ChooseMove` method checks `Battle.ValidTargetLoc`

## Future Enhancements
- Could add visual indicators on the battle field showing which Pokemon are targetable
- Could highlight the default target based on game rules
- Could show move effectiveness indicators for each target
