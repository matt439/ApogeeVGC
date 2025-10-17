using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Sim.Ui;

public enum BattleAddId
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
    Immune,
    DetailsChange,
    FormeChange,
    Player,
    SetBoost,
    Boost,
    Unboost,
    Message,
    TeamPreview,
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

    public static void PrintAbilityChangeEvent(Pokemon pokemon, Ability oldAbility,
        Ability newAbility, IEffect sourceEffect, Pokemon? sourcePokemon = null)
    {
        if (sourcePokemon is not null)
        {
            Console.WriteLine(
                $"{pokemon.Name}'s {oldAbility.Name} was changed to {newAbility.Name} due to" +
                $"{sourcePokemon.Name}'s {sourceEffect.Name}!");
        }
        else
        {
            Console.WriteLine(
                $"{pokemon.Name}'s {oldAbility.Name} was changed to {newAbility.Name} due to {sourceEffect.Name}!");
        }
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

    public static void PrintFailEvent(Pokemon pokemon, IEffect? effect = null)
    {
        if (effect is not null)
        {
            Console.WriteLine($"{pokemon.Name}'s {effect.Name} failed!");
        }
        else
        {
            Console.WriteLine($"{pokemon.Name} failed!");
        }
    }

    public static void PrintFieldStartEvent(Condition fieldCondition, IEffect? sourceEffect = null,
        Pokemon? sourcePokemon = null)
    {
        if (sourceEffect is not null && sourcePokemon is not null)
        {
            Console.WriteLine($"{fieldCondition.Name} started due to {sourcePokemon.Name}'s {sourceEffect.Name}!");
        }
        else if (sourceEffect is not null)
        {
            Console.WriteLine($"{fieldCondition.Name} started due to {sourceEffect.Name}!");
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

    public static void PrintImmuneEvent(Pokemon pokemon, IEffect? source = null)
    {
        Console.WriteLine($"{pokemon.Name} is immune!");
    }

    public static void PrintDetailsChangeEvent(Pokemon pokemon, Pokemon.PokemonDetails details)
    {
        Console.WriteLine($"{pokemon.Name}'s details changed: {details}");
    }

    public static void PrintFormeChangeEvent(Pokemon pokemon, SpecieId newForme, string? message = null,
        IEffect? sourceEffect = null)
    {
        Console.WriteLine($"{pokemon.Name} changed forme to {newForme}!");
    }

    public static void PrintHealEvent(Pokemon pokemon, int health)
    {
        Console.WriteLine($"{pokemon.Name} healed {health} HP!");
    }

    public static void PrintSetBoostEvent(Pokemon target, BoostId boostId, int boost, IEffect sourceEffect)
    {
        string statName = boostId.ConvertToString();
        string boostText = boost > 0 ? $"rose by {boost} stage(s)" : $"fell by {-boost} stage(s)";
        Console.WriteLine($"{target.Name}'s {statName} {boostText} due to {sourceEffect.Name}!");
    }

    public static void PrintBoostEvent(Pokemon target, BoostId boostId, int boost, IEffect sourceEffect)
    {
        string statName = boostId.ConvertToString();
        string boostText = boost > 0 ? $"rose by {boost} stage(s)" : $"fell by {-boost} stage(s)";
        Console.WriteLine($"{target.Name}'s {statName} {boostText} due to {sourceEffect.Name}!");
    }

    public static void PrintUnboostEvent(Pokemon target, BoostId boostId, int boost, IEffect sourceEffect)
    {
        string statName = boostId.ConvertToString();
        string boostText = boost > 0 ? $"rose by {boost} stage(s)" : $"fell by {-boost} stage(s)";
        Console.WriteLine($"{target.Name}'s {statName} {boostText} due to {sourceEffect.Name}!");
    }

    public static void PrintMessage(string message)
    {
        Console.WriteLine(message);
    }

    public static void PrintTeamPreview()
    {

    }

    public static void PrintWinEvent(Side side)
    {
        Console.WriteLine($"{side.Name} wins!");
    }

    public static void PrintWinEvent(string sideName, string allySideName)
    {
        Console.WriteLine($"{sideName} & {allySideName} win!");
    }

    public static void PrintTieEvent()
    {
        Console.WriteLine("The battle ended in a tie!");
    }

    public static void PrintEmptyLine()
    {
        Console.WriteLine();
    }

    public static void PrintSwapEvent(Pokemon pokemon, int newPosition, string? attributes = null)
    {
        string attributesText = !string.IsNullOrEmpty(attributes) ? $" ({attributes})" : "";
        Console.WriteLine($"{pokemon.Name} swapped to position {newPosition}{attributesText}!");
    }
}