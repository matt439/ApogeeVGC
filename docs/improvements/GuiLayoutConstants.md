# GUI Layout Constants Refactoring

## Summary
Extracted all hardcoded layout values from `BattleRenderer.cs` into descriptive constants to improve maintainability and make it easier to adjust the GUI layout without hunting through the code for magic numbers.

## Changes Made

### Constants Added to BattleRenderer.cs

#### General Spacing and Sizing
- `Padding = 20` - General padding around UI elements (already existed)
- `PokemonSpriteSize = 128` - Size of Pokemon sprite boxes (already existed)
- `InfoTextHeight = 75` - Height reserved for Pokemon info text below sprite (already existed)
- `PokemonSpacing = 20` - Horizontal spacing between Pokemon sprites (replaces hardcoded `Padding` used for spacing)

#### Team Preview Layout
- `TeamPreviewOpponentYOffset = 60` - Offset from top padding for opponent team positioning
- `TeamPreviewLabelYOffset = 30` - Distance of team label above Pokemon sprites

#### In-Battle Layout
- `InBattlePlayerYOffset = 200` - Offset from bottom to position player Pokemon during battle
- `InBattleOpponentYOffset = 120` - Offset from top padding for opponent Pokemon during battle

#### Timer Display
- `TimerXOffset = 200` - Distance from right edge of screen for timer display
- `TimerLineHeight = 25` - Vertical spacing between timer lines

#### Text Layout
- `InfoTextYOffset = 5` - Distance below sprite to start Pokemon info text

#### Lock Order Indicator (Team Preview)
- `LockOrderXPadding = 5` - Padding from right edge of sprite for lock order number
- `LockOrderYPadding = 5` - Padding from top edge of sprite for lock order number
- `LockOrderBackgroundPadding = 2` - Padding around lock order text background
- `LockOrderBackgroundAlpha = 0.7f` - Transparency of lock order background

#### Border Styling
- `BorderThicknessNormal = 2` - Default border thickness for Pokemon sprites
- `BorderThicknessHighlighted = 4` - Border thickness when Pokemon is highlighted or locked

#### HP Color Thresholds
- `HpColorThresholdGreen = 0.5` - HP percentage above which text shows green (>50%)
- `HpColorThresholdYellow = 0.2` - HP percentage above which text shows yellow (>20%, below shows red)

## Benefits

1. **Easy Adjustments**: All layout values are now in one place at the top of the class, organized by category
2. **Self-Documenting**: Constants have descriptive names that explain their purpose
3. **Consistency**: Using named constants ensures consistent spacing throughout the UI
4. **Maintainability**: No need to search through code to find magic numbers when tweaking layout

## How to Adjust the Layout

To change the GUI layout, simply modify the constant values at the top of `BattleRenderer.cs`:

### Example Adjustments

**Make Pokemon sprites larger:**
```csharp
private const int PokemonSpriteSize = 160; // was 128
```

**Increase spacing between Pokemon:**
```csharp
private const int PokemonSpacing = 30; // was 20
```

**Move opponent Pokemon down to avoid timer overlap:**
```csharp
private const int InBattleOpponentYOffset = 150; // was 120
```

**Make highlighted borders more prominent:**
```csharp
private const int BorderThicknessHighlighted = 6; // was 4
```

**Adjust HP color thresholds:**
```csharp
private const double HpColorThresholdGreen = 0.6;  // was 0.5 (green at >60% HP)
private const double HpColorThresholdYellow = 0.3; // was 0.2 (yellow at >30% HP)
```

## Additional Constants in MainBattleUiHelper.cs

### Choice UI Layout Constants

**Button dimensions:**
- `ButtonWidth = 200` - Width of choice buttons
- `ButtonHeight = 40` - Height of choice buttons

**Vertical spacing:**
- `ButtonSpacing = 5` - Normal spacing between buttons
- `ButtonSpacingLarge = 10` - Extra spacing before certain buttons (e.g., "Back" button)

**Menu positioning:**
- `LeftMargin = 700` - X position of choice menu from left edge
- `TopMargin = 300` - Y position of choice menu from top edge

**Text positioning:**
- `InstructionTextYOffset = 80` - Distance above TopMargin for instruction text
- `SelectionStatusY = 650` - Fixed Y position for selection status text at bottom

### Example: Adjust Choice Menu Layout

**Move menu higher:**
```csharp
private const int TopMargin = 250;  // was 300
```

**Increase spacing between move choices:**
```csharp
private const int ButtonSpacing = 10;  // was 5
```

**Make buttons larger:**
```csharp
private const int ButtonHeight = 50;  // was 40
private const int ButtonWidth = 250;   // was 200
```

**Move menu to the left:**
```csharp
private const int LeftMargin = 600;  // was 700
```

## Files Modified
- `ApogeeVGC/Gui/Rendering/BattleRenderer.cs` - All layout constants extracted and applied throughout
- `ApogeeVGC/Gui/ChoiceUI/MainBattleUiHelper.cs` - All choice UI layout constants extracted with helper methods
- `ApogeeVGC/Gui/ChoiceUI/ChoiceInputManager.MainBattle.cs` - Updated to use helper methods for text positioning

## Testing
Build successful - all constants properly applied throughout the rendering and choice UI code.
