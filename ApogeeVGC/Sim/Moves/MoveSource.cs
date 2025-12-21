namespace ApogeeVGC.Sim.Moves;

/// <summary>
/// Represents a source from which a Pokemon can learn a move.
/// Based on Pokemon Showdown's MoveSource format (e.g., "9M", "8L40", "7S0").
/// </summary>
public readonly struct MoveSource : IEquatable<MoveSource>
{
    /// <summary>
    /// The generation in which this move source is available (1-9).
    /// </summary>
    public int Generation { get; }

    /// <summary>
    /// The type of move source (TM, Tutor, LevelUp, etc.).
    /// </summary>
    public MoveSourceType SourceType { get; }

    /// <summary>
    /// For LevelUp sources: the level at which the move is learned.
    /// For Event sources: the event index in the species' eventData array.
    /// Null for other source types.
    /// </summary>
    public int? LevelOrIndex { get; }

    /// <summary>
    /// Creates a MoveSource with full OOP parameters.
    /// </summary>
    public MoveSource(int generation, MoveSourceType sourceType, int? levelOrIndex = null)
    {
        if (generation is < 1 or > 9)
        {
            throw new ArgumentOutOfRangeException(nameof(generation), "Generation must be between 1 and 9.");
        }

        Generation = generation;
        SourceType = sourceType;
        LevelOrIndex = levelOrIndex;
    }

    /// <summary>
    /// Creates a MoveSource from a string code (e.g., "9M", "8L40", "7S0").
    /// Used for parsing TypeScript learnset data.
    /// </summary>
    public static MoveSource Parse(string code)
    {
        if (string.IsNullOrEmpty(code) || code.Length < 2)
        {
            throw new ArgumentException("Move source code must be at least 2 characters.", nameof(code));
        }

        if (!int.TryParse(code[..1], out int generation))
        {
            throw new ArgumentException("Invalid generation in move source code.", nameof(code));
        }

        var sourceType = CharToMoveSourceType(code[1]);
        int? levelOrIndex = null;

        if (code.Length > 2 && int.TryParse(code[2..], out int parsed))
        {
            levelOrIndex = parsed;
        }

        return new MoveSource(generation, sourceType, levelOrIndex);
    }

    /// <summary>
    /// Returns the string representation in Pokemon Showdown format.
    /// </summary>
    public override string ToString()
    {
        var typeChar = MoveSourceTypeToChar(SourceType);
        return LevelOrIndex.HasValue 
            ? $"{Generation}{typeChar}{LevelOrIndex.Value}" 
            : $"{Generation}{typeChar}";
    }

    public bool Equals(MoveSource other)
    {
        return Generation == other.Generation && 
               SourceType == other.SourceType && 
               LevelOrIndex == other.LevelOrIndex;
    }

    public override bool Equals(object? obj) => obj is MoveSource other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Generation, SourceType, LevelOrIndex);

    public static bool operator ==(MoveSource left, MoveSource right) => left.Equals(right);

    public static bool operator !=(MoveSource left, MoveSource right) => !left.Equals(right);

    private static char MoveSourceTypeToChar(MoveSourceType sourceType)
    {
        return sourceType switch
        {
            MoveSourceType.Tm => 'M',
            MoveSourceType.Tutor => 'T',
            MoveSourceType.LevelUp => 'L',
            MoveSourceType.Restricted => 'R',
            MoveSourceType.Egg => 'E',
            MoveSourceType.DreamWorld => 'D',
            MoveSourceType.Event => 'S',
            MoveSourceType.Virtual => 'V',
            MoveSourceType.Chain => 'C',
            _ => throw new ArgumentOutOfRangeException(nameof(sourceType), sourceType, null)
        };
    }

    private static MoveSourceType CharToMoveSourceType(char c)
    {
        return c switch
        {
            'M' => MoveSourceType.Tm,
            'T' => MoveSourceType.Tutor,
            'L' => MoveSourceType.LevelUp,
            'R' => MoveSourceType.Restricted,
            'E' => MoveSourceType.Egg,
            'D' => MoveSourceType.DreamWorld,
            'S' => MoveSourceType.Event,
            'V' => MoveSourceType.Virtual,
            'C' => MoveSourceType.Chain,
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, $"Unknown move source type: {c}")
        };
    }
}