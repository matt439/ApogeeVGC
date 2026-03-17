using ApogeeVGC.Mcts;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;

namespace ApogeeVGC.LiveAssist;

/// <summary>
/// Greedy DL player for Showdown: picks the highest-probability action
/// from the policy network each turn. No search — pure model inference.
/// Mirrors the logic in PlayerDLGreedy.GetGreedyDLChoice().
/// </summary>
public sealed class ShowdownPlayerDLGreedy(ModelInference model) : IShowdownPlayer
{
    public string Name => "DL-Greedy";

    public (LegalAction BestA, LegalAction? BestB) ChooseBattle(
        BattlePerspective perspective,
        LegalActionSet actions,
        bool[] maskA,
        bool[] maskB)
    {
        // Single action per slot — no inference needed
        if (actions.SlotA.Count == 1 && actions.SlotB.Count <= 1)
        {
            return (actions.SlotA[0],
                actions.SlotB.Count > 0 ? actions.SlotB[0] : null);
        }

        ModelOutput output = model.Evaluate(perspective);

        // Log value for diagnostics
        float pct = output.Value * 100;
        string indicator = pct >= 55 ? "+" : pct >= 45 ? "~" : "-";
        Console.WriteLine($"  Win estimate: {pct:F1}% [{indicator}]");

        // Slot A: masked softmax → argmax
        float[] probsA = ModelInference.MaskedSoftmax(output.PolicyA, maskA);
        LegalAction bestA = PickBest(actions.SlotA, probsA);

        // Slot B (if doubles)
        LegalAction? bestB = null;
        if (actions.SlotB.Count > 0)
        {
            float[] probsB = ModelInference.MaskedSoftmax(output.PolicyB, maskB);
            LegalAction candidateB = PickBest(actions.SlotB, probsB);

            // Prevent both slots switching to the same Pokemon
            if (bestA.ChoiceType == ChoiceType.Switch &&
                candidateB.ChoiceType == ChoiceType.Switch &&
                bestA.SwitchIndex == candidateB.SwitchIndex)
            {
                candidateB = PickBestExcluding(actions.SlotB, probsB, candidateB.SwitchIndex);
            }

            bestB = candidateB;
        }

        return (bestA, bestB);
    }

    private static LegalAction PickBest(IReadOnlyList<LegalAction> actions, float[] probs)
    {
        LegalAction best = actions[0];
        float bestProb = probs[actions[0].VocabIndex];

        for (int i = 1; i < actions.Count; i++)
        {
            float p = probs[actions[i].VocabIndex];
            if (p > bestProb)
            {
                bestProb = p;
                best = actions[i];
            }
        }
        return best;
    }

    private static LegalAction PickBestExcluding(
        IReadOnlyList<LegalAction> actions, float[] probs, int excludeSwitchIndex)
    {
        LegalAction best = default;
        float bestProb = float.NegativeInfinity;

        foreach (LegalAction action in actions)
        {
            if (action.ChoiceType == ChoiceType.Switch && action.SwitchIndex == excludeSwitchIndex)
                continue;

            float p = probs[action.VocabIndex];
            if (p > bestProb)
            {
                bestProb = p;
                best = action;
            }
        }
        return best;
    }
}
