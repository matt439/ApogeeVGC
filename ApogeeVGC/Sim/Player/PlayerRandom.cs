using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Player;

public class PlayerRandom(SideId sideId, PlayerOptions options, IBattleController battleController)
    : IPlayer
{
    public SideId SideId { get; } = sideId;
    public PlayerOptions Options { get; } = options;
    public PlayerUiType UiType => PlayerUiType.None;
    public IBattleController BattleController { get; } = battleController;
    public bool PrintDebug { get; } = options.PrintDebug;

    private readonly Prng _random = options.Seed is null ? new Prng(null) : new Prng(options.Seed);

    // Synchronous version for MCTS and fast simulations
    public Choice GetChoiceSync(IChoiceRequest choiceRequest, BattleRequestType requestType,
        BattlePerspective perspective)
    {
        if (PrintDebug)
        {
            Console.WriteLine($"[PlayerRandom.GetChoiceSync] Called for {SideId}");
        }

        // Return empty choice - battle will call AutoChoose on the Side
        var choice = new Choice
        {
            Actions = new List<ChosenAction>(),
            CantUndo = false,
            Error = string.Empty,
            ForcedSwitchesLeft = 0,
            ForcedPassesLeft = 0,
            SwitchIns = new HashSet<int>(),
            Terastallize = false,
        };

        if (PrintDebug)
        {
            Console.WriteLine(
                $"[PlayerRandom.GetChoiceSync] Returning empty choice for auto-selection");
        }

        return choice;
    }

    // Fast sync version for MCTS rollouts (IPlayer)
    public Choice GetNextChoiceSync(IChoiceRequest choice, BattlePerspective perspective)
    {
        return GetNextChoiceFromAll(choice);
    }

    // Simplified async version (IPlayer)
    public Task<Choice> GetNextChoiceAsync(IChoiceRequest choiceRequest,
        BattlePerspective perspective,
        CancellationToken cancellationToken)
    {
        Choice choice = GetNextChoiceFromAll(choiceRequest);
        ChoiceSubmitted?.Invoke(this, choice);
        return Task.FromResult(choice);
    }

    // Full async version for backward compatibility (IPlayer)
    public Task<Choice> GetNextChoiceAsync(IChoiceRequest choiceRequest,
        BattleRequestType requestType,
        BattlePerspective perspective, CancellationToken cancellationToken)
    {
        Choice choice = GetNextChoiceFromAll(choiceRequest);
        ChoiceRequested?.Invoke(this, new ChoiceRequestEventArgs
        {
            Choice = choiceRequest,
            TimeLimit = TimeSpan.FromSeconds(45),
            RequestTime = DateTime.UtcNow,
        });
        ChoiceSubmitted?.Invoke(this, choice);
        return Task.FromResult(choice);
    }

    public void UpdateUi(BattlePerspective perspective)
    {
        // Random player doesn't have a UI to update
    }

    public void UpdateMessages(IEnumerable<BattleMessage> messages)
    {
        // Random player doesn't need to receive messages
    }

    // Events from interfaces
    public event EventHandler<ChoiceRequestEventArgs>? ChoiceRequested;
    public event EventHandler<Choice>? ChoiceSubmitted;

    // Timeout methods from IPlayer
    public Task NotifyTimeoutWarningAsync(TimeSpan remainingTime)
    {
        // Random player doesn't need timeout warnings since it's automated
        return Task.CompletedTask;
    }

    public Task NotifyChoiceTimeoutAsync()
    {
        // Random player doesn't need timeout notifications since it's automated
        return Task.CompletedTask;
    }

    private Choice GetNextChoiceFromAll(IChoiceRequest request)
    {
        if (PrintDebug)
        {
            Console.WriteLine($"[PlayerRandom] GetNextChoiceFromAll called for {SideId}");
        }

        // Create an empty choice that will be auto-filled by the battle engine
        // The battle's Side.AutoChoose() method will fill in valid random choices
        var choice = new Choice
        {
            Actions = new List<ChosenAction>(),
            CantUndo = false,
            Error = string.Empty,
            ForcedSwitchesLeft = 0,
            ForcedPassesLeft = 0,
            SwitchIns = new HashSet<int>(),
            Terastallize = false,
        };

        // For now, return an empty choice which signals the battle to use Side.AutoChoose()
        // This is a valid pattern in Pokemon Showdown - empty choice = auto-choose
        if (PrintDebug)
        {
            Console.WriteLine($"[PlayerRandom] Returning empty choice for auto-selection");
        }

        return choice;
    }
}