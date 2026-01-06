# Switch-In and Animation Fix

## Issues Fixed

### 1. Pokémon Not Appearing After Switch-In
**Problem**: When a Pokémon fainted and was replaced, the replacement Pokémon's sprite did not appear.

**Root Cause**: The `FindPokemonForSwitch` method in `GuiBattleState` was using `FirstOrDefault()` incorrectly to find empty slots:
```csharp
var emptySlot = _playerActive.FirstOrDefault(kvp => 
    kvp.Value.IsFainted || !kvp.Value.IsActive);
int slot = emptySlot.Value != null ? emptySlot.Key : 0;
```

This doesn't work because:
- `FirstOrDefault` on a dictionary returns `KeyValuePair<int, PokemonState>` (a struct)
- Checking `emptySlot.Value != null` doesn't work as expected for structs
- If no matching element is found, `default(KeyValuePair)` is returned with Key=0 and Value=null

**Fix**: Replaced with explicit foreach loop:
```csharp
int slot = -1;
foreach (var kvp in _opponentActive)
{
    if (kvp.Value.IsFainted || !kvp.Value.IsActive)
    {
        slot = kvp.Key;
        break;
    }
}
if (slot < 0) slot = 0; // Fallback
```

### 2. State Validation Failures
**Problem**: After a Pokémon fainted, state validation failed with HP mismatches.

**Root Cause**: 
- Fainted Pokémon stayed in `_playerActive/_opponentActive` dictionaries with non-zero HP
- When calculating which slot to switch into, the fainted Pokémon was found but its HP was stale

**Fix**: The fix for issue #1 also resolves this by properly identifying fainted Pokémon slots using `IsFainted` flag.

### 3. No Animations Showing
**Problem**: Battle animations (projectiles, damage indicators) were not displaying.

**Status**: This issue requires further investigation. Possible causes:
- `AnimationCoordinator.ProcessMessage()` may not be creating animations for certain message types
- Animation queue might not be advancing properly
- Pokemon positions might not be registered correctly

**Next Steps**:
1. Add logging to `AnimationCoordinator.ProcessMessage()` to verify it's being called
2. Check if animations are being added to the queue
3. Verify `AnimationManager.HasActiveAnimations` is working correctly
4. Ensure Pokemon positions are registered before animations are triggered

## Testing
Run GUI vs Random Doubles battle and verify:
- ? Pokémon appear correctly after switch-in
- ? State validation passes after faints and switches
- ? Animations display (requires further work)

## Files Modified
- `ApogeeVGC\Gui\State\GuiBattleState.cs`
  - Fixed `FindPokemonForSwitch` slot lookup logic
  - Simplified `HandleDamage` HP calculation
