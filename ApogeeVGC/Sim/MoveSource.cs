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
    Chain,
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