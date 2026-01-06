namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// string
/// </summary>
public abstract record Shared
{
    public static implicit operator Shared(string value) => new SharedString(value);
}

public record SharedString(string Value) : Shared
{
    public override string ToString() => Value;
}
