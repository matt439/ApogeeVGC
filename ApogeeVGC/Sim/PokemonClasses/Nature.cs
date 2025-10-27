using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils;
using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Sim.PokemonClasses;

public enum NatureId
{
    Adamant,
    Bashful,
    Bold,
    Brave,
    Calm,
    Careful,
    Docile,
    Gentle,
    Hardy,
    Hasty,
    Impish,
    Jolly,
    Lax,
    Lonely,
    Mild,
    Modest,
    Naive,
    Naughty,
    Quiet,
    Quirky,
    Rash,
    Relaxed,
    Sassy,
    Serious,
    Timid,
    None, // Used to hide nature from the opponent
}

public record Nature : IBasicEffect, ICopyable<Nature>, IIdentifiable
{
    public required NatureId Id { get; init; }
    public StatIdExceptHp? Plus { get; init; }
    public StatIdExceptHp? Minus { get; init; }
    public string Name => Id.ToString();
    public string ShowdownId => Id.ToShowdownId();

    public double GetStatModifier(StatIdExceptHp stat)
    {
        if (Plus == stat)
        {
            return 1.1; // 10% increase
        }
        if (Minus == stat)
        {
            return 0.9; // 10% decrease
        }
        return 1.0; // no change
    }

    public bool AffectsFainted { get; init; }

    public Nature Copy()
    {
        return new Nature
        {
            Id = Id,
            Plus = Plus,
            Minus = Minus,
        };
    }
}