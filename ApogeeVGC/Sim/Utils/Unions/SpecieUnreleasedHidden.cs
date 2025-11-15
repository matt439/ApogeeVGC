namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// bool | 'past'
/// </summary>
public abstract record SpecieUnreleasedHidden
{
    public static implicit operator SpecieUnreleasedHidden(bool value) => new BoolSpecieUnreleasedHidden(value);
    public static SpecieUnreleasedHidden FromPast() => new PastSpecieUnreleasedHidden();
}

public record BoolSpecieUnreleasedHidden(bool Value) : SpecieUnreleasedHidden;
public record PastSpecieUnreleasedHidden : SpecieUnreleasedHidden;
