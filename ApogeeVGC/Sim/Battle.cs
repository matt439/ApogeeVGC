using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using ApogeeVGC.Data;
using ApogeeVGC.Player;

namespace ApogeeVGC.Sim;

public class Battle
{
    public required Library Library { get; init; }
    public required Field Field { get; init; }
    public required Side Side1 { get; init; }
    public required Side Side2 { get; init; }
    public int Turn { get; private set; } = 0;

    private int Damage(Pokemon attacker, Pokemon defender, Move move, bool crit = false)
    {
        int level = attacker.Level;
        int attackStat = attacker.GetAttackStat(move);
        int defenseStat = defender.GetDefenseStat(move);
        int basePower = move.BasePower;
        double critModifier = crit ? 1.5 : 1.0;
        double random = 0.85 + new Random().NextDouble() * 0.15; // Random factor between 0.85 and 1.0
        bool stab = attacker.IsStab(move);
        double stabModifier = stab ? 1.5 : 1.0;
        double typeEffectiveness = Library.TypeChart.GetEffectiveness(defender.Specie.Types, move.Type);

        int baseDamage = (int)((2 * level / 5.0 + 2) * basePower * attackStat / defenseStat / 50.0 + 2);
        int critMofified = RoundedDownAtHalf(critModifier * baseDamage);
        int randomModified = RoundedDownAtHalf(random * critMofified);
        int stabModified = RoundedDownAtHalf(stabModifier * randomModified);
        int typeModified = RoundedDownAtHalf(typeEffectiveness * stabModified);
        return Math.Max(1, typeModified);
    }

    public void ApplyChoices(Choice player1Choice, Choice player2Choice)
    {
        PlayerId nextPlayer = MovesNext(player1Choice, player2Choice);
        if (nextPlayer == PlayerId.Player1)
        {
            ApplyChoice(player1Choice, Side1, Side2);
            if (IsWinner() != PlayerId.None) return;
            ApplyChoice(player2Choice, Side2, Side1);
        }
        else
        {
            ApplyChoice(player2Choice, Side2, Side1);
            if (IsWinner() != PlayerId.None) return;
            ApplyChoice(player1Choice, Side1, Side2);
        }
        Turn++;
    }

    public Choice[] GetAvailableChoices(PlayerId playerId)
    {
        return playerId switch
        {
            PlayerId.Player1 => GetAvailableChoices(Side1),
            PlayerId.Player2 => GetAvailableChoices(Side2),
            _ => throw new ArgumentException("Invalid player ID", nameof(playerId))
        };
    }

    public PlayerId IsWinner()
    {
        if (Side1.Team.IsDefeated)
        {
            return PlayerId.Player1;
        }
        if (Side2.Team.IsDefeated)
        {
            return PlayerId.Player2;
        }
        return PlayerId.None;
    }

    public Side GetSide(PlayerId playerId)
    {
        return playerId switch
        {
            PlayerId.Player1 => Side1,
            PlayerId.Player2 => Side2,
            _ => throw new ArgumentException("Invalid player ID", nameof(playerId))
        };
    }

    private PlayerId MovesNext(Choice player1Choice, Choice player2Choice)
    {
        int player1Priority = Priority(player1Choice, Side1);
        int player2Priority = Priority(player2Choice, Side2);

        if (player1Priority > player2Priority)
        {
            return PlayerId.Player1;
        }
        if (player2Priority > player1Priority)
        {
            return PlayerId.Player2;
        }
        // If priorities are equal, use the speed of the active Pokémon to determine who moves first
        Pokemon player1Pokemon = Side1.Team.ActivePokemon;
        Pokemon player2Pokemon = Side2.Team.ActivePokemon;
        if (player1Pokemon.CurrentSpe > player2Pokemon.CurrentSpe)
        {
            return PlayerId.Player1;
        }
        if (player2Pokemon.CurrentSpe > player1Pokemon.CurrentSpe)
        {
            return PlayerId.Player2;
        }
        // Speed ties are resolved randomly
        return new Random().Next(2) == 0 ? PlayerId.Player1 : PlayerId.Player2;
    }

    private static int Priority(Choice choice, Side side)
    {
        if (choice.IsSwitchChoice())
        {
            return 6;
        }
        if (choice.IsMoveChoice())
        {
            int moveIndex = choice.GetMoveIndexFromChoice();
            Pokemon attacker = side.Team.ActivePokemon;
            Move move = attacker.Moves[moveIndex];
            return move.Priority;
        }
        return 0; // Default priority for other choices
    }

    private void ApplyChoice(Choice choice, Side atkSide, Side defSide)
    {
        if (choice.IsSwitchChoice())
        {
            atkSide.Team.ActivePokemonIndex = choice.GetSwitchIndexFromChoice();
        }
        else if (choice.IsMoveChoice())
        {
            int moveIndex = choice.GetMoveIndexFromChoice();
            Pokemon attacker = atkSide.Team.ActivePokemon;
            Move move = attacker.Moves[moveIndex];
            Pokemon defender = defSide.Team.ActivePokemon;

        }
    }

    private static Choice[] GetAvailableChoices(Side side)
    {
        // TODO: Implement logic to determine available choices based on the current state of the battle.

        List<Choice> choices =
        [
            Choice.Move1,
            Choice.Move2,
            Choice.Move3,
            Choice.Move4,
        ];

        int[] switchablePokemon = side.Team.SwitchOptionIndexes;

        if (switchablePokemon.Length > 0)
        {
            choices.AddRange(switchablePokemon.Select(ChoiceTools.GetChoiceFromSwitchIndex));
        }

        return choices.ToArray();
    }

    private static int RoundedDownAtHalf(double value)
    {
        return (int)(value + 0.5 - double.Epsilon);
    }
}

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
        foreach (Choice t in availableChoices)
        {
            sb.AppendLine(GenerateChoiceString(battle, perspective, t));
        }
        sb.Append("Please enter the number of your choice:");
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
        sb.Append($"{choice.GetChoiceName()}: ");
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
        sb.Append($"{choice.GetChoiceName()}: ");
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
        sb.AppendLine($" {activePokemon.CurrentHp.ToString()} / {activePokemon.UnmodifiedStats.Hp.ToString()}");
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
        int maxHp = pokemon.UnmodifiedStats.Hp;
        int currentHp = pokemon.CurrentHp;
        int filledLength = (int)((double)currentHp / maxHp * HpBarLength);
        string filledPart = new('█', filledLength);
        string emptyPart = new('░', HpBarLength - filledLength);
        return $"[{filledPart}{emptyPart}]";
    }

    private static void PrintTurnStart(int turn)
    {
        int turnNumber = turn + 1;
        string turnString = turnNumber.ToString(CultureInfo.InvariantCulture);
        Console.WriteLine($"##########  Turn {turnString}  ##########\n");
    }
}

public static class BattleGenerator
{
    public static Battle GenerateTestBattle(Library library)
    {
        return new Battle
        {
            Library = library,
            Field = new Field(),
            Side1 = SideGenerator.GenerateTestSide(library),
            Side2 = SideGenerator.GenerateTestSide(library)
        };
    }
}