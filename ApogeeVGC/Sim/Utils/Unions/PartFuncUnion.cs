using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.SpeciesClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// Part | (() => { side: SideID, secret: string, shared: string })
/// Represents a battle log part that can be either a direct value or a function that generates side-specific content.
/// </summary>
public abstract record PartFuncUnion
{
    public static implicit operator PartFuncUnion(Part part) => new PartPartFuncUnion(part);
    public static implicit operator PartFuncUnion(Func<SideSecretSharedResult> func) => new FuncPartFuncUnion(func);

    // Convenience implicit conversions for Part types
    public static implicit operator PartFuncUnion(string value) => new PartPartFuncUnion(value);
    public static implicit operator PartFuncUnion(int value) => new PartPartFuncUnion(value);
public static implicit operator PartFuncUnion(double value) => new PartPartFuncUnion(value);
    public static implicit operator PartFuncUnion(bool value) => new PartPartFuncUnion(value);
    public static implicit operator PartFuncUnion(Pokemon pokemon) => new PartPartFuncUnion(pokemon);
    public static implicit operator PartFuncUnion(Side side) => new PartPartFuncUnion(side);
public static implicit operator PartFuncUnion(ActiveMove move) => new PartPartFuncUnion(move);
    public static implicit operator PartFuncUnion(Undefined value) => new PartPartFuncUnion(value);

 public static implicit operator PartFuncUnion(Item item) => EffectUnionFactory.ToPart(item);
    public static implicit operator PartFuncUnion(Ability ability) => EffectUnionFactory.ToPart(ability);
 public static implicit operator PartFuncUnion(Species species) => EffectUnionFactory.ToPart(species);
  public static implicit operator PartFuncUnion(Condition condition) => EffectUnionFactory.ToPart(condition);
  public static implicit operator PartFuncUnion(Format format) => EffectUnionFactory.ToPart(format);

    public static PartFuncUnion FromIEffect(IEffect effect) => Part.FromIEffect(effect);
}

public record PartPartFuncUnion(Part Part) : PartFuncUnion;
public record FuncPartFuncUnion(Func<SideSecretSharedResult> Func) : PartFuncUnion;
