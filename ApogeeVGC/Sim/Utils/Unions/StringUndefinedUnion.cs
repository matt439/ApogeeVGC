namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// string | undefined
/// </summary>
public abstract record StringUndefinedUnion
{
    public static implicit operator StringUndefinedUnion(string value) =>
 new StringStringUndefinedUnion(value);
  public static implicit operator StringUndefinedUnion(Undefined value) =>
     new UndefinedStringUndefinedUnion(value);
}

public record StringStringUndefinedUnion(string Value) : StringUndefinedUnion;
public record UndefinedStringUndefinedUnion(Undefined Value) : StringUndefinedUnion;
