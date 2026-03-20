using ApogeeVGC.Mcts.Ensemble;
using ApogeeVGC.Mcts.Ensemble.MiniModels;
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
/// MCTS player using the mini-model ensemble for root priors
/// and heuristic leaf evaluation.
/// </summary>
public sealed class PlayerMctsEnsemble : IPlayer
{
    private readonly MctsSearchEnsemble _search;
    private readonly Random _targetRng = new();

    public SideId SideId { get; }
    public PlayerOptions Options { get; }
    public PlayerUiType UiType => PlayerUiType.None;
    public IBattleController BattleController { get; }
    public bool PrintDebug { get; }

    public event EventHandler<ChoiceRequestEventArgs>? ChoiceRequested;
    public event EventHandler<Choice>? ChoiceSubmitted;

    private PlayerMctsEnsemble(
        SideId sideId,
        PlayerOptions options,
        IBattleController battleController,
        MctsSearchEnsemble search)
    {
        SideId = sideId;
        Options = options;
        BattleController = battleController;
        PrintDebug = options.PrintDebug;
        _search = search;
    }

    /// <summary>
    /// Factory method that creates the ensemble player with default mini-models.
    /// Gracefully handles missing opponent model — those mini-models just get null predictions.
    /// </summary>
    public static PlayerMctsEnsemble Create(
        SideId sideId,
        PlayerOptions options,
        IBattleController battleController)
    {
        // Build mini-models
        IMiniModel[] models =
        [
            new DamageMaxMiniModel(),
            new KOSeekingMiniModel(),
            new KOAvoidanceMiniModel(),
            new TypePositioningMiniModel(),
            new DamageMinMiniModel(),
            new SpeedAdvantageMiniModel(),
            new StatusSpreadingMiniModel(),
            new ProtectPredictionMiniModel(),
        ];

        // Load ensemble config (weights) if available
        string configPath = Environment.GetEnvironmentVariable("APOGEE_ENSEMBLE_CONFIG")
                            ?? Path.Combine("Tools", "DLModel", "ensemble_config.json");
        EnsembleConfig config = EnsembleConfig.Load(configPath);

        var ensemble = new EnsembleEvaluator(models, config);

        // Try to load opponent prediction model
        OpponentInference? opponentModel = null;
        StateEncoder? stateEncoder = null;

        if (MctsResources.OpponentModel != null)
        {
            opponentModel = MctsResources.OpponentModel;
            stateEncoder = MctsResources.Encoder;
        }

        MctsConfig mctsConfig = options.MctsConfig ?? new MctsConfig();
        var search = new MctsSearchEnsemble(
            mctsConfig, ensemble, opponentModel, stateEncoder);

        return new PlayerMctsEnsemble(sideId, options, battleController, search);
    }

    public Choice GetChoiceSync(IChoiceRequest choiceRequest, BattleRequestType requestType,
        Func<BattlePerspective> perspectiveFactory)
    {
        return choiceRequest switch
        {
            TeamPreviewRequest tpr => GetTeamPreviewChoice(tpr, perspectiveFactory),
            MoveRequest mr => GetSearchChoice(mr, perspectiveFactory),
            SwitchRequest sr => GetSearchChoiceForSwitch(sr, perspectiveFactory),
            _ => throw new NotImplementedException(
                $"Request type {choiceRequest.GetType().Name} not implemented"),
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

    // ── Team preview (DL if available, else random) ─────────────────

    private Choice GetTeamPreviewChoice(TeamPreviewRequest request,
        Func<BattlePerspective> perspectiveFactory)
    {
        // Use DL team preview if available
        TeamPreviewInference? previewModel = MctsResources.TeamPreviewModel;
        if (previewModel != null)
        {
            try
            {
                BattlePerspective perspective = perspectiveFactory();
                TeamPreviewOutput output = previewModel.Evaluate(perspective);
                var ordered = output.OrderedIndices;

                var actions = new List<ChosenAction>();
                foreach (int idx in ordered)
                {
                    actions.Add(new ChosenAction
                    {
                        Choice = ChoiceType.Team,
                        Index = idx,
                        MoveId = MoveId.None,
                    });
                }
                return new Choice { Actions = actions };
            }
            catch { /* fallthrough to random */ }
        }

        return GetRandomTeamPreviewChoice(request);
    }

    private Choice GetRandomTeamPreviewChoice(TeamPreviewRequest request)
    {
        int teamSize = request.MaxChosenTeamSize ?? 4;
        int totalPokemon = request.Side.Pokemon.Count;
        var indices = Enumerable.Range(0, totalPokemon).ToList();
        var rng = new Random();
        for (int i = indices.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }
        var actions = indices.Take(teamSize)
            .Select(idx => new ChosenAction { Choice = ChoiceType.Team, Index = idx, MoveId = MoveId.None })
            .ToList();
        return new Choice { Actions = actions };
    }

    // ── Search-based choices ─────────────────────────────────────────

    private Choice GetSearchChoice(MoveRequest request, Func<BattlePerspective> perspectiveFactory)
    {
        Battle battle = BattleController.Battle
                        ?? throw new InvalidOperationException("Battle is not initialized");
        BattlePerspective perspective = perspectiveFactory();
        LegalActionSet legalActions = GetLegalActionsForMove(request, perspective);
        (LegalAction actionA, LegalAction? actionB) = _search.Search(battle, SideId, legalActions);
        return BuildChoice(actionA, actionB);
    }

    private Choice GetSearchChoiceForSwitch(SwitchRequest request, Func<BattlePerspective> perspectiveFactory)
    {
        Battle battle = BattleController.Battle
                        ?? throw new InvalidOperationException("Battle is not initialized");
        BattlePerspective perspective = perspectiveFactory();
        LegalActionSet legalActions = GetLegalActionsForSwitch(request, perspective);
        if (legalActions.SlotA.Count == 0 && legalActions.SlotB.Count == 0)
            return new Choice { Actions = [new ChosenAction { Choice = ChoiceType.Pass, MoveId = MoveId.None }] };
        (LegalAction actionA, LegalAction? actionB) = _search.Search(battle, SideId, legalActions);
        return BuildChoice(actionA, actionB);
    }

    // ── Legal action enumeration (same as Standalone) ────────────────

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
        PokemonMoveRequestData? pokemonRequest, MoveRequest request, BattlePerspective perspective)
    {
        if (pokemonRequest == null)
            return [new LegalAction { VocabIndex = 0, ChoiceType = ChoiceType.Pass, MoveId = MoveId.None }];

        var actions = new List<LegalAction>();
        MoveType? teraType = pokemonRequest.CanTerastallize switch
        {
            MoveTypeMoveTypeFalseUnion mtfu => mtfu.MoveType, _ => null,
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
            AddSwitchActions(actions, request.Side, perspective);

        if (actions.Count == 0 && pokemonRequest.Moves.Length > 0)
        {
            PokemonMoveData firstMove = pokemonRequest.Moves[0];
            actions.Add(new LegalAction { VocabIndex = 0, ChoiceType = ChoiceType.Move, MoveId = firstMove.Id, TargetLoc = GetTargetLocation(firstMove.Move.Target) });
        }
        if (actions.Count == 0)
            actions.Add(new LegalAction { VocabIndex = 0, ChoiceType = ChoiceType.Pass, MoveId = MoveId.None });

        return actions;
    }

    private LegalActionSet GetLegalActionsForSwitch(SwitchRequest request, BattlePerspective perspective)
    {
        bool slotANeeds = request.ForceSwitch.Count > 0 && request.ForceSwitch[0];
        bool slotBNeeds = request.ForceSwitch.Count > 1 && request.ForceSwitch[1];
        var slotA = new List<LegalAction>();
        var slotB = new List<LegalAction>();

        if (slotANeeds) { AddSwitchActions(slotA, request.Side, perspective); if (slotA.Count == 0) slotA.Add(new LegalAction { VocabIndex = 0, ChoiceType = ChoiceType.Pass, MoveId = MoveId.None }); }
        else if (slotBNeeds) slotA.Add(new LegalAction { VocabIndex = 0, ChoiceType = ChoiceType.Pass, MoveId = MoveId.None });
        if (slotBNeeds) { AddSwitchActions(slotB, request.Side, perspective); if (slotB.Count == 0) slotB.Add(new LegalAction { VocabIndex = 0, ChoiceType = ChoiceType.Pass, MoveId = MoveId.None }); }

        return new LegalActionSet { SlotA = slotA, SlotB = slotB };
    }

    private static void AddSwitchActions(List<LegalAction> actions, SideRequestData sideData, BattlePerspective perspective)
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
        MoveTarget.Normal or MoveTarget.Any or MoveTarget.AdjacentFoe => _targetRng.Next(1, 3),
        MoveTarget.AdjacentAlly => -_targetRng.Next(1, 3),
        MoveTarget.AdjacentAllyOrSelf => -_targetRng.Next(1, 3),
        _ => 0,
    };

    private static bool IsDisabled(MoveIdBoolUnion? disabled) => disabled is not null && disabled.IsTrue();

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
}
