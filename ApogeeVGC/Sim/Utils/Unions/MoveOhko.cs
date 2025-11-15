namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// bool | 'ice'
/// </summary>
public abstract record MoveOhko
{
    public static implicit operator MoveOhko(bool value) => new BoolMoveOhko(value);
    public static MoveOhko FromIce() => new IceMoveOhko();
}

public record BoolMoveOhko(bool Value) : MoveOhko;
public record IceMoveOhko : MoveOhko;
