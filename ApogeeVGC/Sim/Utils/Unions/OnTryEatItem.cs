using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// bool | ((this: Battle, item: Item, pokemon: Pokemon) => boolean | void)
/// </summary>
public abstract record OnTryEatItem : IUnionEventHandler
{
    public static implicit operator OnTryEatItem(bool value) => new BoolOnTryEatItem(value);
    public static implicit operator OnTryEatItem(Func<Battle, Item, Pokemon, BoolVoidUnion> func) =>
        new FuncOnTryEatItem(func);

 public static OnTryEatItem FromFunc(Func<Battle, Item, Pokemon, BoolVoidUnion> func) =>
       new FuncOnTryEatItem(func);

    public abstract Delegate? GetDelegate();
    public abstract bool IsConstant();
    public abstract object? GetConstantValue();
}

public record BoolOnTryEatItem(bool Value) : OnTryEatItem
{
    public override Delegate? GetDelegate() => null;
    public override bool IsConstant() => true;
    public override object GetConstantValue() => Value;
}

public record FuncOnTryEatItem(Func<Battle, Item, Pokemon, BoolVoidUnion> Func) : OnTryEatItem
{
    public override Delegate GetDelegate() => Func;
    public override bool IsConstant() => false;
    public override object? GetConstantValue() => null;
}
