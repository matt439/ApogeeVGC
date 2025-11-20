# Player 2 Always Wins Bug Fix

## Bug Description
When running random vs random battles with identical teams, Player 2 appeared to win 100% of battles regardless of PRNG seeds used. This indicated what seemed to be a systematic structural bias in the battle system favoring the second player.

## Root Cause
The bug was **not** in the battle mechanics, but in the **winner reporting logic** in `SyncSimulator.cs`.

### The Problem
In `SyncSimulator.DetermineWinner()`, the code compared `Battle.Winner` (which contains the side's **name**, e.g., "Player 1" or "Player 2") against the hardcoded string `"p1"`:

```csharp
bool isP1Winner = Battle.Winner.Equals("p1", StringComparison.OrdinalIgnoreCase);
return isP1Winner ? SimulatorResult.Player1Win : SimulatorResult.Player2Win;
```

Since `Battle.Winner` is set to `side.Name` (e.g., "Player 1" with a space), this comparison **always failed** for Player 1. The code would then fall through to the else case and always return `SimulatorResult.Player2Win`, even when Player 1 actually won the battle.

## Investigation Process
1. **Initial hypothesis**: Turn order bias or damage calculation favoring P2
2. **Testing**: Added extensive logging for damage, turn order, and PRNG shuffles
3. **Discovery**: Speed ties were being properly shuffled, damage was symmetric
4. **Key insight**: Turn order varied (sometimes P1 first, sometimes P2), yet P2 "won" 100%
5. **Final discovery**: Winner detection logic was comparing player names wrong

## Solution
Updated `SyncSimulator.DetermineWinner()` to properly compare against actual side names:

```csharp
private SimulatorResult DetermineWinner()
{
    if (!string.IsNullOrEmpty(Battle!.Winner))
    {
        // Compare against actual side names
        string p1Name = Battle.P1.Name;
        string p2Name = Battle.P2.Name;
        
        if (Battle.Winner.Equals(p1Name, StringComparison.OrdinalIgnoreCase))
        {
            return SimulatorResult.Player1Win;
        }
        else if (Battle.Winner.Equals(p2Name, StringComparison.OrdinalIgnoreCase))
        {
            return SimulatorResult.Player2Win;
        }
        
        // Fallback for backwards compatibility with side IDs
        if (Battle.Winner.Equals("p1", StringComparison.OrdinalIgnoreCase))
        {
            return SimulatorResult.Player1Win;
        }
        else if (Battle.Winner.Equals("p2", StringComparison.OrdinalIgnoreCase))
        {
            return SimulatorResult.Player2Win;
        }
        
        Console.WriteLine($"WARNING: Unknown winner format: '{Battle.Winner}'");
        return SimulatorResult.Tie;
    }

    return DetermineTiebreakWinner();
}
```

## Verification
After the fix, running 100 random vs random battles with identical teams resulted in:
- **Player 1 Wins: 49 (49.0%)**
- **Player 2 Wins: 51 (51.0%)**

This is the expected distribution for truly random battles (approximately 50/50 with normal variance).

## Files Modified
- `ApogeeVGC/Sim/Core/SyncSimulator.cs` - Fixed winner detection logic

## Impact
- **Severity**: Critical (affected all battle simulations)
- **Scope**: Winner reporting only - battle mechanics were working correctly
- **Testing**: All random vs random testing, AI training, and balance testing now report accurate results

## Lessons Learned
1. When investigating bugs, verify assumptions about what the code is actually comparing
2. String comparisons can be subtle - always check what the actual string values are
3. Winner detection should ideally use enums or structured types rather than string comparisons
4. Apparent biases in random systems may be reporting bugs, not RNG issues

## Date
2024-01-XX

## Related Issues
- None - this was the original issue

## See Also
- `docs/bugfixes/Player2AlwaysWinsBug.md` - Original bug investigation notes
