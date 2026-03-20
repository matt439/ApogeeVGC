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
/// Prefers switches that put favorable type matchups on the field.
/// Evaluates how well our resulting active Pokemon match up against
/// the opponent's active Pokemon after each candidate action.
/// Moderate confidence — type advantage matters but isn't everything.
/// </summary>
public sealed class TypePositioningMiniModel : IMiniModel
{
    private static readonly TypeChart TypeChart = new();

    public string Name => "TypePositioning";

    public MiniModelScore[] Evaluate(
        Battle battle, SideId sideId,
        IReadOnlyList<MctsEdge> edges,
        OpponentPrediction? opponentPrediction,
        BattleInfoTracker? tracker = null)
    {
        Side ourSide = sideId == SideId.P1 ? battle.P1 : battle.P2;
        Side oppSide = sideId == SideId.P1 ? battle.P2 : battle.P1;

        var scores = new MiniModelScore[edges.Count];
        float maxScore = float.NegativeInfinity;
        float minScore = float.PositiveInfinity;
        float[] rawScores = new float[edges.Count];

        for (int i = 0; i < edges.Count; i++)
        {
            MctsEdge edge = edges[i];
            float matchup = EvaluateResultingMatchup(battle, ourSide, oppSide, edge);
            rawScores[i] = matchup;
            if (matchup > maxScore) maxScore = matchup;
            if (matchup < minScore) minScore = matchup;
        }

        float range = maxScore - minScore;
        for (int i = 0; i < edges.Count; i++)
        {
            float pref = range > 0.01f
                ? (rawScores[i] - minScore) / range
                : 0.5f;

            scores[i] = new MiniModelScore { Preference = pref, Confidence = 0.5f };
        }

        return scores;
    }

    private static float EvaluateResultingMatchup(
        Battle battle, Side ourSide, Side oppSide, MctsEdge edge)
    {
        // For non-switch actions, evaluate current matchup
        // For switch actions, evaluate matchup with the switched-in Pokemon

        float ourThreat = 0f;
        float oppThreat = 0f;

        // Slot A
        Pokemon? slotA = GetResultingPokemon(ourSide, edge.ActionA, 0);
        // Slot B
        Pokemon? slotB = edge.ActionB.HasValue
            ? GetResultingPokemon(ourSide, edge.ActionB.Value, 1)
            : null;

        Pokemon?[] ourActive = [slotA, slotB];

        foreach (Pokemon? ours in ourActive)
        {
            if (ours == null || ours.Fainted) continue;

            foreach (Pokemon? opp in oppSide.Active)
            {
                if (opp == null || opp.Fainted) continue;

                ourThreat += BestTypeAdvantage(battle, ours, opp);
                oppThreat += BestTypeAdvantage(battle, opp, ours);
            }
        }

        return ourThreat - oppThreat;
    }

    private static Pokemon? GetResultingPokemon(Side side, LegalAction action, int slotIndex)
    {
        if (action.ChoiceType is ChoiceType.Switch or ChoiceType.InstaSwitch)
        {
            // SwitchIndex is the team index of the Pokemon being switched in
            int idx = action.SwitchIndex;
            if (idx >= 0 && idx < side.Pokemon.Count)
                return side.Pokemon[idx];
        }

        // Non-switch: current active Pokemon stays
        if (slotIndex < side.Active.Count)
            return side.Active[slotIndex];

        return null;
    }

    private static float BestTypeAdvantage(Battle battle, Pokemon attacker, Pokemon target)
    {
        float best = 0f;
        PokemonType[] targetTypes = target.Types;

        foreach (MoveSlot moveSlot in attacker.MoveSlots)
        {
            if (!battle.Library.Moves.TryGetValue(moveSlot.Id, out Move? move))
                continue;

            if (move.Category == MoveCategory.Status || move.BasePower <= 0)
                continue;

            float effectiveness = (float)TypeChart
                .GetMoveEffectiveness(targetTypes, move.Type)
                .ToMultiplier();

            float stab = 1f;
            if (move.Type is not (MoveType.Stellar or MoveType.Unknown))
                if (attacker.HasType(move.Type.ConvertToPokemonType()))
                    stab = 1.5f;

            float score = effectiveness * stab;
            if (score > best) best = score;
        }

        return best;
    }
}
