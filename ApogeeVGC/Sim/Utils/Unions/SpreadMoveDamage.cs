namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// (int | bool | undefined)[]
/// </summary>
public class SpreadMoveDamage : List<BoolIntUndefinedUnion>
{
    public SpreadMoveDamage()
    {
    }

    public SpreadMoveDamage(int capacity) : base(capacity)
    {
    }

    public SpreadMoveDamage(SpreadMoveDamage other) : base(other.Count)
    {
        foreach (BoolIntUndefinedUnion item in other)
        {
    Add(item);
        }
    }
}
