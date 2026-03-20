using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Mcts.Ensemble.MiniModels;

/// <summary>
/// Prefers actions that exploit speed advantage.
/// If we're faster, prefer offensive moves (we strike first).
/// If we're slower and threatened, prefer defensive plays.
/// Accounts for Trick Room and Tailwind.
/// </summary>
public sealed class SpeedAdvantageMiniModel : IMiniModel
{
    public string Name => "SpeedAdvantage";

    public MiniModelScore[] Evaluate(
        Battle battle, SideId sideId,
        IReadOnlyList<MctsEdge> edges,
        OpponentPrediction? opponentPrediction)
    {
        Side ourSide = sideId == SideId.P1 ? battle.P1 : battle.P2;
        Side oppSide = sideId == SideId.P1 ? battle.P2 : battle.P1;

        bool trickRoom = battle.Field.GetPseudoWeather(Sim.Conditions.ConditionId.TrickRoom) != null;

        // Count speed advantages
        int fasterCount = 0;
        int slowerCount = 0;
        int comparisons = 0;

        foreach (Pokemon? ours in ourSide.Active)
        {
            if (ours == null || ours.Fainted) continue;
            float ourSpeed = GetEffectiveSpeed(ours, ourSide, trickRoom);

            foreach (Pokemon? opp in oppSide.Active)
            {
                if (opp == null || opp.Fainted) continue;
                float oppSpeed = GetEffectiveSpeed(opp, oppSide, trickRoom);
                comparisons++;

                if (ourSpeed > oppSpeed) fasterCount++;
                else if (oppSpeed > ourSpeed) slowerCount++;
            }
        }

        if (comparisons == 0)
        {
            var neutral = new MiniModelScore[edges.Count];
            for (int i = 0; i < edges.Count; i++)
                neutral[i] = new MiniModelScore { Preference = 0.5f, Confidence = 0.05f };
            return neutral;
        }

        float speedAdv = (float)(fasterCount - slowerCount) / comparisons;
        float confidence = MathF.Abs(speedAdv) * 0.6f;

        var scores = new MiniModelScore[edges.Count];
        for (int i = 0; i < edges.Count; i++)
        {
            MctsEdge edge = edges[i];
            float pref;

            if (speedAdv > 0)
            {
                // We're faster — prefer offensive moves
                pref = ScoreOffensiveness(edge);
            }
            else
            {
                // We're slower — prefer priority moves or defensive plays
                pref = ScorePriorityOrDefense(battle, edge);
            }

            scores[i] = new MiniModelScore { Preference = pref, Confidence = confidence };
        }

        return scores;
    }

    private static float GetEffectiveSpeed(Pokemon pokemon, Side side, bool trickRoom)
    {
        float speed = pokemon.StoredStats.Spe
                      * (float)BoostsTable.CalculateRegularStatMultiplier(pokemon.Boosts.Spe);

        // Paralysis halves speed
        if (pokemon.Status == Sim.Conditions.ConditionId.Paralysis)
            speed *= 0.5f;

        // Tailwind doubles speed
        if (side.SideConditions.ContainsKey(Sim.Conditions.ConditionId.Tailwind))
            speed *= 2f;

        // Trick Room effectively inverts speed for priority purposes
        if (trickRoom)
            speed = 10000f - speed;

        return speed;
    }

    private static float ScoreOffensiveness(MctsEdge edge)
    {
        float score = 0.3f;
        if (edge.ActionA.ChoiceType == ChoiceType.Move) score += 0.3f;
        if (edge.ActionB.HasValue && edge.ActionB.Value.ChoiceType == ChoiceType.Move) score += 0.3f;
        return MathF.Min(score, 1f);
    }

    private static float ScorePriorityOrDefense(Battle battle, MctsEdge edge)
    {
        float score = 0.3f;
        score += ScoreSlotPriorityOrDefense(battle, edge.ActionA);
        if (edge.ActionB.HasValue)
            score += ScoreSlotPriorityOrDefense(battle, edge.ActionB.Value);
        return MathF.Min(score, 1f);
    }

    private static float ScoreSlotPriorityOrDefense(Battle battle, LegalAction action)
    {
        if (ProtectDetector.IsProtectMove(action)) return 0.25f;
        if (ProtectDetector.IsSwitchAction(action)) return 0.15f;

        // Priority moves are good when we're slower
        if (action.ChoiceType == ChoiceType.Move &&
            battle.Library.Moves.TryGetValue(action.MoveId, out Sim.Moves.Move? move) &&
            move.Priority > 0)
            return 0.3f;

        return 0.1f;
    }
}
