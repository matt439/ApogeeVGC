using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Mcts;

/// <summary>
/// The MCTS search engine. Clones the battle state for each iteration,
/// applies the selected action, lets the opponent auto-choose, advances
/// one turn, and evaluates the resulting state with the neural network.
/// </summary>
public sealed class MctsSearch(MctsConfig config, ModelInference model, ActionMapper actionMapper)
{
    /// <summary>
    /// Run MCTS from the given state and return the best action pair.
    /// </summary>
    public (LegalAction ActionA, LegalAction? ActionB) Search(
        Battle battle,
        SideId sideId,
        IChoiceRequest request,
        BattlePerspective perspective)
    {
        // Get legal actions
        LegalActionSet legalActions = actionMapper.GetLegalActions(request, perspective);

        // Single action, no search needed
        if (legalActions.SlotA.Count == 1 && legalActions.SlotB.Count <= 1)
        {
            return (legalActions.SlotA[0],
                legalActions.SlotB.Count > 0 ? legalActions.SlotB[0] : null);
        }

        // Evaluate the current state for policy priors
        ModelOutput output = model.Evaluate(perspective);

        // Build legal masks and compute softmax priors
        bool[] maskA = actionMapper.BuildLegalMask(legalActions.SlotA);
        float[] probsA = ModelInference.MaskedSoftmax(output.PolicyA, maskA);

        float[]? probsB = null;
        if (legalActions.SlotB.Count > 0)
        {
            bool[] maskB = actionMapper.BuildLegalMask(legalActions.SlotB);
            probsB = ModelInference.MaskedSoftmax(output.PolicyB, maskB);
        }

        // Create root node with edges for all legal action combinations
        MctsNode root = CreateRoot(legalActions, probsA, probsB);

        // Add Dirichlet noise to root priors for exploration
        AddDirichletNoise(root);

        // Run MCTS iterations with forward simulation
        SideId opponentId = sideId == SideId.P1 ? SideId.P2 : SideId.P1;
        for (var i = 0; i < config.NumIterations; i++)
        {
            RunIteration(root, battle, sideId, opponentId);
        }

        // Select the action pair with the most visits
        return SelectBestAction(root);
    }

    private static MctsNode CreateRoot(LegalActionSet legalActions, float[] probsA, float[]? probsB)
    {
        var root = new MctsNode { IsExpanded = true };

        if (legalActions.SlotB.Count == 0)
        {
            // Single-slot decision (e.g., singles or single forced switch)
            foreach (LegalAction actionA in legalActions.SlotA)
            {
                float prior = probsA[actionA.VocabIndex];

                root.Edges.Add(new MctsEdge
                {
                    ActionA = actionA,
                    ActionB = null,
                    PriorP = prior,
                });
            }
        }
        else
        {
            // Joint action space for doubles
            foreach (LegalAction actionA in legalActions.SlotA)
            {
                float priorA = probsA[actionA.VocabIndex];

                foreach (LegalAction actionB in legalActions.SlotB)
                {
                    // Prevent both slots switching to the same Pokemon
                    if (actionA.ChoiceType == ChoiceType.Switch &&
                        actionB.ChoiceType == ChoiceType.Switch &&
                        actionA.SwitchIndex == actionB.SwitchIndex)
                    {
                        continue;
                    }

                    float priorB = probsB![actionB.VocabIndex];
                    float jointPrior = priorA * priorB;

                    root.Edges.Add(new MctsEdge
                    {
                        ActionA = actionA,
                        ActionB = actionB,
                        PriorP = jointPrior,
                    });
                }
            }

            // Normalize joint priors to sum to 1
            NormalizePriors(root);
        }

        return root;
    }

    private static void NormalizePriors(MctsNode node)
    {
        var sum = 0f;
        foreach (MctsEdge t in node.Edges)
            sum += t.PriorP;

        if (sum > 0f)
        {
            foreach (MctsEdge t in node.Edges)
                t.PriorP /= sum;
        }
    }

    /// <summary>
    /// Run a single MCTS iteration: select an edge via PUCT, clone the battle,
    /// apply the action, advance one turn, and evaluate the resulting state.
    /// </summary>
    private void RunIteration(MctsNode root, Battle battle, SideId sideId, SideId opponentId)
    {
        // SELECT: pick the edge with highest PUCT score
        MctsEdge? edge = SelectEdge(root);
        if (edge == null) return;

        // SIMULATE: clone battle, apply our action, opponent auto-chooses, advance one turn
        float leafValue;
        try
        {
            leafValue = SimulateEdge(edge, battle, sideId);
        }
        catch
        {
            // If simulation fails (e.g. invalid state in clone), skip this iteration
            return;
        }

        // BACKPROPAGATE
        edge.VisitCount++;
        edge.TotalValue += leafValue;
        root.VisitCount++;
    }

    /// <summary>
    /// Clone the battle, apply the edge's action for our side, auto-choose for the opponent,
    /// advance one turn, and return the leaf evaluation.
    /// </summary>
    private float SimulateEdge(MctsEdge edge, Battle battle, SideId sideId)
    {
        Battle sim = battle.Copy();

        // Build our choice from the edge's actions
        Choice ourChoice = actionMapper.BuildChoice(edge.ActionA, edge.ActionB);

        // Submit choices: our side uses the selected action, opponent auto-chooses
        Side ourSide = sideId == SideId.P1 ? sim.P1 : sim.P2;
        Side oppSide = sideId == SideId.P1 ? sim.P2 : sim.P1;

        ourSide.Choose(ourChoice);
        oppSide.AutoChoose();

        // Advance the turn
        sim.CommitChoices();

        // Check for terminal state
        if (sim.Ended)
        {
            return GetTerminalValue(sim, sideId);
        }

        // Evaluate the resulting state with the model
        BattlePerspective perspective = sim.GetPerspectiveForSide(sideId);
        ModelOutput output = model.Evaluate(perspective);
        return output.Value;
    }

    /// <summary>
    /// Returns 1.0 for a win, 0.0 for a loss, 0.5 for a tie.
    /// </summary>
    private static float GetTerminalValue(Battle sim, SideId sideId)
    {
        if (string.IsNullOrEmpty(sim.Winner))
            return 0.5f; // Tie

        string ourName = (sideId == SideId.P1 ? sim.P1 : sim.P2).Name;
        return sim.Winner.Equals(ourName, StringComparison.OrdinalIgnoreCase) ? 1.0f : 0.0f;
    }

    /// <summary>
    /// Select the edge with the highest PUCT score.
    /// </summary>
    private MctsEdge? SelectEdge(MctsNode node)
    {
        if (node.Edges.Count == 0) return null;

        MctsEdge? best = null;
        float bestScore = float.NegativeInfinity;
        int parentVisits = node.VisitCount;

        foreach (MctsEdge t in node.Edges)
        {
            float score = PuctScore(t, parentVisits);
            if (score > bestScore)
            {
                bestScore = score;
                best = t;
            }
        }

        return best;
    }

    /// <summary>
    /// PUCT(s,a) = Q(s,a) + c_puct * P(s,a) * sqrt(N(s)) / (1 + N(s,a))
    /// </summary>
    private float PuctScore(MctsEdge edge, int parentVisits)
    {
        float exploitation = edge.Q;
        float exploration = config.CPuct * edge.PriorP *
            MathF.Sqrt(parentVisits) / (1 + edge.VisitCount);
        return exploitation + exploration;
    }

    /// <summary>
    /// Select the action pair with the most visits (robust action selection).
    /// </summary>
    private static (LegalAction, LegalAction?) SelectBestAction(MctsNode root)
    {
        MctsEdge? best = null;
        int bestVisits = -1;

        foreach (MctsEdge t in root.Edges)
        {
            if (t.VisitCount > bestVisits)
            {
                bestVisits = t.VisitCount;
                best = t;
            }
        }

        if (best == null)
        {
            throw new InvalidOperationException("MCTS search produced no results");
        }

        return (best.ActionA, best.ActionB);
    }

    /// <summary>
    /// Add Dirichlet noise to root priors for exploration.
    /// P'(s,a) = (1 - epsilon) * P(s,a) + epsilon * noise[a]
    /// </summary>
    private void AddDirichletNoise(MctsNode root)
    {
        if (root.Edges.Count == 0) return;

        float epsilon = config.DirichletEpsilon;
        float[] noise = SampleDirichlet(root.Edges.Count, config.DirichletAlpha);

        for (var i = 0; i < root.Edges.Count; i++)
        {
            root.Edges[i].PriorP = (1 - epsilon) * root.Edges[i].PriorP + epsilon * noise[i];
        }
    }

    /// <summary>
    /// Sample from a symmetric Dirichlet distribution using Gamma samples.
    /// </summary>
    private static float[] SampleDirichlet(int n, float alpha)
    {
        Random rng = Random.Shared;
        var samples = new float[n];
        var sum = 0f;

        for (var i = 0; i < n; i++)
        {
            // Sample Gamma(alpha, 1) using Marsaglia and Tsang's method
            samples[i] = SampleGamma(rng, alpha);
            sum += samples[i];
        }

        if (sum > 0f)
        {
            for (var i = 0; i < n; i++)
                samples[i] /= sum;
        }

        return samples;
    }

    /// <summary>
    /// Sample from Gamma(alpha, 1) distribution.
    /// For alpha >= 1: Marsaglia and Tsang's method.
    /// For alpha < 1: boost method.
    /// </summary>
    private static float SampleGamma(Random rng, float alpha)
    {
        if (alpha < 1f)
        {
            // Boost: sample Gamma(alpha+1, 1) and scale by U^(1/alpha)
            float sample = SampleGamma(rng, alpha + 1f);
            var u = (float)rng.NextDouble();
            return sample * MathF.Pow(u, 1f / alpha);
        }

        // Marsaglia and Tsang's method for alpha >= 1
        float d = alpha - 1f / 3f;
        float c = 1f / MathF.Sqrt(9f * d);

        while (true)
        {
            float x, v;
            do
            {
                x = (float)SampleStandardNormal(rng);
                v = 1f + c * x;
            } while (v <= 0f);

            v = v * v * v;
            var u = (float)rng.NextDouble();

            if (u < 1f - 0.0331f * (x * x) * (x * x))
                return d * v;

            if (MathF.Log(u) < 0.5f * x * x + d * (1f - v + MathF.Log(v)))
                return d * v;
        }
    }

    private static double SampleStandardNormal(Random rng)
    {
        // Box-Muller transform
        double u1 = 1.0 - rng.NextDouble(); // avoid log(0)
        double u2 = rng.NextDouble();
        return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
    }
}