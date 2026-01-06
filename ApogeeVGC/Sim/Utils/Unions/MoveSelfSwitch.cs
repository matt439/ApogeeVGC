namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// 'copyvolatile' | 'shedtail' | bool
/// </summary>
public abstract record MoveSelfSwitch
{
    public static MoveSelfSwitch FromCopyVolatile() => new CopyVolatileMoveSelfSwitch();
    public static MoveSelfSwitch FromShedTail() => new ShedTailMoveSelfSwitch();
    public static implicit operator MoveSelfSwitch(bool value) => new BoolMoveSelfSwitch(value);
}

public record CopyVolatileMoveSelfSwitch : MoveSelfSwitch;
public record ShedTailMoveSelfSwitch : MoveSelfSwitch;
public record BoolMoveSelfSwitch(bool Value) : MoveSelfSwitch;
