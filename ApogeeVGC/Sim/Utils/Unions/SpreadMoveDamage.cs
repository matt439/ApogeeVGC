namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// (int | bool | undefined)[]
/// </summary>
public class SpreadMoveDamage : List<BoolIntUndefinedUnion>
{
    public SpreadMoveDamage()
    {
    }

    public SpreadMoveDamage(SpreadMoveDamage other)
    {
        foreach (BoolIntUndefinedUnion item in other)
        {
    Add(item);
        }
    }
}
