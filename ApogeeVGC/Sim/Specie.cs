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

public enum GenderName
{
    M,
    F,
    N,
    Empty
}

public class EventInfo
{
    public int Generation { get; set; }
    public int? Level { get; set; }
    public bool? Shiny { get; set; } // true: always shiny, 1: sometimes shiny, false/null: never shiny
    public int? ShinySometimes { get; set; } // Use this if you need to distinguish '1'
    public GenderName? Gender { get; set; }
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

public class StatsTable
{
    public int Hp
    {
        get;
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("value must be positive.");
            }

            field = value;
        }
    }
    public int Atk
    {
        get;
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("value must be positive.");
            }
            field = value;
        }
    } = 0;
    public int Def
    {
        get;
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("value must be positive.");
            }
            field = value;
        }
    } = 0;
    public int SpA
    {
        get;
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("value must be positive.");
            }
            field = value;
        }
    } = 0;
    public int SpD
    {
        get;
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("value must be positive.");
            }
            field = value;
        }
    } = 0;
    public int Spe
    {
        get;
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("value must be positive.");
            }
            field = value;
        }
    } = 0;

    public int BaseStatTotal => Hp + Atk + Def + SpA + SpD + Spe;

    public int GetStat(StatId stat)
    {
        return stat switch
        {
            StatId.Hp => Hp,
            StatId.Atk => Atk,
            StatId.Def => Def,
            StatId.SpA => SpA,
            StatId.SpD => SpD,
            StatId.Spe => Spe,
            _ => throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID.")
        };
    }

    public void SetStat(StatId stat, int value)
    {
        switch (stat)
        {
            case StatId.Hp:
                Hp = value;
                break;
            case StatId.Atk:
                Atk = value;
                break;
            case StatId.Def:
                Def = value;
                break;
            case StatId.SpA:
                SpA = value;
                break;
            case StatId.SpD:
                SpD = value;
                break;
            case StatId.Spe:
                Spe = value;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID.");
        }
    }

    public static bool IsValidIv(int stat)
    {
        return stat is >= 0 and <= 31;
    }

    public static bool IsValidEv(int stat)
    {
        return stat is >= 0 and <= 255;
    }

    public bool IsValidIvs()
    {
        return IsValidIv(Hp) && IsValidIv(Atk) && IsValidIv(Def) &&
               IsValidIv(SpA) && IsValidIv(SpD) && IsValidIv(Spe);
    }

    public bool IsValidEvs()
    {
        return IsValidEv(Hp) && IsValidEv(Atk) && IsValidEv(Def) &&
               IsValidEv(SpA) && IsValidEv(SpD) && IsValidEv(Spe) &&
               BaseStatTotal <= 510;
    }

    public static StatsTable PerfectIvs => new()
    {
        Hp = 31,
        Atk = 31,
        Def = 31,
        SpA = 31,
        SpD = 31,
        Spe = 31,
    };

    public StatsTable(StatsTable other)
    {
        Hp = other.Hp;
        Atk = other.Atk;
        Def = other.Def;
        SpA = other.SpA;
        SpD = other.SpD;
        Spe = other.Spe;
    }

    public StatsTable()
    {
    }
}

public record SpeciesAbility
{
    public AbilityId Slot0 { get; init; }
    public AbilityId? Slot1 { get; init; }
    public AbilityId? Hidden { get; init; }
    public AbilityId? Special { get; init; }
}


public record Specie
{
    public int Num { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? BaseSpecies { get; init; }
    public string? Forme { get; init; }
    public List<PokemonType> Types { get; init; } = [];
    public GenderName Gender { get; init; }
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