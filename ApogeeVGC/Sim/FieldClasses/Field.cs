using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.FieldClasses;

public enum FieldId
{
    Default,
}

public class Field
{
    public IBattle Battle { get; init; }
    public FieldId Id { get; init; } = FieldId.Default;

    public ConditionId Weather { get; set; }
    public EffectState WeatherState { get; set; }
    public ConditionId Terrain { get; set; }
    public EffectState TerrainState { get; set; }
    public Dictionary<ConditionId, EffectState> PseudoWeather { get; init; } = [];

    public Field(IBattle battle)
    {
        Battle = battle;
        WeatherState = Battle.InitEffectState();
        TerrainState = Battle.InitEffectState();
    }

    public bool SetTerrain(IEffect status, Pokemon? source = null, IEffect? sourceEffect = null)
    {
        if (status is not Condition condition)
        {
            throw new ArgumentException("Status must be a terrain condition", nameof(status));
        }

        // Fall back to battle effect if sourceEffect not provided
        sourceEffect ??= Battle.Effect;
        
        // Fall back to event target if source not provided
        if (source == null && Battle.Event?.Target is not null)
        {
            source = Battle.Event.Target;
        }
        
        // Source is required
        if (source == null)
        {
            throw new InvalidOperationException("Setting terrain without a source");
        }

        // If terrain is already set to this terrain, return false (no change)
        if (Terrain == condition.Id)
        {
            return false;
        }

        // Save previous terrain state in case we need to rollback
        ConditionId prevTerrain = Terrain;
        EffectState prevTerrainState = TerrainState;
        
        // Set the new terrain
        Terrain = condition.Id;
        TerrainState = Battle.InitEffectState(status.EffectStateId, source, source.GetSlot(), 0);


        // If the terrain has a custom duration callback, use it
        if (condition.DurationCallback != null)
        {
            TerrainState.Duration = condition.DurationCallback(Battle, source, source, sourceEffect);
        }

        // Try to start the terrain - if it fails, rollback
        RelayVar? startResult = Battle.SingleEvent(
            EventId.FieldStart,
            condition,
            TerrainState,
            this,
            source,
            sourceEffect
        );
        
        if (startResult is BoolRelayVar { Value: false } or null)
        {
            // Rollback to previous state
            Terrain = prevTerrain;
            TerrainState = prevTerrainState;
            return false;
        }

        // Trigger terrain change event for all handlers
        Battle.EachEvent(EventId.TerrainChange, sourceEffect);
        
        return true;
    }

    public bool IsTerrain(ConditionId terrain, PokemonSideBattleUnion? target)
    {
        throw new NotImplementedException();
    }

    public bool RemovePseudoWeather(Condition status)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Checks if any active Pokémon has an ability that suppresses weather effects.
    /// 
    /// Abilities like Cloud Nine and Air Lock prevent weather from having any effect
    /// on the battle while the Pokémon with the ability is active.
    /// 
    /// Returns true if:
    /// - There is an active Pokémon that is not fainted
    /// - The Pokémon is not ignoring its ability (e.g. from Gastro Acid)
    /// - The Pokémon's ability has the SuppressWeather flag set to true
    /// - The ability state is not ending (ability hasn't been removed this turn)
    /// </summary>
    public bool SuppressingWeather()
    {
        return Battle.Sides.Any(side =>
            (from pokemon in side.Active where !pokemon.Fainted where !pokemon.IgnoringAbility()
                let ability = pokemon.GetAbility() where ability.SuppressWeather select pokemon).Any(pokemon =>
                !(pokemon.AbilityState.Ending ?? false)));
    }

    public Field Copy()
    {
        throw new NotImplementedException();
    }
}

