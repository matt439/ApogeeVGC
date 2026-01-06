# Reflect Side Condition Display Fix

## Problem Summary
When Grimmsnarl used Reflect, the side condition was being applied correctly internally, but no visual feedback was displayed to the console player. The move would show "Grimmsnarl used Reflect!" but the message "Reflect raised Matt's team's Defense!" and the side condition status in the field display section were missing.

## Root Cause
The issue had two parts:

### Part 1: Missing Message Parsing
The `OnSideStart` event handler for Reflect was being invoked correctly and calling `battle.Add("-sidestart", side, "Reflect")`, which successfully added the log entry `|-sidestart|p1|Reflect|` to the battle log. However, the `ParseLogToMessages()` method in `Battle.Logging.cs` did not have a case to parse `-sidestart` or `-sideend` messages. This meant the log entry existed but the console player couldn't parse and display it.

### Part 2: Missing Side Condition Display in UI
Side conditions (Reflect, Light Screen, Tailwind) were not included in the battle perspective data sent to players, so even if the parsing worked, the conditions wouldn't appear in the field status section like Electric Terrain does.

## Solution

### Part 1: Add Message Parsing
Added parsing cases for `-sidestart` and `-sideend` messages in `ParseLogToMessages()`:

```csharp
"-sidestart" when parts.Length > 3 =>
    new GenericMessage 
    { 
        Text = $"{parts[3]} raised {GetSideName(parts[2])}'s team's {GetStatNameForCondition(parts[3])}!" 
    },

"-sideend" when parts.Length > 3 =>
    new GenericMessage 
    { 
        Text = $"{GetSideName(parts[2])}'s {parts[3]} wore off!" 
    },
```

Also added helper methods:
- `GetSideName()`: Converts side ID (e.g., "p1") to player name (e.g., "Matt")
- `GetStatNameForCondition()`: Maps condition names to stat descriptions (e.g., "Reflect" ? "Defense", "Light Screen" ? "Special Defense")

### Part 2: Add Side Conditions to Perspective
Modified perspective classes to include side condition information:

**Updated `SidePlayerPerspective` and `SideOpponentPerspective`:**
```csharp
public IReadOnlyDictionary<ConditionId, int?> SideConditionsWithDuration { get; init; } = new Dictionary<ConditionId, int?>();
```

**Updated `GetPlayerPerspective()` and `GetOpponentPerspective()` in `Side.Core.cs`:**
```csharp
SideConditionsWithDuration = SideConditions.ToDictionary(
    kvp => kvp.Key,
    kvp => kvp.Value.Duration
).AsReadOnly(),
```

**Updated `PlayerConsole.RenderBattleState()` to display side conditions:**
```csharp
// Display side conditions for both sides
string opponentSideState = GetSideConditionsDisplay(perspective.OpponentSide.SideConditionsWithDuration, "Opponent");
if (!string.IsNullOrEmpty(opponentSideState))
{
    AnsiConsole.MarkupLine($"[bold]{opponentSideState}[/]");
}

string playerSideState = GetSideConditionsDisplay(perspective.PlayerSide.SideConditionsWithDuration, "Your Team");
if (!string.IsNullOrEmpty(playerSideState))
{
    AnsiConsole.MarkupLine($"[bold]{playerSideState}[/]");
}
```

Added helper methods:
- `GetSideConditionsDisplay()`: Formats side conditions with durations
- `GetSideConditionDisplayName()`: Gets friendly names for side conditions
- `GetSideConditionColor()`: Returns appropriate Spectre.Console colors for each condition type

## Files Modified

### Battle.Logging.cs
- Added parsing cases for `-sidestart` and `-sideend` in `ParseLogToMessages()`
- Added `GetSideName()` helper method
- Added `GetStatNameForCondition()` helper method

### SidePlayerPerspective.cs
- Added `SideConditionsWithDuration` property

### SideOpponentPerspective.cs
- Added `SideConditionsWithDuration` property

### Side.Core.cs
- Updated `GetPlayerPerspective()` to populate side conditions
- Updated `GetOpponentPerspective()` to populate side conditions

### PlayerConsole.cs
- Updated `RenderBattleState()` to display side conditions
- Added `GetSideConditionsDisplay()` method
- Added `GetSideConditionDisplayName()` method
- Added `GetSideConditionColor()` method

## Testing
After the fix, when Grimmsnarl uses Reflect:

**Message Output:**
```
Grimmsnarl (Side 1) used Reflect!
Reflect raised Matt's team's Defense!
```

**Field Display:**
```
Field: Electric:5

Your Team: Reflect:8

?????????????????????????????????????????????????????????????????????????????????????????
?                 Opponent                  ?                 Your Team                 ?
...
```

The `:8` indicates 8 turns remaining (extended from 5 by Light Clay held item).

## Key Insight
Side conditions were functioning correctly in the battle engine but were completely invisible to players because:
1. The log parser didn't know how to interpret `-sidestart` messages
2. The perspective system didn't expose side condition data to UI layers

This is similar to previous event handler bugs, but in this case the handlers were working fine - it was purely a UI/logging issue.

## Keywords
`side condition`, `Reflect`, `Light Screen`, `Tailwind`, `log parsing`, `battle perspective`, `console UI`, `visual feedback`, `message display`
