using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Mcts;

/// <summary>
/// Simple heuristic battle state evaluation for standalone MCTS.
/// Returns a value in [0, 1] estimating win probability from the given side's perspective.
/// </summary>
public static class HeuristicEval
{
    /// <summary>
    /// Evaluate a battle state using HP fraction and alive count.
    /// </summary>
    public static float Evaluate(Battle battle, SideId sideId)
    {
        Side ourSide = sideId == SideId.P1 ? battle.P1 : battle.P2;
        Side oppSide = sideId == SideId.P1 ? battle.P2 : battle.P1;

        float ourScore = ComputeSideScore(ourSide);
        float oppScore = ComputeSideScore(oppSide);

        // Map difference to [0, 1] via sigmoid-like transform
        float diff = ourScore - oppScore;
        return 1f / (1f + MathF.Exp(-3f * diff));
    }

    /// <summary>
    /// Computes a side's score as the average of HP fraction and alive fraction.
    /// Returns a value in [0, 1].
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
            {
                hpSum += (float)pokemon.Hp / pokemon.MaxHp;
            }

            if (!pokemon.Fainted)
            {
                alive++;
            }
        }

        float hpFraction = hpSum / total;
        float aliveFraction = (float)alive / total;

        return (hpFraction + aliveFraction) / 2f;
    }
}
