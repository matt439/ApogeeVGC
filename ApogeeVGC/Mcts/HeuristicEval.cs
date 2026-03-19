using ApogeeVGC.Data;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Mcts;

/// <summary>
/// Heuristic battle state evaluation for MCTS.
/// Returns a value in [0, 1] estimating win probability from the given side's perspective.
/// Factors: HP/alive count, type matchup advantage, status conditions, stat boosts.
/// </summary>
public static class HeuristicEval
{
    private static readonly TypeChart TypeChart = new();

    /// <summary>
    /// Evaluate a battle state using HP, type matchups, status, and boosts.
    /// </summary>
    public static float Evaluate(Battle battle, SideId sideId)
    {
        Side ourSide = sideId == SideId.P1 ? battle.P1 : battle.P2;
        Side oppSide = sideId == SideId.P1 ? battle.P2 : battle.P1;

        float ourScore = ComputeSideScore(ourSide);
        float oppScore = ComputeSideScore(oppSide);

        float matchupAdv = ComputeMatchupAdvantage(battle, ourSide, oppSide);
        float statusAdv = ComputeStatusAdvantage(ourSide, oppSide);
        float boostAdv = ComputeBoostAdvantage(ourSide, oppSide);

        // HP/alive is the primary signal; matchup, status, boosts are secondary
        float diff = (ourScore - oppScore)
                     + 0.15f * matchupAdv
                     + 0.10f * statusAdv
                     + 0.05f * boostAdv;

        return 1f / (1f + MathF.Exp(-3f * diff));
    }

    /// <summary>
    /// Computes a side's score as the average of HP fraction and alive fraction.
    /// </summary>
    private static float ComputeSideScore(Side side)
    {
        int total = side.Pokemon.Count;
        if (total == 0) return 0f;

        float hpSum = 0f;
        int alive = 0;

        foreach (Pokemon pokemon in side.Pokemon)
        {
            if (pokemon.MaxHp > 0)
                hpSum += (float)pokemon.Hp / pokemon.MaxHp;

            if (!pokemon.Fainted)
                alive++;
        }

        float hpFraction = hpSum / total;
        float aliveFraction = (float)alive / total;

        return (hpFraction + aliveFraction) / 2f;
    }

    /// <summary>
    /// Computes type matchup advantage for active Pokemon.
    /// Positive = our active Pokemon have better offensive coverage.
    /// Returns roughly [-1, 1].
    /// </summary>
    private static float ComputeMatchupAdvantage(Battle battle, Side ourSide, Side oppSide)
    {
        float ourThreat = 0f;
        float oppThreat = 0f;
        int ourCount = 0;
        int oppCount = 0;

        // Our active Pokemon's best move effectiveness vs each opponent active
        foreach (Pokemon? ourPoke in ourSide.Active)
        {
            if (ourPoke == null || ourPoke.Fainted) continue;

            foreach (Pokemon? oppPoke in oppSide.Active)
            {
                if (oppPoke == null || oppPoke.Fainted) continue;

                float best = BestMoveScore(battle, ourPoke, oppPoke);
                ourThreat += best;
                ourCount++;
            }
        }

        // Opponent active Pokemon's best move effectiveness vs each of our active
        foreach (Pokemon? oppPoke in oppSide.Active)
        {
            if (oppPoke == null || oppPoke.Fainted) continue;

            foreach (Pokemon? ourPoke in ourSide.Active)
            {
                if (ourPoke == null || ourPoke.Fainted) continue;

                float best = BestMoveScore(battle, oppPoke, ourPoke);
                oppThreat += best;
                oppCount++;
            }
        }

        float ourAvg = ourCount > 0 ? ourThreat / ourCount : 0f;
        float oppAvg = oppCount > 0 ? oppThreat / oppCount : 0f;

        return ourAvg - oppAvg;
    }

    /// <summary>
    /// Best move score for attacker against target.
    /// Returns a normalized score where 1.0 = strong super-effective STAB, 0 = no damaging moves.
    /// </summary>
    private static float BestMoveScore(Battle battle, Pokemon attacker, Pokemon target)
    {
        float best = 0f;
        PokemonType[] targetTypes = target.GetTypes();

        foreach (MoveSlot moveSlot in attacker.MoveSlots)
        {
            if (!battle.Library.Moves.TryGetValue(moveSlot.Id, out Move? move))
                continue;

            if (move.Category == MoveCategory.Status || move.BasePower <= 0)
                continue;

            float effectiveness = (float)TypeChart
                .GetMoveEffectiveness(targetTypes, move.Type)
                .ToMultiplier();

            if (effectiveness == 0f) continue; // immune

            // STAB check
            float stab = 1f;
            if (move.Type is not (MoveType.Stellar or MoveType.Unknown))
            {
                PokemonType moveAsPokemonType = move.Type.ConvertToPokemonType();
                if (attacker.HasType(moveAsPokemonType))
                    stab = 1.5f;
            }

            // Normalize: BasePower 100, 1x effectiveness, no STAB = 100
            // Score is relative to that baseline
            float score = move.BasePower * effectiveness * stab / 100f;

            if (score > best)
                best = score;
        }

        return best;
    }

    /// <summary>
    /// Computes status condition advantage.
    /// Negative status on opponent = advantage for us.
    /// Returns roughly [-1, 1].
    /// </summary>
    private static float ComputeStatusAdvantage(Side ourSide, Side oppSide)
    {
        float ourPenalty = 0f;
        float oppPenalty = 0f;

        foreach (Pokemon? poke in ourSide.Active)
        {
            if (poke == null || poke.Fainted) continue;
            ourPenalty += StatusPenalty(poke);
        }

        foreach (Pokemon? poke in oppSide.Active)
        {
            if (poke == null || poke.Fainted) continue;
            oppPenalty += StatusPenalty(poke);
        }

        // Opponent having worse status = advantage for us
        return oppPenalty - ourPenalty;
    }

    /// <summary>
    /// Returns a penalty value for a Pokemon's status condition.
    /// Higher = worse for the Pokemon.
    /// </summary>
    private static float StatusPenalty(Pokemon pokemon)
    {
        return pokemon.Status switch
        {
            ConditionId.Sleep => 0.4f,      // Can't act
            ConditionId.Freeze => 0.4f,     // Can't act
            ConditionId.Burn => pokemon.HasAbility(AbilityId.Guts) ? -0.1f : 0.2f,
            ConditionId.Paralysis => 0.15f, // Speed cut + 25% full para
            ConditionId.Toxic => 0.2f,      // Escalating damage
            ConditionId.Poison => 0.1f,     // Steady residual damage
            _ => 0f,
        };
    }

    /// <summary>
    /// Computes stat boost advantage for active Pokemon.
    /// Positive boosts = advantage. Returns roughly [-1, 1].
    /// </summary>
    private static float ComputeBoostAdvantage(Side ourSide, Side oppSide)
    {
        float ourBoosts = 0f;
        float oppBoosts = 0f;

        foreach (Pokemon? poke in ourSide.Active)
        {
            if (poke == null || poke.Fainted) continue;
            ourBoosts += SumBoosts(poke);
        }

        foreach (Pokemon? poke in oppSide.Active)
        {
            if (poke == null || poke.Fainted) continue;
            oppBoosts += SumBoosts(poke);
        }

        // Normalize by max reasonable boost sum (~12 for +2 in all 6 stats)
        return (ourBoosts - oppBoosts) / 12f;
    }

    /// <summary>
    /// Sum of all boost stages for a Pokemon. Offensive boosts weighted more.
    /// </summary>
    private static float SumBoosts(Pokemon pokemon)
    {
        BoostsTable b = pokemon.Boosts;
        // Offensive boosts matter more than defensive
        return b.Atk * 1.2f + b.SpA * 1.2f + b.Spe * 1.0f
             + b.Def * 0.8f + b.SpD * 0.8f;
    }
}
