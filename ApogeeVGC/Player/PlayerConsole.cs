using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Ui;

namespace ApogeeVGC.Player;

public class PlayerConsole(PlayerId playerId, Battle battle) : IPlayer
{
    public PlayerId PlayerId { get; } = playerId;
    public Battle Battle { get; } = battle;

    public BattleChoice GetNextChoice(BattleChoice[] availableChoices)
    {
        if (Battle.IsTeamPreview) // check if in Team Preview phase
        {
            UiGenerator.PrintTeamPreviewUi(Battle.GetSide(PlayerId.OpposingPlayerId()));
        }
        else // In Battle phase
        {
            UiGenerator.PrintBattleUi(Battle, PlayerId);
        }

        UiGenerator.PrintChoices(availableChoices);
        BattleChoice choice = GetChoiceFromConsole(availableChoices);
        return choice;
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