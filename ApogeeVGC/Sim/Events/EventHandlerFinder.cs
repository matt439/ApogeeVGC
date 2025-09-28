//using ApogeeVGC.Data;
//using ApogeeVGC.Sim.Core;
//using ApogeeVGC.Sim.Effects;
//using ApogeeVGC.Sim.Events;
//using ApogeeVGC.Sim.GameObjects;
//using ApogeeVGC.Sim.Moves;
//using ApogeeVGC.Sim.PokemonClasses;
//using ApogeeVGC.Sim.FieldClasses;
//using ApogeeVGC.Sim.Stats;

//namespace ApogeeVGC.Sim.Events;

///// <summary>
///// Represents a discovered event handler, similar to Showdown's EventListener
///// </summary>
//public class EventHandler
//{
//    public required IEffect Effect { get; init; }
//    public required Delegate Callback { get; init; }
//    public required object? State { get; init; }
//    public required object EffectHolder { get; init; }
//    public int Priority { get; init; }
//    public int Order { get; init; }
//    public int SubOrder { get; init; }
//    public Pokemon? Target { get; set; }
//    public int? Index { get; set; }
//    public Action? EndCallback { get; init; }
//}

///// <summary>
///// Event handler discovery system similar to Showdown's findEventHandlers
///// </summary>
//public static class EventHandlerFinder
//{
//    /// <summary>
//    /// Find all event handlers for a given event, similar to Showdown's findEventHandlers
//    /// </summary>
//    /// <param name="battleContext">The battle context</param>
//    /// <param name="eventId">The event ID to find handlers for</param>
//    /// <param name="target">The target of the event</param>
//    /// <param name="source">The source of the event (optional)</param>
//    /// <returns>List of event handlers, sorted by priority</returns>
//    public static List<EventHandler> FindEventHandlers(
//        BattleContext battleContext, 
//        EventId eventId, 
//        object? target = null, 
//        Pokemon? source = null)
//    {
//        var handlers = new List<EventHandler>();
//        var library = battleContext.Library;
        
//        // Handle array targets (multiple Pokemon)
//        if (target is Pokemon[] pokemonArray)
//        {
//            for (int i = 0; i < pokemonArray.Length; i++)
//            {
//                var pokemon = pokemonArray[i];
//                var pokemonHandlers = FindEventHandlers(battleContext, eventId, pokemon, source);
                
//                // Add index information for array targets
//                foreach (var handler in pokemonHandlers)
//                {
//                    handler.Target = pokemon;
//                    handler.Index = i;
//                }
                
//                handlers.AddRange(pokemonHandlers);
//            }
//            return SortHandlers(handlers, eventId);
//        }
        
//        // Find handlers based on target type
//        switch (target)
//        {
//            case Pokemon pokemon:
//                handlers.AddRange(FindPokemonEventHandlers(library, pokemon, eventId));
//                handlers.AddRange(FindAllyEventHandlers(library, pokemon, eventId));
//                handlers.AddRange(FindFoeEventHandlers(library, pokemon, eventId));
//                handlers.AddRange(FindSideEventHandlers(library, pokemon.Side, eventId));
//                break;
                
//            case Side side:
//                handlers.AddRange(FindSideEventHandlers(library, side, eventId));
//                // Bubble down to active Pokemon if needed
//                foreach (var activePokemon in side.ActivePokemon)
//                {
//                    handlers.AddRange(FindPokemonEventHandlers(library, activePokemon, eventId));
//                }
//                break;
                
//            case Field field:
//                handlers.AddRange(FindFieldEventHandlers(library, field, eventId));
//                break;
                
//            case null:
//                // Battle-level event
//                break;
//        }
        
//        // Add source-based handlers
//        if (source != null)
//        {
//            handlers.AddRange(FindSourceEventHandlers(library, source, eventId));
//        }
        
//        // Add field and battle handlers
//        handlers.AddRange(FindFieldEventHandlers(library, battleContext.Field, eventId));
//        handlers.AddRange(FindBattleEventHandlers(library, battleContext, eventId));
        
//        return SortHandlers(handlers, eventId);
//    }
    
//    /// <summary>
//    /// Find event handlers for a Pokemon (status, ability, item, etc.)
//    /// </summary>
//    private static List<EventHandler> FindPokemonEventHandlers(Library library, Pokemon pokemon, EventId eventId)
//    {
//        var handlers = new List<EventHandler>();
        
//        // Status handlers
//        if (pokemon.Status != null)
//        {
//            var statusHandler = library.GetDelegate<Delegate>(eventId, pokemon.Status.Id.ToString());
//            if (statusHandler != null)
//            {
//                handlers.Add(new EventHandler
//                {
//                    Effect = pokemon.Status,
//                    Callback = statusHandler,
//                    State = pokemon.StatusState,
//                    EffectHolder = pokemon,
//                    Priority = GetPriority(pokemon.Status, eventId)
//                });
//            }
//        }
        
//        // Ability handlers
//        if (pokemon.Ability != null)
//        {
//            var abilityHandler = library.GetAbilityDelegate<Delegate>(eventId, pokemon.Ability.Id);
//            if (abilityHandler != null)
//            {
//                handlers.Add(new EventHandler
//                {
//                    Effect = pokemon.Ability,
//                    Callback = abilityHandler,
//                    State = pokemon.AbilityState,
//                    EffectHolder = pokemon,
//                    Priority = GetPriority(pokemon.Ability, eventId)
//                });
//            }
//        }
        
//        // Item handlers
//        if (pokemon.Item != null)
//        {
//            var itemHandler = library.GetItemDelegate<Delegate>(eventId, pokemon.Item.Id);
//            if (itemHandler != null)
//            {
//                handlers.Add(new EventHandler
//                {
//                    Effect = pokemon.Item,
//                    Callback = itemHandler,
//                    State = pokemon.ItemState,
//                    EffectHolder = pokemon,
//                    Priority = GetPriority(pokemon.Item, eventId)
//                });
//            }
//        }
        
//        // Volatile status handlers
//        foreach (var volatile in pokemon.Volatiles.Values)
//        {
//            var volatileHandler = library.GetDelegate<Delegate>(eventId, volatile.Id.ToString());
//            if (volatileHandler != null)
//            {
//                handlers.Add(new EventHandler
//                {
//                    Effect = volatile,
//                    Callback = volatileHandler,
//                    State = pokemon.VolatileStates.GetValueOrDefault(volatile.Id),
//                    EffectHolder = pokemon,
//                    Priority = GetPriority(volatile, eventId)
//                });
//            }
//        }
        
//        return handlers;
//    }
    
//    /// <summary>
//    /// Find ally event handlers (onAlly events)
//    /// </summary>
//    private static List<EventHandler> FindAllyEventHandlers(Library library, Pokemon pokemon, EventId eventId)
//    {
//        var handlers = new List<EventHandler>();
        
//        // Convert eventId to ally event (e.g., "Damage" -> "AllyDamage")
//        var allyEventName = $"Ally{eventId}";
        
//        foreach (var ally in pokemon.AlliesAndSelf())
//        {
//            // Check if ally has handlers for this ally event
//            var allyHandlers = FindPokemonEventHandlers(library, ally, eventId);
//            handlers.AddRange(allyHandlers);
//        }
        
//        return handlers;
//    }
    
//    /// <summary>
//    /// Find foe event handlers (onFoe events)
//    /// </summary>
//    private static List<EventHandler> FindFoeEventHandlers(Library library, Pokemon pokemon, EventId eventId)
//    {
//        var handlers = new List<EventHandler>();
        
//        // Convert eventId to foe event (e.g., "Damage" -> "FoeDamage")
//        var foeEventName = $"Foe{eventId}";
        
//        foreach (var foe in pokemon.Foes())
//        {
//            // Check if foe has handlers for this foe event
//            var foeHandlers = FindPokemonEventHandlers(library, foe, eventId);
//            handlers.AddRange(foeHandlers);
//        }
        
//        return handlers;
//    }
    
//    /// <summary>
//    /// Find source event handlers (onSource events)
//    /// </summary>
//    private static List<EventHandler> FindSourceEventHandlers(Library library, Pokemon source, EventId eventId)
//    {
//        var handlers = new List<EventHandler>();
        
//        // Convert eventId to source event (e.g., "Damage" -> "SourceDamage")
//        var sourceEventName = $"Source{eventId}";
        
//        var sourceHandlers = FindPokemonEventHandlers(library, source, eventId);
//        handlers.AddRange(sourceHandlers);
        
//        return handlers;
//    }
    
//    /// <summary>
//    /// Find side event handlers (side conditions)
//    /// </summary>
//    private static List<EventHandler> FindSideEventHandlers(Library library, Side side, EventId eventId)
//    {
//        var handlers = new List<EventHandler>();
        
//        // Side conditions
//        foreach (var sideCondition in side.SideConditions.Values)
//        {
//            var conditionHandler = library.GetDelegate<Delegate>(eventId, sideCondition.Id.ToString());
//            if (conditionHandler != null)
//            {
//                handlers.Add(new EventHandler
//                {
//                    Effect = sideCondition,
//                    Callback = conditionHandler,
//                    State = side.SideConditionStates.GetValueOrDefault(sideCondition.Id),
//                    EffectHolder = side,
//                    Priority = GetPriority(sideCondition, eventId)
//                });
//            }
//        }
        
//        return handlers;
//    }
    
//    /// <summary>
//    /// Find field event handlers (weather, terrain, pseudo-weather)
//    /// </summary>
//    private static List<EventHandler> FindFieldEventHandlers(Library library, Field field, EventId eventId)
//    {
//        var handlers = new List<EventHandler>();
        
//        // Weather handlers
//        if (field.Weather != null)
//        {
//            var weatherHandler = library.GetDelegate<Delegate>(eventId, field.Weather.Id.ToString());
//            if (weatherHandler != null)
//            {
//                handlers.Add(new EventHandler
//                {
//                    Effect = field.Weather,
//                    Callback = weatherHandler,
//                    State = field.WeatherState,
//                    EffectHolder = field,
//                    Priority = GetPriority(field.Weather, eventId)
//                });
//            }
//        }
        
//        // Terrain handlers
//        if (field.Terrain != null)
//        {
//            var terrainHandler = library.GetDelegate<Delegate>(eventId, field.Terrain.Id.ToString());
//            if (terrainHandler != null)
//            {
//                handlers.Add(new EventHandler
//                {
//                    Effect = field.Terrain,
//                    Callback = terrainHandler,
//                    State = field.TerrainState,
//                    EffectHolder = field,
//                    Priority = GetPriority(field.Terrain, eventId)
//                });
//            }
//        }
        
//        // Pseudo-weather handlers
//        foreach (var pseudoWeather in field.PseudoWeathers.Values)
//        {
//            var pseudoHandler = library.GetDelegate<Delegate>(eventId, pseudoWeather.Id.ToString());
//            if (pseudoHandler != null)
//            {
//                handlers.Add(new EventHandler
//                {
//                    Effect = pseudoWeather,
//                    Callback = pseudoHandler,
//                    State = field.PseudoWeatherStates.GetValueOrDefault(pseudoWeather.Id),
//                    EffectHolder = field,
//                    Priority = GetPriority(pseudoWeather, eventId)
//                });
//            }
//        }
        
//        return handlers;
//    }
    
//    /// <summary>
//    /// Find battle-level event handlers
//    /// </summary>
//    private static List<EventHandler> FindBattleEventHandlers(Library library, BattleContext battleContext, EventId eventId)
//    {
//        var handlers = new List<EventHandler>();
        
//        // Global/format handlers could be registered here
//        // This would be similar to Showdown's format handlers
        
//        return handlers;
//    }
    
//    /// <summary>
//    /// Sort handlers based on priority, similar to Showdown's sorting logic
//    /// </summary>
//    private static List<EventHandler> SortHandlers(List<EventHandler> handlers, EventId eventId)
//    {
//        // Different events use different sorting strategies (like in Showdown)
//        var specialSortEvents = new[] 
//        { 
//            EventId.OnTryHit, 
//            EventId.OnDamage 
//        };
        
//        if (specialSortEvents.Contains(eventId))
//        {
//            // Special left-to-right order
//            return handlers.OrderBy(h => h.Order).ThenBy(h => h.SubOrder).ToList();
//        }
//        else
//        {
//            // Speed-based sorting (faster Pokemon first)
//            return handlers.OrderByDescending(h => h.Priority)
//                          .ThenByDescending(h => GetSpeed(h.EffectHolder))
//                          .ThenBy(h => h.SubOrder)
//                          .ToList();
//        }
//    }
    
//    /// <summary>
//    /// Get priority for an effect and event
//    /// </summary>
//    private static int GetPriority(IEffect effect, EventId eventId)
//    {
//        // This could be enhanced to read priority from effect data
//        // For now, return default priority based on effect type
//        return effect.EffectType switch
//        {
//            EffectType.Ability => 100,
//            EffectType.Item => 90,
//            EffectType.Status => 80,
//            EffectType.Move => 70,
//            EffectType.Condition => 60,
//            _ => 50
//        };
//    }
    
//    /// <summary>
//    /// Get speed for priority sorting
//    /// </summary>
//    private static int GetSpeed(object effectHolder)
//    {
//        return effectHolder switch
//        {
//            Pokemon pokemon => pokemon.GetStat(StatId.Spe),
//            _ => 0
//        };
//    }
//}

///// <summary>
///// Extension method to make event handler discovery easy to use
///// </summary>
//public static class BattleContextEventExtensions
//{
//    /// <summary>
//    /// Find all event handlers for a given event
//    /// </summary>
//    public static List<EventHandler> FindEventHandlers(
//        this BattleContext battleContext,
//        EventId eventId,
//        object? target = null,
//        Pokemon? source = null)
//    {
//        return EventHandlerFinder.FindEventHandlers(battleContext, eventId, target, source);
//    }
    
//    /// <summary>
//    /// Run an event with all discovered handlers (similar to Showdown's runEvent)
//    /// </summary>
//    public static T RunEvent<T>(
//        this BattleContext battleContext,
//        EventId eventId,
//        T relayVar,
//        object? target = null,
//        Pokemon? source = null,
//        IEffect? sourceEffect = null)
//    {
//        var handlers = battleContext.FindEventHandlers(eventId, target, source);
        
//        foreach (var handler in handlers)
//        {
//            // Execute handler with proper parameters
//            var result = ExecuteHandler(handler, relayVar, target, source, sourceEffect);
            
//            if (result != null)
//            {
//                relayVar = (T)result;
                
//                // Stop if relay var becomes falsy (similar to Showdown logic)
//                if (IsFalsy(relayVar))
//                {
//                    break;
//                }
//            }
//        }
        
//        return relayVar;
//    }
    
//    private static object? ExecuteHandler(EventHandler handler, object relayVar, object? target, Pokemon? source, IEffect? sourceEffect)
//    {
//        // This would need to handle different delegate types and invoke them properly
//        // Similar to how Showdown calls handler.callback.apply(this, args)
        
//        try
//        {
//            return handler.Callback.Method.Invoke(handler.Callback.Target, new[] { relayVar, target, source, sourceEffect });
//        }
//        catch (Exception ex)
//        {
//            // Log error and continue
//            Console.WriteLine($"Error executing event handler: {ex.Message}");
//            return null;
//        }
//    }
    
//    private static bool IsFalsy(object value)
//    {
//        return value switch
//        {
//            null => true,
//            bool b => !b,
//            int i => i == 0,
//            _ => false
//        };
//    }
//}