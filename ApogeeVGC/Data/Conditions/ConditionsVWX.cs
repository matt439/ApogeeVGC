using ApogeeVGC.Sim.Conditions;

namespace ApogeeVGC.Data.Conditions;

public partial record Conditions
{
    private partial Dictionary<ConditionId, Condition> CreateConditionsVWX()
    {
        return new Dictionary<ConditionId, Condition>
        {
            // Placeholder for conditions V-X
        };
    }
}
