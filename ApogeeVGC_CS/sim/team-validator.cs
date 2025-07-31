namespace ApogeeVGC_CS.sim
{
    public enum SourceType
    {
        E, // Egg
        S, // Event
        D, // Dream World
        V, // Virtual Console or Let's Go transfer
    }

    public class PokemonSource
    {
        public required int Generation 
        {
            get;
            init // 1-9
            {
                if (value is < 1 or > 9)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Generation must be between 1 and 9.");
                }
                field = value;
            }
        }
        public required SourceType Type { get; init; }
        public string? Source { get; init; }
    }

    public class PokemonSources(int sourcesBefore = 0, int sourcesAfter = 0)
    {
        public required List<PokemonSource> Sources { get; init; } = [];
        public required int SourcesBefore { get; init; } = sourcesBefore;
        public required int SourcesAfter { get; init; } = sourcesAfter;
        public bool? IsHidden { get; init; } = null;
        public List<Id>? LimitedEggMoves { get; init; } = null;
        public List<Id>? PossiblyLimitedEggMoves { get; init; }
        public List<Id>? TradebackLimitedEggMoves { get; init; }
        public List<Id>? LevelUpEggMoves { get; init; }
        public List<Id>? PomegEggMoves { get; init; }
        public string? PomegEventEgg { get; init; }
        public int? EventOnlyMinSourceGen { get; init; }
        public List<string>? LearnsetDomain { get; init; }
        public int MoveEvoCarryCount { get; init; } = 0;
        public string? BabyOnly { get; init; }
        public string? SketchMove { get; init; }
        public required int DreamWorldMoveCount { get; init; } = 0;
        public string? Hm { get; init; }
        public bool? IsFromPokemonGo { get; init; }
        public string? PokemonGoSource { get; init; }
        public List<string>? RestrictiveMoves { get; init; }
        public Id? RestrictedMove { get; init; }

        public int Size()
        {
            throw new NotImplementedException("size method is not implemented yet.");
        }

        public void Add(PokemonSource source, Id? limitedEggMove = null)
        {
            throw new NotImplementedException("Add method is not implemented yet.");
        }

        public void AddGen(int sourceGen)
        {
            throw new NotImplementedException("AddGen method is not implemented yet.");
        }

        public int MinSourceGen()
        {
            throw new NotImplementedException("MinSourceGen method is not implemented yet.");
        }

        public int MaxSourceGen()
        {
            throw new NotImplementedException("MinSourceGen method is not implemented yet.");
        }

        public void IntersectWith(PokemonSource other)
        {
            throw new NotImplementedException();
        }
    }

    public class TeamValidator
    {
        public required Format Format { get; init; }
        public required ModdedDex Dex { get; init; }
        public required int Gen { get; init; }
        public required RuleTable RuleTable { get; init; }
        public required int MinSourceGen { get; init; }
        //public Func<object, string> ToId { get; }

        public TeamValidator(string format, ModdedDex dex)
            : this(dex.Formats.Get(format), dex) { }

        public TeamValidator(Format format, ModdedDex dex)
        {
            Format = format;
            if (Format.EffectType != EffectType.Format)
            {
                throw new ArgumentException($"format should be a 'Format', but was a '{Format.EffectType}'");
            }
            Dex = dex.ForFormat(Format);
            Gen = Dex.Gen;
            RuleTable = Dex.Formats.GetRuleTable(Format);
            MinSourceGen = RuleTable.MinSourceGen;
        }
    }
}