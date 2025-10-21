using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Ui;
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

    public bool SetWeather(ConditionId status, Pokemon? source = null, IEffect? sourceEffect = null)
    {
        Condition condition = Battle.Library.Conditions[status];
        return SetWeather(condition, source, sourceEffect);
    }

    public bool SetWeather(Condition status, Pokemon? source = null, IEffect? sourceEffect = null)
    {
        // Fall back to battle effect if sourceEffect not provided
        sourceEffect ??= Battle.Effect;
        
        // Fall back to event target if source not provided
        if (source == null && Battle.Event?.Target is PokemonSingleEventTarget pset)
        {
            source = pset.Pokemon;
        }

        // Check if weather is already set to this weather
        if (Weather == status.Id)
        {
            // Special case for abilities in Gen 6+ or if weather has no duration
            if (sourceEffect.EffectType == EffectType.Ability)
            {
                if (Battle.Gen > 5 || WeatherState.Duration == 0)
                {
                    return false;
                }
            }
            // For other effects in Gen 3+
            // Note: Original TS code also checks for Sandstorm in Gen 1-2, but that condition
            // doesn't exist in the codebase yet, so we skip that check
            else if (Battle.Gen > 2)
            {
                return false;
            }
        }

        // Run the SetWeather event to check if weather change is allowed
        if (source != null)
        {
            RelayVar? result = Battle.RunEvent(EventId.SetWeather, source, source, status);
            
            // If the event blocked the weather change
            if (result is null or BoolRelayVar { Value: false })
            {
                if (result is not BoolRelayVar { Value: false }) return false;

                // Check if source effect is a move with weather-setting capability
                // (In the original TS, this checks if the move has a weather property)
                // Since we don't have that property, we can check if it's a move with a condition
                if (sourceEffect is Move { Condition.EffectType: EffectType.Weather })
                {
                    UiGenerator.PrintFailEvent(source, sourceEffect);
                }
                else if (sourceEffect.EffectType == EffectType.Ability)
                {
                    UiGenerator.PrintAbilityEvent(source, sourceEffect.Name);
                    UiGenerator.PrintFailEvent(source);
                }
                return false;
            }
        }

        // Save previous weather state in case we need to rollback
        ConditionId prevWeather = Weather;
        EffectState prevWeatherState = WeatherState;
        
        // Set the new weather
        Weather = status.Id;
        WeatherState = Battle.InitEffectState(status.EffectStateId, source, source?.GetSlot(), 0);

        // If the weather has a duration specified, use it
        if (status.Duration.HasValue)
        {
            WeatherState.Duration = status.Duration.Value;
        }

        // If the weather has a custom duration callback, use it
        if (status.DurationCallback != null)
        {
            if (source == null)
            {
                throw new InvalidOperationException("Setting weather without a source");
            }
            WeatherState.Duration = status.DurationCallback(Battle, source, source, sourceEffect);
        }

        // Try to start the weather - if it fails, rollback
        RelayVar? startResult = Battle.SingleEvent(
            EventId.FieldStart,
            status,
            WeatherState,
            this,
            SingleEventSource.FromNullablePokemon(source),
            sourceEffect
        );
        
        if (startResult is BoolRelayVar { Value: false })
        {
            // Rollback to previous state
            Weather = prevWeather;
            WeatherState = prevWeatherState;
            return false;
        }

        // Trigger weather change event for all handlers
        Battle.EachEvent(EventId.WeatherChange, sourceEffect);
        
        return true;
    }

    public bool ClearWeather()
    {
        if (Weather == ConditionId.None) return false;
        Condition prevWeather = GetWeather();
        EffectState weatherState = WeatherState;
        Battle.SingleEvent(EventId.FieldEnd, prevWeather, weatherState, this);
        Weather = ConditionId.None;
        Battle.ClearEffectState(ref weatherState);
        Battle.EachEvent(EventId.WeatherChange);
        return true;
    }

    public ConditionId EffectiveWeather()
    {
        return SuppressingWeather() ? ConditionId.None : Weather;
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
            (from pokemon in side.Active
                where !pokemon.Fainted
                where !pokemon.IgnoringAbility()
                let ability = pokemon.GetAbility()
                where ability.SuppressWeather
                select pokemon).Any(pokemon =>
                !(pokemon.AbilityState.Ending ?? false)));
    }

    public bool IsWeather(ConditionId weather)
    {
        ConditionId ourWeather = EffectiveWeather();
        return ourWeather == weather;
    }

    public bool IsWeather(List<ConditionId> weather)
    {
        ConditionId ourWeather = EffectiveWeather();
        return weather.Contains(ourWeather);
    }

    public Condition GetWeather()
    {
        return Battle.Library.Conditions[Weather];
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
        if (source == null && Battle.Event?.Target is PokemonSingleEventTarget pset)
        {
            source = pset.Pokemon;
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

    public bool ClearTerrain()
    {
        if (Terrain == ConditionId.None) return false;
        Condition prevTerrain = GetTerrain();
        EffectState terrainState = TerrainState;
        Battle.SingleEvent(EventId.FieldEnd, prevTerrain, terrainState, this);
        Terrain = ConditionId.None;
        Battle.ClearEffectState(ref terrainState);
        Battle.EachEvent(EventId.TerrainChange);
        return true;
    }

    public ConditionId EffectiveTerrain(PokemonSideBattleUnion? target)
    {
        if (Battle.Event is not null && target is null)
        {
            target = PokemonSideBattleUnion.FromNullableSingleEventTarget(Battle.Event.Target);
        }

        return Battle.RunEvent(EventId.TryTerrain,
            RunEventTarget.FromNullablePokemonSideBattleUnion(target)) is not (null or BoolRelayVar { Value: false })
            ? Terrain
            : ConditionId.None;
    }

    public bool IsTerrain(ConditionId terrain, PokemonSideBattleUnion? target)
    {
        ConditionId ourTerrain = EffectiveTerrain(target);
        return ourTerrain == terrain;
    }

    public bool IsTerrain(IReadOnlyList<ConditionId> terrain, PokemonSideBattleUnion? target)
    {
        ConditionId ourTerrain = EffectiveTerrain(target);
        return terrain.Contains(ourTerrain);
    }

    public Condition GetTerrain()
    {
        return Battle.Library.Conditions[Terrain];
    }

    //addPseudoWeather(
    //    status: string | Condition,
    //source: Pokemon | 'debug' | null = null,
    //sourceEffect: Effect | null = null
    //) : boolean {
    //    if (!source && this.battle.event?.target) source = this.battle.event.target;
    //    if (source === 'debug') source = this.battle.sides[0].active[0];
    //    status = this.battle.dex.conditions.get(status);

    //    let state = this.pseudoWeather[status.id];
    //    if (state) {
    //        if (!(status as any).onFieldRestart) return false;
    //        return this.battle.singleEvent('FieldRestart', status, state, this, source, sourceEffect);
    //    }
    //    state = this.pseudoWeather[status.id] = this.battle.initEffectState({
    //        id: status.id,
    //        source,
    //        sourceSlot: source?.getSlot(),
    //        duration: status.duration,

    //    });
    //    if (status.durationCallback) {
    //        if (!source) throw new Error(`setting fieldcond without a source`);
    //        state.duration = status.durationCallback.call(this.battle, source, source, sourceEffect);
    //    }
    //    if (!this.battle.singleEvent('FieldStart', status, state, this, source, sourceEffect)) {
    //        delete this.pseudoWeather[status.id];
    //        return false;
    //    }
    //    this.battle.runEvent('PseudoWeatherChange', source, source, status);
    //    return true;
    //}

    public bool AddPseudoWeather(ConditionId status, Pokemon? source = null, IEffect? sourceEffect = null)
    {
        throw new NotImplementedException();
    }

    public bool AddPseudoWeather(Condition status, Pokemon? source = null, IEffect? sourceEffect = null)
    {
        throw new NotImplementedException();
    }

    public Condition? GetPseudoWeather(ConditionId status)
    {
        throw new NotImplementedException();
    }

    public Condition? GetPseudoWeather(Condition status)
    {
        throw new NotImplementedException();
    }

    public bool RemovePseudoWeather(Condition status)
    {
        PseudoWeather.TryGetValue(status.Id, out EffectState? state);
        if (state is null) return false;
        Battle.SingleEvent(EventId.FieldEnd, status, state, this);
        PseudoWeather.Remove(status.Id);
        return true;
    }

    public bool RemovePseudoWeather(ConditionId status)
    {
        return RemovePseudoWeather(Battle.Library.Conditions[status]);
    }

    public void Destroy()
    {
        throw new NotImplementedException();
    }
    
    public Field Copy()
    {
        throw new NotImplementedException();
    }
}


