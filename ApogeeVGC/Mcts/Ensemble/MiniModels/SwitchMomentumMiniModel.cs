using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Mcts.Ensemble.MiniModels;

/// <summary>
/// Penalizes switching when the active Pokemon just switched in recently.
/// Prevents the switching loop where the bot endlessly swaps back and forth.
/// Also penalizes switching when the current Pokemon has boosted stats.
/// </summary>
public sealed class SwitchMomentumMiniModel : IMiniModel
{
    public string Name => "SwitchMomentum";

    public MiniModelScore[] Evaluate(
        Battle battle, SideId sideId,
        IReadOnlyList<MctsEdge> edges,
        OpponentPrediction? opponentPrediction,
        BattleInfoTracker? tracker = null)
    {
        Side ourSide = sideId == SideId.P1 ? battle.P1 : battle.P2;

        // Check if any of our active Pokemon just switched in
        // (activeTurns == 0 means they entered this turn or very recently)
        bool[] justSwitchedIn = new bool[ourSide.Active.Count];
        bool[] hasBoosted = new bool[ourSide.Active.Count];
        bool anyRelevant = false;

        for (int i = 0; i < ourSide.Active.Count; i++)
        {
            Pokemon? poke = ourSide.Active[i];
            if (poke == null || poke.Fainted) continue;

            // ActiveTurns tracks how many turns this Pokemon has been active
            if (poke.ActiveTurns <= 1)
            {
                justSwitchedIn[i] = true;
                anyRelevant = true;
            }

            // Check if Pokemon has positive boosts worth keeping
            int totalBoosts = poke.Boosts.Atk + poke.Boosts.Def + poke.Boosts.SpA
                              + poke.Boosts.SpD + poke.Boosts.Spe;
            if (totalBoosts > 0)
            {
                hasBoosted[i] = true;
                anyRelevant = true;
            }
        }

        if (!anyRelevant)
        {
            // No one just switched in and no boosts — low relevance
            var low = new MiniModelScore[edges.Count];
            for (int i = 0; i < edges.Count; i++)
                low[i] = new MiniModelScore { Preference = 0.5f, Confidence = 0.1f };
            return low;
        }

        var scores = new MiniModelScore[edges.Count];
        for (int i = 0; i < edges.Count; i++)
        {
            MctsEdge edge = edges[i];
            float penalty = 0f;

            // Penalize switching slot A if it just came in or has boosts
            if (edge.ActionA.ChoiceType is ChoiceType.Switch or ChoiceType.InstaSwitch)
            {
                if (justSwitchedIn.Length > 0 && justSwitchedIn[0]) penalty += 0.3f;
                if (hasBoosted.Length > 0 && hasBoosted[0]) penalty += 0.2f;
            }

            // Penalize switching slot B if it just came in or has boosts
            if (edge.ActionB.HasValue &&
                edge.ActionB.Value.ChoiceType is ChoiceType.Switch or ChoiceType.InstaSwitch)
            {
                if (justSwitchedIn.Length > 1 && justSwitchedIn[1]) penalty += 0.3f;
                if (hasBoosted.Length > 1 && hasBoosted[1]) penalty += 0.2f;
            }

            scores[i] = new MiniModelScore
            {
                Preference = MathF.Max(0f, 0.7f - penalty),
                Confidence = anyRelevant ? 0.7f : 0.1f,
            };
        }

        return scores;
    }
}
