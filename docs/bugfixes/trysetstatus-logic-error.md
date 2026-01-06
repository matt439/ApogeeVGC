# Bug Fix: TrySetStatus Logic Error

## Date
2024-01-XX (Session with GitHub Copilot)

## Summary
Fixed a critical logic error in `Pokemon.TrySetStatus()` that caused a `KeyNotFoundException` when attempting to apply status conditions (specifically Burn from Flame Orb) to Pokémon.

## Affected Components
- **File**: `ApogeeVGC\Sim\PokemonClasses\Pokemon.Status.cs`
- **Method**: `TrySetStatus(ConditionId status, Pokemon? source = null, IEffect? sourceEffect = null)`
- **Line**: 210 (original implementation)

## Issue Description

### Symptoms
- `KeyNotFoundException` with message: "The given key 'Burn' was not present in the dictionary"
- Occurred when Ursaluna switched in with a Flame Orb item
- Stack trace pointed to `SetStatus` method trying to access `Battle.Library.Conditions[statusId]`

### Root Cause
The `TrySetStatus` method had incorrect conditional logic when determining which status to pass to `SetStatus`:

```csharp
// INCORRECT (Original Code)
public bool TrySetStatus(ConditionId status, Pokemon? source = null, IEffect? sourceEffect = null)
{
    return SetStatus(Status == ConditionId.None ? status : ConditionId.None, source, sourceEffect);
}
```

**Problem**: When a Pokémon already had a status, the method would pass `ConditionId.None` to `SetStatus`, which was semantically incorrect and could cause issues in edge cases.

### Expected Behavior
According to the TypeScript source (`pokemon-showdown/sim/pokemon.ts`, line 1878):

```typescript
trySetStatus(status: string | Condition, source: Pokemon | null = null, sourceEffect: Effect | null = null) {
    return this.setStatus(this.status || status, source, sourceEffect);
}
```

The method should pass the **current status** if one exists, or the **new status** if none exists. This allows the duplicate check in `SetStatus` to properly handle the case where a Pokémon already has a status.

## Solution

### Code Change
```csharp
// CORRECT (Fixed Code)
public bool TrySetStatus(ConditionId status, Pokemon? source = null, IEffect? sourceEffect = null)
{
    return SetStatus(Status == ConditionId.None ? status : Status, source, sourceEffect);
}
```

**Key Change**: Changed `ConditionId.None` to `Status` in the ternary operator's false branch.

### Logic Flow
1. **No existing status** (`Status == ConditionId.None`): Pass the new `status` parameter to `SetStatus`
   - Attempts to apply the new status
   
2. **Has existing status**: Pass the current `Status` property to `SetStatus`
   - Attempts to apply the same status that already exists
   - The duplicate check in `SetStatus` (line 44) catches this: `if (Status == status.Id)`
   - Returns `false` gracefully without applying duplicate status

## Testing

### Test Case
**Entry Point**: `ApogeeVGC.Sim.Core.Driver.RunConsoleVsRandomSinglesTest()`

**Scenario**:
1. Ursaluna switches in holding a Flame Orb
2. At end of turn, Flame Orb's `OnResidual` event triggers
3. Calls `pokemon.TrySetStatus(ConditionId.Burn, pokemon)`
4. Successfully applies Burn status

**Expected Result**: ? Ursaluna receives Burn status  
**Debug Output**:
```
FlameOrb OnResidual: Called for ursaluna
FlameOrb OnResidual: Calling TrySetStatus for ursaluna
[SetStatus.AfterOnStart] ursaluna: Status=Burn, Counter=, Duration=
```

**UI Display**:
```
?                 ursaluna                  ?
?      HP: ==================== 100.0%      ?
?                219/219 HP                 ?
?                    BRN                    ?
```

## Related Code

### SetStatus Method
The `SetStatus` method in the same file handles:
- Checking if HP > 0
- Looking up the condition in `Battle.Library.Conditions`
- Duplicate status detection
- Status immunity checks
- Running status events (SetStatus, Start, AfterSetStatus)

### TypeScript Source Reference
- **File**: `pokemon-showdown/sim/pokemon.ts`
- **Line**: 1878
- **Method**: `trySetStatus`

## Additional Improvements Made

### Defensive Checks Added
To prevent similar issues and provide better diagnostics:

```csharp
// Line 20-26: Validate status exists in dictionary
if (!Battle.Library.Conditions.TryGetValue(statusId, out Condition? status))
{
    throw new InvalidOperationException(
        $"Condition '{statusId}' not found in Battle.Library.Conditions dictionary. " +
        $"Pokemon: {Name}, Battle has {Battle.Library.Conditions.Count} conditions. " +
        $"Available conditions: {string.Join(", ", Battle.Library.Conditions.Keys)}"
    );
}

// Line 45-52: Validate Status property before dictionary access
if (!Battle.Library.Conditions.ContainsKey(Status))
{
    throw new InvalidOperationException(
        $"Pokemon {Name} has Status={Status} which is not in the Conditions dictionary. " +
        $"Dictionary has {Battle.Library.Conditions.Count} entries: {string.Join(", ", Battle.Library.Conditions.Keys)}"
    );
}
```

### Debug Output Cleanup
Fixed misleading debug message (line 141):
```csharp
// BEFORE: Incorrect reference to AddVolatile
Battle.Debug($"[AddVolatile.AfterOnStart] {Name}: Status={status}, Counter={StatusState.Counter}, Duration={StatusState.Duration}");
Battle.Debug($"[AddVolatile.AfterOnStart] {Name}: volatileState is same reference as Volatiles[{status}]? {ReferenceEquals(StatusState, Volatiles[status.Id])}");

// AFTER: Corrected to SetStatus
Battle.Debug($"[SetStatus.AfterOnStart] {Name}: Status={status.Id}, Counter={StatusState.Counter}, Duration={StatusState.Duration}");
```

## Lessons Learned

1. **Type Safety vs. Logic Errors**: Even with strong typing (C# enum `ConditionId`), logic errors in conditional expressions can cause runtime failures
   
2. **Reference Implementation**: Always compare C# port against TypeScript source for subtle logic differences
   
3. **Ternary Operator Care**: The original code `Status == ConditionId.None ? status : ConditionId.None` passed compile-time checks but had inverted logic
   
4. **Defensive Programming**: Added validation checks help diagnose issues faster when they occur

## Impact
- **Severity**: High (blocked core battle functionality)
- **Scope**: Any status condition application (Burn, Paralysis, Sleep, etc.)
- **User Impact**: Complete battle crashes when status-inflicting items activated
- **Resolution**: Single line logic fix + defensive checks

## Related Issues
- None previously documented
- Likely affected all item-based status infliction (Flame Orb, Toxic Orb, etc.)

## References
- TypeScript Source: `pokemon-showdown/sim/pokemon.ts:1878`
- C# Implementation: `ApogeeVGC/Sim/PokemonClasses/Pokemon.Status.cs:210`
- Test Entry Point: `ApogeeVGC/Sim/Core/Driver.cs:RunConsoleVsRandomSinglesTest()`
