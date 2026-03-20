namespace ApogeeVGC.Mcts.Ensemble;

/// <summary>
/// Score for a single candidate action from one mini-model.
/// </summary>
public readonly struct MiniModelScore
{
    /// <summary>How strongly this model prefers this action (0 = avoid, 1 = strongly prefer).</summary>
    public float Preference { get; init; }

    /// <summary>How relevant this model's opinion is right now (0 = irrelevant, 1 = critical).</summary>
    public float Confidence { get; init; }
}

/// <summary>
/// A focused heuristic evaluator that scores candidate joint actions
/// from one strategic perspective (e.g., damage maximization, KO avoidance).
/// </summary>
public interface IMiniModel
{
    /// <summary>Human-readable name for logging and debugging.</summary>
    string Name { get; }

    /// <summary>
    /// Score each candidate edge from the given battle state.
    /// Returns one MiniModelScore per edge.
    /// </summary>
    /// <param name="battle">The battle state to evaluate.</param>
    /// <param name="sideId">Which side we are playing as.</param>
    /// <param name="edges">Candidate joint actions to score.</param>
    /// <param name="opponentPrediction">Optional opponent action predictions from DL model.</param>
    MiniModelScore[] Evaluate(
        Sim.BattleClasses.Battle battle,
        Sim.Core.SideId sideId,
        IReadOnlyList<MctsEdge> edges,
        OpponentPrediction? opponentPrediction);
}
