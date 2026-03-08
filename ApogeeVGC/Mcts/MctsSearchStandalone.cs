using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Mcts;

/// <summary>
/// Standalone MCTS search that uses uniform priors and heuristic evaluation.
/// No dependency on DL models, vocab, or information tracking.
/// Iterations run in parallel — Battle.Copy() now clears cached ActiveMove
/// instances so copies don't share mutable MoveHitData dictionaries.
/// </summary>
public sealed class MctsSearchStandalone(MctsConfig config)
{
    /// <summary>
    /// Run MCTS from the given state and return the best action pair.
    /// Uses uniform priors and heuristic leaf evaluation.
    /// </summary>
    public (LegalAction ActionA, LegalAction? ActionB) Search(
        Battle battle,
        SideId sideId,
        LegalActionSet legalActions)
    {
        // Single action, no search needed
        if (legalActions.SlotA.Count == 1 && legalActions.SlotB.Count <= 1)
        {
            return (legalActions.SlotA[0],
                legalActions.SlotB.Count > 0 ? legalActions.SlotB[0] : null);
        }

        // Create root node with uniform priors
        MctsNode root = CreateRoot(legalActions);

        // Add Dirichlet noise to root priors for exploration
        AddDirichletNoise(root);

        // Run MCTS iterations in parallel
        int maxParallelism = config.MaxDegreeOfParallelism ?? Environment.ProcessorCount;
        Parallel.For(0, config.NumIterations,
            new ParallelOptions { MaxDegreeOfParallelism = maxParallelism },
            _ => RunIteration(root, battle, sideId));

        // Select the action pair with the most visits
        return SelectBestAction(root);
    }

    private static MctsNode CreateRoot(LegalActionSet legalActions)
    {
        var root = new MctsNode { IsExpanded = true };

        if (legalActions.SlotB.Count == 0)
        {
            // Single-slot decision
            float uniformPrior = 1f / legalActions.SlotA.Count;
            foreach (LegalAction actionA in legalActions.SlotA)
            {
                root.Edges.Add(new MctsEdge
                {
                    ActionA = actionA,
                    ActionB = null,
                    PriorP = uniformPrior,
                });
            }
        }
        else
        {
            // Joint action space for doubles — enumerate valid combinations
            foreach (LegalAction actionA in legalActions.SlotA)
            {
                foreach (LegalAction actionB in legalActions.SlotB)
                {
                    // Prevent both slots switching to the same Pokemon
                    if (actionA.ChoiceType == ChoiceType.Switch &&
                        actionB.ChoiceType == ChoiceType.Switch &&
                        actionA.SwitchIndex == actionB.SwitchIndex)
                    {
                        continue;
                    }

                    root.Edges.Add(new MctsEdge
                    {
                        ActionA = actionA,
                        ActionB = actionB,
                        PriorP = 1f, // Will be normalized below
                    });
                }
            }

            // Normalize to uniform
            if (root.Edges.Count > 0)
            {
                float uniform = 1f / root.Edges.Count;
                foreach (MctsEdge edge in root.Edges)
                    edge.PriorP = uniform;
            }
        }

        return root;
    }

    private void RunIteration(MctsNode root, Battle battle, SideId sideId)
    {
        MctsEdge? edge = SelectEdge(root);
        if (edge == null) return;

        float leafValue;
        try
        {
            leafValue = SimulateEdge(edge, battle, sideId);
        }
        catch
        {
            return;
        }

        // Thread-safe backpropagation
        Interlocked.Increment(ref edge._visitCount);
        InterlockedAddFloat(ref edge._totalValue, leafValue);
        Interlocked.Increment(ref root._visitCount);
    }

    private static void InterlockedAddFloat(ref float location, float value)
    {
        float initialValue, computedValue;
        do
        {
            initialValue = location;
            computedValue = initialValue + value;
        } while (Interlocked.CompareExchange(ref location, computedValue, initialValue) != initialValue);
    }

    /// <summary>
    /// Clone the battle, apply the edge's action, auto-choose for opponent,
    /// advance one turn, and evaluate with heuristic.
    /// </summary>
    private static float SimulateEdge(MctsEdge edge, Battle battle, SideId sideId)
    {
        Battle sim = battle.Copy();

        // Build our choice from the edge's actions
        Choice ourChoice = BuildChoice(edge.ActionA, edge.ActionB);

        Side ourSide = sideId == SideId.P1 ? sim.P1 : sim.P2;
        Side oppSide = sideId == SideId.P1 ? sim.P2 : sim.P1;

        ourSide.Choose(ourChoice);
        oppSide.AutoChoose();

        sim.CommitChoices();

        // Check for terminal state
        if (sim.Ended)
        {
            return GetTerminalValue(sim, sideId);
        }

        // Evaluate with heuristic
        return HeuristicEval.Evaluate(sim, sideId);
    }

    private static float GetTerminalValue(Battle sim, SideId sideId)
    {
        if (string.IsNullOrEmpty(sim.Winner))
            return 0.5f;

        string ourName = (sideId == SideId.P1 ? sim.P1 : sim.P2).Name;
        return sim.Winner.Equals(ourName, StringComparison.OrdinalIgnoreCase) ? 1.0f : 0.0f;
    }

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

    private float PuctScore(MctsEdge edge, int parentVisits)
    {
        float exploitation = edge.Q;
        float exploration = config.CPuct * edge.PriorP *
            MathF.Sqrt(parentVisits) / (1 + edge.VisitCount);
        return exploitation + exploration;
    }

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
            throw new InvalidOperationException("MCTS search produced no results");

        return (best.ActionA, best.ActionB);
    }

    /// <summary>
    /// Build a Choice from action pair without depending on ActionMapper.
    /// </summary>
    private static Choice BuildChoice(LegalAction slotA, LegalAction? slotB)
    {
        var actions = new List<ChosenAction> { BuildChosenAction(slotA) };
        if (slotB.HasValue)
            actions.Add(BuildChosenAction(slotB.Value));
        return new Choice { Actions = actions };
    }

    private static ChosenAction BuildChosenAction(LegalAction action)
    {
        return action.ChoiceType switch
        {
            ChoiceType.Move => new ChosenAction
            {
                Choice = ChoiceType.Move,
                MoveId = action.MoveId,
                TargetLoc = action.TargetLoc,
                Terastallize = action.Terastallize,
                Mega = action.Mega,
            },
            ChoiceType.Switch => new ChosenAction
            {
                Choice = ChoiceType.Switch,
                MoveId = Sim.Moves.MoveId.None,
                Index = action.SwitchIndex,
            },
            _ => new ChosenAction
            {
                Choice = ChoiceType.Pass,
                MoveId = Sim.Moves.MoveId.None,
            },
        };
    }

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

    private static float[] SampleDirichlet(int n, float alpha)
    {
        Random rng = Random.Shared;
        var samples = new float[n];
        var sum = 0f;

        for (var i = 0; i < n; i++)
        {
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

    private static float SampleGamma(Random rng, float alpha)
    {
        if (alpha < 1f)
        {
            float sample = SampleGamma(rng, alpha + 1f);
            var u = (float)rng.NextDouble();
            return sample * MathF.Pow(u, 1f / alpha);
        }

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
        double u1 = 1.0 - rng.NextDouble();
        double u2 = rng.NextDouble();
        return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
    }
}
