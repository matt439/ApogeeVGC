using System.Collections.ObjectModel;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FormatClasses;

namespace ApogeeVGC.Data;


public record Rulesets
{
    public IReadOnlyDictionary<RuleId, Format> RulesetData { get; }

    public Rulesets()
    {
        RulesetData = new ReadOnlyDictionary<RuleId, Format>(_rulesetData);
    }

    private readonly Dictionary<RuleId, Format> _rulesetData = new()
    {
        [RuleId.Standard] = new Format
        {
            FormatEffectType = FormatEffectType.ValidatorRule,
            Name = "Standard",
            Desc = "The standard ruleset for all official Smogon singles tiers (Ubers, OU, etc.)",
            Ruleset = [], //TODO: fill in rules
        },
    };
}