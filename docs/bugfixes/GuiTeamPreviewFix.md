# GUI Team Preview Fix

## Problem
When running GUI battles, no Pokémon were displayed during the team preview phase. The screen showed an empty battlefield with no Pokémon sprites or information.

## Root Cause
The `BattleRenderer` was receiving perspectives with the wrong `BattlePerspectiveType`. When `RequestPlayerChoices()` was called during team preview, it sent perspectives with type `InBattle` instead of `TeamPreview`.

### Technical Details
1. `Battle.Requests.RequestPlayerChoices()` (line 650) called `UpdateAllPlayersUi()` without any parameters
2. `Battle.Logging.UpdateAllPlayersUi()` has a default parameter: `BattlePerspectiveType battlePerspectiveType = BattlePerspectiveType.InBattle`
3. This caused all perspectives to be created with `InBattle` type
4. `BattleRenderer.Render()` routes based on perspective type:
   - `InBattle` ? `RenderInBattle()` (only shows active Pokémon)
   - `TeamPreview` ? `RenderTeamPreview()` (shows full team)
5. Since active Pokémon are empty/null during team preview, nothing was rendered

## Solution
Modified `Battle.Requests.RequestPlayerChoices()` to determine the correct perspective type based on `RequestState` and pass it explicitly to `UpdateAllPlayersUi()`:

```csharp
// Determine the perspective type based on request state
BattlePerspectiveType perspectiveType = RequestState == RequestState.TeamPreview
    ? BattlePerspectiveType.TeamPreview
    : BattlePerspectiveType.InBattle;
UpdateAllPlayersUi(perspectiveType);
```

## Files Changed
- **ApogeeVGC/Sim/BattleClasses/Battle.Requests.cs** (line 648-650)
  - Added logic to determine correct perspective type before calling `UpdateAllPlayersUi()`

## Testing
After the fix:
1. Run the program with `DriverMode.GuiVsRandomDoubles` or `DriverMode.GuiVsRandomSingles`
2. The team preview screen now correctly displays:
   - All 6 Pokémon from your team (bottom row)
   - All 6 Pokémon from opponent's team (top row)
   - Pokémon sprites, names, levels, items, and status
   - Click to select lead Pokémon

## Impact
- **Scope**: GUI-only bug (console player was unaffected)
- **Severity**: High - Made GUI battles completely unusable during team preview
- **Risk**: Low - Change is isolated to perspective type determination

## Related Systems
- `BattleRenderer.cs` - Correctly routes based on perspective type (no changes needed)
- `Battle.Validation.RunPickTeam()` - Correctly calls `UpdateAllPlayersUi(BattlePerspectiveType.TeamPreview)` initially
- `Battle.Logging.UpdateAllPlayersUi()` - Default parameter value now properly overridden

## Lessons Learned
1. **Default parameters can hide bugs** - The default `InBattle` value was applied silently
2. **Perspective type matters** - Different phases require different rendering logic
3. **Request state is the source of truth** - `RequestState` correctly tracks battle phase
4. **Console output helped debug** - Logging showed `InBattle` when it should have been `TeamPreview`
