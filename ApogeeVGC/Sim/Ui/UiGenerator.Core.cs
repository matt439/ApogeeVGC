using ApogeeVGC.Player;
using ApogeeVGC.Sim.FieldClasses;
using System.Globalization;
using System.Text;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Ui;

public static partial class UiGenerator
{
    // Constants used across all UI components
    private const string Spacer1 = "==========";
    private const string Spacer2 = "====================";
    private const string TurnSpacer = "####################";
    private const string LevelSpacer = "     ";
    private const string HpBarSpacer = "     ";
    private const int HpBarLength = 20;
    private const string PrimarySpacer = "                               ";

    public static void PrintBattleUi(Battle battle, PlayerId perspective)
    {
        switch (perspective)
        {
            case PlayerId.Player1:
                PrintTurnStart(battle.Turn);
                PrintSecondarySide(battle.GetSide(PlayerId.Player2));
                PrintPrimarySide(battle.GetSide(PlayerId.Player1));
                PrintField(battle.Field);
                break;
            case PlayerId.Player2:
                PrintTurnStart(battle.Turn);
                PrintSecondarySide(battle.GetSide(PlayerId.Player1));
                PrintPrimarySide(battle.GetSide(PlayerId.Player2));
                PrintField(battle.Field);
                break;
            case PlayerId.None:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(perspective), perspective, null);
        }
    }

    public static void PrintBattleUiNew(BattlePerspective perspective)
    {
        PrintTurnStart(perspective.TurnCounter);
        PrintSecondarySide(perspective.OpponentSide);
        PrintPrimarySide(perspective.PlayerSide);
        PrintField(perspective.Field);
    }

    public static void PrintChoices(BattleChoice[] availableChoices)
    {
        if (availableChoices.Length == 0)
        {
            Console.WriteLine("No available choices.");
            return;
        }

        StringBuilder sb = new();
        sb.AppendLine("Available choices:");
        for (int i = 0; i < availableChoices.Length; i++)
        {
            sb.Append($"{i + 1}: ");

            switch (availableChoices[i])
            {
                case TeamPreviewChoice teamPreviewChoice:
                    sb.AppendLine(GenerateTeamPreviewChoiceString(teamPreviewChoice));
                    break;
                case SlotChoice slotChoice:
                    sb.AppendLine(GenerateSlotChoiceString(slotChoice));
                    break;
                default:
                    throw new InvalidOperationException($"Unknown choice type: {availableChoices[i].GetType()}");
            }
            
        }

        sb.Append("Please enter the number of your choice:");
        Console.WriteLine(sb.ToString());
    }

    public static void PrintTeamPreviewUi(Side opponentsSide)
    {
        StringBuilder sb = new();
        sb.AppendLine($"{Spacer1}  Team Preview Phase. Please select your lead Pokemon.  {Spacer1}");
        // print opponent's team
        sb.AppendLine("Opponent's Team:");
        for (int i = 0; i < opponentsSide.Team.PokemonSet.PokemonCount; i++)
        {
            Pokemon pokemon = opponentsSide.Team.PokemonSet.Pokemons[i];
            sb.Append($"{i + 1}: {pokemon.Specie.Name}");
            sb.Append($" (Lv {pokemon.Level})");
            if (pokemon.Item != null) // check for item
            {
                sb.Append($" @ {pokemon.Item.Name}");
            }

            sb.AppendLine();
        }

        sb.AppendLine($"{Spacer2}");
        Console.WriteLine(sb.ToString());
    }

    public static void PrintBattleEnd(string winnerName)
    {
        StringBuilder sb = new();
        sb.AppendLine("The battle has ended.");
        sb.AppendLine($"The winner is {winnerName}!");
        Console.WriteLine(sb.ToString());
    }

    public static void PrintBlankLine()
    {
        Console.WriteLine();
    }

    public static void PrintFieldElementCounter(FieldElement element)
    {
        Console.WriteLine($"{element.Name} has {element.RemainingTurns} turns remaining.");
    }

    private static void PrintTurnStart(int turn)
    {
        string turnString = turn.ToString(CultureInfo.InvariantCulture);
        Console.WriteLine($"\n{TurnSpacer}  Turn {turnString}  {TurnSpacer}\n");
    }
}