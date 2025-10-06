using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Ui;

//public enum UiType
//{
//    Status,
//    Start,
//    End,
//    Activate,
//    Heal,
//    Weather,
//    EndItem,
//    Fail,
//    Prepare,
//    Ability,
//}

public static class UiGenerator
{
    //public static void PrintGenericBattleEvent(UiType type, Pokemon pokemon, )
    //{

    //}

    public static void PrintStatusEvent(Pokemon pokemon, Condition status, IEffect? sourceEffect = null,
        Pokemon? sourcePokemon = null)
    {
        if (sourceEffect is not null && sourcePokemon is not null)
        {
            Console.WriteLine($"{pokemon.Name} is now {status.Name} due to {sourcePokemon.Name}'s {sourceEffect.Name}!");
        }
        else if (sourceEffect is not null)
        {
            Console.WriteLine($"{pokemon.Name} is now {status.Name} due to {sourceEffect.Name}!");
        }
        else
        {
            Console.WriteLine($"{pokemon.Name} is now {status.Name}!");
        }
    }

    public static void PrintEndEvent(Pokemon pokemon, IEffect effect)
    {
        Console.WriteLine($"{effect.Name} has ended on {pokemon.Name}.");
    }

    public static void PrintAbilityEvent(Pokemon pokemon, Ability ability)
    {
        PrintAbilityEvent(pokemon, ability.Name);
    }

    public static void PrintAbilityEvent(Pokemon pokemon, string ability)
    {
        Console.WriteLine($"{pokemon.Name}'s {ability} activated!");
    }

    public static void PrintActivateEvent(Pokemon pokemon, IEffect effect, IEffect? sourceEffect = null)
    {
        if (sourceEffect is not null)
        {
            Console.WriteLine($"{pokemon.Name}'s {effect.Name} activated from {sourceEffect.Name}");
        }
        else
        {
            Console.WriteLine($"{pokemon.Name}'s {effect.Name} activated!");
        }
    }

    public static void PrintStartEvent(Pokemon pokemon, IEffect effect)
    {
        Console.WriteLine($"{effect.Name} started on {pokemon.Name}!");
    }
}