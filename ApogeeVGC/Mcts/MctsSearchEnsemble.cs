using ApogeeVGC.Mcts.Ensemble;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Mcts;

/// <summary>
/// MCTS search with ensemble-informed root priors and heuristic leaf evaluation.
/// Root priors come from the mini-model ensemble (replacing uniform or DL policy).
/// Internal node priors are uniform. Leaf evaluation uses HeuristicEval.
/// </summary>
public sealed class MctsSearchEnsemble(
    MctsConfig config,
    EnsembleEvaluator ensemble,
    OpponentInference? opponentModel = null,
    StateEncoder? stateEncoder = null) : IMctsSearch
{
    private readonly ThreadLocal<Random> _targetRng = new(() => new Random());

    /// <summary>
    /// Optional revealed info tracker for CTS/OTS observability restriction.
    /// Set by the player before each search. Null = full observability.
    /// </summary>
    public BattleInfoTracker? Tracker { get; set; }

    /// <summary>Iteration count from the last search (for diagnostics).</summary>
    public int LastSearchIterations { get; private set; }

    /// <summary>
    /// Number of random turns to simulate at leaf nodes before applying heuristic evaluation.
    /// 0 = pure heuristic (current default). Higher = deeper lookahead but fewer iterations.
    /// Typical values: 0-5. Set via MctsConfig or directly.
    /// </summary>
    public int RolloutDepth { get; set; }

    public (LegalAction ActionA, LegalAction? ActionB) Search(
        Battle battle,
        SideId sideId,
        LegalActionSet legalActions,
        CancellationToken cancellationToken = default)
    {
        if (legalActions.SlotA.Count == 1 && legalActions.SlotB.Count <= 1)
        {
            return (legalActions.SlotA[0],
                legalActions.SlotB.Count > 0 ? legalActions.SlotB[0] : null);
        }

        // Create root with ensemble-informed priors
        MctsNode root = CreateRootWithEnsemblePriors(
            battle, sideId, legalActions);

        AddDirichletNoise(root);

        int maxParallelism = config.MaxDegreeOfParallelism ?? Environment.ProcessorCount;
        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = maxParallelism };

        if (cancellationToken.CanBeCanceled || config.TimeBudgetMs > 0)
        {
            // Time-based or externally cancelled: spin up worker threads that
            // loop until the token fires.  Parallel.For with int.MaxValue
            // partitions poorly, so we use explicit threads instead.
            using var cts = config.TimeBudgetMs > 0
                ? CancellationTokenSource.CreateLinkedTokenSource(cancellationToken)
                : null;
            if (cts != null && config.TimeBudgetMs > 0)
                cts.CancelAfter(config.TimeBudgetMs);

            CancellationToken token = cts?.Token ?? cancellationToken;

            Thread[] workers = new Thread[maxParallelism];
            for (int t = 0; t < maxParallelism; t++)
            {
                workers[t] = new Thread(() =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        Battle sim = battle.Copy();
                        RunIteration(root, sim, sideId);
                        sim.RecycleClonedStates();
                    }
                }) { IsBackground = true };
                workers[t].Start();
            }
            for (int t = 0; t < maxParallelism; t++)
                workers[t].Join();
        }
        else
        {
            Parallel.For(0, config.NumIterations, parallelOptions, _ =>
            {
                Battle sim = battle.Copy();
                RunIteration(root, sim, sideId);
                sim.RecycleClonedStates();
            });
        }

        LastSearchIterations = root.VisitCount;
        return SelectBestAction(root);
    }

    /// <summary>
    /// Create root node with priors from the ensemble evaluator.
    /// </summary>
    private MctsNode CreateRootWithEnsemblePriors(
        Battle battle, SideId sideId, LegalActionSet legalActions)
    {
        var root = new MctsNode { IsExpanded = true };
        PopulateEdges(root, legalActions);

        if (root.Edges.Count == 0) return root;

        // Get opponent prediction if model is available
        OpponentPrediction? oppPred = null;
        if (opponentModel != null && stateEncoder != null)
        {
            try
            {
                BattlePerspective perspective = battle.GetPerspectiveForSide(sideId, BattlePerspectiveType.InBattle);
                oppPred = opponentModel.Predict(perspective);
            }
            catch
            {
                // Graceful fallback — ensemble works without opponent prediction
            }
        }

        // Get ensemble priors
        float[] priors = ensemble.ScoreEdges(battle, sideId, root.Edges, oppPred, Tracker);

        for (int i = 0; i < root.Edges.Count; i++)
            root.Edges[i].PriorP = priors[i];

        return root;
    }

    private const int MaxDepth = 50;

    private float RunIteration(MctsNode node, Battle sim, SideId sideId, int depth = 0)
    {
        if (node.IsTerminal)
        {
            Interlocked.Increment(ref node._visitCount);
            return node.TerminalValue;
        }

        if (depth >= MaxDepth)
        {
            // Prevent infinite recursion — evaluate and return
            Interlocked.Increment(ref node._visitCount);
            return HeuristicEval.Evaluate(sim, sideId);
        }

        MctsEdge? edge = SelectEdge(node);
        if (edge == null) return 0.5f;

        // Virtual loss: immediately claim this edge so other threads explore elsewhere.
        // Increment visit count now (with implicit value 0 = pessimistic) to lower
        // this edge's Q and PUCT score for concurrent readers.
        Interlocked.Increment(ref edge._visitCount);
        Interlocked.Increment(ref node._visitCount);

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
            // Simulation error — keep virtual loss as the real result (0.5 neutral).
            // Visit already counted above; just add the value.
            leafValue = 0.5f;
            InterlockedAddFloat(ref edge._totalValue, leafValue);
            return leafValue;
        }

        if (sim.Ended)
        {
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
            lock (node.ExpandLock)
            {
                if (edge.Child is not { IsExpanded: true })
                {
                    edge.Child ??= new MctsNode();
                    ExpandNodeFromBattle(edge.Child, sim, sideId);
                }
            }
            // Leaf evaluation: optional random rollout then heuristic
            leafValue = EvaluateLeaf(sim, sideId);
        }
        else
        {
            leafValue = RunIteration(edge.Child, sim, sideId, depth + 1);
        }

        // Backpropagate: visit already counted by virtual loss above,
        // just add the real evaluation value.
        InterlockedAddFloat(ref edge._totalValue, leafValue);

        return leafValue;
    }

    /// <summary>
    /// Evaluate a leaf node: run K random turns then apply heuristic.
    /// If RolloutDepth=0, just applies heuristic directly (fastest).
    /// </summary>
    private float EvaluateLeaf(Battle sim, SideId sideId)
    {
        if (RolloutDepth <= 0)
            return HeuristicEval.Evaluate(sim, sideId, Tracker);

        // Random rollout for K turns
        for (int k = 0; k < RolloutDepth; k++)
        {
            if (sim.Ended) break;

            try
            {
                Side s1 = sim.P1;
                Side s2 = sim.P2;
                s1.RandomAutoChoose(_targetRng.Value!);
                s2.RandomAutoChoose(_targetRng.Value!);
                sim.CommitChoices();
            }
            catch
            {
                break; // Simulation error — stop rollout
            }
        }

        if (sim.Ended)
            return GetTerminalValue(sim, sideId);

        return HeuristicEval.Evaluate(sim, sideId, Tracker);
    }

    // ── Shared infrastructure (same as MctsSearchStandalone) ─────────

    private static void InterlockedAddFloat(ref float location, float value)
    {
        float initialValue, computedValue;
        do
        {
            initialValue = location;
            computedValue = initialValue + value;
        } while (Interlocked.CompareExchange(ref location, computedValue, initialValue) != initialValue);
    }

    private static void PopulateEdges(MctsNode node, LegalActionSet legalActions)
    {
        if (legalActions.SlotB.Count == 0)
        {
            float uniformPrior = 1f / legalActions.SlotA.Count;
            foreach (LegalAction actionA in legalActions.SlotA)
            {
                node.Edges.Add(new MctsEdge
                {
                    ActionA = actionA, ActionB = null, PriorP = uniformPrior,
                });
            }
        }
        else
        {
            foreach (LegalAction actionA in legalActions.SlotA)
            foreach (LegalAction actionB in legalActions.SlotB)
            {
                if (actionA.ChoiceType == ChoiceType.Switch &&
                    actionB.ChoiceType == ChoiceType.Switch &&
                    actionA.SwitchIndex == actionB.SwitchIndex)
                    continue;

                node.Edges.Add(new MctsEdge
                {
                    ActionA = actionA, ActionB = actionB, PriorP = 1f,
                });
            }

            if (node.Edges.Count > 0)
            {
                float uniform = 1f / node.Edges.Count;
                foreach (MctsEdge edge in node.Edges)
                    edge.PriorP = uniform;
            }
        }

        node.IsExpanded = true;
    }

    private void ExpandNodeFromBattle(MctsNode node, Battle sim, SideId sideId)
    {
        LegalActionSet actions = GetLegalActionsFromBattle(sim, sideId);
        if (actions.SlotA.Count == 0) { node.IsExpanded = true; return; }
        // Internal nodes get uniform priors (ensemble only at root)
        PopulateEdges(node, actions);
    }

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
            actions.Add(new LegalAction { VocabIndex = 0, ChoiceType = ChoiceType.Move, MoveId = move.Id, TargetLoc = targetLoc });
            if (teraType.HasValue)
                actions.Add(new LegalAction { VocabIndex = 0, ChoiceType = ChoiceType.Move, MoveId = move.Id, TargetLoc = targetLoc, Terastallize = teraType });
            if (megaEvo.HasValue)
                actions.Add(new LegalAction { VocabIndex = 0, ChoiceType = ChoiceType.Move, MoveId = move.Id, TargetLoc = targetLoc, Mega = megaEvo });
        }

        if (pokemonRequest.Trapped != true)
            AddSwitchActions(actions, request.Side);

        if (actions.Count == 0 && pokemonRequest.Moves.Length > 0)
        {
            PokemonMoveData firstMove = pokemonRequest.Moves[0];
            actions.Add(new LegalAction { VocabIndex = 0, ChoiceType = ChoiceType.Move, MoveId = firstMove.Id, TargetLoc = GetTargetLocation(firstMove.Move.Target) });
        }

        if (actions.Count == 0)
            actions.Add(new LegalAction { VocabIndex = 0, ChoiceType = ChoiceType.Pass, MoveId = MoveId.None });

        return actions;
    }

    private static LegalActionSet GetLegalActionsForSwitch(SwitchRequest request)
    {
        bool slotANeeds = request.ForceSwitch.Count > 0 && request.ForceSwitch[0];
        bool slotBNeeds = request.ForceSwitch.Count > 1 && request.ForceSwitch[1];

        var slotA = new List<LegalAction>();
        var slotB = new List<LegalAction>();

        if (slotANeeds) { AddSwitchActions(slotA, request.Side); if (slotA.Count == 0) slotA.Add(new LegalAction { VocabIndex = 0, ChoiceType = ChoiceType.Pass, MoveId = MoveId.None }); }
        else if (slotBNeeds) slotA.Add(new LegalAction { VocabIndex = 0, ChoiceType = ChoiceType.Pass, MoveId = MoveId.None });

        if (slotBNeeds) { AddSwitchActions(slotB, request.Side); if (slotB.Count == 0) slotB.Add(new LegalAction { VocabIndex = 0, ChoiceType = ChoiceType.Pass, MoveId = MoveId.None }); }

        return new LegalActionSet { SlotA = slotA, SlotB = slotB };
    }

    private static void AddSwitchActions(List<LegalAction> actions, SideRequestData sideData)
    {
        for (int i = 0; i < sideData.Pokemon.Count; i++)
        {
            PokemonSwitchRequestData pokemon = sideData.Pokemon[i];
            if (pokemon.Active || pokemon.Condition == ConditionId.Fainted || pokemon.Reviving) continue;
            actions.Add(new LegalAction { VocabIndex = 0, ChoiceType = ChoiceType.Switch, MoveId = MoveId.None, SwitchIndex = i });
        }
    }

    private int GetTargetLocation(MoveTarget targetType) => targetType switch
    {
        MoveTarget.Normal or MoveTarget.Any or MoveTarget.AdjacentFoe => _targetRng.Value!.Next(1, 3),
        MoveTarget.AdjacentAlly => -_targetRng.Value!.Next(1, 3),
        MoveTarget.AdjacentAllyOrSelf => -_targetRng.Value!.Next(1, 3),
        _ => 0,
    };

    private static bool IsDisabled(MoveIdBoolUnion? disabled) => disabled is not null && disabled.IsTrue();

    private MctsEdge? SelectEdge(MctsNode node)
    {
        if (node.Edges.Count == 0) return null;
        MctsEdge? best = null;
        float bestScore = float.NegativeInfinity;
        int parentVisits = node.VisitCount;
        foreach (MctsEdge edge in node.Edges)
        {
            float score = edge.Q + config.CPuct * edge.PriorP * MathF.Sqrt(parentVisits) / (1 + edge.VisitCount);
            if (score > bestScore) { bestScore = score; best = edge; }
        }
        return best;
    }

    private static (LegalAction, LegalAction?) SelectBestAction(MctsNode root)
    {
        MctsEdge? best = null;
        int bestVisits = -1;
        foreach (MctsEdge edge in root.Edges)
            if (edge.VisitCount > bestVisits) { bestVisits = edge.VisitCount; best = edge; }
        if (best == null) throw new InvalidOperationException("MCTS search produced no results");
        return (best.ActionA, best.ActionB);
    }

    private static float GetTerminalValue(Battle sim, SideId sideId)
    {
        if (string.IsNullOrEmpty(sim.Winner)) return 0.5f;
        string ourName = (sideId == SideId.P1 ? sim.P1 : sim.P2).Name;
        return sim.Winner.Equals(ourName, StringComparison.OrdinalIgnoreCase) ? 1.0f : 0.0f;
    }

    private static Choice BuildChoice(LegalAction slotA, LegalAction? slotB)
    {
        var actions = new List<ChosenAction> { BuildChosenAction(slotA) };
        if (slotB.HasValue) actions.Add(BuildChosenAction(slotB.Value));
        return new Choice { Actions = actions };
    }

    private static ChosenAction BuildChosenAction(LegalAction action) => action.ChoiceType switch
    {
        ChoiceType.Move => new ChosenAction { Choice = ChoiceType.Move, MoveId = action.MoveId, TargetLoc = action.TargetLoc, Terastallize = action.Terastallize, Mega = action.Mega },
        ChoiceType.Switch => new ChosenAction { Choice = ChoiceType.Switch, MoveId = MoveId.None, Index = action.SwitchIndex },
        _ => new ChosenAction { Choice = ChoiceType.Pass, MoveId = MoveId.None },
    };

    private void AddDirichletNoise(MctsNode root)
    {
        if (root.Edges.Count == 0) return;
        float epsilon = config.DirichletEpsilon;
        float[] noise = SampleDirichlet(root.Edges.Count, config.DirichletAlpha);
        for (int i = 0; i < root.Edges.Count; i++)
            root.Edges[i].PriorP = (1 - epsilon) * root.Edges[i].PriorP + epsilon * noise[i];
    }

    private static float[] SampleDirichlet(int n, float alpha)
    {
        Random rng = Random.Shared;
        var samples = new float[n];
        float sum = 0f;
        for (int i = 0; i < n; i++) { samples[i] = SampleGamma(rng, alpha); sum += samples[i]; }
        if (sum > 0f) for (int i = 0; i < n; i++) samples[i] /= sum;
        return samples;
    }

    private static float SampleGamma(Random rng, float alpha)
    {
        if (alpha < 1f) { float sample = SampleGamma(rng, alpha + 1f); return sample * MathF.Pow((float)rng.NextDouble(), 1f / alpha); }
        float d = alpha - 1f / 3f, c = 1f / MathF.Sqrt(9f * d);
        while (true)
        {
            float x, v;
            do { x = (float)SampleStandardNormal(rng); v = 1f + c * x; } while (v <= 0f);
            v = v * v * v;
            float u2 = (float)rng.NextDouble();
            if (u2 < 1f - 0.0331f * (x * x) * (x * x)) return d * v;
            if (MathF.Log(u2) < 0.5f * x * x + d * (1f - v + MathF.Log(v))) return d * v;
        }
    }

    private static double SampleStandardNormal(Random rng)
    {
        double u1 = 1.0 - rng.NextDouble(), u2 = rng.NextDouble();
        return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
    }
}
