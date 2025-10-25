using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleAsync
{
    /// <summary>
    /// Returns whether a proposed target location for a move is valid.
    /// Validates targeting based on move type, battlefield positions, and adjacency rules.
    /// </summary>
    /// <param name="targetLoc">The target location (0 = no target, positive = foe, negative = ally)</param>
    /// <param name="source">The Pokémon using the move</param>
    /// <param name="targetType">The move's targeting type (e.g., Normal, AdjacentAlly, Any)</param>
    /// <returns>True if the target location is valid for this move, false otherwise</returns>
    public bool ValidTargetLoc(int targetLoc, Pokemon source, MoveTarget targetType)
    {
        // targetLoc of 0 is always valid (no specific target)
        if (targetLoc == 0) return true;

        // Get the number of active slots per side
        int numSlots = ActivePerHalf;

        // Get the source's relative location
        int sourceLoc = source.GetLocOf(source);

        // Target location must be within valid range
        if (Math.Abs(targetLoc) > numSlots) return false;

        // Determine if targeting self
        bool isSelf = sourceLoc == targetLoc;

        // Determine if targeting a foe (positive locations are foes in team battles)
        bool isFoe = targetLoc > 0;

        // Calculate the location directly across from the target
        int acrossFromTargetLoc = -(numSlots + 1 - targetLoc);

        // Determine if the target is adjacent to the source
        bool isAdjacent;
        if (targetLoc > 0)
        {
            // For foes: check if the across-field position is within 1 slot
            isAdjacent = Math.Abs(acrossFromTargetLoc - sourceLoc) <= 1;
        }
        else
        {
            // For allies: check if positions differ by exactly 1
            isAdjacent = Math.Abs(targetLoc - sourceLoc) == 1;
        }

        // Check validity based on move's target type
        return targetType switch
        {
            // Normal moves target any adjacent Pokémon
            MoveTarget.RandomNormal or
            MoveTarget.Scripted or
            MoveTarget.Normal => isAdjacent,

            // Ally-only moves
            MoveTarget.AdjacentAlly => isAdjacent && !isFoe,

            // Ally or self
            MoveTarget.AdjacentAllyOrSelf => isAdjacent && !isFoe || isSelf,

            // Foe-only moves
            MoveTarget.AdjacentFoe => isAdjacent && isFoe,

            // Any target except self
            MoveTarget.Any => !isSelf,

            // Self-targeting moves
            MoveTarget.Self => isSelf,

            // Unknown target type - invalid
            _ => false,
        };
    }

    public bool ValidTarget(Pokemon target, Pokemon source, MoveTarget targetType)
    {
        return ValidTargetLoc(source.GetLocOf(target), source, targetType);
    }

    public Pokemon? GetTarget(Pokemon pokemon, MoveId moveId, int targetLoc, Pokemon? originalTarget = null)
    {
        Move move = Library.Moves[moveId];
        return GetTarget(pokemon, move, targetLoc, originalTarget);
    }

    public Pokemon? GetTarget(Pokemon pokemon, Move move, int targetLoc, Pokemon? originalTarget = null)
    {
        bool tracksTarget = move.TracksTarget ?? false;

        // Stalwart and Propeller Tail abilities set tracksTarget in ModifyMove, 
        // but ModifyMove happens after getTarget, so we need to manually check for them here
        if (pokemon.HasAbility([AbilityId.Stalwart, AbilityId.PropellerTail]))
        {
            tracksTarget = true;
        }

        // If move tracks target and original target is still active, target it
        if (tracksTarget && originalTarget?.IsActive == true)
        {
            return originalTarget;
        }

        // Smart target moves (like Dragon Darts) intelligently retarget
        // banning Dragon Darts from directly targeting itself is done in side.ts, but
        // Dragon Darts can target itself if Ally Switch is used afterwards
        if (move.SmartTarget == true)
        {
            Pokemon curTarget = pokemon.GetAtLoc(targetLoc);
            return curTarget is { Fainted: false } ? curTarget : GetRandomTarget(pokemon, move);
        }

        // Fails if the target is the user and the move can't target its own position
        int selfLoc = pokemon.GetLocOf(pokemon);

        // Check if move is trying to target self when it shouldn't
        if ((move.Target is MoveTarget.AdjacentAlly or MoveTarget.Any or MoveTarget.Normal) &&
            targetLoc == selfLoc &&
            !pokemon.Volatiles.ContainsKey(ConditionId.LockedMove) && // Two-turn move (e.g., Fly, Dig)
            !pokemon.Volatiles.ContainsKey(ConditionId.IceBall) &&
            !pokemon.Volatiles.ContainsKey(ConditionId.Rollout))
        {
            // Future moves can target the user
            return (move.Flags.FutureMove == true) ? pokemon : null;
        }

        // Check if target location is valid
        if (move.Target != MoveTarget.RandomNormal && ValidTargetLoc(targetLoc, pokemon, move.Target))
        {
            Pokemon target = pokemon.GetAtLoc(targetLoc);

            // Handle fainted targets
            if (target.Fainted)
            {
                // Check if target is an ally
                if (target.IsAlly(pokemon))
                {
                    // Gen 5: AdjacentAllyOrSelf moves retarget to user when ally faints
                    if (move.Target == MoveTarget.AdjacentAllyOrSelf && Gen != 5)
                    {
                        return pokemon;
                    }
                    // Target is a fainted ally: attack shouldn't retarget
                    return target;
                }
            }

            // Target is unfainted: use selected target location
            if (target is { Fainted: false })
            {
                return target;
            }

            // Chosen target not valid, retarget randomly with getRandomTarget
        }

        return GetRandomTarget(pokemon, move);
    }

    public Pokemon? GetRandomTarget(Pokemon pokemon, MoveId move)
    {
        return GetRandomTarget(pokemon, Library.Moves[move]);
    }

    public Pokemon? GetRandomTarget(Pokemon pokemon, Move move)
    {
        // A move was used without a chosen target

        // For instance: Metronome chooses Ice Beam. Since the user didn't
        // choose a target when choosing Metronome, Ice Beam's target must
        // be chosen randomly.

        // The target is chosen randomly from possible targets, EXCEPT that
        // moves that can target either allies or foes will only target foes
        // when used without an explicit target.

        // Self-targeting moves always target the user
        if (move.Target is MoveTarget.Self or
                           MoveTarget.All or
                           MoveTarget.AllySide or
                           MoveTarget.AllyTeam or
                           MoveTarget.AdjacentAllyOrSelf)
        {
            return pokemon;
        }

        // Adjacent ally targeting
        if (move.Target == MoveTarget.AdjacentAlly)
        {
            // In singles, there are no allies to target
            if (GameType == GameType.Singles) return null;

            // Get adjacent allies and return random one if available
            var adjacentAllies = pokemon.AdjacentAllies();
            return adjacentAllies.Count > 0 ? Sample(adjacentAllies) : null;
        }

        // Singles battles: target the opponent's active Pokémon
        if (GameType == GameType.Singles)
        {
            return pokemon.Side.Foe.Active[0];
        }

        // Triples battles (activePerHalf > 2)
        if (ActivePerHalf > 2)
        {
            // Adjacent-only moves need special handling
            if (move.Target is MoveTarget.AdjacentFoe or
                              MoveTarget.Normal or
                              MoveTarget.RandomNormal)
            {
                // Even if a move can target an ally, auto-resolution will never make it target an ally
                // i.e. if both your opponents faint before you use Flamethrower, it will fail 
                // instead of targeting your ally
                var adjacentFoes = pokemon.AdjacentFoes();
                if (adjacentFoes.Count > 0)
                {
                    return Sample(adjacentFoes);
                }

                // No valid target at all, return slot directly across for any possible redirection
                // This calculates the position mirrored across the field
                int mirrorPosition = pokemon.Side.Foe.Active.Count - 1 - pokemon.Position;
                return pokemon.Side.Foe.Active[mirrorPosition];
            }
        }

        // Default: return random foe, or first active foe if random fails
        return pokemon.Side.RandomFoe() ?? pokemon.Side.Foe.Active[0];
    }

    /// <summary>
    /// Updates the target of the most recently logged move.
    /// Used when a move's target changes after it was initially logged (e.g., through redirection).
    /// </summary>
    /// <param name="newTarget">The new target Pokémon to display in the move log</param>
    public void RetargetLastMove(Pokemon newTarget)
    {
        // No last move to retarget
        if (LastMoveLine < 0) return;

        // Parse the log line (format: |move|attacker|moveName|target|...)
        string[] parts = Log[LastMoveLine].Split('|');

        // Index 4 is the target field in the move log format
        if (parts.Length > 4)
        {
            parts[4] = newTarget.ToString();
            Log[LastMoveLine] = string.Join("|", parts);
        }
    }
}