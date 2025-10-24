using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Extensions;
using static ApogeeVGC.Sim.PokemonClasses.Pokemon;

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

    public static void PrintActivateEvent(Pokemon pokemon, IEffect effect, string detail)
    {
        Console.WriteLine($"{pokemon.Name}'s {effect.Name} activated! {detail}");
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

    public static void PrintFailEvent(Pokemon pokemon, string message)
    {
        Console.WriteLine($"{pokemon.Name} failed! {message}");
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

    public static void PrintImmuneEvent(Pokemon pokemon, string message)
    {
        Console.WriteLine($"{pokemon.Name} is immune! {message}");
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

    ///// <summary>
    ///// Prints a heal event message.
    ///// Handles various healing scenarios including healing from effects, abilities, and conditions.
    ///// </summary>
    ///// <param name="target">The Pokemon being healed</param>
    ///// <param name="fromEffect">Optional effect name causing the healing (e.g., "drain", "wish")</param>
    ///// <param name="ofPokemon">Optional source Pokemon for the healing</param>
    ///// <param name="silent">If true, prints a silent heal message</param>
    //public static void PrintHealEvent(Pokemon target, string? fromEffect = null,
    //    Pokemon? ofPokemon = null, bool silent = false)
    //{
    //    PokemonHealth health = target.GetHealth();
    //    string healthStatus = FormatHealthStatus(health);

    //    if (silent)
    //    {
    //        // Silent heal (e.g., from Leech Seed, Rest)
    //        Console.WriteLine($"-heal|{target}|{healthStatus}|[silent]");
    //        return;
    //    }

    //    if (string.IsNullOrEmpty(fromEffect))
    //    {
    //        // Simple heal (from move or no effect)
    //        Console.WriteLine($"-heal|{target}|{healthStatus}");
    //    }
    //    else if (ofPokemon != null)
    //    {
    //        // Heal from effect with source Pokemon
    //        Console.WriteLine($"-heal|{target}|{healthStatus}|[from] {fromEffect}|[of] {ofPokemon}");
    //    }
    //    else
    //    {
    //        // Heal from effect without source
    //        Console.WriteLine($"-heal|{target}|{healthStatus}|[from] {fromEffect}");
    //    }
    //}

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

    public static void PrintTurnEvent(int turnNumber)
    {
        Console.WriteLine($"Turn {turnNumber}");
    }

    public static void PrintBigError(string error)
    {
        Console.WriteLine(error);
    }

    ///// <summary>
    ///// Prints a damage event message.
    ///// Handles various damage scenarios including damage from effects, abilities, and conditions.
    ///// </summary>
    ///// <param name="target">The Pokemon taking damage</param>
    ///// <param name="fromEffect">Optional effect name causing the damage (e.g., "burn", "confusion")</param>
    ///// <param name="ofPokemon">Optional source Pokemon for the damage</param>
    ///// <param name="silent">If true, prints a silent damage message (no console output)</param>
    ///// <param name="isPartiallyTrapped">If true, includes the [partiallytrapped] tag</param>
    //public static void PrintDamageEvent(Pokemon target, string? fromEffect = null,
    //    Pokemon? ofPokemon = null, bool silent = false, bool isPartiallyTrapped = false)
    //{
    //    PokemonHealth health = target.GetHealth();
    //    string healthStatus = FormatHealthStatus(health);

    //    if (silent)
    //    {
    //        // Silent damage (e.g., from Powder)
    //        Console.WriteLine($"-damage|{target}|{healthStatus}|[silent]");
    //        return;
    //    }

    //    if (isPartiallyTrapped && !string.IsNullOrEmpty(fromEffect))
    //    {
    //        // Partially trapped damage (e.g., from Bind, Wrap)
    //        Console.WriteLine($"-damage|{target}|{healthStatus}|[from] {fromEffect}|[partiallytrapped]");
    //        return;
    //    }

    //    if (string.IsNullOrEmpty(fromEffect))
    //    {
    //        // Simple damage (from move or no effect)
    //        Console.WriteLine($"-damage|{target}|{healthStatus}");
    //    }
    //    else if (ofPokemon != null)
    //    {
    //        // Damage from effect with source Pokemon
    //        Console.WriteLine($"-damage|{target}|{healthStatus}|[from] {fromEffect}|[of] {ofPokemon}");
    //    }
    //    else
    //    {
    //        // Damage from effect without source
    //        Console.WriteLine($"-damage|{target}|{healthStatus}|[from] {fromEffect}");
    //    }
    //}

    ///// <summary>
    ///// Formats a Pokemon's health status for display in messages.
    ///// Returns format like "100/100" or "50/100 slp" depending on status.
    ///// </summary>
    //private static string FormatHealthStatus(PokemonHealth health)
    //{
    //    throw new NotImplementedException();
    //}

    public static void PrintFaintEvent(Pokemon pokemon)
    {
        Console.WriteLine($"{pokemon.Name} fainted!");
    }

    public static void PrintDetailsChangeEvent(Pokemon pokemon, PokemonDetails details, bool silent = false)
    {
        if (silent)
        {
            Console.WriteLine($"-detailschange|{pokemon}|{details}|[silent]");
        }
        else
        {
            Console.WriteLine($"-detailschange|{pokemon}|{details}");
        }
    }

    public static void PrintTransformEvent(Pokemon pokemon1, Pokemon pokemon2, IEffect? effect = null)
    {
        if (effect is not null)
        {
            Console.WriteLine($"{pokemon1.Name} transformed into {pokemon2.Name} due to {effect.Name}!");
        }
        else
        {
            Console.WriteLine($"{pokemon1.Name} transformed into {pokemon2.Name}!");
        }
    }

    public static void PrintEndItemEvent(Pokemon pokemon, Item item, string? extraDetail = null)
    {
        Console.WriteLine(
            $"{pokemon.Name}'s {item.Name} was consumed!{(extraDetail is not null ? " " + extraDetail : "")}");
    }

    public static void PrintSwitchEvent(Pokemon pokemon, bool isDrag, IEffect? effect = null)
    {
        if (isDrag)
        {
            Console.WriteLine(effect is not null
                ? $"{pokemon.Name} was dragged out by {effect.Name}!"
                : $"{pokemon.Name} was dragged out!");
        }
        else
        {
            Console.WriteLine(effect is not null
                ? $"{pokemon.Name} switched in due to {effect.Name}!"
                : $"{pokemon.Name} switched in!");
        }
    }

    public static void PrintHint(string hint)
    {
        Console.WriteLine($"[hint] {hint}");
    }

    public static void PrintCantEvent(Pokemon pokemon, string reason, ActiveMove move)
    {
        Console.WriteLine($"{pokemon.Name} can't use {move.Name} because {reason}!");
    }

    public static void PrintMoveEvent(Pokemon pokemon, string moveName, Pokemon? target = null, string? attrs = null)
    {
        string message = $"{pokemon.Name} used {moveName}";
        if (target != null)
        {
            message += $" on {target.Name}";
        }
        if (!string.IsNullOrEmpty(attrs))
        {
            message += $" {attrs}";
        }
        Console.WriteLine(message);
    }

    public static void PrintMissEvent(Pokemon pokemon, Pokemon target)
    {
        Console.WriteLine($"{pokemon.Name}'s attack missed {target.Name}!");
    }

    public static void PrintClearPositiveBoostEvent(Pokemon target, Pokemon pokemon, ActiveMove move)
    {
        Console.WriteLine($"{pokemon.Name}'s {move.Name} cleared all positive stat boosts from {target.Name}!");
    }

    public static void PrintOhkoEvent()
    {
        Console.WriteLine("One-Hit KO move succeeded!");
    }

    public static void PrintAnimationEvent(Pokemon pokemon, string moveName, Pokemon target)
    {
        Console.WriteLine($"Animation: {pokemon.Name} uses {moveName} on {target.Name}");
    }

    public static void PrintHitCountEvent(Pokemon pokemon, int hitCount)
    {
        Console.WriteLine($"{pokemon.Name} hit {hitCount} time(s)!");
    }

    public static void PrintSuperEffectiveEvent(Pokemon pokemon)
    {
        Console.WriteLine("It's super effective!");
    }

    public static void PrintResistedEvent(Pokemon pokemon)
    {
        Console.WriteLine("It's not very effective...");
    }

    public static void PrintCritEvent(Pokemon pokemon)
    {
        Console.WriteLine("A critical hit!");
    }
}