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
    /// Handles special Gen 5+ logic where abilities/items use onStart during SwitchIn instead of a separate Start event.
    /// </summary>
    /// <param name="target">The target of the event (Pokemon, Side, Field, or Battle)</param>
    /// <param name="effect">The effect to check for handlers (Ability, Item, Condition, etc.)</param>
    /// <param name="callbackName">The event to look for handlers for</param>
    /// <returns>EventHandlerInfo if the effect has a handler for this event, null otherwise</returns>
    private EventHandlerInfo? GetHandlerInfo(RunEventTarget target, IEffect effect,
        EventId callbackName)
    {
        EventHandlerInfo? handlerInfo = effect.GetEventHandlerInfo(callbackName);

        // Special case: In Gen 5+, abilities and items trigger onStart during SwitchIn
        // instead of having a separate Start event
        // This matches the behavior in pokemon-showdown where getCallback has special logic:
        // callback === undefined && target instanceof Pokemon && this.gen >= 5 && 
        // callbackName === 'onSwitchIn' && !('Ability' | 'Item').includes(effect.effectType)
        if (handlerInfo is null &&
            target is PokemonRunEventTarget &&
            Gen >= 5 &&
            callbackName == EventId.SwitchIn &&
            effect.GetEventHandlerInfo(EventId.AnySwitchIn) ==
            null && // Check onAnySwitchIn doesn't exist
            (IsAbilityOrItem(effect) || IsInnateAbilityOrItem(effect)))
        {
            handlerInfo = effect.GetEventHandlerInfo(EventId.Start);
        }

        return handlerInfo;
    }

    private EventHandlerInfo? GetHandlerInfo(Pokemon pokemon, IEffect effect, EventId callbackName)
    {
        return GetHandlerInfo(new PokemonRunEventTarget(pokemon), effect, callbackName);
    }

    private EventHandlerInfo? GetHandlerInfo(Field field, IEffect effect, EventId callbackName)
    {
        return GetHandlerInfo(new FieldRunEventTarget(field), effect, callbackName);
    }

    private EventHandlerInfo? GetHandlerInfo(Side side, IEffect effect, EventId callbackName)
    {
        return GetHandlerInfo(new SideRunEventTarget(side), effect, callbackName);
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

            if (pokemon.IsActive || (source?.IsActive ?? false))
            {
                handlers = FindPokemonEventHandlers(pokemon, eventName);

                if (prefixedHandlers)
                {
                    // Check allies (including self) for Ally and Any prefixed events
                    foreach (Pokemon allyActive in pokemon.AlliesAndSelf())
                    {
                        EventId allyEventId =
                            EventIdInfo.CombinePrefixWithEvent(EventPrefix.Ally, eventName,
                                Library.Events);
                        handlers.AddRange(FindPokemonEventHandlers(allyActive, allyEventId));

                        EventId anyEventId =
                            EventIdInfo.CombinePrefixWithEvent(EventPrefix.Any, eventName,
                                Library.Events);
                        handlers.AddRange(FindPokemonEventHandlers(allyActive, anyEventId));
                    }

                    // Check foes for Foe and Any prefixed events
                    foreach (Pokemon foeActive in pokemon.Foes())
                    {
                        EventId foeEventId =
                            EventIdInfo.CombinePrefixWithEvent(EventPrefix.Foe, eventName,
                                Library.Events);
                        handlers.AddRange(FindPokemonEventHandlers(foeActive, foeEventId));

                        EventId anyEventId =
                            EventIdInfo.CombinePrefixWithEvent(EventPrefix.Any, eventName,
                                Library.Events);
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
            EventId sourceEventId =
                EventIdInfo.CombinePrefixWithEvent(EventPrefix.Source, eventName, Library.Events);
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
                            EventId foeEventId = EventIdInfo.CombinePrefixWithEvent(EventPrefix.Foe,
                                eventName, Library.Events);
                            handlers.AddRange(FindPokemonEventHandlers(active, foeEventId));
                        }

                        if (prefixedHandlers)
                        {
                            EventId anyEventId = EventIdInfo.CombinePrefixWithEvent(EventPrefix.Any,
                                eventName, Library.Events);
                            handlers.AddRange(FindPokemonEventHandlers(active, anyEventId));
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
                        handlers.AddRange(FindSideEventHandlers(side, eventName));
                    }
                    else if (prefixedHandlers)
                    {
                        EventId foeEventId =
                            EventIdInfo.CombinePrefixWithEvent(EventPrefix.Foe, eventName,
                                Library.Events);
                        handlers.AddRange(FindSideEventHandlers(side, foeEventId));
                    }

                    if (prefixedHandlers)
                    {
                        EventId anyEventId =
                            EventIdInfo.CombinePrefixWithEvent(EventPrefix.Any, eventName,
                                Library.Events);
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
        return FindPokemonEventHandlersInternal(pokemon, callbackName, getKey, customHolder)
            .ToList();
    }

    private IEnumerable<EventListener> FindPokemonEventHandlersInternal(Pokemon pokemon,
        EventId callbackName,
        EffectStateKey? getKey = null, Pokemon? customHolder = null)
    {
        // Materialize all data BEFORE yielding to avoid re-entrancy issues
        Condition status = pokemon.GetStatus();
        EffectState statusState = pokemon.StatusState;

        List<(ConditionId id, EffectState state, Condition condition)> volatiles = [];
        volatiles.AddRange(pokemon.Volatiles.Keys.ToList()
            .Select(id => (id, pokemon.Volatiles[id], Library.Conditions[id])));

        Ability ability = pokemon.GetAbility();
        EffectState abilityState = pokemon.AbilityState;

        Item item = pokemon.GetItem();
        EffectState itemState = pokemon.ItemState;

        Species species = pokemon.BaseSpecies;
        EffectState speciesState = pokemon.SpeciesState;

        List<(ConditionId id, EffectState state, Condition condition)> slotConditions = [];
        Side side = pokemon.Side;
        if (pokemon.Position < side.SlotConditions.Count)
        {
            var slotConditionsDict =
                side.SlotConditions[pokemon.Position];
            foreach ((ConditionId conditionId, EffectState slotConditionState) in slotConditionsDict
                         .ToList()) // Materialize
            {
                slotConditions.Add((conditionId, slotConditionState,
                    Library.Conditions[conditionId]));
            }
        }

        // Now yield results using materialized data

        // Check status condition (paralysis, burn, etc.)
        EventHandlerInfo? handlerInfo = GetHandlerInfo(pokemon, status, callbackName);
        if (handlerInfo != null || (getKey != null && statusState.GetProperty(getKey) != null))
        {
            yield return ResolvePriority(new EventListenerWithoutPriority
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
            }, callbackName);
        }

        // Check volatile conditions (confusion, flinch, etc.)
        foreach ((ConditionId _, EffectState volatileState, Condition volatileCondition) in
                 volatiles)
        {
            handlerInfo = GetHandlerInfo(pokemon, volatileCondition, callbackName);
            if (handlerInfo != null ||
                (getKey != null && volatileState.GetProperty(getKey) != null))
            {
                yield return ResolvePriority(new EventListenerWithoutPriority
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
                }, callbackName);
            }
        }

        // Check ability
        handlerInfo = GetHandlerInfo(pokemon, ability, callbackName);
        if (handlerInfo != null || (getKey != null && abilityState.GetProperty(getKey) != null))
        {
            yield return ResolvePriority(new EventListenerWithoutPriority
            {
                Effect = ability,
                HandlerInfo = handlerInfo,
                State = abilityState,
                End = customHolder == null
                    ? EffectDelegate.FromNullableDelegate(pokemon.ClearAbility)
                    : null,
                EffectHolder = customHolder ?? pokemon,
            }, callbackName);
        }

        // Check held item
        handlerInfo = GetHandlerInfo(pokemon, item, callbackName);
        if (handlerInfo != null || (getKey != null && itemState.GetProperty(getKey) != null))
        {
            yield return ResolvePriority(new EventListenerWithoutPriority
            {
                Effect = item,
                HandlerInfo = handlerInfo,
                State = itemState,
                End = customHolder == null
                    ? EffectDelegate.FromNullableDelegate(pokemon.ClearItem)
                    : null,
                EffectHolder = customHolder ?? pokemon,
            }, callbackName);
        }

        // Check species (for species-specific events)
        handlerInfo = GetHandlerInfo(pokemon, species, callbackName);
        if (handlerInfo != null)
        {
            yield return ResolvePriority(new EventListenerWithoutPriority
            {
                Effect = species,
                HandlerInfo = handlerInfo,
                State = speciesState,
                End = null, // Species can't be removed
                EffectHolder = customHolder ?? pokemon,
            }, callbackName);
        }

        // Check slot conditions (Stealth Rock trap, etc.)
        foreach ((ConditionId conditionId, EffectState slotConditionState,
                     Condition slotCondition) in slotConditions)
        {
            handlerInfo = GetHandlerInfo(pokemon, slotCondition, callbackName);
            if (handlerInfo != null ||
                (getKey != null && slotConditionState.GetProperty(getKey) != null))
            {
                yield return ResolvePriority(new EventListenerWithoutPriority
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
                }, callbackName);
            }
        }
    }

    private List<EventListener> FindBattleEventHandlers(EventId callbackName,
        EffectStateKey? getKey = null,
        Pokemon? customHolder = null)
    {
        List<EventListener> handlers = [];

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

        return handlers;
    }

    private List<EventListener> FindFieldEventHandlers(Field field, EventId callbackName,
        EffectStateKey? getKey = null, Pokemon? customHolder = null)
    {
        return FindFieldEventHandlersInternal(field, callbackName, getKey, customHolder).ToList();
    }

    private IEnumerable<EventListener> FindFieldEventHandlersInternal(Field field,
        EventId callbackName,
        EffectStateKey? getKey = null, Pokemon? customHolder = null)
    {
        // Check pseudo-weather effects (Trick Room, Gravity, etc.)
        foreach (ConditionId id in field.PseudoWeather.Keys)
        {
            EffectState pseudoWeatherState = field.PseudoWeather[id];
            Condition pseudoWeather = Library.Conditions[id];
            EventHandlerInfo? handlerInfo = GetHandlerInfo(field, pseudoWeather, callbackName);

            if (handlerInfo != null ||
                (getKey != null && pseudoWeatherState.GetProperty(getKey) != null))
            {
                yield return ResolvePriority(new EventListenerWithoutPriority
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
                }, callbackName);
            }
        }

        // Check weather effect
        Condition weather = field.GetWeather();
        EventHandlerInfo? weatherHandlerInfo = GetHandlerInfo(field, weather, callbackName);
        if (weatherHandlerInfo != null ||
            (getKey != null && Field.WeatherState.GetProperty(getKey) != null))
        {
            yield return ResolvePriority(new EventListenerWithoutPriority
            {
                Effect = weather,
                HandlerInfo = weatherHandlerInfo,
                State = Field.WeatherState,
                End = customHolder == null
                    ? EffectDelegate.FromNullableDelegate(field.ClearWeather)
                    : null,
                EffectHolder = customHolder is null ? field : customHolder,
            }, callbackName);
        }

        // Check terrain effect
        Condition terrain = field.GetTerrain();
        EventHandlerInfo? terrainHandlerInfo = GetHandlerInfo(field, terrain, callbackName);
        if (terrainHandlerInfo != null ||
            (getKey != null && field.TerrainState.GetProperty(getKey) != null))
        {
            yield return ResolvePriority(new EventListenerWithoutPriority
            {
                Effect = terrain,
                HandlerInfo = terrainHandlerInfo,
                State = field.TerrainState,
                End = customHolder == null
                    ? EffectDelegate.FromNullableDelegate(field.ClearTerrain)
                    : null,
                EffectHolder = customHolder is null ? field : customHolder,
            }, callbackName);
        }
    }

    private List<EventListener> FindSideEventHandlers(Side side, EventId callbackName,
        EffectStateKey? getKey = null, Pokemon? customHolder = null)
    {
        return FindSideEventHandlersInternal(side, callbackName, getKey, customHolder).ToList();
    }

    private IEnumerable<EventListener> FindSideEventHandlersInternal(Side side,
        EventId callbackName,
        EffectStateKey? getKey = null, Pokemon? customHolder = null)
    {
        foreach (ConditionId id in side.SideConditions.Keys)
        {
            EffectState sideConditionData = side.SideConditions[id];
            Condition sideCondition = Library.Conditions[id];
            EventHandlerInfo? handlerInfo = GetHandlerInfo(side, sideCondition, callbackName);
            if (handlerInfo != null ||
                (getKey != null && sideConditionData.GetProperty(getKey) != null))
            {
                yield return ResolvePriority(new EventListenerWithoutPriority
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
                }, callbackName);
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