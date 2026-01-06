using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// Side | bool
/// </summary>
public abstract record SideBoolUnion
{
    public abstract bool IsTrue();
    public static implicit operator SideBoolUnion(Side side) => new SideSideBoolUnion(side);
    public static implicit operator SideBoolUnion(bool value) => new BoolSideBoolUnion(value);
}

public record SideSideBoolUnion(Side Side) : SideBoolUnion
{
public override bool IsTrue() => true;
}

public record BoolSideBoolUnion(bool Value) : SideBoolUnion
{
    public override bool IsTrue() => Value;
}
