using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Mcts;

/// <summary>
/// Full-depth MCTS search with DL model policy priors and value evaluation.
/// Uses the same tree structure as MctsSearchStandalone but replaces:
/// - Uniform priors → Neural network policy head (masked softmax)
/// - Heuristic evaluation → Neural network value head
/// </summary>
public sealed class MctsSearchDL(MctsConfig config, ModelInference model, ActionMapper actionMapper)
{
    private readonly Random _targetRng = new();

    /// <summary>
    /// Run MCTS from the given state and return the best action pair.
    /// Uses DL policy priors at root and value evaluation at leaf nodes.
    /// </summary>
    public (LegalAction ActionA, LegalAction? ActionB) Search(
        Battle battle,
        SideId sideId,
        IChoiceRequest request,
        BattlePerspective perspective)
    {
        // Get legal actions with vocab indices for model mapping
        LegalActionSet legalActions = actionMapper.GetLegalActions(request, perspective);

        // Single action, no search needed
        if (legalActions.SlotA.Count == 1 && legalActions.SlotB.Count <= 1)
        {
            return (legalActions.SlotA[0],
                legalActions.SlotB.Count > 0 ? legalActions.SlotB[0] : null);
        }

        // Evaluate current state for policy priors
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

        // Create root node with DL policy priors
        MctsNode root = CreateRoot(legalActions, probsA, probsB);

        // Add Dirichlet noise to root priors for exploration
        AddDirichletNoise(root);

        // Run MCTS iterations — each copies the battle and traverses/expands the tree
        for (int i = 0; i < config.NumIterations; i++)
        {
            Battle sim = battle.Copy();
            RunIteration(root, sim, sideId);
        }

        // Select the action pair with the most visits
        return SelectBestAction(root);
    }

    /// <summary>
    /// One MCTS iteration: select down the tree, expand a leaf, evaluate, backpropagate.
    /// The simulation (battle copy) is advanced as we descend, so only one Copy() per iteration.
    /// </summary>
    private float RunIteration(MctsNode node, Battle sim, SideId sideId)
    {
        // Terminal node — return cached value
        if (node.IsTerminal)
        {
            node.VisitCount++;
            return node.TerminalValue;
        }

        // Select best edge using PUCT
        MctsEdge? edge = SelectEdge(node);
        if (edge == null) return 0.5f;

        // Apply our action + random opponent, advance the simulation one turn
        Side ourSide = sideId == SideId.P1 ? sim.P1 : sim.P2;
        Side oppSide = sideId == SideId.P1 ? sim.P2 : sim.P1;

        Choice ourChoice = BuildChoice(edge.ActionA, edge.ActionB);

        float leafValue;

        try
        {
            ourSide.Choose(ourChoice);
            oppSide.AutoChoose();
            sim.CommitChoices();
        }
        catch
        {
            // Simulation error — treat as neutral
            leafValue = 0.5f;
            edge.VisitCount++;
            edge.TotalValue += leafValue;
            node.VisitCount++;
            return leafValue;
        }

        if (sim.Ended)
        {
            // Game ended — terminal value
            leafValue = GetTerminalValue(sim, sideId);

            // Mark child as terminal so future visits short-circuit
            edge.Child ??= new MctsNode();
            edge.Child.IsTerminal = true;
            edge.Child.TerminalValue = leafValue;
        }
        else if (edge.Child is not { IsExpanded: true })
        {
            // Leaf node — expand with DL priors and evaluate with value network
            edge.Child ??= new MctsNode();
            leafValue = ExpandAndEvaluate(edge.Child, sim, sideId);
        }
        else
        {
            // Internal node — recurse deeper
            leafValue = RunIteration(edge.Child, sim, sideId);
        }

        // Backpropagate
        edge.VisitCount++;
        edge.TotalValue += leafValue;
        node.VisitCount++;

        return leafValue;
    }

    // ── Node creation and expansion ────────────────────────────────────

    private static MctsNode CreateRoot(LegalActionSet legalActions, float[] probsA, float[]? probsB)
    {
        var root = new MctsNode { IsExpanded = true };

        if (legalActions.SlotB.Count == 0)
        {
            // Single-slot decision
            foreach (LegalAction actionA in legalActions.SlotA)
            {
                root.Edges.Add(new MctsEdge
                {
                    ActionA = actionA,
                    ActionB = null,
                    PriorP = probsA[actionA.VocabIndex],
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
                    root.Edges.Add(new MctsEdge
                    {
                        ActionA = actionA,
                        ActionB = actionB,
                        PriorP = priorA * priorB,
                    });
                }
            }

            NormalizePriors(root);
        }

        return root;
    }

    /// <summary>
    /// Expand a child node using DL model policy priors and return the value head evaluation.
    /// Runs a single model inference to get both policy and value for the new state.
    /// </summary>
    private float ExpandAndEvaluate(MctsNode node, Battle sim, SideId sideId)
    {
        // Get the perspective for model evaluation
        BattlePerspective perspective = sim.GetPerspectiveForSide(sideId);
        ModelOutput output = model.Evaluate(perspective);

        // Get legal actions from the battle state
        Side ourSide = sideId == SideId.P1 ? sim.P1 : sim.P2;
        IChoiceRequest? request = ourSide.ActiveRequest;

        if (request is MoveRequest or SwitchRequest)
        {
            LegalActionSet legalActions = actionMapper.GetLegalActions(request, perspective);

            if (legalActions.SlotA.Count > 0)
            {
                // Build DL policy priors for child edges
                bool[] maskA = actionMapper.BuildLegalMask(legalActions.SlotA);
                float[] probsA = ModelInference.MaskedSoftmax(output.PolicyA, maskA);

                float[]? probsB = null;
                if (legalActions.SlotB.Count > 0)
                {
                    bool[] maskB = actionMapper.BuildLegalMask(legalActions.SlotB);
                    probsB = ModelInference.MaskedSoftmax(output.PolicyB, maskB);
                }

                PopulateEdgesWithPriors(node, legalActions, probsA, probsB);
            }
            else
            {
                node.IsExpanded = true;
            }
        }
        else
        {
            node.IsExpanded = true;
        }

        // Return value network evaluation
        return output.Value;
    }

    private static void PopulateEdgesWithPriors(MctsNode node, LegalActionSet legalActions,
        float[] probsA, float[]? probsB)
    {
        if (legalActions.SlotB.Count == 0)
        {
            foreach (LegalAction actionA in legalActions.SlotA)
            {
                node.Edges.Add(new MctsEdge
                {
                    ActionA = actionA,
                    ActionB = null,
                    PriorP = probsA[actionA.VocabIndex],
                });
            }
        }
        else
        {
            foreach (LegalAction actionA in legalActions.SlotA)
            {
                float priorA = probsA[actionA.VocabIndex];

                foreach (LegalAction actionB in legalActions.SlotB)
                {
                    if (actionA.ChoiceType == ChoiceType.Switch &&
                        actionB.ChoiceType == ChoiceType.Switch &&
                        actionA.SwitchIndex == actionB.SwitchIndex)
                    {
                        continue;
                    }

                    float priorB = probsB![actionB.VocabIndex];
                    node.Edges.Add(new MctsEdge
                    {
                        ActionA = actionA,
                        ActionB = actionB,
                        PriorP = priorA * priorB,
                    });
                }
            }

            NormalizePriors(node);
        }

        node.IsExpanded = true;
    }

    private static void NormalizePriors(MctsNode node)
    {
        float sum = 0f;
        foreach (MctsEdge edge in node.Edges)
            sum += edge.PriorP;

        if (sum > 0f)
        {
            foreach (MctsEdge edge in node.Edges)
                edge.PriorP /= sum;
        }
    }

    // ── PUCT selection ─────────────────────────────────────────────────

    private MctsEdge? SelectEdge(MctsNode node)
    {
        if (node.Edges.Count == 0) return null;

        MctsEdge? best = null;
        float bestScore = float.NegativeInfinity;
        int parentVisits = node.VisitCount;

        foreach (MctsEdge edge in node.Edges)
        {
            float score = PuctScore(edge, parentVisits);
            if (score > bestScore)
            {
                bestScore = score;
                best = edge;
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

    // ── Result selection ───────────────────────────────────────────────

    private static (LegalAction, LegalAction?) SelectBestAction(MctsNode root)
    {
        MctsEdge? best = null;
        int bestVisits = -1;

        foreach (MctsEdge edge in root.Edges)
        {
            if (edge.VisitCount > bestVisits)
            {
                bestVisits = edge.VisitCount;
                best = edge;
            }
        }

        if (best == null)
            throw new InvalidOperationException("MCTS search produced no results");

        return (best.ActionA, best.ActionB);
    }

    private static float GetTerminalValue(Battle sim, SideId sideId)
    {
        if (string.IsNullOrEmpty(sim.Winner))
            return 0.5f;

        string ourName = (sideId == SideId.P1 ? sim.P1 : sim.P2).Name;
        return sim.Winner.Equals(ourName, StringComparison.OrdinalIgnoreCase) ? 1.0f : 0.0f;
    }

    // ── Choice building ────────────────────────────────────────────────

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
                MoveId = MoveId.None,
                Index = action.SwitchIndex,
            },
            _ => new ChosenAction
            {
                Choice = ChoiceType.Pass,
                MoveId = MoveId.None,
            },
        };
    }

    // ── Dirichlet noise ────────────────────────────────────────────────

    private void AddDirichletNoise(MctsNode root)
    {
        if (root.Edges.Count == 0) return;

        float epsilon = config.DirichletEpsilon;
        float[] noise = SampleDirichlet(root.Edges.Count, config.DirichletAlpha);

        for (int i = 0; i < root.Edges.Count; i++)
        {
            root.Edges[i].PriorP = (1 - epsilon) * root.Edges[i].PriorP + epsilon * noise[i];
        }
    }

    private static float[] SampleDirichlet(int n, float alpha)
    {
        Random rng = Random.Shared;
        var samples = new float[n];
        float sum = 0f;

        for (int i = 0; i < n; i++)
        {
            samples[i] = SampleGamma(rng, alpha);
            sum += samples[i];
        }

        if (sum > 0f)
        {
            for (int i = 0; i < n; i++)
                samples[i] /= sum;
        }

        return samples;
    }

    private static float SampleGamma(Random rng, float alpha)
    {
        if (alpha < 1f)
        {
            float sample = SampleGamma(rng, alpha + 1f);
            float u = (float)rng.NextDouble();
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
            float u2 = (float)rng.NextDouble();

            if (u2 < 1f - 0.0331f * (x * x) * (x * x))
                return d * v;

            if (MathF.Log(u2) < 0.5f * x * x + d * (1f - v + MathF.Log(v)))
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
