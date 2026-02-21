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
        if (!effect.HasAnyEventHandlers) return null;
        return effect.GetEventHandlerInfo(callbackName, prefix, suffix);
    }

    private EventHandlerInfo? GetHandlerInfo(Field field, IEffect effect, EventId callbackName,
        EventPrefix prefix = EventPrefix.None, EventSuffix suffix = EventSuffix.None)
    {
        if (!effect.HasAnyEventHandlers) return null;
        return effect.GetEventHandlerInfo(callbackName, prefix, suffix);
    }

    private EventHandlerInfo? GetHandlerInfo(Side side, IEffect effect, EventId callbackName,
        EventPrefix prefix = EventPrefix.None, EventSuffix suffix = EventSuffix.None)
    {
        if (!effect.HasAnyEventHandlers) return null;
        return effect.GetEventHandlerInfo(callbackName, prefix, suffix);
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

        // Determine Side target: either from original SideRunEventTarget or bubbled up from Pokemon
        Side? targetSide = null;
        if (target is SideRunEventTarget sideTarget)
        {
            targetSide = sideTarget.Side;
        }

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
                    // Check allies (including self) for Ally and Any prefixed events — batched
                    foreach (Pokemon allyActive in pokemon.AlliesAndSelf())
                    {
                        // Guard against null allies (defensive)
                        if (allyActive is null) continue;

                        FindPokemonEventHandlersPrefixed(handlers, allyActive, eventName,
                            EventPrefix.Ally, EventPrefix.Any);
                    }

                    // Check foes for Foe and Any prefixed events — batched
                    foreach (Pokemon foeActive in pokemon.Foes())
                    {
                        // Guard against null foes (defensive)
                        if (foeActive is null) continue;

                        FindPokemonEventHandlersPrefixed(handlers, foeActive, eventName,
                            EventPrefix.Foe, EventPrefix.Any);
                    }
                }

                // Bubble up to the Side (no allocation — just set the local variable)
                targetSide = pokemon.Side;
            }
        }

        // Check source Pokemon for Source prefixed events
        if (source != null && prefixedHandlers)
        {
            FindPokemonEventHandlers(handlers, source, eventName, EventPrefix.Source);
        }

        // Handle Side target
        if (targetSide != null)
        {
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
                            if (prefixedHandlers)
                            {
                                FindPokemonEventHandlers(handlers, active, eventName,
                                    EventPrefix.Any);
                            }
                        }
                        else if (prefixedHandlers)
                        {
                            FindPokemonEventHandlersPrefixed(handlers, active, eventName,
                                EventPrefix.Foe, EventPrefix.Any);
                        }
                    }
                }

                // Handle Side conditions (but not for ally sides in multi battles)
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
    /// Cached EffectDelegate for slot condition removal (non-capturing lambda, shared across all calls).
    /// </summary>
    private static readonly EffectDelegate SlotConditionRemoveEndDelegate =
        EffectDelegate.FromNullableDelegate(
            new Action<Side, Pokemon, ConditionId>((s, _, id) => s.RemoveSideCondition(id)))!;

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

        bool hasCustomHolder = customHolder != null;
        EffectHolder effectHolder = customHolder ?? pokemon;

        // Check status condition (paralysis, burn, etc.)
        EventHandlerInfo? handlerInfo = GetHandlerInfo(pokemon, status, callbackName, prefix);
        if (handlerInfo != null || (getKey != null && statusState.GetProperty(getKey) != null))
        {
            handlers.Add(ResolvePriority(new EventListener
            {
                Effect = status,
                HandlerInfo = handlerInfo,
                State = statusState,
                End = hasCustomHolder ? null : pokemon.ClearStatusEndDelegate,
                EndCallArgs = hasCustomHolder ? null : Pokemon.ClearStatusEndCallArgs,
                EffectHolder = effectHolder,
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
                handlers.Add(ResolvePriority(new EventListener
                {
                    Effect = volatileCondition,
                    HandlerInfo = handlerInfo,
                    State = volatileState,
                    End = hasCustomHolder ? null : pokemon.RemoveVolatileEndDelegate,
                    EndCallArgs = hasCustomHolder ? null : [volatileCondition],
                    EffectHolder = effectHolder,
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
            handlers.Add(ResolvePriority(new EventListener
            {
                Effect = ability,
                HandlerInfo = handlerInfo,
                State = abilityState,
                End = hasCustomHolder ? null : pokemon.ClearAbilityEndDelegate,
                EffectHolder = effectHolder,
            }, callbackName));
        }

        // Check held item
        handlerInfo = GetHandlerInfo(pokemon, item, callbackName, prefix);
        if (handlerInfo != null || (getKey != null && itemState.GetProperty(getKey) != null))
        {
            handlers.Add(ResolvePriority(new EventListener
            {
                Effect = item,
                HandlerInfo = handlerInfo,
                State = itemState,
                End = hasCustomHolder ? null : pokemon.ClearItemEndDelegate,
                EffectHolder = effectHolder,
            }, callbackName));
        }

        // Check species (for species-specific events)
        handlerInfo = GetHandlerInfo(pokemon, species, callbackName, prefix);
        if (handlerInfo != null)
        {
            handlers.Add(ResolvePriority(new EventListener
            {
                Effect = species,
                HandlerInfo = handlerInfo,
                State = speciesState,
                End = null, // Species can't be removed
                EffectHolder = effectHolder,
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
                    handlers.Add(ResolvePriority(new EventListener
                    {
                        Effect = slotCondition,
                        HandlerInfo = handlerInfo,
                        State = slotConditionState,
                        End = hasCustomHolder ? null : SlotConditionRemoveEndDelegate,
                        EndCallArgs = [side, pokemon, conditionId],
                        EffectHolder = effectHolder,
                    }, callbackName));
                }
            }
        }
    }

    /// <summary>
    /// Finds event handlers for a Pokemon checking two prefixes in a single pass over effects.
    /// Skips effects that have no prefixed handlers for efficient batch lookups.
    /// Only used for non-None prefix pairs from <see cref="FindEventHandlers"/> (Ally+Any, Foe+Any).
    /// </summary>
    private void FindPokemonEventHandlersPrefixed(List<EventListener> handlers, Pokemon pokemon,
        EventId callbackName, EventPrefix prefix1, EventPrefix prefix2)
    {
        Condition status = pokemon.GetStatus();
        EffectState statusState = pokemon.StatusState;

        Ability ability = pokemon.GetAbility();
        EffectState abilityState = pokemon.AbilityState;

        Item item = pokemon.GetItem();
        EffectState itemState = pokemon.ItemState;

        Side side = pokemon.Side;
        EffectHolder effectHolder = pokemon;

        // Check status condition — skip if it has no prefixed handlers
        if (status.HasPrefixedHandlers)
        {
            EventHandlerInfo? handlerInfo = status.GetEventHandlerInfo(callbackName, prefix1);
            if (handlerInfo != null)
            {
                handlers.Add(ResolvePriority(new EventListener
                {
                    Effect = status,
                    HandlerInfo = handlerInfo,
                    State = statusState,
                    End = pokemon.ClearStatusEndDelegate,
                    EndCallArgs = Pokemon.ClearStatusEndCallArgs,
                    EffectHolder = effectHolder,
                }, callbackName));
            }

            handlerInfo = status.GetEventHandlerInfo(callbackName, prefix2);
            if (handlerInfo != null)
            {
                handlers.Add(ResolvePriority(new EventListener
                {
                    Effect = status,
                    HandlerInfo = handlerInfo,
                    State = statusState,
                    End = pokemon.ClearStatusEndDelegate,
                    EndCallArgs = Pokemon.ClearStatusEndCallArgs,
                    EffectHolder = effectHolder,
                }, callbackName));
            }
        }

        // Check volatile conditions — iterate once for both prefixes
        foreach ((ConditionId volatileId, EffectState volatileState) in pokemon.Volatiles)
        {
            Condition volatileCondition = Library.Conditions[volatileId];
            if (!volatileCondition.HasPrefixedHandlers) continue;

            EventHandlerInfo? handlerInfo = volatileCondition.GetEventHandlerInfo(callbackName, prefix1);
            if (handlerInfo != null)
            {
                handlers.Add(ResolvePriority(new EventListener
                {
                    Effect = volatileCondition,
                    HandlerInfo = handlerInfo,
                    State = volatileState,
                    End = pokemon.RemoveVolatileEndDelegate,
                    EndCallArgs = [volatileCondition],
                    EffectHolder = effectHolder,
                }, callbackName));
            }

            handlerInfo = volatileCondition.GetEventHandlerInfo(callbackName, prefix2);
            if (handlerInfo != null)
            {
                handlers.Add(ResolvePriority(new EventListener
                {
                    Effect = volatileCondition,
                    HandlerInfo = handlerInfo,
                    State = volatileState,
                    End = pokemon.RemoveVolatileEndDelegate,
                    EndCallArgs = [volatileCondition],
                    EffectHolder = effectHolder,
                }, callbackName));
            }
        }

        // Check ability
        if (ability.HasPrefixedHandlers)
        {
            EventHandlerInfo? handlerInfo = ability.GetEventHandlerInfo(callbackName, prefix1);
            if (handlerInfo != null)
            {
                handlers.Add(ResolvePriority(new EventListener
                {
                    Effect = ability,
                    HandlerInfo = handlerInfo,
                    State = abilityState,
                    End = pokemon.ClearAbilityEndDelegate,
                    EffectHolder = effectHolder,
                }, callbackName));
            }

            handlerInfo = ability.GetEventHandlerInfo(callbackName, prefix2);
            if (handlerInfo != null)
            {
                handlers.Add(ResolvePriority(new EventListener
                {
                    Effect = ability,
                    HandlerInfo = handlerInfo,
                    State = abilityState,
                    End = pokemon.ClearAbilityEndDelegate,
                    EffectHolder = effectHolder,
                }, callbackName));
            }
        }

        // Check held item
        if (item.HasPrefixedHandlers)
        {
            EventHandlerInfo? handlerInfo = item.GetEventHandlerInfo(callbackName, prefix1);
            if (handlerInfo != null)
            {
                handlers.Add(ResolvePriority(new EventListener
                {
                    Effect = item,
                    HandlerInfo = handlerInfo,
                    State = itemState,
                    End = pokemon.ClearItemEndDelegate,
                    EffectHolder = effectHolder,
                }, callbackName));
            }

            handlerInfo = item.GetEventHandlerInfo(callbackName, prefix2);
            if (handlerInfo != null)
            {
                handlers.Add(ResolvePriority(new EventListener
                {
                    Effect = item,
                    HandlerInfo = handlerInfo,
                    State = itemState,
                    End = pokemon.ClearItemEndDelegate,
                    EffectHolder = effectHolder,
                }, callbackName));
            }
        }

        // Species never has prefixed handlers — skip

        // Check slot conditions
        if (pokemon.Position < side.SlotConditions.Count)
        {
            foreach ((ConditionId conditionId, EffectState slotConditionState) in
                side.SlotConditions[pokemon.Position])
            {
                Condition slotCondition = Library.Conditions[conditionId];
                if (!slotCondition.HasPrefixedHandlers) continue;

                EventHandlerInfo? handlerInfo = slotCondition.GetEventHandlerInfo(callbackName, prefix1);
                if (handlerInfo != null)
                {
                    handlers.Add(ResolvePriority(new EventListener
                    {
                        Effect = slotCondition,
                        HandlerInfo = handlerInfo,
                        State = slotConditionState,
                        End = SlotConditionRemoveEndDelegate,
                        EndCallArgs = [side, pokemon, conditionId],
                        EffectHolder = effectHolder,
                    }, callbackName));
                }

                handlerInfo = slotCondition.GetEventHandlerInfo(callbackName, prefix2);
                if (handlerInfo != null)
                {
                    handlers.Add(ResolvePriority(new EventListener
                    {
                        Effect = slotCondition,
                        HandlerInfo = handlerInfo,
                        State = slotConditionState,
                        End = SlotConditionRemoveEndDelegate,
                        EndCallArgs = [side, pokemon, conditionId],
                        EffectHolder = effectHolder,
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
        EventHandlerInfo? handlerInfo = !Format.HasAnyEventHandlers
            ? null
            : Format.GetEventHandlerInfo(callbackName);

        if (handlerInfo != null || (getKey != null && FormatData.GetProperty(getKey) != null))
        {
            handlers.Add(ResolvePriority(new EventListener
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
                handlers.Add(ResolvePriority(new EventListener
                {
                    Effect = pseudoWeather,
                    HandlerInfo = handlerInfo,
                    State = pseudoWeatherState,
                    End = customHolder == null
                        ? field.RemovePseudoWeatherEndDelegate
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
            handlers.Add(ResolvePriority(new EventListener
            {
                Effect = weather,
                HandlerInfo = weatherHandlerInfo,
                State = Field.WeatherState,
                End = customHolder == null
                    ? field.ClearWeatherEndDelegate
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
            handlers.Add(ResolvePriority(new EventListener
            {
                Effect = terrain,
                HandlerInfo = terrainHandlerInfo,
                State = field.TerrainState,
                End = customHolder == null
                    ? field.ClearTerrainEndDelegate
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
                handlers.Add(ResolvePriority(new EventListener
                {
                    Effect = sideCondition,
                    HandlerInfo = handlerInfo,
                    State = sideConditionData,
                    End = customHolder == null
                        ? side.RemoveSideConditionEndDelegate
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