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

public partial class BattleAsync
{
    private EffectDelegate? GetCallback(RunEventTarget target, IEffect effect, EventId callbackName)
    {
        EffectDelegate? del = effect.GetDelegate(callbackName);
        Delegate? callback = del?.GetDelegate();

        // Special case: In Gen 5+, abilities and items trigger onStart during SwitchIn
        // instead of having a separate Start event
        if (callback is null &&
            target is PokemonRunEventTarget &&
            Gen >= 5 &&
            callbackName == EventId.SwitchIn &&
            effect.GetDelegate(EventId.AnySwitchIn) == null && // Check onAnySwitchIn doesn't exist
            (IsAbilityOrItem(effect) || IsInnateAbilityOrItem(effect)))
        {
            del = effect.GetDelegate(EventId.Start);
            callback = del?.GetDelegate();
        }

        return EffectDelegate.FromNullableDelegate(callback);
    }

    private EffectDelegate? GetCallback(Pokemon pokemon, IEffect effect, EventId callbackName)
    {
        return GetCallback(new PokemonRunEventTarget(pokemon), effect, callbackName);
    }

    private EffectDelegate? GetCallback(Field field, IEffect effect, EventId callbackName)
    {
        return GetCallback(new FieldRunEventTarget(field), effect, callbackName);
    }

    private EffectDelegate? GetCallback(Side side, IEffect effect, EventId callbackName)
    {
        return GetCallback(new SideRunEventTarget(side), effect, callbackName);
    }

    /// <summary>
    /// Finds all event handlers for a given target and event.
    /// Handles Pokemon arrays, event bubbling between Pokemon/Side, and prefixed event variants.
    /// </summary>
    private List<EventListener> FindEventHandlers(RunEventTarget target, EventId eventName, Pokemon? source = null)
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

            if (pokemon.IsActive || (source?.IsActive ?? false))
            {
                handlers = FindPokemonEventHandlers(pokemon, eventName);

                if (prefixedHandlers)
                {
                    // Check allies (including self) for Ally and Any prefixed events
                    foreach (Pokemon allyActive in pokemon.AlliesAndSelf())
                    {
                        EventId allyEventId = EventIdInfo.CombinePrefixWithEvent(EventPrefix.Ally, eventName, Library.Events);
                        handlers.AddRange(FindPokemonEventHandlers(allyActive, allyEventId));

                        EventId anyEventId = EventIdInfo.CombinePrefixWithEvent(EventPrefix.Any, eventName, Library.Events);
                        handlers.AddRange(FindPokemonEventHandlers(allyActive, anyEventId));
                    }

                    // Check foes for Foe and Any prefixed events
                    foreach (Pokemon foeActive in pokemon.Foes())
                    {
                        EventId foeEventId = EventIdInfo.CombinePrefixWithEvent(EventPrefix.Foe, eventName, Library.Events);
                        handlers.AddRange(FindPokemonEventHandlers(foeActive, foeEventId));

                        EventId anyEventId = EventIdInfo.CombinePrefixWithEvent(EventPrefix.Any, eventName, Library.Events);
                        handlers.AddRange(FindPokemonEventHandlers(foeActive, anyEventId));
                    }
                }

                // Bubble up to the Side
                target = new SideRunEventTarget(pokemon.Side);
            }
        }

        // Check source Pokemon for Source prefixed events
        if (source != null && prefixedHandlers)
        {
            EventId sourceEventId = EventIdInfo.CombinePrefixWithEvent(EventPrefix.Source, eventName, Library.Events);
            handlers.AddRange(FindPokemonEventHandlers(source, sourceEventId));
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
                            handlers.AddRange(FindPokemonEventHandlers(active, eventName));
                        }
                        else if (prefixedHandlers)
                        {
                            EventId foeEventId = EventIdInfo.CombinePrefixWithEvent(EventPrefix.Foe, eventName, Library.Events);
                            handlers.AddRange(FindPokemonEventHandlers(active, foeEventId));
                        }

                        if (prefixedHandlers)
                        {
                            EventId anyEventId = EventIdInfo.CombinePrefixWithEvent(EventPrefix.Any, eventName, Library.Events);
                            handlers.AddRange(FindPokemonEventHandlers(active, anyEventId));
                        }
                    }
                }

                // Handle Side conditions (but not for ally sides in multi battles)
                if (side.N < 2)
                {
                    if (side == targetSide)
                    {
                        handlers.AddRange(FindSideEventHandlers(side, eventName));
                    }
                    else if (prefixedHandlers)
                    {
                        EventId foeEventId = EventIdInfo.CombinePrefixWithEvent(EventPrefix.Foe, eventName, Library.Events);
                        handlers.AddRange(FindSideEventHandlers(side, foeEventId));
                    }

                    if (prefixedHandlers)
                    {
                        EventId anyEventId = EventIdInfo.CombinePrefixWithEvent(EventPrefix.Any, eventName, Library.Events);
                        handlers.AddRange(FindSideEventHandlers(side, anyEventId));
                    }
                }
            }
        }

        // Always check Field and Battle handlers
        handlers.AddRange(FindFieldEventHandlers(Field, eventName));
        handlers.AddRange(FindBattleEventHandlers(eventName));

        return handlers;
    }

    /// <summary>
    /// Finds all event handlers for a Pokemon by checking its status, volatiles, ability, item, species, and slot conditions.
    /// This collects all effects that might respond to a specific event for this Pokemon.
    /// </summary>
    /// <param name="pokemon">The Pokemon to find handlers for</param>
    /// <param name="callbackName">The event to find handlers for</param>
    /// <param name="getKey">Optional property key to check in effect states (e.g., "duration")</param>
    /// <param name="customHolder">Optional custom effect holder (for special cases)</param>
    /// <returns>List of event listeners that can handle this event</returns>
    private List<EventListener> FindPokemonEventHandlers(Pokemon pokemon, EventId callbackName,
        EffectStateKey? getKey = null, Pokemon? customHolder = null)
    {
        List<EventListenerWithoutPriority> handlersWithoutPriority = [];

        // Check status condition (paralysis, burn, etc.)
        Condition status = pokemon.GetStatus();
        EffectDelegate? callback = GetCallback(pokemon, status, callbackName);
        if (callback != null || (getKey != null && pokemon.StatusState.GetProperty(getKey) != null))
        {
            handlersWithoutPriority.Add(new EventListenerWithoutPriority
            {
                Effect = status,
                Callback = callback,
                State = pokemon.StatusState,
                End = customHolder == null
                    ? EffectDelegate.FromNullableDelegate(new Action<bool>(_ => pokemon.ClearStatus()))
                    : null,
                EffectHolder = customHolder ?? pokemon,
            });
        }

        // Check volatile conditions (confusion, flinch, etc.)
        // Create a snapshot of keys to avoid modification during enumeration
        foreach (ConditionId id in pokemon.Volatiles.Keys.ToArray())
        {
            // Check if the volatile still exists (it might have been removed by a previous handler)
            if (!pokemon.Volatiles.TryGetValue(id, out EffectState? volatileState))
            {
                continue;
            }

            Condition volatileCondition = Library.Conditions[id];
            callback = GetCallback(pokemon, volatileCondition, callbackName);
            if (callback != null || (getKey != null && volatileState.GetProperty(getKey) != null))
            {
                handlersWithoutPriority.Add(new EventListenerWithoutPriority
                {
                    Effect = volatileCondition,
                    Callback = callback,
                    State = volatileState,
                    End = customHolder == null
                        ? EffectDelegate.FromNullableDelegate((Func<Condition, bool>)pokemon.RemoveVolatile)
                        : null,
                    EffectHolder = customHolder ?? pokemon,
                });
            }
        }

        // Check ability
        Ability ability = pokemon.GetAbility();
        callback = GetCallback(pokemon, ability, callbackName);
        if (callback != null || (getKey != null && pokemon.AbilityState.GetProperty(getKey) != null))
        {
            handlersWithoutPriority.Add(new EventListenerWithoutPriority
            {
                Effect = ability,
                Callback = callback,
                State = pokemon.AbilityState,
                End = customHolder == null
                    ? EffectDelegate.FromNullableDelegate(pokemon.ClearAbility)
                    : null,
                EffectHolder = customHolder ?? pokemon,
            });
        }

        // Check held item
        Item item = pokemon.GetItem();
        callback = GetCallback(pokemon, item, callbackName);
        if (callback != null || (getKey != null && pokemon.ItemState.GetProperty(getKey) != null))
        {
            handlersWithoutPriority.Add(new EventListenerWithoutPriority
            {
                Effect = item,
                Callback = callback,
                State = pokemon.ItemState,
                End = customHolder == null
                    ? EffectDelegate.FromNullableDelegate(pokemon.ClearItem)
                    : null,
                EffectHolder = customHolder ?? pokemon,
            });
        }

        // Check species (for species-specific events)
        Species species = pokemon.BaseSpecies;
        callback = GetCallback(pokemon, species, callbackName);
        if (callback != null)
        {
            handlersWithoutPriority.Add(new EventListenerWithoutPriority
            {
                Effect = species,
                Callback = callback,
                State = pokemon.SpeciesState,
                End = null, // Species can't be removed
                EffectHolder = customHolder ?? pokemon,
            });
        }

        // Check slot conditions (Stealth Rock trap, etc.)
        // Only check if Pokemon has a valid active position
        Side side = pokemon.Side;
        if (pokemon.Position >= 0 && pokemon.Position < side.SlotConditions.Count)
        {
            Dictionary<ConditionId, EffectState> slotConditions = side.SlotConditions[pokemon.Position];
            if (slotConditions != null)
            {
                // Create a snapshot of keys to avoid modification during enumeration
                foreach ((ConditionId conditionId, EffectState slotConditionState) in slotConditions.ToArray())
                {
                    if (slotConditionState == null) continue;

                    Condition slotCondition = Library.Conditions[conditionId];
                    callback = GetCallback(pokemon, slotCondition, callbackName);
                    if (callback != null || (getKey != null && slotConditionState.GetProperty(getKey) != null))
                    {
                        handlersWithoutPriority.Add(new EventListenerWithoutPriority
                        {
                            Effect = slotCondition,
                            Callback = callback,
                            State = slotConditionState,
                            End = customHolder == null
                                ? EffectDelegate.FromNullableDelegate(new Action<ConditionId>(id =>
                                    side.RemoveSideCondition(id)))
                                : null,
                            EndCallArgs = [side, pokemon, conditionId],
                            EffectHolder = customHolder ?? pokemon,
                        });
                    }
                }
            }
        }

        // Now resolve priorities for all handlers (after dictionary enumeration is complete)
        List<EventListener> handlers = new List<EventListener>(handlersWithoutPriority.Count);
        foreach (var handler in handlersWithoutPriority)
        {
            handlers.Add(ResolvePriority(handler, callbackName));
        }

        return handlers;
    }

    private List<EventListener> FindBattleEventHandlers(EventId callbackName, EffectStateKey? getKey = null,
        Pokemon? customHolder = null)
    {
        List<EventListener> handlers = [];

        // Check format (ruleset) for handlers
        EffectDelegate? callback = GetCallback(RunEventTarget.FromIBattle(this), Format, callbackName);

        if (callback != null || (getKey != null && FormatData.GetProperty(getKey) != null))
        {
            handlers.Add(ResolvePriority(new EventListenerWithoutPriority
            {
                Effect = Format,
                Callback = callback,
                State = FormatData,
                End = null,
                EffectHolder = customHolder ?? EffectHolder.FromIBattle(this),
            }, callbackName));
        }

        // Check custom event handlers registered in this.Events
        // In TypeScript: if (this.events && (callback = this.events[callbackName]) !== undefined)
        if (Events?.GetDelegate(callbackName) is { } eventDelegate)
        {
            // The Events object contains dynamically registered handlers with their priorities
            // These need to be processed differently from the static Format handlers

            // Since we don't have the full EventHandlerData structure yet,
            // this is a placeholder for when Events is properly implemented
            // In the TypeScript version, each handler in callback array has:
            // - target (the effect)
            // - callback (the function)
            // - priority, order, subOrder (for sorting)

            // TODO: Implement full handling of Events with priorities and multiple handlers

            // For now, just add a basic handler
            EffectState? state = null;
            if (Events.Effect is { EffectType: EffectType.Format })
            {
                state = FormatData;
            }

            handlers.Add(new EventListener
            {
                Effect = Events.Effect ?? Format,
                Callback = eventDelegate,
                State = state,
                End = null,
                EffectHolder = customHolder ?? EffectHolder.FromIBattle(this),
                // These would come from the handler data in a full implementation
                Order = IntFalseUnion.FromInt(0),
                Priority = 0,
                SubOrder = 0,
                EffectOrder = 0,
                Speed = 0,
            });
        }

        return handlers;
    }

    private List<EventListener> FindFieldEventHandlers(Field field, EventId callbackName,
        EffectStateKey? getKey = null, Pokemon? customHolder = null)
    {
        List<EventListenerWithoutPriority> handlersWithoutPriority = [];

        // Check pseudo-weather effects (Trick Room, Gravity, etc.)
        // Create a snapshot of keys to avoid modification during enumeration
        foreach (ConditionId id in field.PseudoWeather.Keys.ToArray())
        {
            // Check if the pseudo-weather still exists
            if (!field.PseudoWeather.TryGetValue(id, out EffectState? pseudoWeatherState))
            {
                continue;
            }

            Condition pseudoWeather = Library.Conditions[id];
            EffectDelegate? callback = GetCallback(field, pseudoWeather, callbackName);

            if (callback != null || (getKey != null && pseudoWeatherState.GetProperty(getKey) != null))
            {
                handlersWithoutPriority.Add(new EventListenerWithoutPriority
                {
                    Effect = pseudoWeather,
                    Callback = callback,
                    State = pseudoWeatherState,
                    End = customHolder == null
                        ? EffectDelegate.FromNullableDelegate((Func<ConditionId, bool>)field.RemovePseudoWeather)
                        : null,
                    EffectHolder = customHolder is null ? field : customHolder,
                });
            }
        }

        // Check weather effect
        Condition weather = field.GetWeather();
        EffectDelegate? weatherCallback = GetCallback(field, weather, callbackName);
        if (weatherCallback != null || (getKey != null && Field.WeatherState.GetProperty(getKey) != null))
        {
            handlersWithoutPriority.Add(new EventListenerWithoutPriority
            {
                Effect = weather,
                Callback = weatherCallback,
                State = Field.WeatherState,
                End = customHolder == null
                    ? EffectDelegate.FromNullableDelegate(field.ClearWeather)
                    : null,
                EffectHolder = customHolder is null ? field : customHolder,
            });
        }

        // Check terrain effect
        Condition terrain = field.GetTerrain();
        EffectDelegate? terrainCallback = GetCallback(field, terrain, callbackName);
        if (terrainCallback != null || (getKey != null && field.TerrainState.GetProperty(getKey) != null))
        {
            handlersWithoutPriority.Add(new EventListenerWithoutPriority
            {
                Effect = terrain,
                Callback = terrainCallback,
                State = field.TerrainState,
                End = customHolder == null
                    ? EffectDelegate.FromNullableDelegate(field.ClearTerrain)
                    : null,
                EffectHolder = customHolder is null ? field : customHolder,
            });
        }

        // Resolve priorities after all dictionary enumerations
        List<EventListener> handlers = new List<EventListener>(handlersWithoutPriority.Count);
        foreach (var handler in handlersWithoutPriority)
        {
            handlers.Add(ResolvePriority(handler, callbackName));
        }

        return handlers;
    }

    private List<EventListener> FindSideEventHandlers(Side side, EventId callbackName,
        EffectStateKey? getKey = null, Pokemon? customHolder = null)
    {
        List<EventListenerWithoutPriority> handlersWithoutPriority = [];

        // Create a snapshot of keys to avoid modification during enumeration
        foreach (ConditionId id in side.SideConditions.Keys.ToArray())
        {
            // Check if the side condition still exists
            if (!side.SideConditions.TryGetValue(id, out EffectState? sideConditionData))
            {
                continue;
            }

            Condition sideCondition = Library.Conditions[id];
            EffectDelegate? callback = GetCallback(side, sideCondition, callbackName);
            if (callback != null || (getKey != null && sideConditionData.GetProperty(getKey) != null))
            {
                handlersWithoutPriority.Add(new EventListenerWithoutPriority
                {
                    Effect = sideCondition,
                    Callback = callback,
                    State = sideConditionData,
                    End = customHolder == null
                        ? EffectDelegate.FromNullableDelegate((Func<ConditionId, bool>)side.RemoveSideCondition)
                        : null,
                    EffectHolder = customHolder is null ? side : customHolder,
                });
            }
        }

        // Resolve priorities after all dictionary enumerations
        List<EventListener> handlers = new List<EventListener>(handlersWithoutPriority.Count);
        foreach (var handler in handlersWithoutPriority)
        {
            handlers.Add(ResolvePriority(handler, callbackName));
        }

        return handlers;
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