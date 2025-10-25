using System.Collections.ObjectModel;
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
    };
}