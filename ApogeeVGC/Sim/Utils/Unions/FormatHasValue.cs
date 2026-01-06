namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// bool | 'integer' | 'positive-integer'
/// </summary>
public abstract record FormatHasValue
{
    public static implicit operator FormatHasValue(bool value) => new BoolFormatHasValue(value);
    public static FormatHasValue FromInteger() => new IntegerFormatHasValue();
    public static FormatHasValue FromPositiveInteger() => new PositiveIntegerFormatHasValue();
}

public record BoolFormatHasValue(bool Value) : FormatHasValue;
public record IntegerFormatHasValue : FormatHasValue;
public record PositiveIntegerFormatHasValue : FormatHasValue;
