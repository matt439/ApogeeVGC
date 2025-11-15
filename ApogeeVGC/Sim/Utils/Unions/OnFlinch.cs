using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// ((this: Battle, pokemon: Pokemon, source: null, move: ActiveMove) => boolean | void) | boolean
/// </summary>
public abstract record OnFlinch : IUnionEventHandler
{
    public static implicit operator OnFlinch(Func<Battle, Pokemon, object?, Move, BoolVoidUnion> func) =>
     new OnFlinchFunc(func);
    public static implicit operator OnFlinch(bool value) => new OnFlinchBool(value);

    public abstract Delegate? GetDelegate();
    public abstract bool IsConstant();
    public abstract object? GetConstantValue();
}

public record OnFlinchFunc(Func<Battle, Pokemon, object?, Move, BoolVoidUnion> Func) : OnFlinch
{
    public override Delegate GetDelegate() => Func;
    public override bool IsConstant() => false;
    public override object? GetConstantValue() => null;
}

public record OnFlinchBool(bool Value) : OnFlinch
{
 public override Delegate? GetDelegate() => null;
    public override bool IsConstant() => true;
    public override object GetConstantValue() => Value;
}
