using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Sim.Ui;

public enum UiType
{
    Status,
    Start,
    End,
    Activate,
    Heal,
    Weather,
    EndItem,
    Fail,
    Prepare,
    Ability,
    Cant,
    CureStatus,
    FieldStart,
    FieldEnd,
    SingleTurn,
    SideStart,
    SideEnd,
}

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

    public static void PrintStartEvent(Pokemon pokemon, IEffect effect, IEffect? sourceEffect = null,
        Pokemon? source = null)
    {
        if (sourceEffect is not null && source is not null)
        {
            Console.WriteLine($"{pokemon.Name} is affected by {effect.Name} from {source.Name}'s {sourceEffect.Name}!");
        }
        else if (sourceEffect is not null)
        {
            Console.WriteLine($"{pokemon.Name} is affected by {effect.Name} from {sourceEffect.Name}!");
        }
        else
        {
            Console.WriteLine($"{pokemon.Name} is affected by {effect.Name}!");
        }
    }

    public static void PrintCantEvent(Pokemon pokemon, IEffect effect)
    {
        Console.WriteLine($"{pokemon.Name} can't use {effect.Name}!");
    }

    public static void PrintCureStatusEvent(Pokemon pokemon, Condition status, IEffect? sourceEffect = null)
    {
        if (sourceEffect is not null)
        {
            Console.WriteLine($"{pokemon.Name} is cured of {status.Name} due to {sourceEffect.Name}!");
        }
        else
        {
            Console.WriteLine($"{pokemon.Name} is cured of {status.Name}!");
        }
    }

    public static void PrintFailEvent(Pokemon pokemon)
    {
        Console.WriteLine($"{pokemon.Name}'s move failed!");
    }

    public static void PrintFieldStartEvent(Condition fieldCondition, Pokemon? sourcePokemon = null)
    {
        if (sourcePokemon is not null)
        {
            Console.WriteLine($"{fieldCondition.Name} started due to {sourcePokemon.Name}!");
        }
        else
        {
            Console.WriteLine($"{fieldCondition.Name} started!");
        }
    }

    public static void PrintFieldEndEvent(Condition fieldCondition)
    {
        Console.WriteLine($"{fieldCondition.Name} ended!");
    }

    public static void PrintSingleTurnEvent(Pokemon target, Condition condition)
    {
        Console.WriteLine($"{target.Name} is affected by {condition.Name} for one turn!");
    }

    public static void PrintSideStartEvent(Side side, Condition sideCondition)
    {
        Console.WriteLine($"{sideCondition.Name} is affected by {sideCondition.Name}!");
    }
    public static void PrintSideEndEvent(Side side, Condition sideCondition)
    {
        Console.WriteLine($"{sideCondition.Name} has ended on {side.Name}!");
    }

}