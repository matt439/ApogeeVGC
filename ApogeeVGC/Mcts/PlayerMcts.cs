using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Player;
using ApogeeVGC.Sim.Utils;

using PlayerUiType = ApogeeVGC.Sim.Player.PlayerUiType;

namespace ApogeeVGC.Mcts;

public sealed class PlayerMcts : IPlayer
{
    public SideId SideId { get; }
    public PlayerOptions Options { get; }
    public PlayerUiType UiType => PlayerUiType.None;
    public IBattleController BattleController { get; }
    public bool PrintDebug { get; }

    private readonly MctsSearch _search;
    private readonly ActionMapper _actionMapper;
    private readonly Prng _rng;

    public event EventHandler<ChoiceRequestEventArgs>? ChoiceRequested;
    public event EventHandler<Choice>? ChoiceSubmitted;

    public PlayerMcts(
        SideId sideId,
        PlayerOptions options,
        IBattleController battleController,
        MctsSearch search,
        ActionMapper actionMapper)
    {
        SideId = sideId;
        Options = options;
        BattleController = battleController;
        PrintDebug = options.PrintDebug;
        _search = search;
        _actionMapper = actionMapper;
        _rng = options.Seed is null ? new Prng(null) : new Prng(options.Seed);
    }

    /// <summary>
    /// Factory method that creates a PlayerMcts using MctsResources.
    /// </summary>
    public static PlayerMcts Create(SideId sideId, PlayerOptions options, IBattleController battleController)
    {
        var actionMapper = new ActionMapper(MctsResources.Vocab);
        var search = new MctsSearch(MctsResources.Config, MctsResources.Model, actionMapper);
        return new PlayerMcts(sideId, options, battleController, search, actionMapper);
    }

    public Choice GetChoiceSync(IChoiceRequest choiceRequest, BattleRequestType requestType,
        Func<BattlePerspective> perspectiveFactory)
    {
        if (PrintDebug)
            Console.WriteLine($"[PlayerMcts.GetChoiceSync] Called for {SideId}");

        return choiceRequest switch
        {
            TeamPreviewRequest tpr => GetTeamPreviewChoice(tpr),
            MoveRequest or SwitchRequest => GetSearchChoice(choiceRequest, perspectiveFactory),
            _ => throw new NotImplementedException(
                $"Request type {choiceRequest.GetType().Name} not implemented for MCTS player"),
        };
    }

    public Task<Choice> GetNextChoiceAsync(IChoiceRequest choiceRequest, BattleRequestType requestType,
        BattlePerspective perspective, CancellationToken cancellationToken)
    {
        var choice = GetChoiceSync(choiceRequest, requestType, () => perspective);
        ChoiceSubmitted?.Invoke(this, choice);
        return Task.FromResult(choice);
    }

    public void UpdateUi(BattlePerspective perspective) { }
    public void UpdateEvents(IEnumerable<BattleEvent> events) { }
    public Task NotifyTimeoutWarningAsync(TimeSpan remainingTime) => Task.CompletedTask;
    public Task NotifyChoiceTimeoutAsync() => Task.CompletedTask;

    private Choice GetSearchChoice(IChoiceRequest request, Func<BattlePerspective> perspectiveFactory)
    {
        var perspective = perspectiveFactory();
        var (actionA, actionB) = _search.Search(request, perspective);

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

        return _actionMapper.BuildChoice(actionA, actionB);
    }

    /// <summary>
    /// Team preview: random ordering for now. Team preview model can be integrated later.
    /// </summary>
    private Choice GetTeamPreviewChoice(TeamPreviewRequest request)
    {
        if (PrintDebug)
            Console.WriteLine($"[PlayerMcts] Generating random team preview choice for {SideId}");

        var pokemon = request.Side.Pokemon;
        var order = Enumerable.Range(0, pokemon.Count).ToList();

        // Fisher-Yates shuffle
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
