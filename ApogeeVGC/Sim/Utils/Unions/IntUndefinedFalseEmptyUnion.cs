namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// int | undefined | false | empty
/// </summary>
public abstract record IntUndefinedFalseEmptyUnion
{
    public static implicit operator IntUndefinedFalseEmptyUnion(int value) =>
        new IntIntUndefinedFalseEmptyUnion(value);
    public static implicit operator IntUndefinedFalseEmptyUnion(Undefined value) =>
  new UndefinedIntUndefinedFalseEmptyUnion(value);
    public static IntUndefinedFalseEmptyUnion FromFalse() => new FalseIntUndefinedFalseEmptyUnion();
    public static IntUndefinedFalseEmptyUnion FromEmpty() => new EmptyIntUndefinedFalseEmptyUnion(new Empty());

    public static IntUndefinedFalseEmptyUnion FromIntUndefinedFalseUnion(IntUndefinedFalseUnion value)
    {
      return value switch
        {
          IntIntUndefinedFalseUnion intCase => new IntIntUndefinedFalseEmptyUnion(intCase.Value),
            UndefinedIntUndefinedFalseUnion undefinedCase =>
  new UndefinedIntUndefinedFalseEmptyUnion(undefinedCase.Value),
 FalseIntUndefinedFalseUnion => new FalseIntUndefinedFalseEmptyUnion(),
     _ => throw new InvalidOperationException("Unknown IntUndefinedFalseUnion type"),
        };
    }
}

public record IntIntUndefinedFalseEmptyUnion(int Value) : IntUndefinedFalseEmptyUnion;
public record UndefinedIntUndefinedFalseEmptyUnion(Undefined Value) : IntUndefinedFalseEmptyUnion;
public record FalseIntUndefinedFalseEmptyUnion : IntUndefinedFalseEmptyUnion;
public record EmptyIntUndefinedFalseEmptyUnion(Empty Value) : IntUndefinedFalseEmptyUnion;
