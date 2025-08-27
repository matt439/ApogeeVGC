namespace ApogeeVGC.Sim;

public enum WeatherType
{
    None,
    HarshSunlight,
    Rain,
    Sandstorm,
    Snow,
}

public class Weather
{
    public WeatherType WeatherType { get; init; } = WeatherType.None;
    public int Duration { get; set; }
    public bool PrintDebug { get; init; }


    public Weather Copy()
    {
        return new Weather
        {
            WeatherType = WeatherType,
            Duration = Duration
        };
    }
}
public enum TerrainType
{
    None,
    Electric,
    Grassy,
    Misty,
    Psychic
}

public class Terrain
{
    public TerrainType TerrainType { get; init; } = TerrainType.None;
    public int Duration { get; set; }
    public bool PrintDebug { get; init; }

    public Terrain Copy()
    {
        return new Terrain
        {
            TerrainType = TerrainType,
            Duration = Duration
        };
    }
}

public class PseudoWeather
{
    public string Name { get; init; } = string.Empty;
    public int Duration { get; set; }
    public bool PrintDebug { get; init; }
    public PseudoWeather Copy()
    {
        return new PseudoWeather
        {
            Name = Name,
            Duration = Duration
        };
    }
}

public class Field
{
    public required Weather Weather { get; set; }
    public required Terrain Terrain { get; set; }

    public bool PringDebug { get; init; }
    
    /// <summary>
    /// Creates a copy of this Field for MCTS simulation purposes.
    /// Currently Field is empty, so just create a new instance.
    /// </summary>
    /// <returns>A new Field instance</returns>
    public Field Copy()
    {
        return new Field
        {
            Weather = Weather.Copy(),
            Terrain = Terrain.Copy(),
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
            Weather = new Weather { PrintDebug = printDebug },
            Terrain = new Terrain { PrintDebug = printDebug },
            PringDebug = printDebug,
        };
    }
}