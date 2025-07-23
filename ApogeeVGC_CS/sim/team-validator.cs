using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApogeeVGC_CS.sim
{
    // Describes a possible way to get a Pokémon (e.g., "2Eparent", "5D", "7V")
    public class PokemonSource
    {
        public string Value { get; set; } = string.Empty;
        public PokemonSource(string value) { Value = value; }
        public override string ToString() => Value;
    }

    public class PokemonSources
    {
        // Specific possible sources
        public List<PokemonSource> Sources { get; set; } = new();
        // If nonzero, allows all sources from this gen and earlier
        public int SourcesBefore { get; set; }
        // Requires sources from this gen or later
        public int SourcesAfter { get; set; }
        public bool? IsHidden { get; set; }

        // Optional lists for egg/event/tradeback moves, etc.
        public List<string>? LimitedEggMoves { get; set; }
        public List<string>? PossiblyLimitedEggMoves { get; set; }
        public List<string>? TradebackLimitedEggMoves { get; set; }
        public List<string>? LevelUpEggMoves { get; set; }
        public List<string>? PomegEggMoves { get; set; }
        public string? PomegEventEgg { get; set; }
        public int? EventOnlyMinSourceGen { get; set; }
        public List<string>? LearnsetDomain { get; set; }
        public int MoveEvoCarryCount { get; set; }
        public string? BabyOnly { get; set; }
        public string? SketchMove { get; set; }
        public int DreamWorldMoveCount { get; set; }
        public string? Hm { get; set; }
        public bool? IsFromPokemonGo { get; set; }
        public string? PokemonGoSource { get; set; }
        public List<string>? RestrictiveMoves { get; set; }
        public string? RestrictedMove { get; set; }

        public PokemonSources(int sourcesBefore = 0, int sourcesAfter = 0)
        {
            Sources = new List<PokemonSource>();
            SourcesBefore = sourcesBefore;
            SourcesAfter = sourcesAfter;
            IsHidden = null;
            LimitedEggMoves = null;
            MoveEvoCarryCount = 0;
            DreamWorldMoveCount = 0;
        }
    }

    public class TeamValidator
    {
        public Format Format { get; }
        public ModdedDex Dex { get; }
        public int Gen { get; }
        public RuleTable RuleTable { get; }
        public int MinSourceGen { get; }
        public Func<object, string> ToID { get; }

        public TeamValidator(object format, ModdedDex dex)
        {
            Format = dex.Formats.Get(format);
            if (Format.EffectType != "Format")
            {
                throw new ArgumentException($"format should be a 'Format', but was a '{Format.EffectType}'");
            }
            Dex = dex.ForFormat(Format);
            Gen = Dex.Gen;
            RuleTable = Dex.Formats.GetRuleTable(Format);
            MinSourceGen = RuleTable.MinSourceGen;
            ToID = Dex.ToID;
        }
    }
}