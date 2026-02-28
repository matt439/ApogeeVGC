# Player Classes Debug Output Conditional Fix

## Summary
Made all debug console output in player classes conditional based on the `PrintDebug` property from `PlayerOptions`.

## Changes Made

### 1. PlayerRandom.cs
Wrapped all Console.WriteLine debug statements with `if (PrintDebug)` checks:
- **GetChoiceSync()**: 2 debug statements wrapped
- **GetNextChoiceFromAll()**: 2 debug statements wrapped

### 2. PlayerGui.cs
Wrapped all Console.WriteLine debug statements with `if (PrintDebug)` checks:
- **Constructor**: 4 debug statements wrapped (GuiWindow and ChoiceCoordinator initialization logging)
- **GetNextChoiceAsync()**: 2 debug statements wrapped (method entry and coordinator usage)
- **NotifyTimeoutWarningAsync()**: 1 debug statement wrapped
- **NotifyChoiceTimeoutAsync()**: 1 debug statement wrapped

### 3. PlayerConsole.cs
**No changes needed** - This class only contains user-facing UI output via `AnsiConsole`, not debug logging. All Console output in PlayerConsole is intentional user interface content that should always be displayed.

## Configuration

The `PrintDebug` property is set via `PlayerOptions`:

```csharp
PlayerOptions playerOptions = new()
{
    Type = PlayerType.Console,
    Name = "Player1",
    Team = team,
    PrintDebug = false  // Set to true to enable debug output
};
```

## Behavior

### When PrintDebug = false (default)
- No internal debug console output from PlayerRandom or PlayerGui
- Clean console output showing only essential information
- User-facing UI from PlayerConsole remains visible

### When PrintDebug = true
- Full debug logging from PlayerRandom showing:
  - Choice requests and responses
  - Auto-selection notifications
- Full debug logging from PlayerGui showing:
  - Constructor initialization details
  - GuiWindow and ChoiceCoordinator hash codes
  - Choice request handling
  - Timeout notifications

## Benefits
1. **Cleaner Output**: Users can run battles without verbose debug logging
2. **Consistent Pattern**: Matches the existing `Battle.DebugMode` pattern used elsewhere in the codebase
3. **Easy Debugging**: Developers can enable debug output per-player when needed
4. **Thread-Safe**: Debug logging doesn't interfere with MonoGame's single-threaded rendering

## Testing
Build successful with all changes. The debug output can now be controlled independently for each player via their `PlayerOptions.PrintDebug` property.
