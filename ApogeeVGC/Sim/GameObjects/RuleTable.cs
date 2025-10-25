using System.Data;

namespace ApogeeVGC.Sim.GameObjects;

public class RuleTable : Dictionary<RuleId, Rule>
{
    public int? PickedTeamSize { get; set; }

    public bool Has(RuleId rule)
    {
        return ContainsKey(rule);
    }

    public bool Has(AbilityId ability)
    {
        // Check if there's a rule that corresponds to banning this ability
        // In Pokemon Showdown, banned abilities are stored as rules with the same name
        // We need to convert the AbilityId to a RuleId by finding a rule with the same name
        // For now, we'll check if a RuleId exists with the ability's name

        // Try to parse the ability ID as a rule ID
        if (Enum.TryParse(ability.ToString(), out RuleId ruleId))
        {
            return ContainsKey(ruleId);
        }

        return false;
    }
}