using System.Data;

namespace ApogeeVGC.Sim.GameObjects;

public class RuleTable : Dictionary<RuleId, Rule>
{
    public int? PickedTeamSize { get; set; }

    public bool Has(RuleId rule)
    {
        throw new NotImplementedException();
    }

    public bool Has(AbilityId ability)
    {
        throw new NotImplementedException();
    }
}