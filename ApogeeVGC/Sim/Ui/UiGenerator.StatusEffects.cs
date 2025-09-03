using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Stats;
using System.Text;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Sim.Ui;

public static partial class UiGenerator
{
    // Switch and Pokemon State Actions
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

    public static void PrintForceSwitchOutAction(string trainerName, Pokemon switchedOut)
    {
        StringBuilder sb = new();
        sb.AppendLine($"{switchedOut.Name} returned to {trainerName}.");
        Console.WriteLine(sb.ToString());
    }

    public static void PrintForceSwitchInAction(string trainerName, Pokemon switchedIn)
    {
        StringBuilder sb = new();
        sb.AppendLine($"{trainerName} sent out {switchedIn.Name}!");
        Console.WriteLine(sb.ToString());
    }

    public static void PrintFaintedAction(Pokemon pokemon)
    {
        StringBuilder sb = new();
        sb.AppendLine($"{pokemon.Name} fainted!");
        Console.WriteLine(sb.ToString());
    }

    public static void PrintFlinch(Pokemon flinchee)
    {
        Console.WriteLine($"{flinchee.Name} flinched and couldn't move!");
    }

    // Status Conditions
    public static void PrintBurnDamage(Pokemon target, int damage)
    {
        Console.WriteLine($"{target.Name} takes {damage} damage from its burn.");
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

    public static void PrintParalysisStart(Pokemon target)
    {
        Console.WriteLine($"{target.Name} was paralysed!");
    }

    public static void PrintParalysisStartFromAbility(Pokemon target, Ability ability)
    {
        Console.WriteLine($"{target.Name} was paralysed by {ability.Name}!");
    }

    public static void PrintParalysisPrevention(Pokemon pokemon)
    {
        Console.WriteLine($"{pokemon.Name} is fully paralyzed and can't move!");
    }

    public static void PrintLeechSeedStart(Pokemon target)
    {
        Console.WriteLine($"{target.Name} was seeded!");
    }

    public static void PrintLeechSeedDamage(Pokemon target, int damage, Pokemon source, int heal)
    {
        Console.WriteLine($"{target.Name} is hurt by Leech Seed and lost {damage} HP!");
        Console.WriteLine($"{source.Name} restored {heal} HP!");
    }

    // Field Effects
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

    public static void PrintTailwindStart(string trainerName)
    {
        Console.WriteLine($"A tailwind blew from behind {trainerName}'s team!");
    }

    public static void PrintTailwindEnd(string trainerName)
    {
        Console.WriteLine($"{trainerName}'s tailwind petered out.");
    }

    public static void PrintReflectStart(string trainerName)
    {
        Console.WriteLine($"{trainerName}'s team became stronger against physical attacks!");
    }

    public static void PrintReflectEnd(string trainerName)
    {
        Console.WriteLine($"{trainerName}'s reflect wore off.");
    }

    public static void PrintLightScreenStart(string trainerName)
    {
        Console.WriteLine($"{trainerName}'s team became stronger against special attacks!");
    }

    public static void PrintLightScreenEnd(string trainerName)
    {
        Console.WriteLine($"{trainerName}'s light screen wore off.");
    }

    public static void PrintElectricTerrainStart()
    {
        Console.WriteLine("The battlefield became electrified!");
    }

    public static void PrintElectricTerrainEnd()
    {
        Console.WriteLine("The electric terrain disappeared.");
    }

    // Stat Modifications
    public static void PrintStatModifierChange(Pokemon pokemon, StatId stat, int change)
    {
        string pokemonName = pokemon.Name;
        string statName = stat.ConvertToString();
        StringBuilder sb = new();
        switch (change)
        {
            case 1:
                sb.AppendLine($"{pokemonName}'s {statName} rose!");
                break;
            case 2:
                sb.AppendLine($"{pokemonName}'s {statName} rose sharply!");
                break;
            case >= 3 and <= 12:
                sb.AppendLine($"{pokemonName}'s {statName} rose drastically!");
                break;
            case -1:
                sb.AppendLine($"{pokemonName}'s {statName} fell!");
                break;
            case -2:
                sb.AppendLine($"{pokemonName}'s {statName} harshly fell!");
                break;
            case <= -3 and >= -12:
                sb.AppendLine($"{pokemonName}'s {statName} severely fell!");
                break;
            case 0:
                throw new InvalidOperationException("Stat change cannot be zero.");
            default:
                throw new InvalidOperationException("Stat change must be between -12 and 12, excluding 0.");
        }

        Console.WriteLine(sb.ToString());
    }

    public static void PrintStatModifierTooHigh(Pokemon pokemon, StatId stat)
    {
        string pokemonName = pokemon.Name;
        string statName = stat.ConvertToString();
        Console.WriteLine($"{pokemonName}'s {statName} won't go any higher!");
    }

    public static void PrintStatModifierTooLow(Pokemon pokemon, StatId stat)
    {
        string pokemonName = pokemon.Name;
        string statName = stat.ConvertToString();
        Console.WriteLine($"{pokemonName}'s {statName} won't go any lower!");
    }
}