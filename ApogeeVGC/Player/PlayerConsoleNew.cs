using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Ui;

namespace ApogeeVGC.Player;

public class PlayerConsoleNew(PlayerId playerId) : IPlayerNew
{
    public PlayerId PlayerId { get; } = playerId;

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
        ChoiceSubmitted?.Invoke(this, choice);
        return Task.FromResult(choice);
    }

    public event EventHandler<ChoiceRequestEventArgs>? ChoiceRequested;
    public event EventHandler<BattleChoice>? ChoiceSubmitted;

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