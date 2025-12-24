using ApogeeVGC.Sim.Conditions;

namespace ApogeeVGC.Data.Conditions;

public partial record Conditions
{
    private partial Dictionary<ConditionId, Condition> CreateConditionsYz()
    {
        return new Dictionary<ConditionId, Condition>
        {
            // Placeholder for conditions Y-Z
        };
    }
}
