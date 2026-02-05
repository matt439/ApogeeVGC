using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// ((this: Battle, pokemon: Pokemon) => boolean | void) | boolean
/// Note: TypeScript signature shows source/move params but they're always undefined in runEvent('Flinch', pokemon)
/// </summary>
public abstract record OnFlinch : IUnionEventHandler
{
    public static implicit operator OnFlinch(Func<Battle, Pokemon, BoolVoidUnion> func) =>
        new OnFlinchFunc(func);
    public static implicit operator OnFlinch(bool value) => new OnFlinchBool(value);

    public abstract Delegate? GetDelegate();
    public abstract bool IsConstant();
    public abstract object? GetConstantValue();
}

public record OnFlinchFunc(Func<Battle, Pokemon, BoolVoidUnion> Func) : OnFlinch
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
