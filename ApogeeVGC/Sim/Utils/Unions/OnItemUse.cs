using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// (this: Battle, pokemon: Pokemon) => BoolVoidUnion | false
/// Used for Item.OnUse event handlers that can:
/// - Return void to allow item use (default behavior)
/// - Return true to explicitly allow item use
/// - Return false to block item use
/// </summary>
public abstract record OnItemUse : IUnionEventHandler
{
    public static implicit operator OnItemUse(Func<Battle, Pokemon, BoolVoidUnion> func) =>
        new OnItemUseFunc(func);

    public static implicit operator OnItemUse(Action<Battle, Pokemon> func) =>
        new OnItemUseActionFunc(func);

    public static OnItemUse FromFalse() => new OnItemUseFalse();

    public abstract Delegate? GetDelegate();
    public abstract bool IsConstant();
    public abstract object? GetConstantValue();
}

/// <summary>
/// OnItemUse variant with Func that returns BoolVoidUnion for validation.
/// </summary>
public record OnItemUseFunc(Func<Battle, Pokemon, BoolVoidUnion> Func) : OnItemUse
{
    public override Delegate GetDelegate() => Func;
    public override bool IsConstant() => false;
    public override object? GetConstantValue() => null;
}

/// <summary>
/// OnItemUse variant with Action (void return) for backwards compatibility.
/// Implicitly treated as "allow" (returns void).
/// </summary>
public record OnItemUseActionFunc(Action<Battle, Pokemon> Func) : OnItemUse
{
    public override Delegate GetDelegate() => Func;
    public override bool IsConstant() => false;
    public override object? GetConstantValue() => null;
}

/// <summary>
/// OnItemUse constant false - blocks item use.
/// </summary>
public record OnItemUseFalse : OnItemUse
{
    public override Delegate? GetDelegate() => null;
    public override bool IsConstant() => true;
    public override object? GetConstantValue() => false;
}
