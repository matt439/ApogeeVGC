using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Mcts.Ensemble.MiniModels;

/// <summary>
/// Deprioritizes single-target attacks aimed at an opponent slot that is
/// predicted to Protect. Uses the opponent prediction model output.
/// Prefers spread moves or targeting the other slot when Protect is likely.
/// Without opponent prediction model, falls back to heuristic (opponent
/// used Protect last turn → likely to fail this turn).
/// </summary>
public sealed class ProtectPredictionMiniModel : IMiniModel
{
    public string Name => "ProtectPrediction";

    public MiniModelScore[] Evaluate(
        Battle battle, SideId sideId,
        IReadOnlyList<MctsEdge> edges,
        OpponentPrediction? opponentPrediction)
    {
        Side oppSide = sideId == SideId.P1 ? battle.P2 : battle.P1;

        // Estimate Protect probability per opponent slot
        float[] protectProb = new float[2];

        if (opponentPrediction.HasValue && MctsResources.IsInitialized)
        {
            // Use DL opponent prediction
            Vocab? vocab = null;
            try { vocab = MctsResources.Vocab; } catch { }

            protectProb[0] = ProtectDetector.EstimateProtectProbability(
                opponentPrediction, vocab, 0);
            protectProb[1] = ProtectDetector.EstimateProtectProbability(
                opponentPrediction, vocab, 1);
        }
        else
        {
            // Heuristic fallback: check if opponent just used Protect
            // (consecutive Protect has 1/3 chance of working)
            for (int i = 0; i < oppSide.Active.Count && i < 2; i++)
            {
                Pokemon? opp = oppSide.Active[i];
                if (opp == null || opp.Fainted) continue;

                // If opponent has Protect volatile (used it recently), less likely to work again
                if (opp.Volatiles.ContainsKey(Sim.Conditions.ConditionId.Stall))
                    protectProb[i] = 0.1f; // Low prob — consecutive Protect usually fails
                else
                    protectProb[i] = 0.15f; // Default mild Protect assumption
            }
        }

        float maxProtect = MathF.Max(protectProb[0], protectProb[1]);
        if (maxProtect < 0.1f)
        {
            // No significant Protect likelihood — irrelevant
            var low = new MiniModelScore[edges.Count];
            for (int i = 0; i < edges.Count; i++)
                low[i] = new MiniModelScore { Preference = 0.5f, Confidence = 0.02f };
            return low;
        }

        float confidence = MathF.Min(maxProtect, 0.8f);

        var scores = new MiniModelScore[edges.Count];
        for (int i = 0; i < edges.Count; i++)
        {
            MctsEdge edge = edges[i];
            float pref = 0.5f; // Neutral baseline

            // Penalize single-target moves aimed at Protecting slots
            pref -= PenalizeIntoProtect(battle, edge.ActionA, protectProb);
            if (edge.ActionB.HasValue)
                pref -= PenalizeIntoProtect(battle, edge.ActionB.Value, protectProb);

            // Reward spread moves (hit both, Protect only blocks one)
            pref += RewardSpreadMoves(battle, edge.ActionA);
            if (edge.ActionB.HasValue)
                pref += RewardSpreadMoves(battle, edge.ActionB.Value);

            scores[i] = new MiniModelScore
            {
                Preference = MathF.Max(0f, MathF.Min(pref, 1f)),
                Confidence = confidence,
            };
        }

        return scores;
    }

    private static float PenalizeIntoProtect(Battle battle, LegalAction action, float[] protectProb)
    {
        if (action.ChoiceType != ChoiceType.Move) return 0f;

        if (!battle.Library.Moves.TryGetValue(action.MoveId, out Move? move))
            return 0f;

        // Spread moves hit both slots — partial Protect doesn't fully negate them
        if (move.Target is MoveTarget.AllAdjacentFoes or MoveTarget.AllAdjacent)
            return 0f;

        // Single-target move — check which slot it's aimed at
        // TargetLoc 1 = opponent slot A, 2 = opponent slot B
        int targetSlot = action.TargetLoc > 0 ? action.TargetLoc - 1 : 0;
        if (targetSlot < 2)
            return protectProb[targetSlot] * 0.3f;

        return 0f;
    }

    private static float RewardSpreadMoves(Battle battle, LegalAction action)
    {
        if (action.ChoiceType != ChoiceType.Move) return 0f;

        if (!battle.Library.Moves.TryGetValue(action.MoveId, out Move? move))
            return 0f;

        if (move.Target is MoveTarget.AllAdjacentFoes or MoveTarget.AllAdjacent)
            return 0.05f;

        return 0f;
    }
}
