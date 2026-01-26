using ApogeeVGC.Sim.Abilities;
using System.Data;

namespace ApogeeVGC.Sim.FormatClasses;

/// <summary>
/// Tracks rules, bans, and numeric settings for a format.
/// Mirrors the RuleTable from Pokemon Showdown's dex-formats.ts.
/// </summary>
public class RuleTable : Dictionary<RuleId, Rule>
{
    // Team composition settings
    public int MinTeamSize { get; set; } = 1;
    public int MaxTeamSize { get; set; } = 6;
    public int? PickedTeamSize { get; set; }
    public int? MaxTotalLevel { get; set; }

    // Level settings
    public int MinLevel { get; set; } = 1;
    public int MaxLevel { get; set; } = 100;
    public int DefaultLevel { get; set; } = 100;
    public int? AdjustLevel { get; set; }
    public int? AdjustLevelDown { get; set; }

    // Move and EV settings
    public int MaxMoveCount { get; set; } = 4;
    public int? EvLimit { get; set; }

    // Source gen (for legality checking)
    public int MinSourceGen { get; set; } = 1;

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