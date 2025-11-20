# Choice UI Layout Adjustment Guide

## Quick Reference: Adjusting Move/Switch Choice Layout

All layout constants for the move/switch choice UI are located in:
**`ApogeeVGC/Gui/ChoiceUI/MainBattleUiHelper.cs`**

## Layout Constants

### Button Size
```csharp
private const int ButtonWidth = 200;   // Width of each button
private const int ButtonHeight = 40;   // Height of each button
```

### Vertical Spacing (MOST COMMONLY ADJUSTED)
```csharp
private const int ButtonSpacing = 5;        // Normal spacing between buttons
private const int ButtonSpacingLarge = 10;  // Extra spacing before "Back" button
```

### Menu Position
```csharp
private const int LeftMargin = 700;  // X position (distance from left edge)
private const int TopMargin = 300;   // Y position (distance from top edge)
```

### Text Positioning
```csharp
private const int InstructionTextYOffset = 80;  // Distance above menu for instructions
private const int SelectionStatusY = 650;       // Y position of status text at bottom
```

## Common Adjustments

### 1. Increase Spacing Between Choices
**Problem:** Buttons are too close together, hard to distinguish
```csharp
private const int ButtonSpacing = 10;  // Change from 5 to 10
```

### 2. Make Buttons Easier to Click
**Problem:** Buttons feel cramped
```csharp
private const int ButtonHeight = 50;   // Change from 40 to 50
private const int ButtonSpacing = 10;  // Change from 5 to 10
```

### 3. Move Menu Higher on Screen
**Problem:** Menu overlaps with player Pokemon sprites
```csharp
private const int TopMargin = 250;  // Change from 300 to 250
```

### 4. Move Menu Lower on Screen
**Problem:** Menu overlaps with instruction text
```csharp
private const int TopMargin = 350;  // Change from 300 to 350
```

### 5. Move Menu Left or Right
**Problem:** Menu overlaps with other UI elements
```csharp
private const int LeftMargin = 600;  // Move left (was 700)
// or
private const int LeftMargin = 800;  // Move right (was 700)
```

### 6. More Space Before Back Button
**Problem:** Back button too close to other options
```csharp
private const int ButtonSpacingLarge = 20;  // Change from 10 to 20
```

## What Each Constant Controls

### ButtonSpacing (Most Important for Vertical Spacing)
- Controls the gap between each move option
- Controls the gap between move buttons and Tera variant buttons
- Controls the gap between Pokemon switch options

**Used in:**
- Move selection (gaps between moves)
- Switch selection (gaps between Pokemon)
- Main menu (gaps between Battle/Pokemon/Run buttons)

### ButtonSpacingLarge
- Provides extra separation before the "Back" button
- Makes it less likely to accidentally hit "Back" when selecting last option

**Used in:**
- Move selection screen (before Back button)
- Switch selection screen (before Back button)

### TopMargin
- Starting Y position for the first button
- All other buttons positioned relative to this

**Affects:**
- Overall vertical position of the entire choice menu
- Distance from top of screen

### LeftMargin
- X position for all buttons
- All buttons align to this value

**Affects:**
- Overall horizontal position of the entire choice menu
- Distance from left edge of screen

## Visual Layout Reference

```
Screen Top (Y=0)
?
??? Y = 220 (Instruction Text: TopMargin - InstructionTextYOffset)
?   "Select a move for Pokemon 1."
?
??? Y = 300 (TopMargin - First Button)
?   [Move 1 Button]         ? ButtonHeight: 40
??? Y = 345 (+ ButtonSpacing: 5)
?   [Move 2 Button]         ? ButtonHeight: 40
??? Y = 390 (+ ButtonSpacing: 5)
?   [Move 3 Button]         ? ButtonHeight: 40
??? Y = 435 (+ ButtonSpacing: 5)
?   [Move 4 Button]         ? ButtonHeight: 40
??? Y = 485 (+ ButtonSpacingLarge: 10 + ButtonHeight)
?   [Back Button]           ? ButtonHeight: 40
?
??? Y = 650 (SelectionStatusY - Status Text)
?   "P1: Move 2 | P2: Switch to #3"
?
Screen Bottom (Y=720)

All buttons at X = 700 (LeftMargin)
All buttons have Width = 200 (ButtonWidth)
```

## Tips for Experimentation

1. **Make small changes** - Increase/decrease by 5-10 pixels at a time
2. **Test with all screens**:
   - Main menu (Battle/Pokemon/Run)
   - Move selection (with and without Tera options)
   - Switch selection
   - Force switch (when Pokemon faints)
3. **Check both singles and doubles** - Doubles has more buttons (P1 and P2)
4. **Consider screen size** - Default is 1280x720

## Quick Test Checklist
After adjusting constants:
- [ ] Buttons don't overlap with Pokemon sprites
- [ ] Instruction text is visible above buttons
- [ ] Status text is visible below buttons
- [ ] All buttons fit on screen
- [ ] "Back" button is clearly separated
- [ ] Tera options (if present) don't cause overflow

## Recommended Starting Points

**Comfortable Layout (More Space):**
```csharp
private const int ButtonHeight = 45;
private const int ButtonSpacing = 8;
private const int ButtonSpacingLarge = 15;
private const int TopMargin = 280;
```

**Compact Layout (More Options Visible):**
```csharp
private const int ButtonHeight = 35;
private const int ButtonSpacing = 3;
private const int ButtonSpacingLarge = 8;
private const int TopMargin = 300;
```

**Spacious Layout (Easier to Read):**
```csharp
private const int ButtonHeight = 50;
private const int ButtonSpacing = 12;
private const int ButtonSpacingLarge = 20;
private const int TopMargin = 250;
```
