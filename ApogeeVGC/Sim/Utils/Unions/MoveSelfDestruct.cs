namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// 'always' | 'ifHit' | bool
/// </summary>
public abstract record MoveSelfDestruct
{
    public static MoveSelfDestruct FromAlways() => new AlwaysMoveSelfDestruct();
    public static MoveSelfDestruct FromIfHit() => new IfHitMoveSelfDestruct();
    public static implicit operator MoveSelfDestruct(bool value) => new BoolMoveSelfDestruct(value);
}

public record AlwaysMoveSelfDestruct : MoveSelfDestruct;
public record IfHitMoveSelfDestruct : MoveSelfDestruct;
public record BoolMoveSelfDestruct(bool Value) : MoveSelfDestruct;
