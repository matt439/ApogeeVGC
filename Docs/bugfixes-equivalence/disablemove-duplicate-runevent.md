# Duplicate RunEvent(DisableMove) in GetMoveRequestData

## Commit
`e6127435` — Remove duplicate RunEvent(DisableMove) from GetMoveRequestData causing extra PRNG calls

## Symptom
Damage values differed between C# and Showdown (e.g., Moonblast dealing 36 in C# vs 32 in Showdown) despite PRNG final state matching. The randomizer was reading from a shifted position in the PRNG stream.

## Root Cause
`Pokemon.GetMoveRequestData()` cleared all move disabled states and re-ran `Battle.RunEvent(EventId.DisableMove, this)` every time it was called. This method is called from both `Battle.GetRequests()` (generating request data) and `Side.ChooseMove()` (validating move choices).

In Showdown, `getMoveRequestData()` just reads the already-set disabled states — the clearing and RunEvent happen once during `makeRequest` preprocessing (in `battle.ts` lines 1641-1652). The C# equivalent already existed in `Battle.Lifecycle.cs` (lines 158-183).

Each redundant `RunEvent(DisableMove)` call triggered `SpeedSort` on the event handlers, and when handlers had speed ties, the Fisher-Yates shuffle consumed PRNG calls that Showdown didn't make. This shifted the PRNG stream, causing the damage randomizer to produce different values.

## Fix
Removed the clearing loop and `RunEvent(DisableMove)` call from `GetMoveRequestData()`, leaving only a comment explaining that disabled states are set by the lifecycle preprocessing. Also fixed Steelworker and Steely Spirit abilities to return `VoidReturn()` for non-matching move types (matching Showdown's `undefined` return).

## Pattern
**Redundant event dispatch** — if an event is already fired during lifecycle preprocessing, it should NOT be re-fired during data access methods. Re-running events consumes PRNG calls via SpeedSort shuffles.

## Files Changed
- `ApogeeVGC/Sim/PokemonClasses/Pokemon.Requests.cs`
- `ApogeeVGC/Data/Abilities/AbilitiesSTU.cs`
- `ApogeeVGC/Sim/BattleClasses/BattleActions.Damage.cs` (debug trace cleanup)
