using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class Battle
{
    /// <summary>
    /// Gets the event handler info for a given effect and event.
    /// Uses the effect's pre-computed handler cache for O(1) lookups.
    /// SwitchIn→OnStart fallback for abilities/items is pre-computed in the cache.
    /// </summary>
    /// <param name="target">The target of the event (Pokemon, Side, Field, or Battle)</param>
    /// <param name="effect">The effect to check for handlers (Ability, Item, Condition, etc.)</param>
    /// <param name="callbackName">The base event to look for handlers for</param>
    /// <param name="prefix">Event prefix (None, Ally, Foe, Source, Any)</param>
    /// <param name="suffix">Event suffix (None, SwitchIn, RedirectTarget)</param>
    /// <returns>EventHandlerInfo if the effect has a handler for this event, null otherwise</returns>
    private EventHandlerInfo? GetHandlerInfo(RunEventTarget target, IEffect effect,
        EventId callbackName, EventPrefix prefix = EventPrefix.None,
        EventSuffix suffix = EventSuffix.None)
    {
        if (!effect.HasAnyEventHandlers)
            return null;

        return effect.GetEventHandlerInfo(callbackName, prefix, suffix);
    }

    private EventHandlerInfo? GetHandlerInfo(Pokemon pokemon, IEffect effect, EventId callbackName,
        EventPrefix prefix = EventPrefix.None, EventSuffix suffix = EventSuffix.None)
    {
        return GetHandlerInfo(new PokemonRunEventTarget(pokemon), effect, callbackName, prefix,
            suffix);
    }

    private EventHandlerInfo? GetHandlerInfo(Field field, IEffect effect, EventId callbackName,
        EventPrefix prefix = EventPrefix.None, EventSuffix suffix = EventSuffix.None)
    {
        return GetHandlerInfo(new FieldRunEventTarget(field), effect, callbackName, prefix, suffix);
    }

    private EventHandlerInfo? GetHandlerInfo(Side side, IEffect effect, EventId callbackName,
        EventPrefix prefix = EventPrefix.None, EventSuffix suffix = EventSuffix.None)
    {
        return GetHandlerInfo(new SideRunEventTarget(side), effect, callbackName, prefix, suffix);
    }

    /// <summary>
    /// Finds all event handlers for a given target and event.
    /// Implements the complete event handler discovery system including:
    /// - Pokemon array handling with per-element indexing
    /// - Event bubbling from Pokemon up to Side
    /// - Prefixed event variants (Ally, Foe, Source, Any)
    /// - Side condition handling for multi-battles
    /// - Field and Battle-level handlers
    /// 
    /// This is the entry point for event handler discovery and corresponds to
    /// the TypeScript findEventHandlers method in battle.ts.
    /// </summary>
    /// <param name="target">The primary target of the event</param>
    /// <param name="eventName">The event to find handlers for</param>
    /// <param name="source">Optional source Pokemon (triggers Source-prefixed events)</param>
    /// <returns>List of all event listeners that should be invoked for this event</returns>
    private List<EventListener> FindEventHandlers(RunEventTarget target, EventId eventName,
        Pokemon? source = null)
    {
        List<EventListener> handlers = [];

        // Handle array of Pokemon
        if (target is PokemonArrayRunEventTarget arrayTarget)
        {
            for (int i = 0; i < arrayTarget.PokemonList.Length; i++)
            {
                Pokemon pokemon = arrayTarget.PokemonList[i];
                // Recursively find handlers for each Pokemon
                var curHandlers = FindEventHandlers(
                    new PokemonRunEventTarget(pokemon),
                    eventName,
                    source
                );

                // Set the target and index for each handler
                foreach (EventListener handler in curHandlers)
                {
                    handler.Target = pokemon; // Original "effectHolder"
                    handler.Index = i;
                }

                handlers.AddRange(curHandlers);
            }

            return handlers;
        }

        // Events that target a Pokemon normally bubble up to the Side
        bool shouldBubbleDown = target is SideRunEventTarget;

        // Events usually run through EachEvent should never have any handlers besides the base event
        // so don't check for prefixed variants
        bool prefixedHandlers = eventName is not (
            EventId.BeforeTurn or
            EventId.Update or
            EventId.Weather or
            EventId.WeatherChange or
            EventId.TerrainChange
            );

        // Handle Pokemon target
        if (target is PokemonRunEventTarget pokemonTarget)
        {
            Pokemon pokemon = pokemonTarget.Pokemon;

            // Guard against null pokemon (can occur despite non-nullable declaration)
            if (pokemon is null)
            {
                return handlers;
            }

            if (pokemon.IsActive || (source?.IsActive ?? false))
            {
                FindPokemonEventHandlers(handlers, pokemon, eventName);

                if (prefixedHandlers)
                {
                    // Check allies (including self) for Ally and Any prefixed events
                    foreach (Pokemon allyActive in pokemon.AlliesAndSelf())
                    {
                        // Guard against null allies (defensive)
                        if (allyActive is null) continue;

                        FindPokemonEventHandlers(handlers, allyActive, eventName,
                            EventPrefix.Ally);
                        FindPokemonEventHandlers(handlers, allyActive, eventName,
                            EventPrefix.Any);
                    }

                    // Check foes for Foe and Any prefixed events
                    foreach (Pokemon foeActive in pokemon.Foes())
                    {
                        // Guard against null foes (defensive)
                        if (foeActive is null) continue;

                        FindPokemonEventHandlers(handlers, foeActive, eventName,
                            EventPrefix.Foe);
                        FindPokemonEventHandlers(handlers, foeActive, eventName,
                            EventPrefix.Any);
                    }
                }

                // Bubble up to the Side
                target = new SideRunEventTarget(pokemon.Side);
            }
        }

        // Check source Pokemon for Source prefixed events
        if (source != null && prefixedHandlers)
        {
            FindPokemonEventHandlers(handlers, source, eventName, EventPrefix.Source);
        }

        // Handle Side target
        if (target is SideRunEventTarget sideTarget)
        {
            Side targetSide = sideTarget.Side;

            foreach (Side side in Sides)
            {
                // Handle bubble down from Side to active Pokemon
                if (shouldBubbleDown)
                {
                    foreach (Pokemon active in side.Active.OfType<Pokemon>())
                    {
                        if (side == targetSide)
                        {
                            FindPokemonEventHandlers(handlers, active, eventName);
                        }
                        else if (prefixedHandlers)
                        {
                            FindPokemonEventHandlers(handlers, active, eventName,
                                EventPrefix.Foe);
                        }

                        if (prefixedHandlers)
                        {
                            FindPokemonEventHandlers(handlers, active, eventName,
                                EventPrefix.Any);
                        }
                    }
                }

                // Handle Side conditions (but not for ally sides in multi battles)
                // In TypeScript: if (side.n < 2 || !side.allySide)
                // Since AllySide is not implemented (singles/doubles only), we just check side.N < 2
                // In full multi-battle implementation, this would prevent duplicate processing of shared side conditions
                if (side.N < 2)
                {
                    if (side == targetSide)
                    {
                        FindSideEventHandlers(handlers, side, eventName);
                    }
                    else if (prefixedHandlers)
                    {
                        FindSideEventHandlers(handlers, side, eventName, EventPrefix.Foe);
                    }

                    if (prefixedHandlers)
                    {
                        FindSideEventHandlers(handlers, side, eventName, EventPrefix.Any);
                    }
                }
            }
        }

        // Always check Field and Battle handlers
        FindFieldEventHandlers(handlers, Field, eventName);
        FindBattleEventHandlers(handlers, eventName);

        return handlers;
    }

    /// <summary>
    /// Finds all event handlers for a Pokemon by checking its status, volatiles, ability, item, species, and slot conditions.
    /// This collects all effects that might respond to a specific event for this Pokemon.
    /// </summary>
    /// <param name="handlers">The list to add discovered event listeners to</param>
    /// <param name="pokemon">The Pokemon to find handlers for</param>
    /// <param name="callbackName">The base event to find handlers for</param>
    /// <param name="prefix">Optional event prefix (Ally, Foe, Source, Any)</param>
    /// <param name="getKey">Optional property key to check in effect states (e.g., "duration")</param>
    /// <param name="customHolder">Optional custom effect holder (for special cases)</param>
    private void FindPokemonEventHandlers(List<EventListener> handlers, Pokemon pokemon, EventId callbackName,
        EventPrefix prefix = EventPrefix.None, EffectStateKey? getKey = null,
        Pokemon? customHolder = null)
    {
        Condition status = pokemon.GetStatus();
        EffectState statusState = pokemon.StatusState;

        Ability ability = pokemon.GetAbility();
        EffectState abilityState = pokemon.AbilityState;

        Item item = pokemon.GetItem();
        EffectState itemState = pokemon.ItemState;

        Species species = pokemon.BaseSpecies;
        EffectState speciesState = pokemon.SpeciesState;

        Side side = pokemon.Side;

        // Check status condition (paralysis, burn, etc.)
        EventHandlerInfo? handlerInfo = GetHandlerInfo(pokemon, status, callbackName, prefix);
        if (handlerInfo != null || (getKey != null && statusState.GetProperty(getKey) != null))
        {
            handlers.Add(ResolvePriority(new EventListenerWithoutPriority
            {
                Effect = status,
                HandlerInfo = handlerInfo,
                State = statusState,
                End = customHolder == null
                    ? EffectDelegate.FromNullableDelegate(new Action<bool>(_ =>
                        pokemon.ClearStatus()))
                    : null,
                EndCallArgs = customHolder == null ? [false] : null,
                EffectHolder = customHolder ?? pokemon,
            }, callbackName));
        }

        // Check volatile conditions (confusion, flinch, etc.)
        foreach ((ConditionId volatileId, EffectState volatileState) in pokemon.Volatiles)
        {
            Condition volatileCondition = Library.Conditions[volatileId];
            handlerInfo = GetHandlerInfo(pokemon, volatileCondition, callbackName, prefix);
            if (handlerInfo != null ||
                (getKey != null && volatileState.GetProperty(getKey) != null))
            {
                handlers.Add(ResolvePriority(new EventListenerWithoutPriority
                {
                    Effect = volatileCondition,
                    HandlerInfo = handlerInfo,
                    State = volatileState,
                    End = customHolder == null
                        ? EffectDelegate.FromNullableDelegate(
                            (Func<Condition, bool>)pokemon.RemoveVolatile)
                        : null,
                    EndCallArgs = customHolder == null ? [volatileCondition] : null,
                    EffectHolder = customHolder ?? pokemon,
                }, callbackName));
            }
        }

        // Check ability
        handlerInfo = GetHandlerInfo(pokemon, ability, callbackName, prefix);
        if (DisplayUi && callbackName == EventId.SwitchIn)
        {
            Debug($"[FindPokemonEventHandlers] {pokemon.Name} | Ability: {ability.Name} | Event: {callbackName} | Handler: {(handlerInfo != null ? "FOUND" : "NOT FOUND")}");
        }
        if (handlerInfo != null || (getKey != null && abilityState.GetProperty(getKey) != null))
        {
            handlers.Add(ResolvePriority(new EventListenerWithoutPriority
            {
                Effect = ability,
                HandlerInfo = handlerInfo,
                State = abilityState,
                End = customHolder == null
                    ? EffectDelegate.FromNullableDelegate(pokemon.ClearAbility)
                    : null,
                EffectHolder = customHolder ?? pokemon,
            }, callbackName));
        }

        // Check held item
        handlerInfo = GetHandlerInfo(pokemon, item, callbackName, prefix);
        if (handlerInfo != null || (getKey != null && itemState.GetProperty(getKey) != null))
        {
            handlers.Add(ResolvePriority(new EventListenerWithoutPriority
            {
                Effect = item,
                HandlerInfo = handlerInfo,
                State = itemState,
                End = customHolder == null
                    ? EffectDelegate.FromNullableDelegate(pokemon.ClearItem)
                    : null,
                EffectHolder = customHolder ?? pokemon,
            }, callbackName));
        }

        // Check species (for species-specific events)
        handlerInfo = GetHandlerInfo(pokemon, species, callbackName, prefix);
        if (handlerInfo != null)
        {
            handlers.Add(ResolvePriority(new EventListenerWithoutPriority
            {
                Effect = species,
                HandlerInfo = handlerInfo,
                State = speciesState,
                End = null, // Species can't be removed
                EffectHolder = customHolder ?? pokemon,
            }, callbackName));
        }

        // Check slot conditions (Stealth Rock trap, etc.)
        if (pokemon.Position < side.SlotConditions.Count)
        {
            foreach ((ConditionId conditionId, EffectState slotConditionState) in
                side.SlotConditions[pokemon.Position])
            {
                Condition slotCondition = Library.Conditions[conditionId];
                handlerInfo = GetHandlerInfo(pokemon, slotCondition, callbackName, prefix);
                if (handlerInfo != null ||
                    (getKey != null && slotConditionState.GetProperty(getKey) != null))
                {
                    handlers.Add(ResolvePriority(new EventListenerWithoutPriority
                    {
                        Effect = slotCondition,
                        HandlerInfo = handlerInfo,
                        State = slotConditionState,
                        End = customHolder == null
                            ? EffectDelegate.FromNullableDelegate(
                                new Action<Side, Pokemon, ConditionId>((s, _, id) =>
                                    s.RemoveSideCondition(id)))
                            : null,
                        EndCallArgs = [side, pokemon, conditionId],
                        EffectHolder = customHolder ?? pokemon,
                    }, callbackName));
                }
            }
        }
    }

    private void FindBattleEventHandlers(List<EventListener> handlers, EventId callbackName,
        EffectStateKey? getKey = null,
        Pokemon? customHolder = null)
    {
        // Check format (ruleset) for handlers
        EventHandlerInfo? handlerInfo =
            GetHandlerInfo(RunEventTarget.FromBattle(this), Format, callbackName);

        if (handlerInfo != null || (getKey != null && FormatData.GetProperty(getKey) != null))
        {
            handlers.Add(ResolvePriority(new EventListenerWithoutPriority
            {
                Effect = Format,
                HandlerInfo = handlerInfo,
                State = FormatData,
                End = null,
                EffectHolder = customHolder ?? EffectHolder.FromBattle(this),
            }, callbackName));
        }

        // Check custom event handlers registered in this.Events
        // In TypeScript: if (this.events && (callback = this.events[callbackName]) !== undefined)
        if (Events != null && Events.HasHandlers(callbackName))
        {
            // The Events object contains dynamically registered handlers with their priorities
            // In the TypeScript version, this.events[callbackName] returns an array where each handler has:
            // - target (the effect)
            // - callback (the function)
            // - priority, order, subOrder (for sorting)

            foreach (EventHandlerData handler in Events.GetHandlers(callbackName))
            {
                EffectState? state =
                    handler.Target.EffectType == EffectType.Format ? FormatData : null;

                handlers.Add(new EventListener
                {
                    Effect = handler.Target,
                    HandlerInfo = null, // Dynamic handlers don't use EventHandlerInfo
                    State = state,
                    End = null,
                    EffectHolder = customHolder ?? EffectHolder.FromBattle(this),
                    Priority = handler.Priority,
                    Order = IntFalseUnion.FromInt(handler.Order),
                    SubOrder = handler.SubOrder,
                    EffectOrder = 0,
                    Speed = 0,
                });
            }
        }
    }

    private void FindFieldEventHandlers(List<EventListener> handlers, Field field, EventId callbackName,
        EventPrefix prefix = EventPrefix.None, EffectStateKey? getKey = null,
        Pokemon? customHolder = null)
    {
        // Check pseudo-weather effects (Trick Room, Gravity, etc.)
        foreach ((ConditionId id, EffectState pseudoWeatherState) in field.PseudoWeather)
        {
            Condition pseudoWeather = Library.Conditions[id];
            EventHandlerInfo? handlerInfo =
                GetHandlerInfo(field, pseudoWeather, callbackName, prefix);

            if (handlerInfo != null ||
                (getKey != null && pseudoWeatherState.GetProperty(getKey) != null))
            {
                handlers.Add(ResolvePriority(new EventListenerWithoutPriority
                {
                    Effect = pseudoWeather,
                    HandlerInfo = handlerInfo,
                    State = pseudoWeatherState,
                    End = customHolder == null
                        ? EffectDelegate.FromNullableDelegate(
                            (Func<ConditionId, bool>)field.RemovePseudoWeather)
                        : null,
                    EndCallArgs = customHolder == null ? [id] : null,
                    EffectHolder = customHolder is null ? field : customHolder,
                }, callbackName));
            }
        }

        // Check weather effect
        Condition weather = field.GetWeather();
        EventHandlerInfo? weatherHandlerInfo = GetHandlerInfo(field, weather, callbackName, prefix);
        if (weatherHandlerInfo != null ||
            (getKey != null && Field.WeatherState.GetProperty(getKey) != null))
        {
            handlers.Add(ResolvePriority(new EventListenerWithoutPriority
            {
                Effect = weather,
                HandlerInfo = weatherHandlerInfo,
                State = Field.WeatherState,
                End = customHolder == null
                    ? EffectDelegate.FromNullableDelegate(field.ClearWeather)
                    : null,
                EffectHolder = customHolder is null ? field : customHolder,
            }, callbackName));
        }

        // Check terrain effect
        Condition terrain = field.GetTerrain();
        EventHandlerInfo? terrainHandlerInfo = GetHandlerInfo(field, terrain, callbackName, prefix);
        if (terrainHandlerInfo != null ||
            (getKey != null && field.TerrainState.GetProperty(getKey) != null))
        {
            handlers.Add(ResolvePriority(new EventListenerWithoutPriority
            {
                Effect = terrain,
                HandlerInfo = terrainHandlerInfo,
                State = field.TerrainState,
                End = customHolder == null
                    ? EffectDelegate.FromNullableDelegate(field.ClearTerrain)
                    : null,
                EffectHolder = customHolder is null ? field : customHolder,
            }, callbackName));
        }
    }

    private void FindSideEventHandlers(List<EventListener> handlers, Side side, EventId callbackName,
        EventPrefix prefix = EventPrefix.None, EffectStateKey? getKey = null,
        Pokemon? customHolder = null)
    {
        foreach ((ConditionId id, EffectState sideConditionData) in side.SideConditions)
        {
            Condition sideCondition = Library.Conditions[id];
            EventHandlerInfo? handlerInfo =
                GetHandlerInfo(side, sideCondition, callbackName, prefix);
            if (handlerInfo != null ||
                (getKey != null && sideConditionData.GetProperty(getKey) != null))
            {
                handlers.Add(ResolvePriority(new EventListenerWithoutPriority
                {
                    Effect = sideCondition,
                    HandlerInfo = handlerInfo,
                    State = sideConditionData,
                    End = customHolder == null
                        ? EffectDelegate.FromNullableDelegate(
                            (Func<ConditionId, bool>)side.RemoveSideCondition)
                        : null,
                    EndCallArgs = customHolder == null ? [id] : null,
                    EffectHolder = customHolder is null ? side : customHolder,
                }, callbackName));
            }
        }
    }

    #region Helpers

    // Helper method to check if effect is Ability or Item
    private static bool IsAbilityOrItem(IEffect effect)
    {
        return effect.EffectType is EffectType.Ability or EffectType.Item;
    }

    // Helper method to check if effect is an innate ability/item
    private static bool IsInnateAbilityOrItem(IEffect effect)
    {
        if (effect.EffectType != EffectType.Status)
        {
            return false;
        }

        var condition = (Condition)effect;
        return condition.AssociatedItem is not null || condition.AssociatedAbility is not null;
    }

    #endregion
}