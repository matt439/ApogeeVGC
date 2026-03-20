using ApogeeVGC.Data;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Mcts.Ensemble.MiniModels;

/// <summary>
/// Prefers actions that deal the most damage to the opponent.
/// Always has high confidence — damage output is always relevant.
/// </summary>
public sealed class DamageMaxMiniModel : IMiniModel
{
    private static readonly TypeChart TypeChart = new();

    public string Name => "DamageMax";

    public MiniModelScore[] Evaluate(
        Battle battle, SideId sideId,
        IReadOnlyList<MctsEdge> edges,
        OpponentPrediction? opponentPrediction,
        BattleInfoTracker? tracker = null)
    {
        Side ourSide = sideId == SideId.P1 ? battle.P1 : battle.P2;
        Side oppSide = sideId == SideId.P1 ? battle.P2 : battle.P1;

        var scores = new MiniModelScore[edges.Count];
        float maxDamage = 0f;

        // First pass: estimate damage for each edge
        float[] damages = new float[edges.Count];
        for (int i = 0; i < edges.Count; i++)
        {
            MctsEdge edge = edges[i];
            float dmg = EstimateEdgeDamage(battle, ourSide, oppSide, edge);
            damages[i] = dmg;
            if (dmg > maxDamage) maxDamage = dmg;
        }

        // Normalize to [0, 1]
        for (int i = 0; i < edges.Count; i++)
        {
            float pref = maxDamage > 0f ? damages[i] / maxDamage : 0f;
            scores[i] = new MiniModelScore { Preference = pref, Confidence = 0.9f };
        }

        return scores;
    }

    private static float EstimateEdgeDamage(
        Battle battle, Side ourSide, Side oppSide, MctsEdge edge)
    {
        float totalDamage = 0f;

        totalDamage += EstimateActionDamage(battle, ourSide, oppSide, edge.ActionA, 0);
        if (edge.ActionB.HasValue)
            totalDamage += EstimateActionDamage(battle, ourSide, oppSide, edge.ActionB.Value, 1);

        return totalDamage;
    }

    private static float EstimateActionDamage(
        Battle battle, Side ourSide, Side oppSide,
        LegalAction action, int slotIndex)
    {
        if (action.ChoiceType != ChoiceType.Move)
            return 0f;

        Pokemon? attacker = slotIndex < ourSide.Active.Count ? ourSide.Active[slotIndex] : null;
        if (attacker == null || attacker.Fainted) return 0f;

        if (!battle.Library.Moves.TryGetValue(action.MoveId, out Move? move))
            return 0f;

        if (move.Category == MoveCategory.Status || move.BasePower <= 0)
            return 0f;

        // Sum damage across targets
        float damage = 0f;
        bool isSpread = move.Target is MoveTarget.AllAdjacentFoes or MoveTarget.AllAdjacent;
        float spreadPenalty = isSpread && CountAliveOpponents(oppSide) > 1 ? 0.75f : 1f;

        foreach (Pokemon? target in oppSide.Active)
        {
            if (target == null || target.Fainted) continue;

            float effectiveness = (float)TypeChart
                .GetMoveEffectiveness(target.Types, move.Type)
                .ToMultiplier();
            if (effectiveness == 0f) continue;

            float stab = 1f;
            if (move.Type is not (MoveType.Stellar or MoveType.Unknown))
            {
                if (attacker.HasType(move.Type.ConvertToPokemonType()))
                    stab = 1.5f;
            }

            float score = move.BasePower * effectiveness * stab * spreadPenalty / 100f;

            // For single-target moves, only count the primary target
            if (!isSpread)
            {
                damage = MathF.Max(damage, score);
                break;
            }
            else
            {
                damage += score;
            }
        }

        return damage;
    }

    private static int CountAliveOpponents(Side side)
    {
        int count = 0;
        foreach (Pokemon? p in side.Active)
            if (p != null && !p.Fainted) count++;
        return count;
    }
}
