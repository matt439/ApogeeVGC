using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// ((this: Battle, pokemon: Pokemon, type: string) => boolean | void) | boolean
/// </summary>
public abstract record OnNegateImmunity : IUnionEventHandler
{
    public static implicit operator OnNegateImmunity(Func<Battle, Pokemon, PokemonType, BoolVoidUnion> func) =>
        new OnNegateImmunityFunc(func);
    public static implicit operator OnNegateImmunity(bool value) => new OnNegateImmunityBool(value);

    public abstract Delegate? GetDelegate();
    public abstract bool IsConstant();
    public abstract object? GetConstantValue();
}

public record OnNegateImmunityFunc(Func<Battle, Pokemon, PokemonType, BoolVoidUnion> Func)
: OnNegateImmunity
{
    public override Delegate GetDelegate() => Func;
  public override bool IsConstant() => false;
    public override object? GetConstantValue() => null;
}

public record OnNegateImmunityBool(bool Value) : OnNegateImmunity
{
    public override Delegate? GetDelegate() => null;
    public override bool IsConstant() => true;
    public override object GetConstantValue() => Value;
}
