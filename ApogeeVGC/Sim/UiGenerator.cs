using ApogeeVGC.Data;
using ApogeeVGC.Player;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

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
            sb.Append($"{i + 1}: ");
            sb.AppendLine(GenerateChoiceString(battle, perspective, availableChoices[i]));
        }

        sb.Append("Please enter the number of your choice:");
        Console.WriteLine(sb.ToString());
    }

    private const string Spacer1 = "==========";
    private const string Spacer2 = "====================";
    private const string TurnSpacer = "####################";

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

    public static void PrintMoveAction(Pokemon attacker, Move move, int damage, Pokemon defender,
        MoveEffectiveness effectiveness, bool isCrit = false)
    {
        StringBuilder sb = new();
        sb.AppendLine($"{attacker.Name} used {move.Name} on {defender.Name}.");
        if (isCrit)
        {
            sb.AppendLine("Critical hit!");
        }
        switch (effectiveness)
        {
            case MoveEffectiveness.Normal:
                sb.AppendLine("It was a normal hit.");
                break;
            case MoveEffectiveness.SuperEffective2X:
                sb.AppendLine("It's super effective!");
                break;
            case MoveEffectiveness.SuperEffective4X:
                sb.AppendLine("It was extremely super effective!");
                break;
            case MoveEffectiveness.NotVeryEffective05X:
                sb.AppendLine("It's not very effective.");
                break;
            case MoveEffectiveness.NotVeryEffective025X:
                sb.AppendLine("It was extremely not very effective.");
                break;
            case MoveEffectiveness.Immune:
                sb.AppendLine("It had no effect.");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(effectiveness),
                    effectiveness, "Invalid move effectiveness.");
        }
        sb.AppendLine($"It dealt {damage} damage.");
        Console.WriteLine(sb.ToString());
    }

    public static void PrintStruggleAction(Pokemon attacker, int damage,
        int recoil, Pokemon defender)
    {
        StringBuilder sb = new();
        sb.AppendLine($"{attacker.Name} used Struggle on {defender.Name}.");
        sb.AppendLine("It dealt " + damage + " damage.");
        sb.AppendLine(attacker.Name + " took " + recoil + " recoil damage.");
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

    public static void PrintMoveMissAction(Pokemon attacker, Move move, Pokemon defender)
    {
        StringBuilder sb = new();
        sb.AppendLine($"{attacker.Name} used {move.Name} on {defender.Name}.");
        sb.AppendLine("But it missed!");
        Console.WriteLine(sb.ToString());
    }

    public static void PrintBurnDamage(Pokemon target)
    {
        Console.WriteLine($"{target.Name} is hurt by its burn!");
    }

    public static void PrintBurnStart(Pokemon target)
    {
        Console.WriteLine($"{target.Name} was burned!");
    }

    public static void PrintBurnStartFromFlameOrb(Pokemon target)
    {
        Console.WriteLine($"{target.Name} was burned by its Flame Orb!");
    }

    public static void PrintBurnStartFromAbility(Pokemon target, Ability ability)
    {
        Console.WriteLine($"{target.Name} was burned by {ability.Name}!");
    }

    public static void PrintLeechSeedStart(Pokemon target)
    {
        Console.WriteLine($"{target.Name} was seeded!");
    }

    public static void PrintLeechSeedDamage(Pokemon target, Pokemon source, int damage)
    {
        Console.WriteLine($"{target.Name} is hurt by Leech Seed! {source.Name} restored {damage} HP.");
    }

    public static void PrintTrickRoomStart()
    {
        Console.WriteLine("The battlefield is twisted by Trick Room!");
    }

    public static void PrintTrickRoomRestart()
    {
        Console.WriteLine("The twisted dimensions returned to normal");
    }

    public static void PrintTrickRoomEnd()
    {
        Console.WriteLine("The effects of Trick Room wore off.");
    }

    private static string GenerateChoiceString(Battle battle, PlayerId perspective, Choice choice)
    {
        if (choice.IsSelectChoice())
        {
            return GenerateSelectChoiceString(battle, perspective, choice);
        }
        if (choice.IsMoveChoice())
        {
            return GenerateMoveChoiceString(battle, perspective, choice);
        }
        if (choice.IsSwitchChoice())
        {
            return GenerateSwitchChoiceString(battle, perspective, choice);
        }
        if (choice == Choice.Struggle)
        {
            return GenerateStruggleChoiceString();
        }
        throw new ArgumentException("Invalid choice type.", nameof(choice));
    }

    private static string GenerateSelectChoiceString(Battle battle, PlayerId perspective,
        Choice choice)
    {
        Side side = battle.GetSide(perspective);
        int selectIndex = choice.GetSelectIndexFromChoice();
        if (selectIndex < 0 || selectIndex >= side.Team.PokemonSet.PokemonCount)
        {
            throw new ArgumentOutOfRangeException(nameof(choice), "Invalid select choice.");
        }
        Pokemon pokemon = side.Team.PokemonSet.Pokemons[selectIndex];
        if (pokemon == null)
        {
            throw new InvalidOperationException("Select choice cannot be made to a null Pokemon.");
        }
        StringBuilder sb = new();
        sb.Append($"Select: ");
        sb.Append($"{pokemon.Name} ({pokemon.Specie.Name}) ");
        return sb.ToString();
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
        sb.Append(" (");
        sb.Append(move.Pp);
        sb.Append('/');
        sb.Append(move.MaxPp);
        sb.Append(')');
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

    private static string GenerateStruggleChoiceString()
    {
        return "Struggle";
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
        Console.WriteLine($"{TurnSpacer}  Turn {turnString}  {TurnSpacer}\n");
    }
}