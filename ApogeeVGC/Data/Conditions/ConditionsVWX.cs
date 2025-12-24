using ApogeeVGC.Sim.Conditions;

namespace ApogeeVGC.Data.Conditions;

public partial record Conditions
{
    private partial Dictionary<ConditionId, Condition> CreateConditionsVwx()
    {
        return new Dictionary<ConditionId, Condition>
        {
            // Placeholder for conditions V-X
        };
    }
}
