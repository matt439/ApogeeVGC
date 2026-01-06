namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// string | number | Delegate | object
/// Represents arguments for the addMove method that can be strings, numbers, functions, or arbitrary objects.
/// Maps to TypeScript: (string | number | Function | AnyObject)
/// </summary>
public abstract record StringNumberDelegateObjectUnion
{
    public static implicit operator StringNumberDelegateObjectUnion(string value) =>
        new StringStringNumberDelegateObjectUnion(value);
    public static implicit operator StringNumberDelegateObjectUnion(int value) =>
     new IntStringNumberDelegateObjectUnion(value);
    public static implicit operator StringNumberDelegateObjectUnion(double value) =>
        new DoubleStringNumberDelegateObjectUnion(value);
    public static implicit operator StringNumberDelegateObjectUnion(Delegate del) =>
        new DelegateStringNumberDelegateObjectUnion(del);

    // Factory method for explicit object creation
    public static StringNumberDelegateObjectUnion FromObject(object obj) =>
    new ObjectStringNumberDelegateObjectUnion(obj);
}

public record StringStringNumberDelegateObjectUnion(string Value) : StringNumberDelegateObjectUnion;
public record IntStringNumberDelegateObjectUnion(int Value) : StringNumberDelegateObjectUnion;
public record DoubleStringNumberDelegateObjectUnion(double Value) : StringNumberDelegateObjectUnion;
public record DelegateStringNumberDelegateObjectUnion(Delegate Delegate) : StringNumberDelegateObjectUnion;
public record ObjectStringNumberDelegateObjectUnion(object Object) : StringNumberDelegateObjectUnion;
