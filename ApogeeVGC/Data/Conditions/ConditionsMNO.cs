using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;

namespace ApogeeVGC.Data.Conditions;

public partial record Conditions
{
    private partial Dictionary<ConditionId, Condition> CreateConditionsMNO()
    {
        return new Dictionary<ConditionId, Condition>
        {
            [ConditionId.None] = new()
            {
                Id = ConditionId.None,
                Name = "None",
                EffectType = EffectType.Condition,
            },
        };
    }
}
