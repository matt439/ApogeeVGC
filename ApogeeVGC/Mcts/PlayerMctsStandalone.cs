using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Player;
using ApogeeVGC.Sim.Utils;
using ApogeeVGC.Sim.Utils.Unions;
using PlayerUiType = ApogeeVGC.Sim.Player.PlayerUiType;

namespace ApogeeVGC.Mcts;

/// <summary>
/// Standalone MCTS player with no DL model dependencies.
/// Uses uniform priors and heuristic evaluation for testing MCTS mechanics in isolation.
/// </summary>
public sealed class PlayerMctsStandalone(
    SideId sideId,
    PlayerOptions options,
    IBattleController battleController,
    IMctsSearch search)
    : IPlayer
{
    public SideId SideId { get; } = sideId;
    public PlayerOptions Options { get; } = options;
    public PlayerUiType UiType => PlayerUiType.None;
    public IBattleController BattleController { get; } = battleController;
    public bool PrintDebug { get; } = options.PrintDebug;

    private readonly Prng _rng = options.Seed is null ? new Prng(null) : new Prng(options.Seed);
    private readonly Random _targetRng = new();

    public event EventHandler<ChoiceRequestEventArgs>? ChoiceRequested;
    public event EventHandler<Choice>? ChoiceSubmitted;

    public Choice GetChoiceSync(IChoiceRequest choiceRequest, BattleRequestType requestType,
        Func<BattlePerspective> perspectiveFactory)
    {
        return choiceRequest switch
        {
            TeamPreviewRequest tpr => GetRandomTeamPreviewChoice(tpr),
            MoveRequest mr => GetSearchChoice(mr, perspectiveFactory),
            SwitchRequest sr => GetSearchChoiceForSwitch(sr, perspectiveFactory),
            _ => throw new NotImplementedException(
                $"Request type {choiceRequest.GetType().Name} not implemented for standalone MCTS"),
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

    private Choice GetSearchChoice(MoveRequest request, Func<BattlePerspective> perspectiveFactory)
    {
        Battle battle = BattleController.Battle
                        ?? throw new InvalidOperationException("Battle is not initialized");
        BattlePerspective perspective = perspectiveFactory();

        LegalActionSet legalActions = GetLegalActionsForMove(request, perspective);

        (LegalAction actionA, LegalAction? actionB) = search.Search(battle, SideId, legalActions);

        return BuildChoice(actionA, actionB);
    }

    private Choice GetSearchChoiceForSwitch(SwitchRequest request, Func<BattlePerspective> perspectiveFactory)
    {
        Battle battle = BattleController.Battle
                        ?? throw new InvalidOperationException("Battle is not initialized");
        BattlePerspective perspective = perspectiveFactory();

        LegalActionSet legalActions = GetLegalActionsForSwitch(request, perspective);

        // If no actions available, fall back to pass
        if (legalActions.SlotA.Count == 0 && legalActions.SlotB.Count == 0)
        {
            return new Choice { Actions = [new ChosenAction { Choice = ChoiceType.Pass, MoveId = MoveId.None }] };
        }

        (LegalAction actionA, LegalAction? actionB) = search.Search(battle, SideId, legalActions);

        return BuildChoice(actionA, actionB);
    }

    // ── Legal action enumeration (no Vocab dependency) ─────────────────

    private LegalActionSet GetLegalActionsForMove(MoveRequest request, BattlePerspective perspective)
    {
        var slotA = request.Active.Count > 0
            ? GetSlotLegalActions(request.Active[0], request, perspective)
            : [new LegalAction { VocabIndex = 0, ChoiceType = ChoiceType.Pass, MoveId = MoveId.None }];

        var slotB = request.Active.Count > 1
            ? GetSlotLegalActions(request.Active[1], request, perspective)
            : [];

        return new LegalActionSet { SlotA = slotA, SlotB = slotB };
    }

    private List<LegalAction> GetSlotLegalActions(
        PokemonMoveRequestData? pokemonRequest,
        MoveRequest request,
        BattlePerspective perspective)
    {
        if (pokemonRequest == null)
        {
            return [new LegalAction { VocabIndex = 0, ChoiceType = ChoiceType.Pass, MoveId = MoveId.None }];
        }

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
                VocabIndex = 0, // Not used in standalone
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
        bool isTrapped = pokemonRequest.Trapped == true;
        if (!isTrapped)
        {
            AddSwitchActions(actions, request.Side, perspective);
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

    private LegalActionSet GetLegalActionsForSwitch(SwitchRequest request, BattlePerspective perspective)
    {
        bool slotANeeds = request.ForceSwitch.Count > 0 && request.ForceSwitch[0];
        bool slotBNeeds = request.ForceSwitch.Count > 1 && request.ForceSwitch[1];

        var slotAActions = new List<LegalAction>();
        var slotBActions = new List<LegalAction>();

        if (slotANeeds)
        {
            AddSwitchActions(slotAActions, request.Side, perspective);
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
            AddSwitchActions(slotBActions, request.Side, perspective);
            if (slotBActions.Count == 0)
                slotBActions.Add(new LegalAction
                    { VocabIndex = 0, ChoiceType = ChoiceType.Pass, MoveId = MoveId.None });
        }

        return new LegalActionSet { SlotA = slotAActions, SlotB = slotBActions };
    }

    private static void AddSwitchActions(List<LegalAction> actions, SideRequestData sideData,
        BattlePerspective perspective)
    {
        for (var i = 0; i < sideData.Pokemon.Count; i++)
        {
            PokemonSwitchRequestData pokemon = sideData.Pokemon[i];
            if (pokemon.Active || pokemon.Condition == ConditionId.Fainted || pokemon.Reviving)
                continue;

            actions.Add(new LegalAction
            {
                VocabIndex = 0, // Not used in standalone
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
                => _targetRng.Next(1, 3),
            MoveTarget.AdjacentAlly
                => -_targetRng.Next(1, 3),
            MoveTarget.AdjacentAllyOrSelf
                => -_targetRng.Next(1, 3),
            _ => 0,
        };
    }

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

    private static bool IsDisabled(MoveIdBoolUnion? disabled) =>
        disabled is not null && disabled.IsTrue();

    // ── Team preview (random) ──────────────────────────────────────────

    private Choice GetRandomTeamPreviewChoice(TeamPreviewRequest request)
    {
        var pokemon = request.Side.Pokemon;
        int bringCount = request.MaxChosenTeamSize ?? pokemon.Count;
        var order = Enumerable.Range(0, pokemon.Count).ToList();

        for (int i = order.Count - 1; i > 0; i--)
        {
            int j = _rng.Random(0, i + 1);
            (order[i], order[j]) = (order[j], order[i]);
        }

        order = order.Take(bringCount).ToList();

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
