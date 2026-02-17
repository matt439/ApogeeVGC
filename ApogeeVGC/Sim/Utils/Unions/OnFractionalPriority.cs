using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// FractionalPriorityHandler | -0.1
/// Handler signature: (Battle battle, int priority, Pokemon pokemon, ActiveMove move) => double
/// </summary>
public abstract record OnFractionalPriority : IUnionEventHandler
{
    public static implicit operator OnFractionalPriority(FractionalPriorityHandler function) =>
   new OnFractionalPriorityFunc(function);

    private static readonly decimal PriorityValue = new(-0.1);

    public static implicit operator OnFractionalPriority(decimal value) =>
        value == PriorityValue
  ? new OnFrationalPriorityNeg(value)
   : throw new ArgumentException("Must be -0.1 for OnFractionalPriorityNeg");

    public abstract Delegate? GetDelegate();
    public abstract bool IsConstant();
    public abstract object? GetConstantValue();
}

public record OnFractionalPriorityFunc(FractionalPriorityHandler Function) : OnFractionalPriority
{
    public override Delegate GetDelegate() => Function;
    public override bool IsConstant() => false;
    public override object? GetConstantValue() => null;
}

public record OnFrationalPriorityNeg(decimal Value) : OnFractionalPriority
{
  public override Delegate? GetDelegate() => null;
    public override bool IsConstant() => true;
    public override object GetConstantValue() => Value;
}
