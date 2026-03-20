using ApogeeVGC.Data;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Mcts.Ensemble.MiniModels;

/// <summary>
/// Prefers Protect or switch when our active Pokemon face high incoming damage.
/// Confidence scales with how threatened we are.
/// Complementary to KOAvoidance — this activates at lower threat levels.
/// </summary>
public sealed class DamageMinMiniModel : IMiniModel
{
    private static readonly TypeChart TypeChart = new();

    public string Name => "DamageMin";

    public MiniModelScore[] Evaluate(
        Battle battle, SideId sideId,
        IReadOnlyList<MctsEdge> edges,
        OpponentPrediction? opponentPrediction)
    {
        Side ourSide = sideId == SideId.P1 ? battle.P1 : battle.P2;
        Side oppSide = sideId == SideId.P1 ? battle.P2 : battle.P1;

        // Compute threat level for each of our active Pokemon
        float maxThreat = 0f;
        for (int i = 0; i < ourSide.Active.Count; i++)
        {
            Pokemon? ours = ourSide.Active[i];
            if (ours == null || ours.Fainted) continue;

            float threat = ComputeThreat(battle, oppSide, ours);
            if (threat > maxThreat) maxThreat = threat;
        }

        // Confidence proportional to threat level (starts mattering above 0.4)
        float confidence = MathF.Max(0f, (maxThreat - 0.3f) * 1.5f);
        confidence = MathF.Min(confidence, 0.7f);

        if (confidence < 0.05f)
        {
            // Low threat — irrelevant
            var low = new MiniModelScore[edges.Count];
            for (int i = 0; i < edges.Count; i++)
                low[i] = new MiniModelScore { Preference = 0.5f, Confidence = 0.02f };
            return low;
        }

        var scores = new MiniModelScore[edges.Count];
        for (int i = 0; i < edges.Count; i++)
        {
            MctsEdge edge = edges[i];
            float pref = ScoreDefensiveness(edge);
            scores[i] = new MiniModelScore { Preference = pref, Confidence = confidence };
        }

        return scores;
    }

    private static float ScoreDefensiveness(MctsEdge edge)
    {
        float score = 0.3f; // Baseline for attacking moves
        score += ScoreAction(edge.ActionA);
        if (edge.ActionB.HasValue)
            score += ScoreAction(edge.ActionB.Value);
        return MathF.Min(score, 1f);
    }

    private static float ScoreAction(LegalAction action)
    {
        if (ProtectDetector.IsProtectMove(action)) return 0.35f;
        if (ProtectDetector.IsSwitchAction(action)) return 0.25f;
        return 0f;
    }

    private static float ComputeThreat(Battle battle, Side oppSide, Pokemon target)
    {
        float maxDmgFrac = 0f;
        foreach (Pokemon? attacker in oppSide.Active)
        {
            if (attacker == null || attacker.Fainted) continue;
            foreach (MoveSlot ms in attacker.MoveSlots)
            {
                if (!battle.Library.Moves.TryGetValue(ms.Id, out Move? move)) continue;
                if (move.Category == MoveCategory.Status || move.BasePower <= 0) continue;

                float eff = (float)TypeChart.GetMoveEffectiveness(target.Types, move.Type).ToMultiplier();
                if (eff == 0f) continue;

                float stab = 1f;
                if (move.Type is not (MoveType.Stellar or MoveType.Unknown))
                    if (attacker.HasType(move.Type.ConvertToPokemonType())) stab = 1.5f;

                float atk = move.Category == MoveCategory.Physical
                    ? attacker.StoredStats.Atk * (float)BoostsTable.CalculateRegularStatMultiplier(attacker.Boosts.Atk)
                    : attacker.StoredStats.SpA * (float)BoostsTable.CalculateRegularStatMultiplier(attacker.Boosts.SpA);
                float def = move.Category == MoveCategory.Physical
                    ? target.StoredStats.Def * (float)BoostsTable.CalculateRegularStatMultiplier(target.Boosts.Def)
                    : target.StoredStats.SpD * (float)BoostsTable.CalculateRegularStatMultiplier(target.Boosts.SpD);
                if (def <= 0f) def = 1f;

                float dmg = ((2f * 50f / 5f + 2f) * move.BasePower * atk / def / 50f + 2f) * eff * stab * 0.85f;
                float frac = dmg / MathF.Max(target.Hp, 1f);
                if (frac > maxDmgFrac) maxDmgFrac = frac;
            }
        }
        return maxDmgFrac;
    }
}
