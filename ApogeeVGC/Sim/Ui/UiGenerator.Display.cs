using ApogeeVGC.Player;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using System.Text;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
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

    private static string GenerateDoubleSlotChoiceString(DoubleSlotChoice choice)
    {
        StringBuilder sb = new();
        sb.Append("Double Slot Choice: [");
        sb.Append(GenerateSlotChoiceString(choice.Slot1Choice));
        sb.Append(" | ");
        sb.Append(GenerateSlotChoiceString(choice.Slot2Choice));
        sb.Append("]");
        return sb.ToString();
    }

    private static string GenerateSlotChoiceString(SlotChoice choice)
    {
        return choice switch
        {
            SlotChoice.MoveChoice moveChoice => GenerateMoveChoiceString(moveChoice),
            SlotChoice.SwitchChoice switchChoice => GenerateSwitchChoiceString(switchChoice),
            _ => throw new ArgumentException("Invalid SlotChoice type.", nameof(choice)),
        };
    }

    private static string GenerateMoveChoiceString(SlotChoice.MoveChoice choice)
    {
        if (choice.IsStruggle)
        {
            return "Struggle";

        }
        
        StringBuilder sb = new();
        sb.Append("Move: ");
        sb.Append(choice.Move.Name);
        sb.Append(" (");
        sb.Append(choice.Move.Pp);
        sb.Append('/');
        sb.Append(choice.Move.MaxPp);
        sb.Append(')');

        if (!choice.IsTera) return sb.ToString();

        sb.Append(" + ");
        sb.Append(choice.Attacker.TeraType.ConvertToString());
        sb.Append(" Tera");

        return sb.ToString();
    }

    // Choice Generation Methods
    private static string GenerateBattleChoiceString(BattleChoice choice)
    {
        return choice switch
        {
            DoubleSlotChoice doubleSlotChoice => GenerateDoubleSlotChoiceString(doubleSlotChoice),
            SlotChoice slotChoice => GenerateSlotChoiceString(slotChoice),
            TeamPreviewChoice teamPreviewChoice => GenerateTeamPreviewChoiceString(teamPreviewChoice),
            _ => throw new ArgumentException("Invalid choice type.", nameof(choice)),
        };
    }

    private static string GenerateTeamPreviewChoiceString(TeamPreviewChoice choice)
    {
        StringBuilder sb = new();
        sb.Append("Select: ");
        int i = 1;
        foreach (Pokemon pokemon in choice.Pokemon)
        {
            sb.Append($"{i} {pokemon.Name} ({pokemon.Specie.Name}) ");
            if (i < choice.Pokemon.Count)
            {
                sb.Append("| ");
            }
            i++;
        }
        return sb.ToString();
    }

    private static string GenerateSwitchChoiceString(SlotChoice.SwitchChoice choice)
    {
        StringBuilder sb = new();
        sb.Append("Switch: ");
        sb.Append($"{choice.SwitchInPokemon.Name} ({choice.SwitchInPokemon.Specie.Name}) ");
        return sb.ToString();
    }

    private static void PrintPrimarySide(Side side)
    {
        string remainingInfo = FormatRemainingPokemonInfo(side);

        StringBuilder sb = new();
        switch (side.BattleFormat)
        {
            case BattleFormat.Singles:
                sb.Append(FormatPrimarySlot(side, SlotId.Slot1));
                break;
            case BattleFormat.Doubles:
                sb.Append(FormatPrimarySlot(side, SlotId.Slot1));
                sb.Append(FormatPrimarySlot(side, SlotId.Slot2, PrimarySlot2Spacer));
                break;
            default:
                throw new InvalidOperationException("Unknown battle format.");
        }

        sb.Append(PrimarySpacer);
        sb.AppendLine(remainingInfo);

        Console.WriteLine(sb.ToString());
    }

    private const string SecondarySlot1Spacer = "      ";
    private const string PrimarySlot2Spacer = "       ";

    private static string FormatPrimarySlot(Side side, SlotId slotId, string spacer = "")
    {
        if (slotId is not (SlotId.Slot1 or SlotId.Slot2))
        {
            throw new ArgumentException("Invalid slot for primary side display.", nameof(slotId));
        }

        Pokemon activePokemon = side.GetSlot(slotId);

        string pokemonInfo = FormatPokemonBasicInfo(activePokemon);
        string hpDisplay = FormatHpDisplay(activePokemon, showExactHp: true);
        string conditionsInfo = FormatConditionsInfo(activePokemon);
        string stats = SingleLineStats(activePokemon);
        string statMods = SingleLineStatModifiers(activePokemon);

        StringBuilder sb = new();
        sb.Append(PrimarySpacer);
        sb.Append(spacer);
        sb.AppendLine(pokemonInfo);
        sb.Append(PrimarySpacer);
        sb.Append(spacer);
        sb.Append(HpBarSpacer);
        sb.AppendLine(hpDisplay);
        if (activePokemon.IsTeraUsed)
        {
            sb.Append(PrimarySpacer);
            sb.Append(spacer);
            sb.AppendLine(FormatTeraInfo(activePokemon));
        }
        sb.Append(PrimarySpacer);
        sb.Append(spacer);
        sb.AppendLine(conditionsInfo);
        sb.Append(PrimarySpacer);
        sb.Append(spacer);
        sb.AppendLine(stats);
        sb.Append(PrimarySpacer);
        sb.Append(spacer);
        sb.AppendLine(statMods);

        return sb.ToString();
    }

    private static void PrintSecondarySide(Side side)
    {
        string remainingInfo = FormatRemainingPokemonInfo(side);

        StringBuilder sb = new();
        switch (side.BattleFormat)
        {
            case BattleFormat.Singles:
                sb.AppendLine(FormatSecondarySlot(side, SlotId.Slot1));
                break;
            case BattleFormat.Doubles:
                sb.AppendLine(FormatSecondarySlot(side, SlotId.Slot1, SecondarySlot1Spacer));
                sb.AppendLine(FormatSecondarySlot(side, SlotId.Slot2));
                break;
            default:
                throw new InvalidOperationException("Unknown battle format.");
        }
        sb.AppendLine($"{remainingInfo}\n\n");

        Console.WriteLine(sb.ToString());
    }

    

    private static string FormatSecondarySlot(Side side, SlotId slotId, string spacer = "")
    {
        if (slotId is not (SlotId.Slot1 or SlotId.Slot2))
        {
            throw new ArgumentException("Invalid slot for secondary side display.", nameof(slotId));
        }
        Pokemon activePokemon = side.GetSlot(slotId);

        string pokemonInfo = FormatPokemonBasicInfo(activePokemon);
        string hpDisplay = FormatHpDisplay(activePokemon, showExactHp: false);
        string conditionsInfo = FormatConditionsInfo(activePokemon);

        StringBuilder sb = new();
        sb.Append(spacer);
        sb.AppendLine(pokemonInfo);
        sb.Append(spacer);
        sb.Append(HpBarSpacer);
        sb.AppendLine(hpDisplay);
        if (activePokemon.IsTeraUsed)
        {
            sb.Append(spacer);
            sb.AppendLine(FormatTeraInfo(activePokemon));
        }
        sb.Append(spacer);
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
        return $"Remaining Pokemon: {side.AlivePokemonCount}";
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

    private static void PrintField(Field field)
    {
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