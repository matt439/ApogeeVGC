using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Mcts.Ensemble.MiniModels;

/// <summary>
/// Penalizes plays that concede tempo without strategic compensation.
/// Tempo-negative patterns: double switch, double Protect, double status,
/// or any turn where neither slot applies offensive pressure.
/// </summary>
public sealed class TempoPreservationMiniModel : IMiniModel
{
    public string Name => "TempoPreservation";

    public MiniModelScore[] Evaluate(
        Battle battle, SideId sideId,
        IReadOnlyList<MctsEdge> edges,
        OpponentPrediction? opponentPrediction,
        BattleInfoTracker? tracker = null)
    {
        var scores = new MiniModelScore[edges.Count];

        for (int i = 0; i < edges.Count; i++)
        {
            MctsEdge edge = edges[i];
            if (!edge.ActionB.HasValue)
            {
                // Single-slot — can't assess tempo loss from joint action
                scores[i] = new MiniModelScore { Preference = 0.5f, Confidence = 0.1f };
                continue;
            }

            float penalty = ComputeTempoPenalty(battle, edge);
            float pref = MathF.Max(0f, 1f - penalty);
            float confidence = penalty > 0.1f ? 0.6f : 0.1f;

            scores[i] = new MiniModelScore { Preference = pref, Confidence = confidence };
        }

        return scores;
    }

    private float ComputeTempoPenalty(Battle battle, MctsEdge edge)
    {
        LegalAction a = edge.ActionA;
        LegalAction b = edge.ActionB!.Value;
        float penalty = 0f;

        bool aIsSwitch = ProtectDetector.IsSwitchAction(a);
        bool bIsSwitch = ProtectDetector.IsSwitchAction(b);
        bool aIsProtect = ProtectDetector.IsProtectMove(a);
        bool bIsProtect = ProtectDetector.IsProtectMove(b);
        bool aIsStatus = IsStatusMove(battle, a);
        bool bIsStatus = IsStatusMove(battle, b);
        bool aIsAttack = IsAttackMove(battle, a);
        bool bIsAttack = IsAttackMove(battle, b);

        // Double switch — opponent gets two free attacks
        if (aIsSwitch && bIsSwitch)
            penalty += 0.7f;

        // Double Protect — wastes a full turn
        if (aIsProtect && bIsProtect)
            penalty += 0.6f;

        // Double status — no offensive pressure
        if (aIsStatus && bIsStatus)
            penalty += 0.4f;

        // Switch + Protect — only one slot does something, and it's defensive
        if ((aIsSwitch && bIsProtect) || (aIsProtect && bIsSwitch))
            penalty += 0.3f;

        // No offensive action at all (status + switch, status + protect, etc.)
        if (!aIsAttack && !bIsAttack)
            penalty += 0.2f;

        return MathF.Min(penalty, 1f);
    }

    private static bool IsStatusMove(Battle battle, LegalAction action)
    {
        if (action.ChoiceType != ChoiceType.Move) return false;
        if (ProtectDetector.IsProtectMove(action)) return false;

        if (!battle.Library.Moves.TryGetValue(action.MoveId, out Move? move))
            return false;

        return move.Category == MoveCategory.Status;
    }

    private static bool IsAttackMove(Battle battle, LegalAction action)
    {
        if (action.ChoiceType != ChoiceType.Move) return false;
        if (ProtectDetector.IsProtectMove(action)) return false;

        if (!battle.Library.Moves.TryGetValue(action.MoveId, out Move? move))
            return false;

        return move.Category != MoveCategory.Status && move.BasePower > 0;
    }
}
