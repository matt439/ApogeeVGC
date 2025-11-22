# GUI Team Preview Selection Fix

## Issue
When using the GUI at team preview, selecting Pokemon 3, 4, 5, 6 didn't work properly - only Pokemon 1 and 2 were sent out as leads regardless of the selection. The console player (`PlayerConsole`) worked correctly.

## Root Cause
The GUI's `SubmitTeamPreviewChoice` method in `ChoiceInputManager.TeamPreview.cs` was incorrectly creating team preview actions. It was setting:
- `Index = pokemonIndex` (the original Pokemon index that was selected, e.g., 2, 3, 4, 5)
- `TargetLoc` was not set at all

The `ProcessChosenTeamAction` method in `Side.Choices.cs` (line 570) expects:
- `TargetLoc` = which Pokemon from the original team to use (e.g., 2, 3, 4, 5 for Pokemon #3, #4, #5, #6)
- `Index` = the position in the new team order (e.g., 0, 1, 2, 3 for 1st, 2nd, 3rd, 4th)

When `TargetLoc` wasn't set, the code fell back to using `Index`, but this caused the wrong Pokemon to be selected since `Index` was being used for both purposes.

## Solution
Updated `SubmitTeamPreviewChoice` to match the console player's implementation:
- `Index = newPosition` - the position in the new team order (0, 1, 2, 3)
- `TargetLoc = originalPokemonIndex` - which Pokemon to pick (2, 3, 4, 5 for Pokemon #3, #4, #5, #6)
- `Priority = -newPosition` - earlier positions have higher priority (0, -1, -2, -3)

## Files Modified
- `ApogeeVGC/Gui/ChoiceUI/ChoiceInputManager.TeamPreview.cs` - Fixed `SubmitTeamPreviewChoice` method

## Testing
After the fix:
1. Start a GUI vs Random Doubles battle
2. At team preview, press RIGHT arrow to move through Pokemon
3. Press ENTER on Pokemon 3, 4, 5, 6 to lock them in
4. The selected Pokemon should now be sent out as leads (first two of the locked-in order)

Expected behavior:
- Selecting Pokemon 3, 4, 5, 6 should send out Pokemon 3 and 4 as the starting leads
- The team order should be: Pokemon 3, Pokemon 4, Pokemon 5, Pokemon 6

## Related Code
The console player's correct implementation can be found in:
- `ApogeeVGC/Sim/Player/PlayerConsole.cs` lines 638-673 (`GetTeamPreviewChoiceAsync` method)

The processing logic is in:
- `ApogeeVGC/Sim/SideClasses/Side.Choices.cs` lines 558-587 (`ProcessChosenTeamAction` method)
