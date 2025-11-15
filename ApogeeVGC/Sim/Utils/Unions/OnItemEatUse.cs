using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// (this: Battle, pokemon: Pokemon) => void) | false
/// </summary>
public abstract record OnItemEatUse : IUnionEventHandler
{
    public static implicit operator OnItemEatUse(Action<Battle, Pokemon> func) =>
   new OnItemEatUseFunc(func);
    public static OnItemEatUse FromFalse() => new OnItemEatUseFalse();

    public abstract Delegate? GetDelegate();
    public abstract bool IsConstant();
    public abstract object? GetConstantValue();
}

public record OnItemEatUseFunc(Action<Battle, Pokemon> Func) : OnItemEatUse
{
    public override Delegate GetDelegate() => Func;
    public override bool IsConstant() => false;
    public override object? GetConstantValue() => null;
}

public record OnItemEatUseFalse : OnItemEatUse
{
    public override Delegate? GetDelegate() => null;
    public override bool IsConstant() => true;
    public override object? GetConstantValue() => null;
}
