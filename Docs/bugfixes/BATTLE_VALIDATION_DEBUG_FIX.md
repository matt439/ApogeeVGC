# Battle.Validation.cs Debug Output Conditional Fix

## Summary
Made all debug console output in `Battle.Validation.cs` conditional based on the `Battle.DebugMode` property, consistent with the pattern used throughout the rest of the battle system.

## Changes Made

### Battle.Validation.cs - RunPickTeam() Method
Wrapped all 9 Console.WriteLine debug statements with `if (DebugMode)` checks:

1. **Method entry**: `"[RunPickTeam] STARTING"`
2. **Format handler call**: `"[RunPickTeam] Calling Format.OnTeamPreview"`
3. **Subformat handler calls**: `"[RunPickTeam] Calling subFormat.OnTeamPreview for {rule}"`
4. **Request state check**: `"[RunPickTeam] Checking RequestState: {RequestState}"`
5. **Early return**: `"[RunPickTeam] Team preview already set up by handler, returning"`
6. **Picked team size**: `"[RunPickTeam] PickedTeamSize = {RuleTable.PickedTeamSize}"`
7. **Making request**: `"[RunPickTeam] Making team preview request"`
8. **After request**: `"[RunPickTeam] After MakeRequest, RequestState = {RequestState}"`
9. **Method exit**: `"[RunPickTeam] COMPLETED"`

## Pattern Consistency

This change maintains consistency with the debug output pattern established in:
- `Battle.Lifecycle.cs` - Battle start and turn loop logging
- `Battle.Requests.cs` - Request processing and choice handling
- `Side.Choices.cs` - Side choice processing
- `Pokemon.Requests.cs` - Move request data logging

## Configuration

The `DebugMode` property is set via `BattleOptions`:

```csharp
BattleOptions battleOptions = new()
{
    Id = FormatId.CustomSingles,
    Player1Options = player1Options,
  Player2Options = player2Options,
    Debug = false  // Controls Battle.DebugMode
};
```

## Behavior

### When Debug = false (default)
- No internal debug output from `RunPickTeam()`
- Clean console showing only user-facing information
- Team preview processes silently

### When Debug = true
- Full debug logging showing:
  - Team preview initialization flow
  - Format handler invocations
  - Request state transitions
  - Team selection processing steps

## Impact

The `RunPickTeam()` method is called during battle initialization to handle team preview. With this change:
- Users running `RunGuiVsRandomSinglesTest` with `Debug = false` will no longer see these internal state transitions
- Developers can still enable full debug output when troubleshooting team preview issues
- Output aligns with user expectations based on the `Debug` setting

## Testing
Build successful with all changes. The debug output in `Battle.Validation.cs` now respects the `Battle.DebugMode` setting, completing the debug output cleanup across all battle subsystems.
