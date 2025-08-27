namespace ApogeeVGC.Sim;

public enum MoveSourceType
{
    Tm,
    Tutor,
    LevelUp,
    Restricted,
    Egg,
    DreamWorld,
    Event,
    Virtual,
    Chain
}

public class MoveSource
{
    public int Generation
    {
        get => field;
        init
        {
            if (value is < 1 or > 9)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Generation must be between 1 and 9.");
            }
            field = value;
        }
    }
    public MoveSourceType SourceType { get; init; }
    public string Details { get; init; }

    public MoveSource(int generation, MoveSourceType sourceType, string details = "")
    {
        Generation = generation;
        SourceType = sourceType;
        Details = details;
    }

    public MoveSource(int generation, string sourceType, string details = "")
    {
        Generation = generation;
        SourceType = StringToMoveSourceType(sourceType);
        Details = details;
    }

    public MoveSource(string code)
    {
        string firstChar = code[..1];
        if (!int.TryParse(firstChar, out int generation))
        {
            throw new ArgumentOutOfRangeException(nameof(code), "Invalid generation in move source code.");
        }
        Generation = generation;
        string sourceTypeStr = code.Substring(1, 1);
        SourceType = StringToMoveSourceType(sourceTypeStr);
        Details = code.Length > 2 ? code[2..] : string.Empty;
    }

    public override string ToString()
    {
        return $"{Generation}{MoveSourceTypeToString(SourceType)}{Details}";
    }

    private static string MoveSourceTypeToString(MoveSourceType sourceType)
    {
        return sourceType switch
        {
            MoveSourceType.Tm => "M",
            MoveSourceType.Tutor => "T",
            MoveSourceType.LevelUp => "L",
            MoveSourceType.Restricted => "R",
            MoveSourceType.Egg => "E",
            MoveSourceType.DreamWorld => "D",
            MoveSourceType.Event => "S",
            MoveSourceType.Virtual => "V",
            MoveSourceType.Chain => "C",
            _ => throw new ArgumentOutOfRangeException(nameof(sourceType), sourceType, null)
        };
    }
    private static MoveSourceType StringToMoveSourceType(string sourceType)
    {
        return sourceType switch
        {
            "M" => MoveSourceType.Tm,
            "T" => MoveSourceType.Tutor,
            "L" => MoveSourceType.LevelUp,
            "R" => MoveSourceType.Restricted,
            "E" => MoveSourceType.Egg,
            "D" => MoveSourceType.DreamWorld,
            "S" => MoveSourceType.Event,
            "V" => MoveSourceType.Virtual,
            "C" => MoveSourceType.Chain,
            _ => throw new ArgumentOutOfRangeException(nameof(sourceType), sourceType, null)
        };
    }
}

public enum GenderId
{
    M,
    F,
    N,
    Empty
}

public static class GenderIdExtensions
{
    public static string GenderIdString(this GenderId id)
    {
        return id switch
        {
            GenderId.M => "M",
            GenderId.F => "F",
            GenderId.N => string.Empty,
            GenderId.Empty => string.Empty,
            _ => throw new ArgumentOutOfRangeException(nameof(id), id, null)
        };
    }
}

public class EventInfo
{
    public int Generation { get; set; }
    public int? Level { get; set; }
    public bool? Shiny { get; set; } // true: always shiny, 1: sometimes shiny, false/null: never shiny
    public int? ShinySometimes { get; set; } // Use this if you need to distinguish '1'
    public GenderId? Gender { get; set; }
    public string? Nature { get; set; }
    public Dictionary<StatId, int>? Ivs { get; set; }
    public int? PerfectIvs { get; set; }
    public bool? IsHidden { get; set; }
    public List<AbilityId>? Abilities { get; set; }
    public int? MaxEggMoves { get; set; }
    public List<string>? Moves { get; set; }
    public string? Pokeball { get; set; }
    public string? From { get; set; }
    public bool? Japan { get; set; }
    public bool? EmeraldEventEgg { get; set; }
}

public record Learnset
{
    public Dictionary<string, List<MoveSource>>? LearnsetData { get; init; }
    public List<EventInfo>? EventData { get; init; }
    public bool? EventOnly { get; init; }
    public List<EventInfo>? Encounters { get; init; }
    public bool? Exists { get; init; }
}

public enum SpecieId
{
    CalyrexIce,
    Miraidon,
    Ursaluna,
    Volcarona,
    Grimmsnarl,
    IronHands,
}

public record SpeciesAbility
{
    public AbilityId Slot0 { get; init; }
    public AbilityId? Slot1 { get; init; }
    public AbilityId? Hidden { get; init; }
    public AbilityId? Special { get; init; }
}


public record Specie : IEffect
{
    public EffectType EffectType => EffectType.Specie;
    public int Num { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? BaseSpecies { get; init; }
    public string? Forme { get; init; }
    public List<PokemonType> Types { get; init; } = [];
    public GenderId Gender { get; init; }
    public StatsTable BaseStats { get; init; } = new();
    public SpeciesAbility Abilities { get; init; } = new();
    public double Height { get; init; } // in meters
    public double Weight { get; init; } // in kilograms
    public string Color { get; init; } = string.Empty;
    public int Gen { get; init; }
    // Egg groups
    // Changes from
    // Gender ratio
    // Prevo
    // EvoType
    // EboCondition
    // Other forms
    // FormeOrder

}