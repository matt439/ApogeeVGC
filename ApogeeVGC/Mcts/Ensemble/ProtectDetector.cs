namespace ApogeeVGC.Mcts.Ensemble;

/// <summary>
/// Maps opponent prediction model output to behavioral categories.
/// Requires knowledge of the action vocab to identify Protect-type moves.
/// </summary>
public static class ProtectDetector
{
    /// <summary>
    /// Protect-type move IDs that grant full protection for the turn.
    /// </summary>
    private static readonly HashSet<Sim.Moves.MoveId> ProtectMoves =
    [
        Sim.Moves.MoveId.Protect,
        Sim.Moves.MoveId.Detect,
        Sim.Moves.MoveId.BanefulBunker,
        Sim.Moves.MoveId.SpikyShield,
        Sim.Moves.MoveId.SilkTrap,
        Sim.Moves.MoveId.BurningBulwark,
    ];

    /// <summary>
    /// Check if a LegalAction is a Protect-type move.
    /// </summary>
    public static bool IsProtectMove(LegalAction action)
    {
        return action.ChoiceType == Sim.Choices.ChoiceType.Move
               && ProtectMoves.Contains(action.MoveId);
    }

    /// <summary>
    /// Check if a LegalAction is a switch.
    /// </summary>
    public static bool IsSwitchAction(LegalAction action)
    {
        return action.ChoiceType is Sim.Choices.ChoiceType.Switch
            or Sim.Choices.ChoiceType.InstaSwitch;
    }

    /// <summary>
    /// Estimate the probability that the opponent will use Protect on a given slot,
    /// based on the opponent prediction model's output and the vocab.
    /// Returns 0 if no opponent prediction is available.
    /// </summary>
    public static float EstimateProtectProbability(
        OpponentPrediction? prediction, Vocab? vocab, int slotIndex)
    {
        if (prediction == null || vocab == null) return 0f;

        float[] logits = slotIndex == 0 ? prediction.Value.PolicyA : prediction.Value.PolicyB;
        if (logits == null || logits.Length == 0) return 0f;

        // Softmax over all actions
        float max = float.NegativeInfinity;
        foreach (float l in logits)
            if (l > max) max = l;

        float sum = 0f;
        float protectSum = 0f;

        for (int i = 0; i < logits.Length; i++)
        {
            float p = MathF.Exp(logits[i] - max);
            sum += p;

            // Check if this vocab index corresponds to a Protect move
            string actionKey = vocab.GetActionKey(i);
            if (IsProtectActionName(actionKey))
                protectSum += p;
        }

        return sum > 0f ? protectSum / sum : 0f;
    }

    private static bool IsProtectActionName(string actionName)
    {
        return actionName is "move:Protect" or "move:Detect"
            or "move:Baneful Bunker" or "move:Spiky Shield"
            or "move:Silk Trap" or "move:Burning Bulwark";
    }
}
