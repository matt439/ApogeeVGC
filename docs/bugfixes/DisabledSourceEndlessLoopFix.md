# Disabled Source Endless Loop Fix

## Problem Summary

After a bug fix involving JSON serialization and the move 'Imprison' (commit 06a1221), battle simulations entered persistent endless loops. Random battles consistently timed out after 3000ms, failing at around 493-499 battles out of 500 in the incremental test runner.

**Symptoms**:
- `BattleTimeoutException` thrown consistently across different moves (Incinerate, InfernalParade, Inferno, etc.)
- Battles running indefinitely when players tried to select disabled moves
- Same seed combinations reliably reproduced the timeout

## Root Cause

The bug was introduced in commit 06a1221 which changed `DisabledSource` from `IEffect?` to `EffectStateId?` for safer JSON serialization. However, this change exposed a critical flaw in the choice validation system:

### The Infinite Loop Mechanism

1. **Request Creation**: At turn start, `GetMoveRequestData()` creates `PokemonMoveRequestData` with `DisabledSource` set to an `EffectStateId` (e.g., `ItemEffectStateId(ItemId.ChoiceSpecs)`)

2. **Choice Validation**: Player selects a disabled move ? `ChooseMove` calls `EmitChoiceError` with `UpdateDisabledRequestForMove` callback

3. **Update Detection But No Modification**: `UpdateDisabledRequestForMove` checks:
   ```csharp
   bool needsUpdate = m.Disabled is null or BoolMoveIdBoolUnion { Value: false } ||
                      m.DisabledSource != disabledSource;
   
   if (needsUpdate)
   {
       updated = true;  // ? Only sets flag, doesn't modify data!
   }
   ```

4. **Stale Request Re-emission**: `EmitRequest` serializes the **unchanged** `ActiveRequest` with `Update = true` flag

5. **AI Receives Stale Data**: AI gets the same incorrect disabled state and makes the same invalid choice

6. **Repeat Forever**: Steps 2-5 loop indefinitely ? `BattleTimeoutException`

### Why `UpdateDisabledRequestForMove` Didn't Update

The method **detected** that an update was needed but **never actually modified** the data:

```csharp
private BoolVoidUnion UpdateDisabledRequestForMove(Pokemon pokemon, PokemonMoveRequestData req,
    MoveId moveid, EffectStateId? disabledSource)
{
    bool updated = UpdateDisabledRequest(pokemon, req);

    foreach (PokemonMoveData m in req.Moves)
    {
        if (m.Id != moveid) continue;

        bool needsUpdate = m.Disabled is null or BoolMoveIdBoolUnion { Value: false } ||
                           m.DisabledSource != disabledSource;

        if (needsUpdate)
        {
            updated = true;  // ? BUG: Only flagged, never updated m.Disabled or m.DisabledSource
        }

        break;
    }

    return BoolVoidUnion.FromBool(updated);
}
```

### Why This Wasn't Caught Before

The original code had the same logic flaw:
```csharp
// Old code (also buggy but hidden):
disabledSource = m.DisabledSource.Name;  // string
m.DisabledSource?.Name != disabledSource  // string comparison
```

The bug existed but was masked because:
- String comparisons might coincidentally match even with stale data
- `IEffect` references could point to the same singleton instance
- The bug only manifested when `DisabledSource` values actually differed between requests

The refactor to `EffectStateId` exposed the flaw by making value comparisons more precise and reliable.

## Technical Details

### Data Flow Problem

**Turn Start Flow**:
```
Battle.GetRequests()
  ? Pokemon.GetMoveRequestData()
     ? Clears MoveSlots (DisabledSource = null)
     ? RunEvent(EventId.DisableMove)
        ? ChoiceLock.OnDisableMove sets DisabledSource
     ? GetMoves() copies DisabledSource to PokemonMoveData
  ? Stored in ActiveRequest
```

**Choice Validation Flow**:
```
Player selects disabled move
  ? Side.ChooseMove()
     ? GetMoves() creates fresh PokemonMoveData (different instance!)
     ? Detects move is disabled
     ? EmitChoiceError(UpdateDisabledRequestForMove)
        ? UpdateRequestForPokemon gets ActiveRequest.Active[index]
        ? UpdateDisabledRequestForMove compares:
           - disabledSource (from fresh GetMoves())
           - m.DisabledSource (from stale ActiveRequest)
        ? Returns true (needs update) but doesn't modify
        ? EmitRequest serializes unchanged ActiveRequest
```

### Why `PokemonMoveData.DisabledSource` Was `init`

Originally defined as:
```csharp
public record PokemonMoveData
{
    public MoveIdBoolUnion? Disabled { get; set; }
    public EffectStateId? DisabledSource { get; init; }  // ? init = immutable after construction
}
```

Records in C# use `init` for immutability by default. This prevents accidental modification but also prevents **intentional** updates in response to game state changes.

## Solution

### Part 1: Make `DisabledSource` Mutable

Changed `DisabledSource` from `init` to `set` accessor:

```csharp
public record PokemonMoveData
{
    public MoveIdBoolUnion? Disabled { get; set; }
    public EffectStateId? DisabledSource { get; set; }  // ? Now mutable
    public int Pp { get; init; }
    public int MaxPp { get; init; }
}
```

**File**: `ApogeeVGC/Sim/Choices/PokemonMoveRequestData.cs`

### Part 2: Actually Update the Data

Modified `UpdateDisabledRequestForMove` to modify properties when update is needed:

```csharp
private BoolVoidUnion UpdateDisabledRequestForMove(Pokemon pokemon, PokemonMoveRequestData req,
    MoveId moveid, EffectStateId? disabledSource)
{
    bool updated = UpdateDisabledRequest(pokemon, req);

    foreach (PokemonMoveData m in req.Moves)
    {
        if (m.Id != moveid) continue;

        bool needsUpdate = m.Disabled is null or BoolMoveIdBoolUnion { Value: false } ||
                           m.DisabledSource != disabledSource;

        if (needsUpdate)
        {
            // ? FIX: Actually update the disabled state
            m.Disabled = true;
            m.DisabledSource = disabledSource;
            updated = true;
        }

        break;
    }

    return BoolVoidUnion.FromBool(updated);
}
```

**File**: `ApogeeVGC/Sim/SideClasses/Side.Choices.cs`

## Impact

**Before Fix**:
- Battles timeout consistently when disabled moves are selected
- Random vs Random battles fail ~99% of the time at ~495 battles
- Incremental test runner cannot progress past moves with disabled states

**After Fix**:
- Disabled move choices properly rejected with updated request data
- AI receives accurate move availability information
- Battles complete normally
- Random vs Random test success rate returns to expected levels

## Testing

Verified fix resolves the issue:
1. Built solution successfully with no compilation errors
2. Previous failing seed combinations (e.g., P1=2539, P2=2540, Battle=2541) now complete normally
3. Incremental test runner can progress past moves that trigger disabled states (Incinerate, InfernalParade, etc.)

## Related Issues

This fix addresses issues introduced in commit 06a1221:
- Changed `DisabledSource` from `IEffect?` to `EffectStateId?` for safer JSON serialization
- Updated `MoveSlot.DisabledSource` and `PokemonMoveData.DisabledSource` types
- Modified `DisableMove` method to use `EffectStateId` instead of `IEffect`

The refactor improved serialization safety but exposed the dormant "detect but don't update" bug in `UpdateDisabledRequestForMove`.

## Prevention Guidelines

### Pattern: Request Update Callbacks Must Actually Update

When implementing update callbacks for `EmitChoiceError`:
1. ? **Don't** just return `true` if update is needed
2. ? **Do** actually modify the request data before returning `true`
3. ? Verify the updated data will serialize correctly
4. ? Test with scenarios that trigger the update path

### Pattern: Mutable vs Immutable Properties

When using C# records:
1. Use `init` for data that should never change after construction (IDs, base values)
2. Use `set` for game state that changes in response to events (disabled state, PP, etc.)
3. Consider whether update callbacks need to modify the property
4. Document why each property is mutable or immutable

### Testing Pattern: Detect Update Loops

When debugging timeout issues:
1. Check if the same request is being emitted repeatedly
2. Look for update callbacks that detect changes but don't apply them
3. Verify that `updated = true` is accompanied by actual data modification
4. Add logging to track request serialization and re-emission

## Files Modified

1. **ApogeeVGC/Sim/Choices/PokemonMoveRequestData.cs**
   - Changed `DisabledSource` from `init` to `set`

2. **ApogeeVGC/Sim/SideClasses/Side.Choices.cs**
   - Modified `UpdateDisabledRequestForMove` to actually update `Disabled` and `DisabledSource`

## Keywords

`endless loop`, `infinite loop`, `battle timeout`, `BattleTimeoutException`, `disabled move`, `DisabledSource`, `EffectStateId`, `UpdateDisabledRequestForMove`, `EmitChoiceError`, `choice validation`, `request update`, `JSON serialization`, `init accessor`, `set accessor`, `mutable`, `immutable`, `record type`, `ChoiceLock`, `Choice item`, `incremental test`, `random battles`, `stale data`, `request re-emission`
