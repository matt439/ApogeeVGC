using ApogeeVGC.Player;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Core;
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
    private static string GenerateChoiceString(Battle battle, PlayerId perspective, Choice choice)
    {
        if (choice.IsTeamPreviewChoice())
        {
            return GenerateTeamPreviewChoiceString(battle, perspective, choice);
        }

        StringBuilder sb = new();
        var (slot1Choice, slot2Choice) = choice.DecodeDoublesChoice();
        Choice?[] slotChoices = [slot1Choice, slot2Choice];

        int i = 1;
        foreach (var slotChoice in slotChoices)
        {
            if (slotChoice is null)
            {
                i++;
                continue;
            }

            var ch = (Choice)slotChoice;

            if (!ch.IsMoveChoice() && !ch.IsSwitchChoice() &&
                ch != Choice.Struggle)
            {
                throw new ArgumentException("Invalid choice type in doubles choice.", nameof(ch));
            }

            if (ch.IsMoveChoice())
            {
                sb.Append(GenerateMoveChoiceString(battle, perspective, ch, (SlotId)i));
            }
            else if (ch.IsSwitchChoice())
            {
                sb.Append(GenerateSwitchChoiceString(battle, perspective, ch));
            }
            else if (ch == Choice.Struggle)
            {
                sb.Append(GenerateStruggleChoiceString());
            }
            i++;
        }
        return sb.ToString();

        //if (choice.IsMoveChoice())
        //{
        //    if (slot1Choice != null)
        //    {
        //        sb.Append(GenerateMoveChoiceString(battle, perspective, choice, 0));
        //    }
        //    if (slot2Choice == null) return sb.ToString();

        //    if (slot1Choice != null)
        //    {
        //        sb.Append(" | ");
        //    }
        //    sb.Append(GenerateMoveChoiceString(battle, perspective, choice, 1));
        //    return sb.ToString();
        //}

        //if (choice.IsSwitchChoice())
        //{
        //    if (slo1)

        //    return GenerateSwitchChoiceString(battle, perspective, choice);
        //}

        //if (choice == Choice.Struggle)
        //{
        //    return GenerateStruggleChoiceString();
        //}

        //throw new ArgumentException("Invalid choice type.", nameof(choice));
    }

    private static string GenerateTeamPreviewChoiceString(Battle battle, PlayerId perspective, Choice choice)
    {
        (int slot1Index, int slot2Index, int slot3Index, int slot4Index) = choice.DecodeTeamPreviewChoice();

        if (slot1Index < 1 || slot1Index > battle.GetSide(perspective).Team.PokemonSet.PokemonCount ||
            slot2Index < 1 || slot2Index > battle.GetSide(perspective).Team.PokemonSet.PokemonCount ||
            slot3Index < 1 || slot3Index > battle.GetSide(perspective).Team.PokemonSet.PokemonCount ||
            slot4Index < 1 || slot4Index > battle.GetSide(perspective).Team.PokemonSet.PokemonCount)
        {
            throw new ArgumentOutOfRangeException(nameof(choice), "Invalid team preview choice.");
        }

        var selections = new[] { slot1Index, slot2Index, slot3Index, slot4Index };
        if (selections.Distinct().Count() != 4)
        {
            throw new ArgumentOutOfRangeException(nameof(choice), "Team preview choice must have 4 unique Pokémon.");
        }

        Side side = battle.GetSide(perspective);

        Pokemon slot1Pokemon = side.Team.PokemonSet.Pokemons[slot1Index - 1];
        Pokemon slot2Pokemon = side.Team.PokemonSet.Pokemons[slot2Index - 1];
        Pokemon slot3Pokemon = side.Team.PokemonSet.Pokemons[slot3Index - 1];
        Pokemon slot4Pokemon = side.Team.PokemonSet.Pokemons[slot4Index - 1];

        if (slot1Pokemon == null || slot2Pokemon == null || slot3Pokemon == null || slot4Pokemon == null)
        {
            throw new InvalidOperationException("Select choice cannot be made to a null Pokemon.");
        }

        StringBuilder sb = new();
        sb.Append($"Select: ");
        sb.Append($"1:{slot1Pokemon.Name} ({slot1Pokemon.Specie.Name}) - ");
        sb.Append($"2:{slot2Pokemon.Name} ({slot2Pokemon.Specie.Name}) - ");
        sb.Append($"3:{slot3Pokemon.Name} ({slot3Pokemon.Specie.Name}) - ");
        sb.Append($"4:{slot4Pokemon.Name} ({slot4Pokemon.Specie.Name})");
        return sb.ToString();
    }

    private static string GenerateMoveChoiceString(Battle battle, PlayerId perspective, Choice choice, SlotId slot)
    {
        Side side = battle.GetSide(perspective);

        Pokemon? activePokemon = slot == SlotId.Slot1
            ? side.Team.Slot1Pokemon
            : side.Team.Slot2Pokemon;
        if (activePokemon == null)
        {
            throw new InvalidOperationException("Move choice cannot be made when the active Pokemon is null.");
        }

        int moveIndex = choice.GetMoveNumber();
        if (moveIndex < 0 || moveIndex >= activePokemon.Moves.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(choice), "Invalid move choice.");
        }

        Move move = activePokemon.Moves[moveIndex];
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

    //private static string GenerateMoveWithTeraChoiceString(Battle battle, PlayerId perspective, Choice choice)
    //{
    //    Side side = battle.GetSide(perspective);
    //    int moveIndex = choice.GetMoveNumber();
    //    if (moveIndex < 0 || moveIndex >= side.Team.ActivePokemon.Moves.Length)
    //    {
    //        throw new ArgumentOutOfRangeException(nameof(choice), "Invalid move choice.");
    //    }

    //    Move move = side.Team.ActivePokemon.Moves[moveIndex];
    //    if (move == null)
    //    {
    //        throw new InvalidOperationException("Move choice cannot be made to a null Move.");
    //    }

    //    StringBuilder sb = new();
    //    sb.Append("Move: ");
    //    sb.Append(move.Name);
    //    sb.Append(" (");
    //    sb.Append(move.Pp);
    //    sb.Append('/');
    //    sb.Append(move.MaxPp);
    //    sb.Append(')');
    //    sb.Append(" + ");
    //    sb.Append(side.Team.ActivePokemon.TeraType.ConvertToString());
    //    sb.Append(" Tera");
    //    return sb.ToString();
    //}

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

    // Pokemon Display Methods
    private static void PrintPrimarySide(Battle battle, PlayerId perspective)
    {
        Side side = battle.GetSide(perspective);
        string remainingInfo = FormatRemainingPokemonInfo(side);

        StringBuilder sb = new();
        sb.Append(FormatPrimarySlot1(battle, perspective));
        sb.Append(FormatPrimarySlot2(battle, perspective));
        sb.Append(PrimarySpacer);
        sb.AppendLine(remainingInfo);
        Console.WriteLine(sb.ToString());
    }

    private static string FormatPrimarySlot1(Battle battle, PlayerId perspective)
    {
        Side side = battle.GetSide(perspective);
        Pokemon? activePokemon = side.Team.Slot1Pokemon;

        if (activePokemon == null) return string.Empty;

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

        return sb.ToString();
    }

    private static string FormatPrimarySlot2(Battle battle, PlayerId perspective)
    {
        Side side = battle.GetSide(perspective);
        Pokemon? activePokemon = side.Team.Slot2Pokemon;

        if (activePokemon == null) return string.Empty;

        string pokemonInfo = FormatPokemonBasicInfo(activePokemon);
        string hpDisplay = FormatHpDisplay(activePokemon, showExactHp: true);
        string conditionsInfo = FormatConditionsInfo(activePokemon);
        string stats = SingleLineStats(activePokemon);
        string statMods = SingleLineStatModifiers(activePokemon);
        string remainingInfo = FormatRemainingPokemonInfo(side);

        StringBuilder sb = new();
        sb.Append(PrimarySpacer);
        sb.Append(PrimarySlot2Spacer);
        sb.AppendLine(pokemonInfo);
        sb.Append(PrimarySpacer);
        sb.Append(PrimarySlot2Spacer);
        sb.Append(HpBarSpacer);
        sb.AppendLine(hpDisplay);
        if (activePokemon.IsTeraUsed)
        {
            sb.Append(PrimarySpacer);
            sb.Append(PrimarySlot2Spacer);
            sb.AppendLine(FormatTeraInfo(activePokemon));
        }
        sb.Append(PrimarySpacer);
        sb.Append(PrimarySlot2Spacer);
        sb.AppendLine(conditionsInfo);
        sb.Append(PrimarySpacer);
        sb.Append(PrimarySlot2Spacer);
        sb.AppendLine(stats);
        sb.Append(PrimarySpacer);
        sb.Append(PrimarySlot2Spacer);
        sb.AppendLine(statMods);
        sb.Append(PrimarySpacer);
        sb.Append(PrimarySlot2Spacer);
        sb.AppendLine(remainingInfo);

        return sb.ToString();
    }

    private static void PrintSecondarySide(Battle battle, PlayerId perspective)
    {
        Side side = battle.GetSide(perspective);
        string remainingInfo = FormatRemainingPokemonInfo(side);

        StringBuilder sb = new();
        sb.AppendLine(FormatSecondarySlot1(battle, perspective));
        sb.AppendLine(FormatSecondarySlot2(battle, perspective));
        sb.AppendLine($"{remainingInfo}\n\n");
        Console.WriteLine(sb.ToString());
    }

    private static string FormatSecondarySlot1(Battle battle, PlayerId perspective)
    {
        Side side = battle.GetSide(perspective);
        Pokemon? activePokemon = side.Team.Slot1Pokemon;

        if (activePokemon == null) return string.Empty;

        string pokemonInfo = FormatPokemonBasicInfo(activePokemon);
        string hpDisplay = FormatHpDisplay(activePokemon, showExactHp: false);
        string conditionsInfo = FormatConditionsInfo(activePokemon);

        StringBuilder sb = new();
        sb.Append(SecondarySlot1Spacer);
        sb.AppendLine(pokemonInfo);
        sb.Append(SecondarySlot1Spacer);
        sb.Append(HpBarSpacer);
        sb.AppendLine(hpDisplay);
        if (activePokemon.IsTeraUsed)
        {
            sb.Append(SecondarySlot1Spacer);
            sb.AppendLine(FormatTeraInfo(activePokemon));
        }
        sb.Append(SecondarySlot1Spacer);
        sb.AppendLine(conditionsInfo);

        return sb.ToString();
    }

    private static string FormatSecondarySlot2(Battle battle, PlayerId perspective)
    {
        Side side = battle.GetSide(perspective);
        Pokemon? activePokemon = side.Team.Slot2Pokemon;
        if (activePokemon == null) return string.Empty;
        string pokemonInfo = FormatPokemonBasicInfo(activePokemon);
        string hpDisplay = FormatHpDisplay(activePokemon, showExactHp: false);
        string conditionsInfo = FormatConditionsInfo(activePokemon);
        StringBuilder sb = new();
        sb.AppendLine(pokemonInfo);
        sb.Append(HpBarSpacer);
        sb.AppendLine(hpDisplay);
        if (activePokemon.IsTeraUsed)
        {
            sb.AppendLine(FormatTeraInfo(activePokemon));
        }
        sb.AppendLine(conditionsInfo);
        return sb.ToString();
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

    private static void PrintField(Battle battle)
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