using System.Collections.ObjectModel;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FormatClasses;

namespace ApogeeVGC.Data;

public record Formats
{
    public IReadOnlyDictionary<FormatId, Format> FormatData { get; }

    public Formats()
    {
        FormatData = new ReadOnlyDictionary<FormatId, Format>(_formatData);
    }

    private readonly Dictionary<FormatId, Format> _formatData = new()
    {
        [FormatId.Gen9Ou] = new Format
        {
            Name = "[Gen 9] OU",
            Ruleset = [RuleId.Standard], //TODO: fill in rules
            Banlist = [], //TODO: fill in bans
        },
        [FormatId.CustomSingles] = new Format
        {
            Name = "Custom Singles",
            Ruleset = [],
            Banlist = [],
            RuleTable = new RuleTable
            {
                PickedTeamSize = 6, // Full team preview - pick order of all 6
            },
        },
        [FormatId.CustomSinglesBlind] = new Format
        {
            Name = "Custom Singles (Blind)",
            Ruleset = [],
            Banlist = [],
            RuleTable = new RuleTable
            {
                PickedTeamSize = 0, // No team preview - team order is fixed
            },
        },
        [FormatId.CustomDoubles] = new Format
        {
            Name = "Custom Doubles",
            GameType = GameType.Doubles,
            Ruleset = [],
            Banlist = [],
            RuleTable = new RuleTable
            {
                PickedTeamSize = 4, // Doubles preview - pick 4 from 6
            },
        },
    };
}