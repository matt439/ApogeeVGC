using ApogeeVGC.Sim.Items;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// Item | false
/// </summary>
public abstract record ItemFalseUnion
{
    public static implicit operator ItemFalseUnion(Item item) => new ItemItemFalseUnion(item);
public static ItemFalseUnion FromFalse() => new FalseItemFalseUnion();
}

public record ItemItemFalseUnion(Item Item) : ItemFalseUnion;
public record FalseItemFalseUnion : ItemFalseUnion;
