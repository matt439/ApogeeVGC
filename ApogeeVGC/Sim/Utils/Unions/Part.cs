using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.SpeciesClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// string | number | boolean | Pokemon | Side | Effect | Move | undefined
/// </summary>
public abstract record Part
{
public static implicit operator Part(string value) => new StringPart(value);
    public static implicit operator Part(int value) => new IntPart(value);
    public static implicit operator Part(double value) => new DoublePart(value);
    public static implicit operator Part(bool value) => new BoolPart(value);
    public static implicit operator Part(Pokemon? pokemon) =>
  pokemon == null ? new StringPart(string.Empty) : new PokemonPart(pokemon);
    public static implicit operator Part(Side side) => new SidePart(side);
    public static implicit operator Part(ActiveMove move) => new MovePart(move);

    public static implicit operator Part(Item item) => EffectUnionFactory.ToPart(item);
    public static implicit operator Part(Ability ability) => EffectUnionFactory.ToPart(ability);
    public static implicit operator Part(Species species) => EffectUnionFactory.ToPart(species);
    public static implicit operator Part(Condition condition) => EffectUnionFactory.ToPart(condition);
    public static implicit operator Part(Format format) => EffectUnionFactory.ToPart(format);

    public static Part FromIEffect(IEffect effect)
    {
        return effect switch
   {
       Item item => EffectUnionFactory.ToPart(item),
            Ability ability => EffectUnionFactory.ToPart(ability),
   Species species => EffectUnionFactory.ToPart(species),
  Condition condition => EffectUnionFactory.ToPart(condition),
         Format format => EffectUnionFactory.ToPart(format),
ActiveMove activeMove => EffectUnionFactory.ToPart(activeMove),
_ => throw new InvalidOperationException("Invalid IEffect type."),
      };
    }

    public static Part FromUndefined() => new UndefinedPart(new Undefined());

    public static implicit operator Part(Undefined value) => new UndefinedPart(value);

    public static Part? FromNullable<T>(T? value) where T : class
    {
    return value switch
        {
  null => null,
            string s => new StringPart(s),
            Pokemon p => new PokemonPart(p),
   Side s => new SidePart(s),
    ActiveMove m => new MovePart(m),
            IEffect e => new EffectPart(e),
            _ => throw new InvalidOperationException($"Unsupported type: {typeof(T)}"),
        };
    }
}

public record StringPart(string Value) : Part;
public record IntPart(int Value) : Part;
public record DoublePart(double Value) : Part;
public record BoolPart(bool Value) : Part;
public record PokemonPart(Pokemon Pokemon) : Part;
public record SidePart(Side Side) : Part;
public record MovePart(ActiveMove Move) : Part;
public record EffectPart(IEffect Effect) : Part;
public record UndefinedPart(Undefined Value) : Part;
