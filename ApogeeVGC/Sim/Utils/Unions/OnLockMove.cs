using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// string | ((this: Battle, pokemon: Pokemon) => void | string)    
/// </summary>
public abstract record OnLockMove : IUnionEventHandler
{
    public static implicit operator OnLockMove(MoveId moveId) => new OnLockMoveMoveId(moveId);
    public static implicit operator OnLockMove(Func<Battle, Pokemon, MoveIdVoidUnion> func) =>
        new OnLockMoveFunc(func);

    public abstract Delegate? GetDelegate();
    public abstract bool IsConstant();
  public abstract object? GetConstantValue();
}

public record OnLockMoveMoveId(MoveId Id) : OnLockMove
{
    public override Delegate? GetDelegate() => null;
  public override bool IsConstant() => true;
    public override object GetConstantValue() => Id;
}

public record OnLockMoveFunc(Func<Battle, Pokemon, MoveIdVoidUnion> Func) : OnLockMove
{
    public override Delegate GetDelegate() => Func;
    public override bool IsConstant() => false;
    public override object? GetConstantValue() => null;
}
