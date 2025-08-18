using ApogeeVGC.Player;
using System.Globalization;
using System.Text;

namespace ApogeeVGC.Sim;

public static class UiGenerator
{
    public static void PrintBattleUi(Battle battle, PlayerId perspective)
    {
        switch (perspective)
        {
            case PlayerId.Player1:
                PrintTurnStart(battle.Turn);
                PrintSecondarySide(battle, PlayerId.Player2);
                PrintPrimarySide(battle, PlayerId.Player1);
                break;
            case PlayerId.Player2:
                PrintTurnStart(battle.Turn);
                PrintSecondarySide(battle, PlayerId.Player1);
                PrintPrimarySide(battle, PlayerId.Player2);
                break;
            case PlayerId.None:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(perspective), perspective, null);
        }
    }

    public static void PrintChoices(Battle battle, PlayerId perspective)
    {
        var availableChoices = battle.GetAvailableChoices(perspective);
        if (availableChoices.Length == 0)
        {
            Console.WriteLine("No available choices.");
            return;
        }
        StringBuilder sb = new();
        sb.AppendLine("Available choices:");
        for (int i = 0; i < availableChoices.Length; i++)
        {
            sb.AppendLine($"{i + 1}: {GenerateChoiceString(battle, perspective, availableChoices[i])}");
        }

        sb.Append("Please enter the number of your choice:");
        Console.WriteLine(sb.ToString());
    }

    public static void PrintMoveAction(Pokemon attacker, Move move, int damage, Pokemon defender)
    {
        StringBuilder sb = new();
        sb.AppendLine($"{attacker.Name} used {move.Name} on {defender.Name}.");
        sb.AppendLine($"It dealt {damage} damage.");
        Console.WriteLine(sb.ToString());
    }

    public static void PrintSwitchAction(string trainerName, Pokemon switchedOut, Pokemon switchedIn)
    {
        StringBuilder sb = new();
        sb.AppendLine($"{trainerName} withdrew {switchedOut.Name}. {trainerName} sent out {switchedIn.Name}!");
        Console.WriteLine(sb.ToString());
    }

    public static void PrintFaintedSelectAction(string trainerName, Pokemon pokemon)
    {
        StringBuilder sb = new();
        sb.AppendLine($"{trainerName} selected {pokemon.Name} to switch in.");
        Console.WriteLine(sb.ToString());
    }
    public static void PrintFaintedAction(Pokemon pokemon)
    {
        StringBuilder sb = new();
        sb.AppendLine($"{pokemon.Name} fainted!");
        Console.WriteLine(sb.ToString());
    }

    public static void PrintBattleEnd(string winnerName)
    {
        StringBuilder sb = new();
        sb.AppendLine("The battle has ended.");
        sb.AppendLine($"The winner is {winnerName}!");
        Console.WriteLine(sb.ToString());
    }

    private static string GenerateChoiceString(Battle battle, PlayerId perspective, Choice choice)
    {
        if (choice.IsMoveChoice())
        {
            return GenerateMoveChoiceString(battle, perspective, choice);
        }
        if (choice.IsSwitchChoice())
        {
            return GenerateSwitchChoiceString(battle, perspective, choice);
        }
        throw new ArgumentException("Invalid choice type.", nameof(choice));
    }

    private static string GenerateMoveChoiceString(Battle battle, PlayerId perspective, Choice choice)
    {
        Side side = battle.GetSide(perspective);
        int moveIndex = choice.GetMoveIndexFromChoice();
        if (moveIndex < 0 || moveIndex >= side.Team.ActivePokemon.Moves.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(choice), "Invalid move choice.");
        }
        Move move = side.Team.ActivePokemon.Moves[moveIndex];
        if (move == null)
        {
            throw new InvalidOperationException("Move choice cannot be made to a null Move.");
        }
        StringBuilder sb = new();
        sb.Append("Move: ");
        sb.Append(move.Name);
        return sb.ToString();
    }

    private static string GenerateSwitchChoiceString(Battle battle, PlayerId perspective, Choice choice)
    {
        Side side = battle.GetSide(perspective);
        int switchIndex = choice.GetSwitchIndexFromChoice();
        if (switchIndex < 0 || switchIndex >= side.Team.PokemonSet.PokemonCount)
        {
            throw new ArgumentOutOfRangeException(nameof(choice), "Invalid switch choice.");
        }
        Pokemon pokemon = side.Team.PokemonSet.Pokemons[switchIndex];
        if (pokemon == null)
        {
            throw new InvalidOperationException("Switch choice cannot be made to a null Pokemon.");
        }
        StringBuilder sb = new();
        sb.Append($"Switch: ");
        sb.Append($"{pokemon.Name} ({pokemon.Specie.Name}) ");
        return sb.ToString();
    }

    private const string LevelSpacer = "     ";
    private const string HpBarSpacer = "     ";
    private const int HpBarLength = 20;
    private const string PrimarySpacer = "                               ";

    private static void PrintPrimarySide(Battle battle, PlayerId perspective)
    {
        Side side = battle.GetSide(perspective);
        Pokemon activePokemon = side.Team.ActivePokemon;
        StringBuilder sb = new();
        sb.Append(PrimarySpacer);
        sb.Append($"{activePokemon.Name} ({activePokemon.Specie.Name}) ");
        sb.Append($"{activePokemon.Gender.GenderIdString()}");
        sb.Append(LevelSpacer);
        sb.AppendLine($"Lv{activePokemon.Level}");
        sb.Append(PrimarySpacer);
        sb.Append(HpBarSpacer);
        sb.Append(CreateHpBar(activePokemon));
        sb.AppendLine($" {activePokemon.CurrentHp.ToString()} / {activePokemon.UnmodifiedHp.ToString()}");
        sb.Append(PrimarySpacer);
        sb.AppendLine($"Remaining Pokemon: {side.Team.PokemonSet.AlivePokemonCount}\n\n");
        Console.WriteLine(sb.ToString());
    }

    private static void PrintSecondarySide(Battle battle, PlayerId perspective)
    {
        Side side = battle.GetSide(perspective);
        Pokemon activePokemon = side.Team.ActivePokemon;
        StringBuilder sb = new();
        sb.Append($"{activePokemon.Name} ({activePokemon.Specie.Name}) ");
        sb.Append($"{activePokemon.Gender.GenderIdString()}");
        sb.Append(LevelSpacer);
        sb.AppendLine($"Lv{activePokemon.Level}");
        sb.Append(HpBarSpacer);
        sb.Append(CreateHpBar(activePokemon));
        sb.Append(' ');
        sb.Append(activePokemon.CurrentHpPercentage.ToString());
        sb.AppendLine("%");
        sb.AppendLine($"Remaining Pokemon: {side.Team.PokemonSet.AlivePokemonCount}\n\n");
        Console.WriteLine(sb.ToString());
    }

    private static string CreateHpBar(Pokemon pokemon)
    {
        //int maxHp = pokemon.UnmodifiedStats.Hp;
        //int currentHp = pokemon.CurrentHp;
        int filledLength = (int)(pokemon.CurrentHpRatio * HpBarLength);
        string filledPart = new('█', filledLength);
        string emptyPart = new('░', HpBarLength - filledLength);
        return $"[{filledPart}{emptyPart}]";
    }

    private static void PrintTurnStart(int turn)
    {
        int turnNumber = turn + 1;
        string turnString = turnNumber.ToString(CultureInfo.InvariantCulture);
        Console.WriteLine($"####################  Turn {turnString}  ####################\n");
    }
}