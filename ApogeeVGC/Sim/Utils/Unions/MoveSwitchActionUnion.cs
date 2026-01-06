using ApogeeVGC.Sim.Actions;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// MoveAction | SwitchAction
/// </summary>
public abstract record MoveSwitchActionUnion
{
    public static implicit operator MoveSwitchActionUnion(MoveAction moveAction) =>
        new MoveActionMoveSwitchActionUnion(moveAction);
    public static implicit operator MoveSwitchActionUnion(SwitchAction switchAction) =>
  new SwitchActionMoveSwitchActionUnion(switchAction);
}

public record MoveActionMoveSwitchActionUnion(MoveAction MoveAction) : MoveSwitchActionUnion;
public record SwitchActionMoveSwitchActionUnion(SwitchAction SwitchAction) : MoveSwitchActionUnion;
