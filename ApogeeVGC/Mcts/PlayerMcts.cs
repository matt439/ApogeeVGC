using System.Diagnostics;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Player;
using ApogeeVGC.Sim.Utils;
using PlayerUiType = ApogeeVGC.Sim.Player.PlayerUiType;

namespace ApogeeVGC.Mcts;

public sealed class PlayerMcts(
    SideId sideId,
    PlayerOptions options,
    IBattleController battleController,
    MctsSearch search,
    ActionMapper actionMapper,
    BattleInfoTracker infoTracker)
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

    /// <summary>
    /// The information tracker for this player, tracking what opponent info has been revealed.
    /// </summary>
    public BattleInfoTracker InfoTracker => infoTracker;

    /// <summary>
    /// Factory method that creates a PlayerMcts using MctsResources.
    /// </summary>
    public static PlayerMcts Create(SideId sideId, PlayerOptions options, IBattleController battleController)
    {
        var actionMapper = new ActionMapper(MctsResources.Vocab);
        var search = new MctsSearch(MctsResources.Config, MctsResources.Model, actionMapper);
        var infoTracker = new BattleInfoTracker(sideId, MctsResources.Library);
        return new PlayerMcts(sideId, options, battleController, search, actionMapper, infoTracker);
    }

    public Choice GetChoiceSync(IChoiceRequest choiceRequest, BattleRequestType requestType,
        Func<BattlePerspective> perspectiveFactory)
    {
        if (PrintDebug)
            Console.WriteLine($"[PlayerMcts.GetChoiceSync] Called for {SideId}");

        return choiceRequest switch
        {
            TeamPreviewRequest tpr => GetTeamPreviewChoice(tpr, perspectiveFactory),
            MoveRequest or SwitchRequest => GetSearchChoice(choiceRequest, perspectiveFactory),
            _ => throw new NotImplementedException(
                $"Request type {choiceRequest.GetType().Name} not implemented for MCTS player"),
        };
    }

    public Task<Choice> GetNextChoiceAsync(IChoiceRequest choiceRequest, BattleRequestType requestType,
        BattlePerspective perspective, CancellationToken cancellationToken)
    {
        Choice choice = GetChoiceSync(choiceRequest, requestType, () => perspective);
        ChoiceSubmitted?.Invoke(this, choice);
        return Task.FromResult(choice);
    }

    public void UpdateUi(BattlePerspective perspective)
    {
    }

    public void UpdateEvents(IEnumerable<BattleEvent> events)
    {
        infoTracker.ProcessEvents(events);
    }

    public Task NotifyTimeoutWarningAsync(TimeSpan remainingTime) => Task.CompletedTask;
    public Task NotifyChoiceTimeoutAsync() => Task.CompletedTask;

    private Choice GetSearchChoice(IChoiceRequest request, Func<BattlePerspective> perspectiveFactory)
    {
        Battle battle = BattleController.Battle
                        ?? throw new InvalidOperationException("Battle is not initialized on the controller");
        BattlePerspective rawPerspective = perspectiveFactory();

        // Initialize tracker if this is the first perspective we see
        infoTracker.EnsureInitialized(rawPerspective);

        // Filter the perspective to only include revealed opponent information
        BattlePerspective perspective = infoTracker.FilterPerspective(rawPerspective);

        (LegalAction actionA, var actionB) = search.Search(battle, SideId, request, perspective);

        if (PrintDebug)
        {
            Console.WriteLine($"[PlayerMcts] SlotA: {MctsResources.Vocab.GetActionKey(actionA.VocabIndex)} " +
                              $"(type={actionA.ChoiceType}, tera={actionA.Terastallize})");
            if (actionB.HasValue)
            {
                Console.WriteLine($"[PlayerMcts] SlotB: {MctsResources.Vocab.GetActionKey(actionB.Value.VocabIndex)} " +
                                  $"(type={actionB.Value.ChoiceType}, tera={actionB.Value.Terastallize})");
            }
        }

        return actionMapper.BuildChoice(actionA, actionB);
    }

    /// <summary>
    /// Team preview: uses the TeamPreviewNet model if available, otherwise falls back to random.
    /// The model outputs bring scores (which 4 Pokemon to bring) and lead scores (which 2 to lead).
    /// </summary>
    private Choice GetTeamPreviewChoice(TeamPreviewRequest request, Func<BattlePerspective> perspectiveFactory)
    {
        TeamPreviewInference? previewModel = MctsResources.TeamPreviewModel;
        if (previewModel == null)
            return GetRandomTeamPreviewChoice(request);

        BattlePerspective perspective = perspectiveFactory();
        TeamPreviewOutput output = previewModel.Evaluate(perspective);

        int teamSize = request.Side.Pokemon.Count;
        int bringCount = request.MaxChosenTeamSize ?? 4;

        VgcConfig config = TeamPreviewInference.VgcConfigs[output.ConfigIndex];

        if (PrintDebug)
        {
            Console.WriteLine($"[PlayerMcts] Team preview config #{output.ConfigIndex} for {SideId} ({output.Confidence:P1}):");
            Console.WriteLine($"  Leads: {string.Join(", ", config.Lead.Select(i => request.Side.Pokemon[i].Details))}");
            Console.WriteLine($"  Bench: {string.Join(", ", config.Bench.Select(i => request.Side.Pokemon[i].Details))}");
        }

        var ordered = output.OrderedIndices;

        // Build Choice: map each original index to its new position
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
        if (PrintDebug)
            Console.WriteLine($"[PlayerMcts] Generating random team preview choice for {SideId}");

        var pokemon = request.Side.Pokemon;
        int bringCount = request.MaxChosenTeamSize ?? pokemon.Count;
        var order = Enumerable.Range(0, pokemon.Count).ToList();

        // Fisher-Yates shuffle
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