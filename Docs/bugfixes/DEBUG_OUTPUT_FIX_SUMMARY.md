# Debug Output Fix Summary

## Problem
The `RunGuiVsRandomSinglesTest` method in `Driver.cs` was producing excessive debug console output even when `Debug = false` was set in `BattleOptions`. This was because many Console.WriteLine statements throughout the codebase were not checking the `Battle.DebugMode` property before writing to the console.

## Solution
Wrapped all diagnostic Console.WriteLine statements with conditional checks against `Battle.DebugMode`:

```csharp
if (Battle.DebugMode)
{
    Console.WriteLine("[Debug] Some diagnostic message");
}
```

## Files Modified

### 1. ApogeeVGC\Sim\BattleClasses\Battle.Lifecycle.cs
- Wrapped 8 Console.WriteLine statements in the `Start()` method with `DebugMode` checks
- These statements logged battle initialization steps like "Adding StartGameAction to queue", "RunPickTeam() returned", etc.

### 2. ApogeeVGC\Sim\BattleClasses\Battle.Requests.cs  
- Wrapped Console.WriteLine statements in the following methods:
  - `MakeRequest()`: Entry/exit logging
  - `GetRequests()`: Request type logging
  - `Choose()`: Choice processing logging  
  - `CommitChoices()`: Queue state logging (15+ statements)

### 3. ApogeeVGC\Sim\SideClasses\Side.Choices.cs
- Wrapped Console.WriteLine statements in:
  - `Choose()`: Action processing logging
  - `ProcessChosenTeamAction()`: Team preview selection logging (12+ statements)

### 4. ApogeeVGC\Sim\PokemonClasses\Pokemon.Requests.cs
- Wrapped Console.WriteLine statements in:
  - `GetMoveRequestData()`: Move state logging

### 5. ApogeeVGC\Player\PlayerConsole.cs
- Removed 3 debug Console.WriteLine statements from `GetMoveChoiceAsync()` 
- These were not wrapped with checks since PlayerConsole is user-facing UI and shouldn't have internal debug logging

## Result
When `Debug = false` is set in `BattleOptions`:
- No internal debug console output is produced
- The console only shows intended user-facing output from PlayerConsole (battle UI, move selections, etc.)

When `Debug = true` is set:
- All debug output is shown as before
- Useful for debugging and development

## Testing
Build completed successfully. The changes ensure that `RunGuiVsRandomSinglesTest` with `Debug = false` will no longer produce debug console output like:
- `[Battle.Start] Adding StartGameAction to queue`
- `[MakeRequest] ENTRY: queue size = 1`
- `[Side.Choose] Processing 6 actions for Random`
- `[CommitChoices] Starting, queue size = 1`
- etc.
