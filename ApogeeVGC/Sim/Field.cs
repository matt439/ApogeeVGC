using ApogeeVGC.Player;

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
    /// <summary>
    /// target, source, effect. If returns true, duration is extended.
    /// </summary>
    public Func<Pokemon?, Pokemon, IEffect, bool>? DurationCallback { get; init; }
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
            DurationCallback = DurationCallback,
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
            DurationCallback = DurationCallback,
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
            DurationCallback = DurationCallback,
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

public enum SideConditionId
{
    Tailwind,
    Reflect,
    LightScreen,
}

public class SideCondition : FieldElement
{
    public required SideConditionId Id { get; init; }
    public required int OnSideResidualOrder { get; init; }
    public required int OnSideResidualSubOrder { get; init; }
    /// <summary>
    /// side, context
    /// </summary>
    public Action<Side, BattleContext>? OnSideStart { get; init; }
    /// <summary>
    /// side, context
    /// </summary>
    public Action<Side, BattleContext>? OnSideEnd { get; init; }
    /// <summary>
    /// pokemon, context
    /// </summary>
    public Action<Pokemon, BattleContext>? OnSidePokemonSwitchIn { get; init; }

    public SideCondition Copy()
    {
        return new SideCondition
        {
            Id = Id,
            OnSideResidualOrder = OnSideResidualOrder,
            OnSideResidualSubOrder = OnSideResidualSubOrder,
            OnSideStart = OnSideStart,
            OnSideEnd = OnSideEnd,
            Name = Name,
            IsExtended = IsExtended,
            BaseDuration = BaseDuration,
            DurationExtension = DurationExtension,
            DurationCallback = DurationCallback,
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
    public bool HasAnyPseudoWeather => PseudoWeatherList.Count > 0;
    public List<SideCondition> Side1Conditions { get; init; } = [];
    public bool HasAnySide1Conditions => Side1Conditions.Count > 0;
    public List<SideCondition> Side2Conditions { get; init; } = [];
    public bool HasAnySide2Conditions => Side2Conditions.Count > 0;

    public bool HasWeather(WeatherId weatherId)
    {
        return Weather?.Id == weatherId;
    }

    public void AddWeather(Weather weather, Pokemon sourcePokemon, IEffect sourceEffect,
        Pokemon[] pokemon, BattleContext battleContext)
    {
        Weather?.OnEnd?.Invoke(pokemon, battleContext);
        Weather = weather.Copy();
        Weather.IsExtended = Weather.DurationCallback?.Invoke(null, sourcePokemon, sourceEffect) ?? false;
        Weather.OnStart?.Invoke(pokemon, battleContext);
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

    public void AddTerrain(Terrain terrain, Pokemon sourcePokemon, IEffect sourceEffect,
        Pokemon[] pokemon, BattleContext battleContext)
    {
        Terrain?.OnEnd?.Invoke(pokemon, battleContext);
        Terrain = terrain.Copy();
        Terrain.IsExtended = Terrain.DurationCallback?.Invoke(null, sourcePokemon, sourceEffect) ?? false;
        Terrain.OnStart?.Invoke(pokemon, battleContext);
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

    public void AddPseudoWeather(PseudoWeather pseudoWeather, Pokemon sourcePokemon, IEffect sourceEffect,
        Pokemon[] pokemon, BattleContext battleContext)
    {
        if (PseudoWeatherList.Any(pw => pw.Id == pseudoWeather.Id))
        {
            throw new InvalidOperationException($"PseudoWeather {pseudoWeather.Id} is already" +
                                                $"active on the field.");
        }
        PseudoWeather pseudoWeatherCopy = pseudoWeather.Copy();
        PseudoWeatherList.Add(pseudoWeatherCopy);
        pseudoWeatherCopy.IsExtended = pseudoWeatherCopy.DurationCallback?.Invoke(null, sourcePokemon, sourceEffect)
                                       ?? false;
        pseudoWeatherCopy.OnStart?.Invoke(pokemon, battleContext);
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

    public bool HasSideCondition(SideConditionId sideConditionId, PlayerId playerId)
    {
        switch (playerId)
        {
            case PlayerId.Player1:
                return Side1Conditions.Any(sc => sc.Id == sideConditionId);
            case PlayerId.Player2:
                return Side2Conditions.Any(sc => sc.Id == sideConditionId);
            case PlayerId.None:
            default:
                throw new ArgumentOutOfRangeException(nameof(playerId), "Invalid PlayerId.");
        }
    }

    public SideCondition? GetSideCondition(SideConditionId sideConditionId, PlayerId playerId)
    {
        return playerId switch
        {
            PlayerId.Player1 => Side1Conditions.FirstOrDefault(sc => sc.Id == sideConditionId),
            PlayerId.Player2 => Side2Conditions.FirstOrDefault(sc => sc.Id == sideConditionId),
            PlayerId.None => throw new ArgumentOutOfRangeException(nameof(playerId), "Invalid PlayerId."),
            _ => null,
        };
    }

    public void AddSideCondition(SideCondition sideCondition, Side side, Pokemon sourcePokemon,
        IEffect sourceEffect, BattleContext battleContext)
    {
        List<SideCondition>? sideConditions;
        switch (side.PlayerId)
        {
            case PlayerId.Player1:
                sideConditions = Side1Conditions;
                break;
            case PlayerId.Player2:
                sideConditions = Side2Conditions;
                break;
            case PlayerId.None:
            default:
                throw new ArgumentOutOfRangeException(nameof(side.PlayerId), "Invalid PlayerId.");
        }
        if (sideConditions.Any(sc => sc.Id == sideCondition.Id))
        {
            throw new InvalidOperationException($"SideCondition {sideCondition.Id} is already" +
                                                $"active on the side.");
        }
        SideCondition sideConditionCopy = sideCondition.Copy();
        sideConditions.Add(sideConditionCopy);
        sideConditionCopy.IsExtended = sideConditionCopy.DurationCallback?.Invoke(null, sourcePokemon, sourceEffect)
            ?? false;
        sideConditionCopy.OnSideStart?.Invoke(side, battleContext);
        sideConditionCopy.OnStart?.Invoke(side.Team.AllActivePokemon, battleContext);
    }

    public bool RemoveSideCondition(SideConditionId sideConditionId, Side side, BattleContext battleContext)
    {
        List<SideCondition>? sideConditions;
        switch (side.PlayerId)
        {
            case PlayerId.Player1:
                sideConditions = Side1Conditions;
                break;
            case PlayerId.Player2:
                sideConditions = Side2Conditions;
                break;
            case PlayerId.None:
            default:
                throw new ArgumentOutOfRangeException(nameof(side.PlayerId), "Invalid PlayerId.");
        }
        SideCondition? sideCondition = sideConditions.FirstOrDefault(sc => sc.Id == sideConditionId);
        if (sideCondition is null) return false;
        sideConditions.Remove(sideCondition);
        sideCondition.OnSideEnd?.Invoke(side, battleContext);
        sideCondition.OnEnd?.Invoke(side.Team.AllActivePokemon, battleContext);
        return true;
    }

    public void ReapplySideCondition(SideConditionId sideConditionId, Side side, BattleContext battleContext)
    {
        List<SideCondition>? sideConditions;
        switch (side.PlayerId)
        {
            case PlayerId.Player1:
                sideConditions = Side1Conditions;
                break;
            case PlayerId.Player2:
                sideConditions = Side2Conditions;
                break;
            case PlayerId.None:
            default:
                throw new ArgumentOutOfRangeException(nameof(side.PlayerId), "Invalid PlayerId.");
        }
        SideCondition? sideCondition = sideConditions.FirstOrDefault(sc => sc.Id == sideConditionId);
        if (sideCondition is null)
        {
            throw new InvalidOperationException($"Cannot reapply side condition {sideConditionId}" +
                                                $"when it is not active on the side.");
        }
        sideCondition.OnReapply?.Invoke(this, side.Team.AllActivePokemon, battleContext);
    }

    public void OnPokemonSwitchIn(Pokemon pokemon, PlayerId playerId, BattleContext battleContext)
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

        switch (playerId)
        {
            case PlayerId.Player1:
            {
                foreach (SideCondition sc in Side1Conditions)
                {
                    sc.OnSidePokemonSwitchIn?.Invoke(pokemon, battleContext);
                    sc.OnPokemonSwitchIn?.Invoke(pokemon, battleContext);
                }
                break;
            }
            case PlayerId.Player2:
            {
                foreach (SideCondition sc in Side2Conditions)
                {
                    sc.OnSidePokemonSwitchIn?.Invoke(pokemon, battleContext);
                    sc.OnPokemonSwitchIn?.Invoke(pokemon, battleContext);
                }
                break;
            }
            case PlayerId.None:
            default:
                throw new ArgumentOutOfRangeException(nameof(playerId), "Invalid PlayerId.");
        }
    }

    public void OnTurnStart(Side side1, Side side2, BattleContext battleContext)
    {
        // TODO: Implement OnTurnStart logic if needed
    }

    public void OnTurnEnd(Side side1, Side side2, BattleContext battleContext)
    {
        var pokemon = side1.Team.AllActivePokemon.Concat(side2.Team.AllActivePokemon).ToArray();
        
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

        foreach (SideCondition sc in Side1Conditions.ToList())
        {
            sc.IncrementTurnCounter();
            sc.OnIncrementTurnCounter?.Invoke(pokemon, sc, battleContext);
            if (sc.IsExpired)
            {
                RemoveSideCondition(sc.Id, side1, battleContext);
            }
        }

        foreach (SideCondition sc in Side2Conditions.ToList())
        {
            sc.IncrementTurnCounter();
            sc.OnIncrementTurnCounter?.Invoke(pokemon, sc, battleContext);
            if (sc.IsExpired)
            {
                RemoveSideCondition(sc.Id, side2, battleContext);
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
            Side1Conditions = Side1Conditions.Select(sc => sc.Copy()).ToList(),
            Side2Conditions = Side2Conditions.Select(sc => sc.Copy()).ToList(),
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