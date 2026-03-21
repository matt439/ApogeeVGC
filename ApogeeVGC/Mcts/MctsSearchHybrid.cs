using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Mcts;

/// <summary>
/// Hybrid MCTS search: uniform priors (like standalone) + DL value head for leaf evaluation.
/// Isolates the value head's contribution from the policy head's impact on search.
/// Iterations run in parallel with locked node expansion and atomic backpropagation.
/// </summary>
public sealed class MctsSearchHybrid(MctsConfig config, ModelInference model)
{
    private readonly ThreadLocal<Random> _targetRng = new(() => new Random());

    /// <summary>
    /// Run MCTS from the given state and return the best action pair.
    /// Uses uniform priors at all nodes but DL value head for leaf evaluation.
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
            _ =>
            {
                Battle sim = battle.Copy();
                RunIteration(root, sim, sideId);
            });

        // Select the action pair with the most visits
        return SelectBestAction(root);
    }

    /// <summary>
    /// One MCTS iteration: select down the tree, expand a leaf, evaluate with DL value head, backpropagate.
    /// </summary>
    private float RunIteration(MctsNode node, Battle sim, SideId sideId)
    {
        // Terminal node — return cached value
        if (node.IsTerminal)
        {
            Interlocked.Increment(ref node._visitCount);
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
            oppSide.RandomAutoChoose(_targetRng.Value!);
            sim.CommitChoices();
        }
        catch
        {
            // Simulation error — treat as neutral
            leafValue = 0.5f;
            Interlocked.Increment(ref edge._visitCount);
            InterlockedAddFloat(ref edge._totalValue, leafValue);
            Interlocked.Increment(ref node._visitCount);
            return leafValue;
        }

        if (sim.Ended)
        {
            // Game ended — terminal value
            leafValue = GetTerminalValue(sim, sideId);

            lock (node.ExpandLock)
            {
                edge.Child ??= new MctsNode();
                edge.Child.IsTerminal = true;
                edge.Child.TerminalValue = leafValue;
            }
        }
        else if (edge.Child is not { IsExpanded: true })
        {
            // Leaf node — expand with uniform priors, evaluate with DL value head
            lock (node.ExpandLock)
            {
                if (edge.Child is not { IsExpanded: true })
                {
                    edge.Child ??= new MctsNode();
                    ExpandNodeFromBattle(edge.Child, sim, sideId);
                }
            }

            // Evaluate with confidence-weighted blend of DL value head and heuristic.
            // Confidence = how far the DL prediction is from 0.5 (uncertain).
            // When DL is uncertain (near 0.5), trust the heuristic more.
            BattlePerspective perspective = sim.GetPerspectiveForSide(sideId);
            ModelOutput output = model.Evaluate(perspective);
            float dlValue = output.Value;
            float heuristicValue = HeuristicEval.Evaluate(sim, sideId);
            float confidence = MathF.Abs(dlValue - 0.5f) * 2f; // 0 = uncertain, 1 = confident
            leafValue = confidence * dlValue + (1f - confidence) * heuristicValue;
            MctsLogger.LogValue(leafValue);
        }
        else
        {
            // Internal node — recurse deeper
            leafValue = RunIteration(edge.Child, sim, sideId);
        }

        // Backpropagate with atomic operations
        Interlocked.Increment(ref edge._visitCount);
        InterlockedAddFloat(ref edge._totalValue, leafValue);
        Interlocked.Increment(ref node._visitCount);

        return leafValue;
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

    // ── Node creation and expansion (uniform priors) ────────────────────

    private static MctsNode CreateRoot(LegalActionSet legalActions)
    {
        var root = new MctsNode { IsExpanded = true };
        PopulateEdges(root, legalActions);
        return root;
    }

    private void ExpandNodeFromBattle(MctsNode node, Battle sim, SideId sideId)
    {
        LegalActionSet actions = GetLegalActionsFromBattle(sim, sideId);

        if (actions.SlotA.Count == 0)
        {
            node.IsExpanded = true;
            return;
        }

        PopulateEdges(node, actions);
    }

    private static void PopulateEdges(MctsNode node, LegalActionSet legalActions)
    {
        if (legalActions.SlotB.Count == 0)
        {
            // Single-slot decision
            float uniformPrior = 1f / legalActions.SlotA.Count;
            foreach (LegalAction actionA in legalActions.SlotA)
            {
                node.Edges.Add(new MctsEdge
                {
                    ActionA = actionA,
                    ActionB = null,
                    PriorP = uniformPrior,
                });
            }
        }
        else
        {
            // Joint action space for doubles
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

                    node.Edges.Add(new MctsEdge
                    {
                        ActionA = actionA,
                        ActionB = actionB,
                        PriorP = 1f, // Normalized below
                    });
                }
            }

            // Normalize to uniform
            if (node.Edges.Count > 0)
            {
                float uniform = 1f / node.Edges.Count;
                foreach (MctsEdge edge in node.Edges)
                    edge.PriorP = uniform;
            }
        }

        node.IsExpanded = true;
    }

    // ── Legal action enumeration from battle state ─────────────────────

    private LegalActionSet GetLegalActionsFromBattle(Battle sim, SideId sideId)
    {
        Side ourSide = sideId == SideId.P1 ? sim.P1 : sim.P2;
        IChoiceRequest? request = ourSide.ActiveRequest;

        return request switch
        {
            MoveRequest mr => GetLegalActionsForMove(mr),
            SwitchRequest sr => GetLegalActionsForSwitch(sr),
            _ => new LegalActionSet { SlotA = [], SlotB = [] },
        };
    }

    private LegalActionSet GetLegalActionsForMove(MoveRequest request)
    {
        var slotA = request.Active.Count > 0 && request.Active[0] != null
            ? GetSlotMoveActions(request.Active[0]!, request)
            : [new LegalAction { VocabIndex = 0, ChoiceType = ChoiceType.Pass, MoveId = MoveId.None }];

        var slotB = request.Active.Count > 1 && request.Active[1] != null
            ? GetSlotMoveActions(request.Active[1]!, request)
            : [];

        return new LegalActionSet { SlotA = slotA, SlotB = slotB };
    }

    private List<LegalAction> GetSlotMoveActions(PokemonMoveRequestData pokemonRequest, MoveRequest request)
    {
        var actions = new List<LegalAction>();

        MoveType? teraType = pokemonRequest.CanTerastallize switch
        {
            MoveTypeMoveTypeFalseUnion mtfu => mtfu.MoveType,
            _ => null,
        };

        var megaEvo = pokemonRequest.CanMegaEvo;

        foreach (PokemonMoveData move in pokemonRequest.Moves)
        {
            if (IsDisabled(move.Disabled)) continue;

            int targetLoc = GetTargetLocation(move.Move.Target);

            actions.Add(new LegalAction
            {
                VocabIndex = 0,
                ChoiceType = ChoiceType.Move,
                MoveId = move.Id,
                TargetLoc = targetLoc,
            });

            if (teraType.HasValue)
            {
                actions.Add(new LegalAction
                {
                    VocabIndex = 0,
                    ChoiceType = ChoiceType.Move,
                    MoveId = move.Id,
                    TargetLoc = targetLoc,
                    Terastallize = teraType,
                });
            }

            if (megaEvo.HasValue)
            {
                actions.Add(new LegalAction
                {
                    VocabIndex = 0,
                    ChoiceType = ChoiceType.Move,
                    MoveId = move.Id,
                    TargetLoc = targetLoc,
                    Mega = megaEvo,
                });
            }
        }

        // Add switch actions (if not trapped)
        if (pokemonRequest.Trapped != true)
        {
            AddSwitchActions(actions, request.Side);
        }

        // Fallback: Struggle
        if (actions.Count == 0 && pokemonRequest.Moves.Length > 0)
        {
            PokemonMoveData firstMove = pokemonRequest.Moves[0];
            actions.Add(new LegalAction
            {
                VocabIndex = 0,
                ChoiceType = ChoiceType.Move,
                MoveId = firstMove.Id,
                TargetLoc = GetTargetLocation(firstMove.Move.Target),
            });
        }

        if (actions.Count == 0)
        {
            actions.Add(new LegalAction
                { VocabIndex = 0, ChoiceType = ChoiceType.Pass, MoveId = MoveId.None });
        }

        return actions;
    }

    private static LegalActionSet GetLegalActionsForSwitch(SwitchRequest request)
    {
        bool slotANeeds = request.ForceSwitch.Count > 0 && request.ForceSwitch[0];
        bool slotBNeeds = request.ForceSwitch.Count > 1 && request.ForceSwitch[1];

        var slotAActions = new List<LegalAction>();
        var slotBActions = new List<LegalAction>();

        if (slotANeeds)
        {
            AddSwitchActions(slotAActions, request.Side);
            if (slotAActions.Count == 0)
                slotAActions.Add(new LegalAction
                    { VocabIndex = 0, ChoiceType = ChoiceType.Pass, MoveId = MoveId.None });
        }
        else if (slotBNeeds)
        {
            slotAActions.Add(new LegalAction
                { VocabIndex = 0, ChoiceType = ChoiceType.Pass, MoveId = MoveId.None });
        }

        if (slotBNeeds)
        {
            AddSwitchActions(slotBActions, request.Side);
            if (slotBActions.Count == 0)
                slotBActions.Add(new LegalAction
                    { VocabIndex = 0, ChoiceType = ChoiceType.Pass, MoveId = MoveId.None });
        }

        return new LegalActionSet { SlotA = slotAActions, SlotB = slotBActions };
    }

    private static void AddSwitchActions(List<LegalAction> actions, SideRequestData sideData)
    {
        for (int i = 0; i < sideData.Pokemon.Count; i++)
        {
            PokemonSwitchRequestData pokemon = sideData.Pokemon[i];
            if (pokemon.Active || pokemon.Condition == ConditionId.Fainted || pokemon.Reviving)
                continue;

            actions.Add(new LegalAction
            {
                VocabIndex = 0,
                ChoiceType = ChoiceType.Switch,
                MoveId = MoveId.None,
                SwitchIndex = i,
            });
        }
    }

    private int GetTargetLocation(MoveTarget targetType)
    {
        return targetType switch
        {
            MoveTarget.Normal or MoveTarget.Any or MoveTarget.AdjacentFoe
                => _targetRng.Value!.Next(1, 3),
            MoveTarget.AdjacentAlly
                => -_targetRng.Value!.Next(1, 3),
            MoveTarget.AdjacentAllyOrSelf
                => -_targetRng.Value!.Next(1, 3),
            _ => 0,
        };
    }

    private static bool IsDisabled(MoveIdBoolUnion? disabled) =>
        disabled is not null && disabled.IsTrue();

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
