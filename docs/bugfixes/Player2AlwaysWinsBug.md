# Player 2 Always Wins Bug

## Summary
When running random vs random battles with identical teams, Player 2 wins 100% of battles regardless of PRNG seeds used. This indicates a systematic structural bias in the battle system favoring the second player.

## Reproduction
1. Run `DriverMode.RandomVsRandomSinglesEvaluation` with 100+ battles
2. Both players use identical teams from `TeamGenerator.GenerateTestTeam()`
3. Players use different random seeds
4. Result: Player 2 wins 100/100 battles (0% for Player 1)

## Investigation Findings

### Tests Performed
1. **Swapped PRNG Seeds**: Player 2 still wins 100%
2. **Swapped PlayerOptions**: Player 2 still wins 100%
3. **Independent Seeds** (widely spaced): Player 2 still wins 100%
4. **Different seed ranges**: Player 2 still wins 100%

### Ruled Out
- PRNG seed correlation or bias
- Seed increment pattern (consecutive vs independent)
- Team initialization order
- SpeedSort algorithm correctness
- FractionalPriority calculation
- Action queue initial ordering

### Key Observations
1. Both players make DIFFERENT move choices (verified via logging)
2. Both teams have IDENTICAL stats and Pokemon
3. Speed ties occur frequently (identical teams)
4. Battle PRNG (for speed tie resolution) uses different seed each battle
5. Despite all randomness, Player 2 ALWAYS wins

### Debug Output Example
Turn 1:
- P1 Calyrex (Speed 70) uses Trick Room (Priority -7)
- P2 Ursaluna (Speed 86) uses Protect (Priority +4)
- P2 moves first (correct: higher priority)

Turn 2 (Trick Room active):
- P1 Calyrex (Speed 9930) uses Trick Room (Priority -7)  
- P2 Ursaluna (Speed 9914) uses Facade (Priority 0)
- P2 moves first (correct: higher priority, Trick Room inverts speed but priority still matters)

## Hypotheses to Investigate

### 1. Hidden Player 2 Advantage in Battle Logic
- Is there code that systematically favors P2 in damage calculation?
- Does P2 get more favorable RNG rolls from Battle PRNG?
- Are there any hardcoded advantages for "side 2" or "opponent"?

### 2. Team/Pokemon Reference Sharing
- Could there be object reference sharing causing P1's Pokemon to be weaker?
- Are stat calculations done differently for P1 vs P2?

### 3. TypeScript vs C# Translation Issue
- Is there a subtle difference in how Pokemon Showdown TypeScript handles players?
- Could there be an array indexing or player ID issue?

### 4. Move Selection Bias
- Does the PlayerRandom algorithm have a subtle bias?
- Are certain move selection patterns systematically better?

## Next Steps
1. Compare with Pokemon Showdown TypeScript implementation
2. Add detailed logging of damage calculations for both players
3. Check if P1 and P2 Pokemon objects are truly independent
4. Review all code that treats P1/P2 differently
5. Test with asymmetric teams to see if bias persists

## Files Involved
- `ApogeeVGC/Sim/Core/Driver.cs` - Test harness
- `ApogeeVGC/Sim/Player/PlayerRandom.cs` - Random player implementation
- `ApogeeVGC/Sim/BattleClasses/Battle.Sorting.cs` - Speed tie resolution
- `ApogeeVGC/Sim/BattleClasses/BattleQueue.cs` - Action queue management
- `ApogeeVGC/Sim/BattleClasses/Battle.Requests.cs` - Turn processing

## Impact
- **Severity**: Critical
- **Affects**: All random vs random testing, AI training, balance testing
- **Workaround**: None - bias is systematic

## Additional Notes
- Issue is reproducible 100% of the time
- Not affected by any seed manipulation
- Suggests fundamental design or implementation flaw
- May affect non-random battles if bias exists in core battle logic
