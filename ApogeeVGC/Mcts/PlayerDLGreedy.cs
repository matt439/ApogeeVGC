using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Player;
using ApogeeVGC.Sim.Utils;
using PlayerUiType = ApogeeVGC.Sim.Player.PlayerUiType;

namespace ApogeeVGC.Mcts;

/// <summary>
/// Greedy DL player: picks the highest-probability action from the policy network each turn.
/// No MCTS search — pure model inference. Uses the value head only for diagnostics (via MctsLogger).
/// Useful for isolating DL model quality from search algorithm performance.
/// </summary>
public sealed class PlayerDLGreedy(
    SideId sideId,
    PlayerOptions options,
    IBattleController battleController,
    ModelInference model,
    ActionMapper actionMapper)
    : IPlayer
{
    public SideId SideId { get; } = sideId;
    public PlayerOptions Options { get; } = options;
    public PlayerUiType UiType => PlayerUiType.None;
    public IBattleController BattleController { get; } = battleController;
    public bool PrintDebug { get; } = options.PrintDebug;

    private readonly Prng _rng = options.Seed is null ? new Prng(null) : new Prng(options.Seed);

    public event EventHandler<ChoiceRequestEventArgs>? ChoiceRequested;
    public event EventHandler<Choice>? ChoiceSubmitted;

    public static PlayerDLGreedy Create(SideId sideId, PlayerOptions options, IBattleController battleController)
    {
        var actionMapper = new ActionMapper(MctsResources.Vocab);
        return new PlayerDLGreedy(sideId, options, battleController, MctsResources.Model, actionMapper);
    }

    public Choice GetChoiceSync(IChoiceRequest choiceRequest, BattleRequestType requestType,
        Func<BattlePerspective> perspectiveFactory)
    {
        return choiceRequest switch
        {
            TeamPreviewRequest tpr => GetTeamPreviewChoice(tpr, perspectiveFactory),
            MoveRequest or SwitchRequest => GetGreedyDLChoice(choiceRequest, perspectiveFactory),
            _ => throw new NotImplementedException(
                $"Request type {choiceRequest.GetType().Name} not implemented for DL-Greedy player"),
        };
    }

    public Task<Choice> GetNextChoiceAsync(IChoiceRequest choiceRequest, BattleRequestType requestType,
        BattlePerspective perspective, CancellationToken cancellationToken)
    {
        Choice choice = GetChoiceSync(choiceRequest, requestType, () => perspective);
        ChoiceSubmitted?.Invoke(this, choice);
        return Task.FromResult(choice);
    }

    public void UpdateUi(BattlePerspective perspective) { }
    public void UpdateEvents(IEnumerable<BattleEvent> events) { }
    public Task NotifyTimeoutWarningAsync(TimeSpan remainingTime) => Task.CompletedTask;
    public Task NotifyChoiceTimeoutAsync() => Task.CompletedTask;

    private Choice GetGreedyDLChoice(IChoiceRequest request, Func<BattlePerspective> perspectiveFactory)
    {
        BattlePerspective perspective = perspectiveFactory();
        LegalActionSet legalActions = actionMapper.GetLegalActions(request, perspective);

        // Single action — no inference needed
        if (legalActions.SlotA.Count == 1 && legalActions.SlotB.Count <= 1)
        {
            return actionMapper.BuildChoice(
                legalActions.SlotA[0],
                legalActions.SlotB.Count > 0 ? legalActions.SlotB[0] : null);
        }

        ModelOutput output = model.Evaluate(perspective);

        // Log value prediction for diagnostics
        MctsLogger.LogValue(output.Value);

        // Pick highest-probability legal action for slot A
        bool[] maskA = actionMapper.BuildLegalMask(legalActions.SlotA);
        float[] probsA = ModelInference.MaskedSoftmax(output.PolicyA, maskA);
        LegalAction bestA = PickBestAction(legalActions.SlotA, probsA);

        // Pick highest-probability legal action for slot B (if doubles)
        LegalAction? bestB = null;
        if (legalActions.SlotB.Count > 0)
        {
            bool[] maskB = actionMapper.BuildLegalMask(legalActions.SlotB);
            float[] probsB = ModelInference.MaskedSoftmax(output.PolicyB, maskB);
            LegalAction candidateB = PickBestAction(legalActions.SlotB, probsB);

            // Prevent both slots switching to the same Pokemon
            if (bestA.ChoiceType == ChoiceType.Switch &&
                candidateB.ChoiceType == ChoiceType.Switch &&
                bestA.SwitchIndex == candidateB.SwitchIndex)
            {
                // Pick second-best for slot B
                candidateB = PickBestActionExcluding(legalActions.SlotB, probsB, candidateB.SwitchIndex);
            }

            bestB = candidateB;
        }

        return actionMapper.BuildChoice(bestA, bestB);
    }

    private static LegalAction PickBestAction(IReadOnlyList<LegalAction> actions, float[] probs)
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

    private static LegalAction PickBestActionExcluding(IReadOnlyList<LegalAction> actions, float[] probs,
        int excludeSwitchIndex)
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

    // ── Team preview (same as PlayerMctsDL) ─────────────────────────────

    private Choice GetTeamPreviewChoice(TeamPreviewRequest request, Func<BattlePerspective> perspectiveFactory)
    {
        TeamPreviewInference? previewModel = MctsResources.TeamPreviewModel;
        if (previewModel == null)
            return GetRandomTeamPreviewChoice(request);

        BattlePerspective perspective = perspectiveFactory();
        TeamPreviewOutput output = previewModel.Evaluate(perspective);

        int teamSize = request.Side.Pokemon.Count;
        int bringCount = request.MaxChosenTeamSize ?? 4;

        var bringIndices = Enumerable.Range(0, teamSize)
            .OrderByDescending(i => output.BringScores[i])
            .Take(bringCount)
            .ToList();

        var leadIndices = bringIndices
            .OrderByDescending(i => output.LeadScores[i])
            .Take(2)
            .ToHashSet();

        var brought = new HashSet<int>(bringIndices);
        var ordered = new List<int>();

        ordered.AddRange(bringIndices.Where(i => leadIndices.Contains(i))
            .OrderByDescending(i => output.LeadScores[i]));

        ordered.AddRange(bringIndices.Where(i => !leadIndices.Contains(i))
            .OrderByDescending(i => output.BringScores[i]));

        for (int i = 0; i < teamSize; i++)
        {
            if (!brought.Contains(i))
                ordered.Add(i);
        }

        var actions = ordered.Select((originalIndex, newPosition) => new ChosenAction
        {
            Choice = ChoiceType.Team,
            Pokemon = null,
            MoveId = MoveId.None,
            Index = newPosition,
            TargetLoc = originalIndex,
            Priority = -newPosition,
        }).ToList();

        return new Choice { Actions = actions };
    }

    private Choice GetRandomTeamPreviewChoice(TeamPreviewRequest request)
    {
        var pokemon = request.Side.Pokemon;
        var order = Enumerable.Range(0, pokemon.Count).ToList();

        for (int i = order.Count - 1; i > 0; i--)
        {
            int j = _rng.Random(0, i + 1);
            (order[i], order[j]) = (order[j], order[i]);
        }

        var actions = order.Select((originalIndex, newPosition) => new ChosenAction
        {
            Choice = ChoiceType.Team,
            Pokemon = null,
            MoveId = MoveId.None,
            Index = newPosition,
            TargetLoc = originalIndex,
            Priority = -newPosition,
        }).ToList();

        return new Choice { Actions = actions };
    }
}
