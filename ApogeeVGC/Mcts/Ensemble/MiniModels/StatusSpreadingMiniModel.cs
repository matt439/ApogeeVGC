using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Mcts.Ensemble.MiniModels;

/// <summary>
/// Prefers status moves targeting unstatused opponent Pokemon.
/// Low-moderate confidence — status is valuable but not urgent.
/// </summary>
public sealed class StatusSpreadingMiniModel : IMiniModel
{
    private static readonly HashSet<MoveId> StatusMoves =
    [
        MoveId.ThunderWave,
        MoveId.WillOWisp,
        MoveId.Spore,
        MoveId.SleepPowder,
        MoveId.Toxic,
        MoveId.PoisonPowder,
        MoveId.Glare,
        MoveId.Nuzzle,
        MoveId.Yawn,
        MoveId.DarkVoid,
        MoveId.Hypnosis,
        MoveId.StunSpore,
    ];

    public string Name => "StatusSpreading";

    public MiniModelScore[] Evaluate(
        Battle battle, SideId sideId,
        IReadOnlyList<MctsEdge> edges,
        OpponentPrediction? opponentPrediction)
    {
        Side oppSide = sideId == SideId.P1 ? battle.P2 : battle.P1;

        // Check if any opponent active Pokemon are unstatused
        bool anyUnstatused = false;
        foreach (Pokemon? opp in oppSide.Active)
        {
            if (opp != null && !opp.Fainted && opp.Status == ConditionId.None)
            {
                anyUnstatused = true;
                break;
            }
        }

        if (!anyUnstatused)
        {
            // All opponents already statused — irrelevant
            var low = new MiniModelScore[edges.Count];
            for (int i = 0; i < edges.Count; i++)
                low[i] = new MiniModelScore { Preference = 0.5f, Confidence = 0.02f };
            return low;
        }

        var scores = new MiniModelScore[edges.Count];
        for (int i = 0; i < edges.Count; i++)
        {
            MctsEdge edge = edges[i];
            float pref = 0.3f;

            if (IsStatusAction(edge.ActionA)) pref += 0.35f;
            if (edge.ActionB.HasValue && IsStatusAction(edge.ActionB.Value)) pref += 0.35f;

            scores[i] = new MiniModelScore
            {
                Preference = MathF.Min(pref, 1f),
                Confidence = 0.4f,
            };
        }

        return scores;
    }

    private static bool IsStatusAction(LegalAction action)
    {
        return action.ChoiceType == ChoiceType.Move && StatusMoves.Contains(action.MoveId);
    }
}
