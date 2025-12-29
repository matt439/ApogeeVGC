using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// (this: Battle, item: Item, pokemon: Pokemon, source: Pokemon, move?: ActiveMove) => boolean | void) | boolean
/// Returns false to prevent item removal, true to allow it.
/// </summary>
public abstract record OnTakeItem : IUnionEventHandler
{
    public static implicit operator OnTakeItem(Func<Battle, Item, Pokemon, Pokemon, Move?, BoolVoidUnion> func)
        => new OnTakeItemFunc(func);
    public static implicit operator OnTakeItem(bool value) => new OnTakeItemBool(value);

    public abstract Delegate? GetDelegate();
    public abstract bool IsConstant();
    public abstract object? GetConstantValue();
}

public record OnTakeItemFunc(Func<Battle, Item, Pokemon, Pokemon, Move?, BoolVoidUnion> Func)
    : OnTakeItem
{
    public override Delegate GetDelegate() => Func;
    public override bool IsConstant() => false;
    public override object? GetConstantValue() => null;
}

public record OnTakeItemBool(bool Value) : OnTakeItem
{
    public override Delegate? GetDelegate() => null;
    public override bool IsConstant() => true;
    public override object GetConstantValue() => Value;
}
