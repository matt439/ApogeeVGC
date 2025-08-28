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
    public Action<Field, bool>? OnEnd { get; init; }
    public Action<Field, bool>? OnStart { get; init; }
    public Action<Pokemon, bool>? OnPokemonSwitchIn { get; init; }

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
    //Gravity,
    //MudSport,
    //WaterSport,
    // TODO: Add more pseudo-weathers as needed
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
    public Weather? Weather { get; private set; }
    public Terrain? Terrain { get; private set; }
    private List<PseudoWeather> PseudoWeatherList { get; init; } = [];
    public bool PringDebug { get; init; }
    public bool HasWeather => Weather != null;
    public bool HasTerrain => Terrain != null;

    public void AddWeather(Weather weather)
    {
        Weather?.OnEnd?.Invoke(this, PringDebug);
        Weather = weather;
        weather.OnStart?.Invoke(this, PringDebug);
    }

    public void RemoveWeather()
    {
        if (Weather == null) return;
        Weather.OnEnd?.Invoke(this, PringDebug);
        Weather = null;
    }

    public void AddTerrain(Terrain terrain)
    {
        Terrain?.OnEnd?.Invoke(this, PringDebug);
        Terrain = terrain;
        terrain.OnStart?.Invoke(this, PringDebug);
    }
    public void RemoveTerrain()
    {
        if (Terrain == null) return;
        Terrain.OnEnd?.Invoke(this, PringDebug);
        Terrain = null;
    }

    public bool HasPseudoWeather(PseudoWeatherId pseudoWeatherId)
    {
        return PseudoWeatherList.Any(pw => pw.PseudoWeatherId == pseudoWeatherId);
    }

    public PseudoWeather? GetPseudoWeather(PseudoWeatherId pseudoWeatherId)
    {
        return PseudoWeatherList.FirstOrDefault(pw => pw.PseudoWeatherId == pseudoWeatherId);
    }

    public void AddPseudoWeather(PseudoWeather pseudoWeather)
    {
        if (PseudoWeatherList.Any(pw => pw.PseudoWeatherId == pseudoWeather.PseudoWeatherId))
        {
            throw new InvalidOperationException($"PseudoWeather {pseudoWeather.PseudoWeatherId} is already" +
                                                $"active on the field.");
        }
        PseudoWeatherList.Add(pseudoWeather);
        pseudoWeather.OnStart?.Invoke(this, PringDebug);
    }

    public bool RemovePseudoWeather(PseudoWeatherId pseudoWeatherId)
    {
        PseudoWeather? pseudoWeather = PseudoWeatherList.FirstOrDefault(pw =>
            pw.PseudoWeatherId == pseudoWeatherId);
        if (pseudoWeather == null) return false;
        PseudoWeatherList.Remove(pseudoWeather);
        pseudoWeather.OnEnd?.Invoke(this, PringDebug);
        return true;
    }

    public void OnPokemonSwitchIn(Pokemon pokemon)
    {
        // Get all the OnPokemonSwitchIn actions from Weather, Terrain, and PseudoWeathers
        // Determine the order of execution from their priority (if any)
        // Execute them in order
        Weather?.OnPokemonSwitchIn?.Invoke(pokemon, PringDebug);
        Terrain?.OnPokemonSwitchIn?.Invoke(pokemon, PringDebug);
        foreach (PseudoWeather pw in PseudoWeatherList)
        {
            pw.OnPokemonSwitchIn?.Invoke(pokemon, PringDebug);
        }
    }

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
            PringDebug = printDebug,
        };
    }
}