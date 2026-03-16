using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Player;
using ApogeeVGC.Sim.Utils;
using PlayerUiType = ApogeeVGC.Sim.Player.PlayerUiType;

namespace ApogeeVGC.Mcts;

/// <summary>
/// MCTS player with DL model policy priors and value evaluation, but no information tracking.
/// Uses full-depth tree search (like standalone) with neural network guidance (like full MCTS).
/// Intermediate variant: Standalone + DL models, without the information system.
/// </summary>
public sealed class PlayerMctsDL(
    SideId sideId,
    PlayerOptions options,
    IBattleController battleController,
    MctsSearchDL search,
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

    /// <summary>
    /// Factory method that creates a PlayerMctsDL using MctsResources.
    /// </summary>
    public static PlayerMctsDL Create(SideId sideId, PlayerOptions options, IBattleController battleController)
    {
        var actionMapper = new ActionMapper(MctsResources.Vocab);
        var search = new MctsSearchDL(
            options.MctsConfig ?? MctsResources.Config,
            MctsResources.Model,
            actionMapper);
        return new PlayerMctsDL(sideId, options, battleController, search, actionMapper);
    }

    public Choice GetChoiceSync(IChoiceRequest choiceRequest, BattleRequestType requestType,
        Func<BattlePerspective> perspectiveFactory)
    {
        return choiceRequest switch
        {
            TeamPreviewRequest tpr => GetTeamPreviewChoice(tpr, perspectiveFactory),
            MoveRequest or SwitchRequest => GetSearchChoice(choiceRequest, perspectiveFactory),
            _ => throw new NotImplementedException(
                $"Request type {choiceRequest.GetType().Name} not implemented for MCTS-DL player"),
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

    private Choice GetSearchChoice(IChoiceRequest request, Func<BattlePerspective> perspectiveFactory)
    {
        Battle battle = BattleController.Battle
                        ?? throw new InvalidOperationException("Battle is not initialized on the controller");

        // No info tracking — use raw perspective directly
        BattlePerspective perspective = perspectiveFactory();

        (LegalAction actionA, LegalAction? actionB) = search.Search(battle, SideId, request, perspective, verbose: PrintDebug);

        return actionMapper.BuildChoice(actionA, actionB);
    }

    /// <summary>
    /// Team preview: uses the TeamPreviewNet model if available, otherwise falls back to random.
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

        PreviewConfig config = previewModel.Configs[output.ConfigIndex];

        if (PrintDebug)
        {
            Console.WriteLine("  ┌─ Team Preview ─ DL Config Selection ───────────────────────");
            Console.WriteLine($"  │ My team ({teamSize} Pokemon, bringing {bringCount}):");
            Console.WriteLine($"  │   {"Pokemon",-20} {"Role",8}");
            Console.WriteLine($"  │   {"-------",-20} {"----",8}");
            HashSet<int> leadSet = new(config.Lead);
            HashSet<int> bringSet = new(config.Bring);
            for (int i = 0; i < teamSize; i++)
            {
                string name = perspective.PlayerSide.Pokemon[i].Name;
                string role = leadSet.Contains(i) ? "LEAD" : bringSet.Contains(i) ? "BENCH" : "-";
                Console.WriteLine($"  │   {name,-20} {role,8}");
            }
            Console.WriteLine($"  │ Opponent team:");
            for (int i = 0; i < perspective.OpponentSide.Pokemon.Count; i++)
            {
                string name = perspective.OpponentSide.Pokemon[i].Name;
                Console.WriteLine($"  │   {name,-20}");
            }
            Console.WriteLine($"  │ Confidence: {output.Confidence:P1} (config #{output.ConfigIndex})");
            Console.WriteLine($"  │ Decision:");
            Console.Write("  │   Leads: ");
            Console.WriteLine(string.Join(", ", config.Lead
                .Select(i => perspective.PlayerSide.Pokemon[i].Name)));
            Console.Write("  │   Backs: ");
            Console.WriteLine(string.Join(", ", config.Bench
                .Select(i => perspective.PlayerSide.Pokemon[i].Name)));
            Console.WriteLine("  └────────────────────────────────────────────────────────────");
            Console.WriteLine();
        }

        var ordered = output.OrderedIndices;

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
