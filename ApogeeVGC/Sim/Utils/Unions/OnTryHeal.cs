using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// ((this: Battle, relayVar: number, target: Pokemon, source: Pokemon, effect: Effect) => number | boolean | void)
/// | ((this: Battle, pokemon: Pokemon) => boolean | void)
/// | boolean
/// </summary>
public abstract record OnTryHeal : IUnionEventHandler
{
    public static implicit operator OnTryHeal(
  Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolUnion?> func) =>
   new OnTryHealFunc1(func);
    public static implicit operator OnTryHeal(Func<Battle, Pokemon, bool?> func) =>
   new OnTryHealFunc2(func);
    public static implicit operator OnTryHeal(bool value) => new OnTryHealBool(value);

    public abstract Delegate? GetDelegate();
    public abstract bool IsConstant();
    public abstract object? GetConstantValue();
}

public record OnTryHealFunc1(Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolUnion?> Func) :
 OnTryHeal
{
  public override Delegate GetDelegate() => Func;
    public override bool IsConstant() => false;
    public override object? GetConstantValue() => null;
}

public record OnTryHealFunc2(Func<Battle, Pokemon, bool?> Func) : OnTryHeal
{
public override Delegate GetDelegate() => Func;
    public override bool IsConstant() => false;
    public override object? GetConstantValue() => null;
}

public record OnTryHealBool(bool Value) : OnTryHeal
{
    public override Delegate? GetDelegate() => null;
    public override bool IsConstant() => true;
    public override object GetConstantValue() => Value;
}
