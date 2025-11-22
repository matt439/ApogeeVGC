# Visual Pokemon Box Target Selection

## Overview
Improved the GUI target selection system to use visual Pokemon box selection instead of button lists. When a move requires target selection (e.g., Normal-target moves like Facade), players can now directly click on Pokemon sprites or use arrow keys to cycle through valid targets with visual highlighting.

## Implementation

### Visual Feedback
- **Semi-transparent overlays** appear on Pokemon boxes that can be targeted
  - Red overlay for opponent targets (50% opacity when highlighted, 20% when not)
  - Green overlay for ally targets (50% opacity when highlighted, 20% when not)
- **Highlighted borders** show the currently selected target
  - Red border for opponents
  - Green border for allies
  - Thick (4px) border indicates active selection

### Input Methods

#### Mouse Control
- Click directly on any valid Pokemon box to select it as the target
- Instant selection - no need to navigate through lists

#### Keyboard Control
- **LEFT/RIGHT arrows**: Cycle through valid targets
- **ENTER**: Confirm currently highlighted target
- **ESC**: Go back to move selection

### Architecture Changes

#### BattleRenderer Enhancements
1. **Box Tracking**
   - `_playerPokemonBoxes` and `_opponentPokemonBoxes` dictionaries store Rectangle positions
   - Updated during `RenderInBattlePlayerPokemon()` and `RenderInBattleOpponentPokemon()`
   - Boxes represent the 128x128 sprite areas

2. **Hit Testing API**
   - `GetPlayerPokemonBox(int index)` - Get player Pokemon box by index
   - `GetOpponentPokemonBox(int index)` - Get opponent Pokemon box by index
   - `GetPlayerPokemonBoxes()` - Get all player boxes
   - `GetOpponentPokemonBoxes()` - Get all opponent boxes

3. **Overlay Rendering**
   - `RenderTargetSelectionOverlay(List<int> validTargets, int? highlightedTarget)`
   - Draws semi-transparent overlays on valid targets
   - Draws highlighted border on currently selected target

#### ChoiceInputManager Updates
1. **State Tracking**
   - `ValidTargets` - List of valid target locations for current move
   - `HighlightedTarget` - Currently highlighted target (null if none)
   - `_highlightedTargetIndex` - Index in ValidTargets list for cycling

2. **Visual Setup**
   - `SetupVisualTargetSelection()` - Builds list of valid targets based on move's target type
   - Uses same logic as console player (`PlayerConsole.GetTargetLocationAsync`)
   - Positive target locations for opponents (1, 2, ...)
   - Negative target locations for allies (-1, -2, ...)

3. **Input Processing**
   - `ProcessTargetSelectionInput()` - Keyboard navigation through targets
   - `ProcessTargetSelectionMouseClick()` - Mouse click handling on Pokemon boxes
   - Mouse clicks check if click position intersects any valid Pokemon box

4. **Integration**
   - `SetBattleRenderer()` - Links BattleRenderer for hit testing
   - Overlay rendered in `Render()` method when in target selection state

## User Experience

### Before (Button List)
```
[Target Selection Menu]
[ Opponent 1: Ironhands (183/183 HP) ]
[ Opponent 2: Miraidon (198/198 HP) ]
[ Your Pokemon 2: Volcarona (167/167 HP) ]
[ Back ]
```
- Required navigating a vertical button list
- Text-only, no visual connection to Pokemon on field

### After (Visual Box Selection)
```
Instruction: "Click a Pokemon or use LEFT/RIGHT arrows to select target"

[Semi-transparent overlays appear on Pokemon sprites]
[Red overlay on valid opponent targets]
[Green overlay on valid ally targets]
[Thick border around currently highlighted target]
```
- Direct visual feedback on the battle field
- Intuitive mouse clicking on Pokemon sprites
- Clear distinction between opponent/ally targets via color

## Target Location Encoding
- **Positive integers** (1, 2, ...): Opponent Pokemon slots
  - 1 = Opponent's first Pokemon (left)
  - 2 = Opponent's second Pokemon (right)
- **Negative integers** (-1, -2, ...): Player's Pokemon slots
  - -1 = Player's first Pokemon (left)
  - -2 = Player's second Pokemon (right)

## Move Target Types Supported
All move target types requiring selection:
- `MoveTarget.Normal` - Adjacent Pokemon (opponents or allies in doubles)
- `MoveTarget.AdjacentFoe` - Adjacent opponents
- `MoveTarget.Any` - Any active Pokemon
- `MoveTarget.AdjacentAlly` - Adjacent allies only
- `MoveTarget.AdjacentAllyOrSelf` - Adjacent allies or self
- `MoveTarget.RandomNormal` - Random adjacent (still allows manual selection)

## Files Modified

1. **ApogeeVGC/Gui/Rendering/BattleRenderer.cs**
   - Added `_playerPokemonBoxes` and `_opponentPokemonBoxes` tracking
   - Added Pokemon box getter methods for hit testing
   - Added `RenderTargetSelectionOverlay()` method

2. **ApogeeVGC/Gui/ChoiceUI/ChoiceInputManager.Core.cs**
   - Added `ValidTargets`, `HighlightedTarget`, `_highlightedTargetIndex` fields
   - Added `_battleRenderer` reference for hit testing
   - Added `SetBattleRenderer()` method
   - Updated `Render()` to call overlay renderer

3. **ApogeeVGC/Gui/ChoiceUI/ChoiceInputManager.MainBattle.cs**
   - Added `SetupVisualTargetSelection()` method
   - Updated `SetupMainBattleUi()` for target selection states
   - Added `ProcessTargetSelectionInput()` for keyboard navigation
   - Updated `ProcessMainBattleKeyboardInput()` to route to visual selection

4. **ApogeeVGC/Gui/ChoiceUI/ChoiceInputManager.TeamPreview.cs**
   - Updated `ProcessMouseInput()` to handle target selection clicks
   - Added `ProcessTargetSelectionMouseClick()` method

5. **ApogeeVGC/Gui/ChoiceUI/MainBattleUiHelper.cs**
   - Removed `CreateTargetSelectionButtons()` method (obsolete)
   - Updated instruction text for target selection states

6. **ApogeeVGC/Gui/BattleGame.cs**
   - Connected BattleRenderer to ChoiceInputManager

## Testing
To test this feature:
1. Start a GUI vs Random Doubles battle
2. Select a move with Normal target (Facade, Crunch, etc.)
3. Observe semi-transparent overlays appear on valid targets
4. Either:
   - Click directly on a Pokemon sprite to select it
   - Use LEFT/RIGHT arrows to cycle targets, ENTER to confirm
5. The move should execute with the correct target

## Benefits Over Button Approach
1. **More Intuitive** - Direct visual connection to Pokemon on field
2. **Faster Selection** - Click directly on target instead of navigating list
3. **Better Visual Feedback** - Color-coded overlays (red=opponent, green=ally)
4. **Consistent with Game Feel** - Similar to how actual Pokemon games handle targeting
5. **Less Screen Clutter** - No separate UI panel needed
6. **Dual Input** - Supports both mouse (fast) and keyboard (precise)

## Future Enhancements
- Add hover effects for mouse-over on valid targets
- Show move effectiveness indicators (super effective, not very effective)
- Animate the overlay appearance/selection
- Add sound effects for target selection
- Show attack animation path preview
