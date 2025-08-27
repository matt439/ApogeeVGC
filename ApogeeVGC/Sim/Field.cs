namespace ApogeeVGC.Sim;

/// <summary>
/// Base class for field elements like Weather, Terrain, and Pseudo-Weather.
/// </summary>
public class FieldElement
{
    public required int BaseDuration
    {
        get;
        init
        {
            if (value < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "BaseDuration must be at least 1.");
            }
            field = value;
        }
    }
    public bool IsExtended { get; set; }
    public required int DurationExtension
    {
        get;
        init
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "DurationExtension cannot be negative.");
            }
            field = value;
        }
    }
    public int Duration => IsExtended ? BaseDuration + DurationExtension : BaseDuration;
    public int RemainingTurns => Duration - ElapsedTurns;
    public bool IsExpired => RemainingTurns <= 0;
    public bool PrintDebug { get; init; }
    protected int ElapsedTurns { get; set; }

    public void IncrementTurnCounter()
    {
        if (RemainingTurns > 0)
        {
            ElapsedTurns++;
        }
        else
        {
            throw new InvalidOperationException("Cannot increment turn counter beyond duration.");
        }
    }

    public void ResetTurnCounter()
    {
        ElapsedTurns = 0;
    }
}

public enum WeatherId
{
    HarshSunlight,
    Rain,
    Sandstorm,
    Snow,
}

public class Weather : FieldElement
{
    public required WeatherId WeatherId { get; init; }
    
    public Weather Copy()
    {
        return new Weather
        {
            WeatherId = WeatherId,
            IsExtended = IsExtended,
            BaseDuration = BaseDuration,
            DurationExtension = DurationExtension,
            ElapsedTurns = ElapsedTurns,
        };
    }
}

public enum TerrainId
{
    Electric,
    Grassy,
    Misty,
    Psychic,
}

public class Terrain : FieldElement
{
    public required TerrainId TerrainId { get; init; }

    public Terrain Copy()
    {
        return new Terrain
        {
            TerrainId = TerrainId,
            IsExtended = IsExtended,
            BaseDuration = BaseDuration,
            DurationExtension = DurationExtension,
            ElapsedTurns = ElapsedTurns,
        };
    }
}

public enum PseudoWeatherId
{
    TrickRoom,
}

public class PseudoWeather : FieldElement
{
    public required PseudoWeatherId PseudoWeatherId { get; init; }
    public PseudoWeather Copy()
    {
        return new PseudoWeather
        {
            PseudoWeatherId = PseudoWeatherId,
            IsExtended = IsExtended,
            BaseDuration = BaseDuration,
            DurationExtension = DurationExtension,
            ElapsedTurns = ElapsedTurns,
        };
    }
}

public class Field
{
    public Weather? Weather { get; set; }
    public Terrain? Terrain { get; set; }
    public List<PseudoWeather> PseudoWeatherList { get; set; } = [];
    public bool PringDebug { get; init; }
    public bool HasWeather => Weather != null;
    public bool HasTerrain => Terrain != null;
    public bool HasPseudoWeather => PseudoWeatherList.Count > 0;

    public Field Copy()
    {
        return new Field
        {
            Weather = Weather?.Copy(),
            Terrain = Terrain?.Copy(),
            PseudoWeatherList = PseudoWeatherList.Select(pw => pw.Copy()).ToList(),
            PringDebug = PringDebug
        };
    }
}

public static class FieldGenerator
{
    public static Field GenerateTestField(bool printDebug = false)
    {
        return new Field
        {
            Weather = null,
            Terrain = null,
            PseudoWeatherList = [],
            PringDebug = printDebug,
        };
    }
}