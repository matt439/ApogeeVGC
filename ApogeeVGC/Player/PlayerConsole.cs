using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Ui;

namespace ApogeeVGC.Player;

public class PlayerConsole(PlayerId playerId) : IPlayer
{
    public PlayerId PlayerId { get; } = playerId;

    // Sync version not supported for human players - they need async interaction (IPlayer)
    public BattleChoice GetNextChoiceSync(BattleChoice[] choices, BattlePerspective perspective)
    {
        throw new NotSupportedException("Console players require async interaction and cannot be used in synchronous contexts like MCTS rollouts.");
    }

    // Simplified async version (IPlayer)
    public Task<BattleChoice> GetNextChoiceAsync(BattleChoice[] choices, BattlePerspective perspective,  
        CancellationToken cancellationToken)
    {
        // Default to TurnStart behavior since we don't have BattleRequestType parameter
        UiGenerator.PrintBattleUiNew(perspective);
        UiGenerator.PrintChoices(choices);

        BattleChoice choice = GetChoiceFromConsole(choices);
        ChoiceSubmitted?.Invoke(this, choice);
        return Task.FromResult(choice);
    }

    // Full async version for backward compatibility (IPlayerNew)
    public Task<BattleChoice> GetNextChoiceAsync(BattleChoice[] availableChoices, BattleRequestType requestType,
        BattlePerspective perspective, CancellationToken cancellationToken)
    {
        switch (requestType)
        {
            case BattleRequestType.TurnStart:
            case BattleRequestType.ForceSwitch:
            case BattleRequestType.FaintSwitch:
                UiGenerator.PrintBattleUiNew(perspective);
                break;
            case BattleRequestType.TeamPreview:
                UiGenerator.PrintTeamPreviewUi(perspective.OpponentSide);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(requestType), requestType, null);
        }

        UiGenerator.PrintChoices(availableChoices);

        BattleChoice choice = GetChoiceFromConsole(availableChoices);
        //ChoiceRequested?.Invoke(this, new ChoiceRequestEventArgs
        //{
        //    AvailableChoices = availableChoices,
        //    TimeLimit = TimeSpan.FromSeconds(45),
        //    RequestTime = DateTime.UtcNow,
        //});
        ChoiceSubmitted?.Invoke(this, choice);
        return Task.FromResult(choice);
    }

    // Events from interfaces
    //public event EventHandler<ChoiceRequestEventArgs>? ChoiceRequested;
    public event EventHandler<BattleChoice>? ChoiceSubmitted;

    // Timeout methods from IPlayerNew
    public Task NotifyTimeoutWarningAsync(TimeSpan remainingTime)
    {
        // Show a warning message to the console user
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"⚠️  WARNING: You have {remainingTime.TotalSeconds:F0}" +
                          $"seconds remaining to make your choice!");
        Console.ResetColor();
        Console.Write("> ");

        return Task.CompletedTask;
    }

    public Task NotifyChoiceTimeoutAsync()
    {
        // Notify the user that they timed out
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("⏰ Time's up! A default choice has been selected for you.");
        Console.ResetColor();
        Console.WriteLine();

        return Task.CompletedTask;
    }

    private static BattleChoice GetChoiceFromConsole(BattleChoice[] availableChoices)
    {
        while (true)
        {
            Console.Write("> ");
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int choiceIndex) && choiceIndex > 0 &&
                choiceIndex <= availableChoices.Length)
            {
                Console.Clear();
                return availableChoices[choiceIndex - 1];
            }
            Console.WriteLine("Invalid choice. Please try again:");
        }
    }
}