namespace ApogeeVGC.Sim;

/// <summary>
/// Base class for field elements like Weather, Terrain, and Pseudo-Weather.
/// </summary>
public class FieldElement
{
    public required string Name { get; init; }
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
    public Action<Pokemon[], BattleContext>? OnEnd { get; init; }
    public Action<Pokemon[], BattleContext>? OnStart { get; init; }
    public Action<Field, Pokemon[], BattleContext>? OnReapply { get; init; }
    public Action<Pokemon[], FieldElement, BattleContext>? OnIncrementTurnCounter { get; init; }
    public Action<Pokemon, BattleContext>? OnPokemonSwitchIn { get; init; }

    public void IncrementTurnCounter()
    {
        //if (RemainingTurns > 0)
        //{
        //    ElapsedTurns++;
        //}
        //else
        //{
        //    throw new InvalidOperationException("Cannot increment turn counter beyond duration.");
        //}
        ElapsedTurns++;
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
    public required WeatherId Id { get; init; }
    
    public Weather Copy()
    {
        return new Weather
        {
            Id = Id,
            Name = Name,
            IsExtended = IsExtended,
            BaseDuration = BaseDuration,
            DurationExtension = DurationExtension,
            ElapsedTurns = ElapsedTurns,
            PrintDebug = PrintDebug, // Added missing PrintDebug
            // Note: Action delegates (OnEnd, OnStart, etc.) are shared immutable references
            // since they don't contain mutable state - they're function pointers
            OnEnd = OnEnd,
            OnStart = OnStart,
            OnReapply = OnReapply,
            OnIncrementTurnCounter = OnIncrementTurnCounter,
            OnPokemonSwitchIn = OnPokemonSwitchIn,
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
    public required TerrainId Id { get; init; }

    public Terrain Copy()
    {
        return new Terrain
        {
            Id = Id,
            Name = Name,
            IsExtended = IsExtended,
            BaseDuration = BaseDuration,
            DurationExtension = DurationExtension,
            ElapsedTurns = ElapsedTurns,
            PrintDebug = PrintDebug, // Added missing PrintDebug
            // Note: Action delegates are shared immutable references
            OnEnd = OnEnd,
            OnStart = OnStart,
            OnReapply = OnReapply,
            OnIncrementTurnCounter = OnIncrementTurnCounter,
            OnPokemonSwitchIn = OnPokemonSwitchIn,
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
    public required PseudoWeatherId Id { get; init; }
    public PseudoWeather Copy()
    {
        return new PseudoWeather
        {
            Id = Id,
            Name = Name,
            IsExtended = IsExtended,
            BaseDuration = BaseDuration,
            DurationExtension = DurationExtension,
            ElapsedTurns = ElapsedTurns,
            PrintDebug = PrintDebug, // Added missing PrintDebug
            // Note: Action delegates are shared immutable references
            OnEnd = OnEnd,
            OnStart = OnStart,
            OnReapply = OnReapply,
            OnIncrementTurnCounter = OnIncrementTurnCounter,
            OnPokemonSwitchIn = OnPokemonSwitchIn,
        };
    }
}

public class Field
{
    public Weather? Weather { get; private set; }
    public Terrain? Terrain { get; private set; }
    public bool HasAnyWeather => Weather != null;
    public bool HasAnyTerrain => Terrain != null;
    public List<PseudoWeather> PseudoWeatherList { get; init; } = [];

    public bool HasWeather(WeatherId weatherId)
    {
        return Weather?.Id == weatherId;
    }

    public void AddWeather(Weather weather, Pokemon[] pokemon, BattleContext battleContext)
    {
        Weather?.OnEnd?.Invoke(pokemon, battleContext);
        Weather = weather;
        weather.OnStart?.Invoke(pokemon, battleContext);
    }

    public void RemoveWeather(Pokemon[] pokemon, BattleContext battleContext)
    {
        if (Weather == null) return;
        Weather.OnEnd?.Invoke(pokemon, battleContext);
        Weather = null;
    }

    public void ReapplyWeather(Pokemon[] pokemon, BattleContext battleContext)
    {
        if (Weather is null)
        {
            throw new InvalidOperationException("Cannot reapply weather when no weather is active on the field.");
        }
        Weather.OnReapply?.Invoke(this, pokemon, battleContext);
    }

    public bool HasTerrain(TerrainId terrainId)
    {
        return Terrain?.Id == terrainId;
    }

    public void AddTerrain(Terrain terrain, Pokemon[] pokemon, BattleContext battleContext)
    {
        Terrain?.OnEnd?.Invoke(pokemon, battleContext);
        Terrain = terrain;
        terrain.OnStart?.Invoke(pokemon, battleContext);
    }
    public void RemoveTerrain(Pokemon[] pokemon, BattleContext battleContext)
    {
        if (Terrain == null) return;
        Terrain.OnEnd?.Invoke(pokemon, battleContext);
        Terrain = null;
    }

    public void ReapplyTerrain(Pokemon[] pokemon, BattleContext battleContext)
    {
        if (Terrain is null)
        {
            throw new InvalidOperationException("Cannot reapply terrain when no terrain is active on the field.");
        }
        Terrain.OnReapply?.Invoke(this, pokemon, battleContext);
    }

    public bool HasPseudoWeather(PseudoWeatherId pseudoWeatherId)
    {
        return PseudoWeatherList.Any(pw => pw.Id == pseudoWeatherId);
    }

    public PseudoWeather? GetPseudoWeather(PseudoWeatherId pseudoWeatherId)
    {
        return PseudoWeatherList.FirstOrDefault(pw => pw.Id == pseudoWeatherId);
    }

    public void AddPseudoWeather(PseudoWeather pseudoWeather, Pokemon[] pokemon, BattleContext battleContext)
    {
        if (PseudoWeatherList.Any(pw => pw.Id == pseudoWeather.Id))
        {
            throw new InvalidOperationException($"PseudoWeather {pseudoWeather.Id} is already" +
                                                $"active on the field.");
        }
        PseudoWeatherList.Add(pseudoWeather);
        pseudoWeather.OnStart?.Invoke(pokemon, battleContext);
    }

    public bool RemovePseudoWeather(PseudoWeatherId pseudoWeatherId, Pokemon[] pokemon,
        BattleContext battleContext)
    {
        PseudoWeather? pseudoWeather = PseudoWeatherList.FirstOrDefault(pw =>
            pw.Id == pseudoWeatherId);
        if (pseudoWeather == null) return false;
        PseudoWeatherList.Remove(pseudoWeather);
        pseudoWeather.OnEnd?.Invoke(pokemon, battleContext);
        return true;
    }

    public void ReapplyPseudoWeather(PseudoWeatherId pseudoWeatherId, Pokemon[] pokemon,
        BattleContext battleContext)
    {
        PseudoWeather? pseudoWeather = PseudoWeatherList.FirstOrDefault(pw =>
            pw.Id == pseudoWeatherId);
        if (pseudoWeather is null)
        {
            throw new InvalidOperationException($"Cannot reapply pseudo-weather {pseudoWeatherId}" +
                                                $"when it is not active on the field.");
        }
        pseudoWeather.OnReapply?.Invoke(this, pokemon, battleContext);
    }

    public void OnPokemonSwitchIn(Pokemon pokemon, BattleContext battleContext)
    {
        // Get all the OnPokemonSwitchIn actions from Weather, Terrain, and PseudoWeathers
        // Determine the order of execution from their priority (if any)
        // Execute them in order
        Weather?.OnPokemonSwitchIn?.Invoke(pokemon, battleContext);
        Terrain?.OnPokemonSwitchIn?.Invoke(pokemon, battleContext);
        foreach (PseudoWeather pw in PseudoWeatherList)
        {
            pw.OnPokemonSwitchIn?.Invoke(pokemon, battleContext);
        }
    }

    public void OnTurnEnd(Pokemon[] pokemon, BattleContext battleContext)
    {
        Weather?.IncrementTurnCounter();
        Weather?.OnIncrementTurnCounter?.Invoke(pokemon, Weather, battleContext);
        if (Weather?.IsExpired == true)
        {
            RemoveWeather(pokemon, battleContext);
        }

        Terrain?.IncrementTurnCounter();
        Terrain?.OnIncrementTurnCounter?.Invoke(pokemon, Terrain, battleContext);
        if (Terrain?.IsExpired == true)
        {
            RemoveTerrain(pokemon, battleContext);
        }

        foreach (PseudoWeather pw in PseudoWeatherList.ToList())
        {
            pw.IncrementTurnCounter();
            pw.OnIncrementTurnCounter?.Invoke(pokemon, pw, battleContext);
            if (pw.IsExpired)
            {
                RemovePseudoWeather(pw.Id, pokemon, battleContext);
            }
        }
    }

    public Field Copy()
    {
        return new Field
        {
            Weather = Weather?.Copy(),
            Terrain = Terrain?.Copy(),
            PseudoWeatherList = PseudoWeatherList.Select(pw => pw.Copy()).ToList(),
        };
    }
}

public static class FieldGenerator
{
    public static Field GenerateTestField()
    {
        return new Field();
    }
}