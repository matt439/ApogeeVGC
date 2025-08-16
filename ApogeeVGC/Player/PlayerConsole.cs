using ApogeeVGC.Sim;

namespace ApogeeVGC.Player;

public class PlayerConsole(PlayerId playerId, Battle battle) : IPlayer
{
    public PlayerId PlayerId { get; } = playerId;
    public Battle Battle { get; } = battle;

    public Choice GetNextChoice(Choice[] availableChoices)
    {
        UiGenerator.PrintBattleUi(Battle, PlayerId);
        //PrintChoices(availableChoices);
        UiGenerator.PrintChoices(Battle, PlayerId);
        Choice choice = GetChoiceFromConsole(availableChoices);
        return choice;
    }

    //private static void PrintChoices(Choice[] availableChoices)
    //{
    //    string result = "Available choices:\n";
    //    for (int i = 0; i < availableChoices.Length; i++)
    //    {
    //        result += $"{i + 1}: {ChoiceTools.GetChoiceName(availableChoices[i])}\n";
    //    }
    //    result += "Please enter the number of your choice:";
    //    Console.WriteLine(result);
    //}

    private static Choice GetChoiceFromConsole(Choice[] availableChoices)
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