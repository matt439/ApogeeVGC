using ApogeeVGC.Player;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Stats;
using System.Text;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Sim.Ui;

public static partial class UiGenerator
{
    // Item and Ability Effects
    public static void PrintRockyHelmetDamage(Pokemon attacker, int damage, Pokemon defender)
    {
        Console.WriteLine($"{attacker.Name} took {damage} damage from {defender.Name}'s rocky helmet.");
    }

    public static void PrintLeftoversHeal(Pokemon target, int actualHeal)
    {
        Console.WriteLine($"{target.Name} restored {actualHeal} HP using its Leftovers!");
    }

    public static void PrintChillingNeighActivation(Pokemon pokemon)
    {
        Console.WriteLine($"{pokemon.Name}'s Chilling Neigh ability activated!");
    }

    public static void PrintTeraStart(Pokemon pokemon)
    {
        Console.WriteLine($"{pokemon.Name} has Terastallized into {pokemon.TeraType.ConvertToString()} type!");
    }

    public static void PrintFlameBodyBurn(Pokemon source, Pokemon target)
    {
        Console.WriteLine($"{target.Name} was burned by {source.Name}'s Flame Body!");
    }

    public static void PrintQuarkDriveEnd(Pokemon pokemon)
    {
        Console.WriteLine($"{pokemon.Name}'s Quark Drive wore off.");
    }

    public static void PrintQuarkDriveStart(Pokemon pokemon, StatIdExceptHp stat)
    {
        Console.WriteLine($"{pokemon.Name}'s Quark Drive activated!");
        Console.WriteLine($"{pokemon.Name}'s {stat.ConvertToString()} was boosted.");
    }

    // Choice Generation Methods
    private static string GenerateChoiceString(Core.Battle battle, PlayerId perspective, Choice choice)
    {
        if (choice.IsSelectChoice())
        {
            return GenerateSelectChoiceString(battle, perspective, choice);
        }

        if (choice.IsMoveChoice())
        {
            return GenerateMoveChoiceString(battle, perspective, choice);
        }

        if (choice.IsMoveWithTeraChoice())
        {
            return GenerateMoveWithTeraChoiceString(battle, perspective, choice);
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

    private static string GenerateSelectChoiceString(Core.Battle battle, PlayerId perspective, Choice choice)
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

    private static string GenerateMoveChoiceString(Core.Battle battle, PlayerId perspective, Choice choice)
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

    private static string GenerateMoveWithTeraChoiceString(Core.Battle battle, PlayerId perspective, Choice choice)
    {
        Side side = battle.GetSide(perspective);
        int moveIndex = choice.GetMoveWithTeraIndexFromChoice();
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
        sb.Append(" + ");
        sb.Append(side.Team.ActivePokemon.TeraType.ConvertToString());
        sb.Append(" Tera");
        return sb.ToString();
    }

    private static string GenerateSwitchChoiceString(Core.Battle battle, PlayerId perspective, Choice choice)
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

    // Pokemon Display Methods
    private static void PrintPrimarySide(Core.Battle battle, PlayerId perspective)
    {
        Side side = battle.GetSide(perspective);
        Pokemon activePokemon = side.Team.ActivePokemon;

        string pokemonInfo = FormatPokemonBasicInfo(activePokemon);
        string hpDisplay = FormatHpDisplay(activePokemon, showExactHp: true);
        string conditionsInfo = FormatConditionsInfo(activePokemon);
        string stats = SingleLineStats(activePokemon);
        string statMods = SingleLineStatModifiers(activePokemon);
        string remainingInfo = FormatRemainingPokemonInfo(side);

        StringBuilder sb = new();
        sb.Append(PrimarySpacer);
        sb.AppendLine(pokemonInfo);
        sb.Append(PrimarySpacer);
        sb.Append(HpBarSpacer);
        sb.AppendLine(hpDisplay);
        if (activePokemon.IsTeraUsed)
        {
            sb.Append(PrimarySpacer);
            sb.AppendLine(FormatTeraInfo(activePokemon));
        }
        sb.Append(PrimarySpacer);
        sb.AppendLine(conditionsInfo);
        sb.Append(PrimarySpacer);
        sb.AppendLine(stats);
        sb.Append(PrimarySpacer);
        sb.AppendLine(statMods);
        sb.Append(PrimarySpacer);
        sb.AppendLine(remainingInfo);

        Console.WriteLine(sb.ToString());
    }

    private static void PrintSecondarySide(Core.Battle battle, PlayerId perspective)
    {
        Side side = battle.GetSide(perspective);
        Pokemon activePokemon = side.Team.ActivePokemon;

        string pokemonInfo = FormatPokemonBasicInfo(activePokemon);
        string hpDisplay = FormatHpDisplay(activePokemon, showExactHp: false);
        string conditionsInfo = FormatConditionsInfo(activePokemon);
        string remainingInfo = FormatRemainingPokemonInfo(side);

        StringBuilder sb = new();
        sb.AppendLine(pokemonInfo);
        sb.Append(HpBarSpacer);
        sb.AppendLine(hpDisplay);
        if (activePokemon.IsTeraUsed)
        {
            sb.AppendLine(FormatTeraInfo(activePokemon));
        }
        sb.AppendLine(conditionsInfo);
        sb.AppendLine($"{remainingInfo}\n\n");

        Console.WriteLine(sb.ToString());
    }

    // Formatting Helper Methods
    private static string SingleLineStats(Pokemon pokemon)
    {
        StringBuilder sb = new();
        sb.Append("Atk:");
        sb.Append(pokemon.CurrentAtk);
        sb.Append(", Def:");
        sb.Append(pokemon.CurrentDef);
        sb.Append(", SpA:");
        sb.Append(pokemon.CurrentSpA);
        sb.Append(", SpD:");
        sb.Append(pokemon.CurrentSpD);
        sb.Append(", Spe:");
        sb.Append(pokemon.CurrentSpe);
        return sb.ToString();
    }

    private static string SingleLineStatModifiers(Pokemon pokemon)
    {
        StringBuilder sb = new();
        sb.Append("Atk:");
        sb.Append(pokemon.StatModifiers.Atk);
        sb.Append(", Def:");
        sb.Append(pokemon.StatModifiers.Def);
        sb.Append(", SpA:");
        sb.Append(pokemon.StatModifiers.SpA);
        sb.Append(", SpD:");
        sb.Append(pokemon.StatModifiers.SpD);
        sb.Append(", Spe:");
        sb.Append(pokemon.StatModifiers.Spe);
        sb.Append(", Acc:");
        sb.Append(pokemon.StatModifiers.Accuracy);
        sb.Append(", Eva:");
        sb.Append(pokemon.StatModifiers.Evasion);
        return sb.ToString();
    }

    private static string FormatPokemonBasicInfo(Pokemon activePokemon)
    {
        return $"{activePokemon.Name} ({activePokemon.Specie.Name}) " +
               $"{activePokemon.Gender.GenderIdString()}{LevelSpacer}Lv{activePokemon.Level}";
    }

    private static string FormatHpDisplay(Pokemon activePokemon, bool showExactHp)
    {
        string hpBar = CreateHpBar(activePokemon);

        return showExactHp ?
            $"{hpBar} {activePokemon.CurrentHp} / {activePokemon.UnmodifiedHp}" :
            $"{hpBar} {activePokemon.CurrentHpPercentage}%";
    }

    private static string FormatTeraInfo(Pokemon activePokemon)
    {
        return $"Tera Type: {activePokemon.TeraType.ConvertToString()}";
    }

    private static string FormatConditionsInfo(Pokemon activePokemon)
    {
        StringBuilder sb = new();
        sb.Append("Conditions: ");

        foreach (Condition condition in activePokemon.Conditions)
        {
            sb.Append(condition.Name);
            if (condition.Duration.HasValue)
            {
                sb.Append($"({condition.Duration} turns)");
            }
            if (condition.Counter.HasValue)
            {
                sb.Append($"[Count: {condition.Counter}]");
            }
            sb.Append(", ");
        }

        return sb.ToString();
    }

    private static string FormatRemainingPokemonInfo(Side side)
    {
        return $"Remaining Pokemon: {side.Team.PokemonSet.AlivePokemonCount}";
    }

    private static string CreateHpBar(Pokemon pokemon)
    {
        int filledLength = pokemon.CurrentHp == 0
            ? 0
            : Math.Max(1, (int)(pokemon.CurrentHpRatio * HpBarLength));
        string filledPart = new('█', filledLength);
        string emptyPart = new('░', HpBarLength - filledLength);
        return $"[{filledPart}{emptyPart}]";
    }

    private static void PrintField(Core.Battle battle)
    {
        Field field = battle.Field;

        StringBuilder sb = new();
        sb.AppendLine("****** Field State ******");
        sb.Append("Weather: ");
        if (field.HasAnyWeather)
        {
            sb.Append($"{field.Weather!.Name} ({field.Weather.RemainingTurns} turns remaining)");
        }
        else
        {
            sb.Append("None");
        }
        sb.AppendLine();

        sb.Append("Terrain: ");
        if (field.HasAnyTerrain)
        {
            sb.Append($"{field.Terrain!.Name} ({field.Terrain.RemainingTurns} turns remaining)");
        }
        else
        {
            sb.Append("None");
        }
        sb.AppendLine();

        if (!field.HasAnyPseudoWeather)
        {
            sb.AppendLine("Pseudo-Weathers: None");
        }
        else
        {
            sb.AppendLine("Pseudo-Weathers:");
            foreach (PseudoWeather pseudoWeather in field.PseudoWeatherList)
            {
                sb.AppendLine($"{pseudoWeather.Name} ({pseudoWeather.RemainingTurns} turns remaining)");
            }
        }

        sb.Append("Player 1 Side Conditions: ");
        if (!field.HasAnySide1Conditions)
        {
            sb.Append("None");
        }
        else
        {
            foreach (SideCondition sideCondition in field.Side1Conditions)
            {
                sb.Append($"{sideCondition.Name} ({sideCondition.RemainingTurns} turns remaining), ");
            }
        }
        sb.AppendLine();

        sb.Append("Player 2 Side Conditions: ");
        if (!field.HasAnySide2Conditions)
        {
            sb.Append("None");
        }
        else
        {
            foreach (SideCondition sideCondition in field.Side2Conditions)
            {
                sb.Append($"{sideCondition.Name} ({sideCondition.RemainingTurns} turns remaining), ");
            }
        }

        sb.AppendLine();
        Console.WriteLine(sb.ToString());
    }
}