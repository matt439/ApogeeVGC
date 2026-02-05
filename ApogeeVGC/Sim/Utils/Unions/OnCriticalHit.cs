using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// ((this: Battle, pokemon: Pokemon, source: Pokemon, move: Move) =&gt; boolean | void) | boolean
/// </summary>
public abstract record OnCriticalHit : IUnionEventHandler
{
    public static implicit operator OnCriticalHit(Func<Battle, Pokemon, Pokemon, Move, BoolVoidUnion> function) =>
     new OnCriticalHitFunc(function);
 public static implicit operator OnCriticalHit(bool value) => new OnCriticalHitBool(value);

    public abstract Delegate? GetDelegate();
    public abstract bool IsConstant();
    public abstract object? GetConstantValue();
}

public record OnCriticalHitFunc(Func<Battle, Pokemon, Pokemon, Move, BoolVoidUnion> Function)
: OnCriticalHit
{
    public override Delegate GetDelegate() => Function;
    public override bool IsConstant() => false;
    public override object? GetConstantValue() => null;
}

public record OnCriticalHitBool(bool Value) : OnCriticalHit
{
    public override Delegate? GetDelegate() => null;
    public override bool IsConstant() => true;
    public override object GetConstantValue() => Value;
}
