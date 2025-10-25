using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleAsync
{
    public RelayVar? SingleEvent(EventId eventId, IEffect effect, EffectState? state = null,
        SingleEventTarget? target = null, SingleEventSource? source = null, IEffect? sourceEffect = null,
        RelayVar? relayVar = null, EffectDelegate? customCallback = null)
    {
        // Check for stack overflow
        if (EventDepth >= 8)
        {
            if (DisplayUi)
            {
                Add("message", "STACK LIMIT EXCEEDED");
                Add("message", $"Event: {eventId}");
                Add("message", $"Parent event: {Event.Id}");
            }
            throw new InvalidOperationException("Stack overflow");
        }

        // Check for infinite loop
        if (Log.Count - SentLogPos > 1000)
        {
            if (DisplayUi)
            {
                Add("message", "LINE LIMIT EXCEEDED");
                Add("message", $"Event: {eventId}");
                Add("message", $"Parent event: {Event.Id}");
            }
            throw new InvalidOperationException("Infinite loop");
        }

        // Track if relayVar was explicitly provided
        bool hasRelayVar = relayVar != null;
        relayVar ??= new BoolRelayVar(true);

        // Check if status effect has changed
        if (effect.EffectType == EffectType.Status &&
            target is PokemonSingleEventTarget pokemonTarget)
        {
            Pokemon targetPokemon = pokemonTarget.Pokemon;
            if (effect is Condition condition && targetPokemon.Status != condition.Id)
            {
                // Status has changed; abort the event
                return relayVar;
            }
        }

        // Check if ability is suppressed by Mold Breaker
        if (eventId == EventId.SwitchIn &&
            effect.EffectType == EffectType.Ability &&
            effect is Ability { Flags.Breakable: true } &&
            target is PokemonSingleEventTarget moldbreakerTarget &&
            SuppressingAbility(moldbreakerTarget.Pokemon))
        {
            Debug($"{eventId} handler suppressed by Mold Breaker");
            return relayVar;
        }

        // Check if item is suppressed
        if (eventId != EventId.Start &&
            eventId != EventId.TakeItem &&
            effect.EffectType == EffectType.Item &&
            target is PokemonSingleEventTarget itemTarget &&
            itemTarget.Pokemon.IgnoringItem())
        {
            Debug($"{eventId} handler suppressed by Embargo, Klutz or Magic Room");
            return relayVar;
        }

        // Check if ability is suppressed by Gastro Acid/Neutralizing Gas
        if (eventId != EventId.End &&
            effect.EffectType == EffectType.Ability &&
            target is PokemonSingleEventTarget abilityTarget &&
            abilityTarget.Pokemon.IgnoringAbility())
        {
            Debug($"{eventId} handler suppressed by Gastro Acid or Neutralizing Gas");
            return relayVar;
        }

        // Check if weather is suppressed
        if (effect.EffectType == EffectType.Weather &&
            eventId != EventId.FieldStart &&
            eventId != EventId.FieldResidual &&
            eventId != EventId.FieldEnd &&
            Field.SuppressingWeather())
        {
            Debug($"{eventId} handler suppressed by Air Lock");
            return relayVar;
        }

        // Get the callback - either custom or from the effect
        EffectDelegate? callback = customCallback ?? effect.GetDelegate(eventId);
        if (callback == null) return relayVar;

        // Save parent context
        IEffect parentEffect = Effect;
        EffectState parentEffectState = EffectState;
        Event parentEvent = Event;

        // Set up new event context
        Effect = effect;
        EffectState = state ?? InitEffectState();
        Event = new Event
        {
            Id = eventId,
            Target = target,
            Source = source,
            Effect = sourceEffect,
        };
        EventDepth++;

        // Invoke the callback with appropriate parameters
        RelayVar? returnVal;
        try
        {
            returnVal = InvokeEventCallback(callback, hasRelayVar, relayVar, target, source, sourceEffect);
        }
        finally
        {
            // Restore parent context
            EventDepth--;
            Effect = parentEffect;
            EffectState = parentEffectState;
            Event = parentEvent;
        }

        return returnVal ?? relayVar;
    }

    public RelayVar? RunEvent(EventId eventId, RunEventTarget? target = null, RunEventSource? source = null,
        IEffect? sourceEffect = null, RelayVar? relayVar = null, bool? onEffect = null, bool? fastExit = null)
    {
        // Check for stack overflow
        if (EventDepth >= 8)
        {
            if (DisplayUi)
            {
                Add("message", "STACK LIMIT EXCEEDED");
                Add("message", "PLEASE REPORT IN BUG THREAD");
                Add("message", $"Event: {eventId}");
                Add("message", $"Parent event: {Event.Id}");
            }
            throw new InvalidOperationException("Stack overflow");
        }

        // Default target to Battle if not provided
        target ??= RunEventTarget.FromIBattle(this);

        // Extract the source Pokemon for handler lookup
        Pokemon? effectSource = source switch
        {
            PokemonRunEventSource pokemonSource => pokemonSource.Pokemon,
            _ => null,
        };

        // Find all handlers for this event
        var handlers = FindEventHandlers(target, eventId, effectSource);

        // If onEffect is true, add the sourceEffect's handler at the front
        if (onEffect == true)
        {
            if (sourceEffect == null)
            {
                throw new ArgumentNullException(nameof(sourceEffect), "onEffect passed without an effect");
            }

            EffectDelegate? callback = sourceEffect.GetDelegate(eventId);
            if (callback != null)
            {
                if (target is PokemonArrayRunEventTarget)
                {
                    throw new InvalidOperationException("Cannot use onEffect with array target");
                }

                // Add the effect's callback as the first handler
                handlers.Insert(0, ResolvePriority(new EventListenerWithoutPriority
                {
                    Effect = sourceEffect,
                    Callback = callback,
                    State = InitEffectState(),
                    End = null,
                    EffectHolder = target switch
                    {
                        PokemonRunEventTarget pokemonTarget => pokemonTarget.Pokemon,
                        SideRunEventTarget sideTarget => sideTarget.Side,
                        FieldRunEventTarget fieldTarget => fieldTarget.Field,
                        BattleRunEventTarget => EffectHolder.FromIBattle(this),
                        _ => throw new InvalidOperationException($"Unknown target type: {target.GetType().Name}"),
                    },
                }, eventId));
            }
        }

        // Sort handlers based on event type
        if (eventId is EventId.Invulnerability or EventId.TryHit or EventId.DamagingHit or EventId.EntryHazard)
        {
            handlers.Sort(CompareLeftToRightOrder);
        }
        else if (fastExit == true)
        {
            handlers.Sort(CompareRedirectOrder);
        }
        else
        {
            SpeedSort(handlers);
        }

        // Track if relayVar was explicitly provided
        bool hasRelayVar = relayVar != null;
        relayVar ??= new BoolRelayVar(true);

        // Save parent context
        Event parentEvent = Event;
        Event = new Event
        {
            Id = eventId,
            Target = target switch
            {
                PokemonRunEventTarget pokemonTarget => new PokemonSingleEventTarget(pokemonTarget.Pokemon),
                _ => null,
            },
            Source = source switch
            {
                PokemonRunEventSource pokemonSource => new PokemonSingleEventSource(pokemonSource.Pokemon),
                _ => null,
            },
            Effect = sourceEffect,
            Modifier = 1.0,
        };
        EventDepth++;

        // Handle array targets
        List<RelayVar>? targetRelayVars = null;
        if (target is PokemonArrayRunEventTarget arrayTarget)
        {
            if (relayVar is ArrayRelayVar arrayRelayVar)
            {
                targetRelayVars = arrayRelayVar.Values.ToList();
            }
            else
            {
                // Initialize all to true
                targetRelayVars = new List<RelayVar>(arrayTarget.PokemonList.Length);
                for (int i = 0; i < arrayTarget.PokemonList.Length; i++)
                {
                    targetRelayVars.Add(new BoolRelayVar(true));
                }
            }
        }

        // Execute each handler
        foreach (EventListener handler in handlers)
        {
            // For array targets, check if this specific target's relay var is falsy
            if (handler.Index.HasValue && targetRelayVars != null)
            {
                RelayVar currentRelayVar = targetRelayVars[handler.Index.Value];

                // Skip if falsy (except for DamagingHit with 0 damage)
                if (!IsRelayVarTruthy(currentRelayVar) &&
                    !(eventId == EventId.DamagingHit && currentRelayVar is IntRelayVar { Value: 0 }))
                {
                    continue;
                }

                // Update event target for this handler
                if (handler.Target != null)
                {
                    Event.Target = new PokemonSingleEventTarget(handler.Target);
                }

                // Use this target's relay var
                relayVar = currentRelayVar;
            }

            IEffect effect = handler.Effect;
            EffectHolder effectHolder = handler.EffectHolder;

            // Check if status has changed
            if (effect.EffectType == EffectType.Status &&
                effectHolder is PokemonEffectHolder pokemonHolder)
            {
                var condition = (Condition)effect;
                if (pokemonHolder.Pokemon.Status != condition.Id)
                {
                    // Status changed, skip this handler
                    continue;
                }
            }

            // Check for Mold Breaker suppression
            if (effect.EffectType == EffectType.Ability &&
                effectHolder is PokemonEffectHolder pokemonHolder2)
            {
                var ability = (Ability)effect;
                if ((ability.Flags.Breakable ?? false) && SuppressingAbility(pokemonHolder2.Pokemon))
                {
                    if (DisplayUi)
                    {
                        Debug($"{eventId} handler suppressed by Mold Breaker");
                    }
                    continue;
                }

                // For custom abilities (no num), check if this is an attacking event
                if (ability.Num == 0 && IsAttackingEvent(eventId, sourceEffect))
                {
                    if (DisplayUi)
                    {
                        Debug($"{eventId} handler suppressed by Mold Breaker");
                    }
                    continue;
                }
            }

            // Check for item suppression
            if (eventId is not (EventId.Start or EventId.SwitchIn or EventId.TakeItem) &&
                effect.EffectType == EffectType.Item &&
                effectHolder is PokemonEffectHolder pokemonHolder3 &&
                pokemonHolder3.Pokemon.IgnoringItem())
            {
                if (eventId != EventId.Update && DisplayUi)
                {
                    Debug($"{eventId} handler suppressed by Embargo, Klutz or Magic Room");
                }
                continue;
            }

            // Check for ability suppression
            if (eventId != EventId.End &&
                effect.EffectType == EffectType.Ability &&
                effectHolder is PokemonEffectHolder pokemonHolder4 &&
                pokemonHolder4.Pokemon.IgnoringAbility())
            {
                if (eventId != EventId.Update && DisplayUi)
                {
                    Debug($"{eventId} handler suppressed by Gastro Acid or Neutralizing Gas");
                }
                continue;
            }

            // Check for weather suppression
            if ((effect.EffectType == EffectType.Weather || eventId == EventId.Weather) &&
                eventId is not (EventId.Residual or EventId.End) &&
                Field.SuppressingWeather())
            {
                if (DisplayUi)
                {
                    Debug($"{eventId} handler suppressed by Air Lock");
                }
                continue;
            }

            // Execute the handler
            RelayVar? returnVal = null;

            // Save parent effect context
            IEffect parentEffect = Effect;
            EffectState parentEffectState = EffectState;

            // Set up handler's effect context
            Effect = handler.Effect;
            EffectState = handler.State ?? InitEffectState();
            EffectState.Target = effectHolder switch
            {
                PokemonEffectHolder pokemonEh => new PokemonEffectStateTarget(pokemonEh.Pokemon),
                SideEffectHolder sideEh => new SideEffectStateTarget(sideEh.Side),
                FieldEffectHolder fieldEh => new FieldEffectStateTarget(fieldEh.Field),
                BattleEffectHolder battleEh => EffectStateTarget.FromIBattle(battleEh.Battle),
                _ => null,
            };

            // Invoke the callback if present
            if (handler.Callback != null)
            {
                returnVal = InvokeEventCallback(
                    handler.Callback,
                    hasRelayVar,
                    relayVar,
                    Event.Target,
                    Event.Source,
                    sourceEffect
                );
            }

            // Restore parent effect context
            Effect = parentEffect;
            EffectState = parentEffectState;

            // Process return value
            if (returnVal != null)
            {
                relayVar = returnVal;

                // Check for early exit
                if (!IsRelayVarTruthy(relayVar) || fastExit == true)
                {
                    if (handler.Index.HasValue && targetRelayVars != null)
                    {
                        // Update this target's relay var
                        targetRelayVars[handler.Index.Value] = relayVar;

                        // Check if all targets are falsy
                        if (targetRelayVars.All(rv => !IsRelayVarTruthy(rv)))
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        // Restore event depth and parent event
        EventDepth--;

        // Apply event modifier to numeric relay vars
        if (relayVar is IntRelayVar intRelay && intRelay.Value == Math.Abs(intRelay.Value))
        {
            relayVar = new IntRelayVar(FinalModify(intRelay.Value));
        }

        Event = parentEvent;

        // Return appropriate result
        return target is PokemonArrayRunEventTarget ? new ArrayRelayVar([.. targetRelayVars ?? []]) : relayVar;
    }

    /// <summary>
    /// Runs an event with no source on each Pokémon on the field, in Speed order.
    /// Speed ties are resolved randomly using the battle's PRNG.
    /// </summary>
    public void EachEvent(EventId eventId, IEffect? effect = null, bool? relayVar = null)
    {
        // Get all active Pokémon on the field
        var actives = GetAllActive();

        // Use current battle effect if none provided
        effect ??= Effect;

        // Sort by speed (highest to lowest) with proper speed tie resolution
        SpeedSort(actives, (a, b) => b.Speed.CompareTo(a.Speed));

        // Convert bool? to RelayVar? for RunEvent
        RelayVar? relayVarConverted = relayVar.HasValue ? new BoolRelayVar(relayVar.Value) : null;

        // Run the event on each Pokémon
        foreach (Pokemon pokemon in actives)
        {
            RunEvent(eventId, new PokemonRunEventTarget(pokemon), null, effect, relayVarConverted);
        }

        // Special handling for Weather events in Gen 7+
        if (eventId == EventId.Weather && Gen >= 7)
        {
            // TODO: further research when updates happen
            EachEvent(EventId.Update);
        }
    }

    /// <summary>
    /// Runs an event with no source on each effect on the field, in Speed order.
    /// 
    /// Unlike EachEvent, this contains a lot of other handling and is only intended for
    /// the 'Residual' and 'SwitchIn' events.
    /// </summary>
    /// <param name="eventId">The event to trigger (typically Residual or SwitchIn)</param>
    /// <param name="targets">Optional list of Pokemon to filter handlers for</param>
    public void FieldEvent(EventId eventId, List<Pokemon>? targets = null)
    {
        // Determine if we should track duration for this event
        EffectStateKey? getKey = eventId == EventId.Residual ? EffectStateKey.Duration : null;

        // Build the field and side event IDs based on the base event
        // e.g., "Residual" becomes "FieldResidual" and "SideResidual"
        EventId fieldEventId = GetFieldEventId(eventId);
        EventId sideEventId = GetSideEventId(eventId);
        EventId anyEventId = EventIdInfo.CombinePrefixWithEvent(EventPrefix.Any, eventId, Library.Events);

        // Collect all handlers from field-level effects
        var handlers = FindFieldEventHandlers(Field, fieldEventId, getKey);

        // Collect handlers from sides and active Pokemon
        foreach (Side side in Sides)
        {
            // Add side condition handlers (but not for ally sides in multi battles)
            // In single battles, side.N is always < 2, so this always executes
            if (side.N < 2)
            {
                handlers.AddRange(FindSideEventHandlers(side, sideEventId, getKey));
            }

            // Process each active Pokemon on this side
            foreach (Pokemon active in side.Active.OfType<Pokemon>())
            {
                // For SwitchIn events, also trigger AnySwitchIn handlers
                if (eventId == EventId.SwitchIn)
                {
                    handlers.AddRange(FindPokemonEventHandlers(active, anyEventId));
                }

                // If targets were specified, only process those Pokemon
                if (targets != null && !targets.Contains(active)) continue;

                // Collect handlers from this Pokemon and related effects
                handlers.AddRange(FindPokemonEventHandlers(active, eventId, getKey));
                handlers.AddRange(FindSideEventHandlers(side, eventId, customHolder: active));
                handlers.AddRange(FindFieldEventHandlers(Field, eventId, customHolder: active));
                handlers.AddRange(FindBattleEventHandlers(eventId, getKey, active));
            }
        }

        // Sort handlers by speed order
        SpeedSort(handlers);

        // Execute each handler in order
        while (handlers.Count > 0)
        {
            EventListener handler = handlers[0];
            handlers.RemoveAt(0);

            IEffect effect = handler.Effect;

            // Skip fainted Pokemon unless this is a slot condition
            if (handler.EffectHolder is PokemonEffectHolder { Pokemon.Fainted: true })
            {
                if (handler.State?.IsSlotCondition != true) continue;
            }

            // Handle duration tracking for Residual events
            if (eventId == EventId.Residual && handler is { End: not null, State.Duration: not null })
            {
                handler.State.Duration--;
                if (handler.State.Duration <= 0)
                {
                    // Effect has expired, trigger its end callback
                    List<object?> endCallArgsList = [.. handler.EndCallArgs ?? []];
                    if (endCallArgsList.Count == 0)
                    {
                        endCallArgsList.Add(handler.EffectHolder);
                        endCallArgsList.Add(effect.EffectStateId);
                    }

                    // Invoke the end callback
                    Delegate? endDelegate = handler.End.GetDelegate();
                    endDelegate?.DynamicInvoke([.. endCallArgsList]);

                    if (Ended) return;
                    continue;
                }
            }

            // Verify the effect hasn't been removed by a prior handler
            // (e.g., Toxic Spikes being absorbed during a double switch)
            if (handler.State?.Target != null)
            {
                EffectState? expectedStateLocation = null;

                if (handler.State.Target is PokemonEffectStateTarget pokemonTarget)
                {
                    Pokemon pokemon = pokemonTarget.Pokemon;

                    // Determine where this effect's state should be stored
                    // Check if this is an ability state (not starting with "ability:" prefix means it's the main ability)
                    if (effect is { EffectType: EffectType.Ability, EffectStateId: not AbilityEffectStateId })
                    {
                        expectedStateLocation = pokemon.AbilityState;
                    }
                    // Check if this is an item state (not starting with "item:" prefix means it's the main item)
                    else if (effect is { EffectType: EffectType.Item, EffectStateId: not ItemEffectStateId })
                    {
                        expectedStateLocation = pokemon.ItemState;
                    }
                    else if (effect.EffectType == EffectType.Status)
                    {
                        expectedStateLocation = pokemon.StatusState;
                    }
                    else if (effect.EffectStateId is ConditionEffectStateId conditionId)
                    {
                        pokemon.Volatiles.TryGetValue(conditionId.ConditionId, out expectedStateLocation);
                    }

                    // If the state doesn't match, the effect was removed
                    if (expectedStateLocation != handler.State)
                    {
                        continue;
                    }
                }
                else if (handler.State.Target is SideEffectStateTarget sideTarget &&
                         handler.State.IsSlotCondition != true)
                {
                    Side targetSide = sideTarget.Side;

                    // Check if side condition still exists
                    if (effect.EffectStateId is ConditionEffectStateId conditionId)
                    {
                        if (!targetSide.SideConditions.TryGetValue(conditionId.ConditionId,
                            out EffectState? sideConditionState) ||
                            sideConditionState != handler.State)
                        {
                            continue;
                        }
                    }
                }
                else if (handler.State.Target is FieldEffectStateTarget fieldTarget)
                {
                    Field targetField = fieldTarget.Field;

                    // Determine where this field effect's state should be stored
                    if (effect.EffectType == EffectType.Weather)
                    {
                        expectedStateLocation = targetField.WeatherState;
                    }
                    else if (effect.EffectType == EffectType.Terrain)
                    {
                        expectedStateLocation = targetField.TerrainState;
                    }
                    else if (effect.EffectStateId is ConditionEffectStateId conditionId)
                    {
                        targetField.PseudoWeather.TryGetValue(conditionId.ConditionId,
                            out expectedStateLocation);
                    }

                    // If the state doesn't match, the effect was removed
                    if (expectedStateLocation != handler.State)
                    {
                        continue;
                    }
                }
            }

            // Determine the appropriate event ID based on the effect holder type
            EventId handlerEventId = eventId;
            if (handler.EffectHolder is SideEffectHolder)
            {
                handlerEventId = GetSideEventId(eventId);
            }
            else if (handler.EffectHolder is FieldEffectHolder)
            {
                handlerEventId = GetFieldEventId(eventId);
            }

            // Execute the handler's callback
            if (handler.Callback != null)
            {
                SingleEventTarget? singleEventTarget = handler.EffectHolder switch
                {
                    PokemonEffectHolder pokemonEh => new PokemonSingleEventTarget(pokemonEh.Pokemon),
                    SideEffectHolder sideEh => new SideSingleEventTarget(sideEh.Side),
                    FieldEffectHolder fieldEh => new FieldSingleEventTarget(fieldEh.Field),
                    BattleEffectHolder battleEh => SingleEventTarget.FromIBattle(battleEh.Battle),
                    _ => null,
                };

                SingleEvent(handlerEventId, effect, handler.State, singleEventTarget,
                    null, null, null, handler.Callback);
            }

            // Process any faint messages and check if battle has ended
            FaintMessages();
            if (Ended) return;
        }
    }

    public RelayVar? PriorityEvent(EventId eventId, PokemonSideBattleUnion target, Pokemon? source = null,
        IEffect? effect = null, RelayVar? relayVar = null, bool onEffect = false)
    {
        return RunEvent(eventId, RunEventTarget.FromPokemonSideBattleUnion(target),
            RunEventSource.FromNullablePokemon(source), effect, relayVar, onEffect, true);
    }

    public void OnEvent(EventId eventId, Format target, object[] rest)
    {
        throw new NotImplementedException("This method is for attaching custom event handlers to a Battle." +
                                          "It shouldn't be used in this implementation.");
    }

    #region Helpers

    /// <summary>
    /// Invokes an event callback with the appropriate parameters based on its signature.
    /// Optimized version that avoids DynamicInvoke and reduces allocations.
    /// </summary>
    private RelayVar? InvokeEventCallback(EffectDelegate callback, bool hasRelayVar, RelayVar relayVar,
        SingleEventTarget? target, SingleEventSource? source, IEffect? sourceEffect)
    {
        // Handle non-function callbacks (constants) - fast path
        switch (callback)
        {
            case OnFlinchEffectDelegate { OnFlinch: OnFlinchBool ofb }:
                return new BoolRelayVar(ofb.Value);
            case OnCriticalHitEffectDelegate { OnCriticalHit: OnCriticalHitBool ocb }:
                return new BoolRelayVar(ocb.Value);
            case OnFractionalPriorityEffectDelegate { OnFractionalPriority: OnFrationalPriorityNeg ofpn }:
                return new DecimalRelayVar(ofpn.Value);
            case OnTakeItemEffectDelegate { OnTakeItem: OnTakeItemBool otib }:
                return new BoolRelayVar(otib.Value);
            case OnTryHealEffectDelegate { OnTryHeal: OnTryHealBool othb }:
                return new BoolRelayVar(othb.Value);
            case OnTryEatItemEffectDelegate { OnTryEatItem: BoolOnTryEatItem botei }:
                return new BoolRelayVar(botei.Value);
            case OnNegateImmunityEffectDelegate { OnNegateImmunity: OnNegateImmunityBool onib }:
                return new BoolRelayVar(onib.Value);
            case OnLockMoveEffectDelegate { OnLockMove: OnLockMoveMoveId olmmi }:
                return new MoveIdRelayVar(olmmi.Id);
        }

        // Extract the actual delegate and invoke directly based on known signatures
        // This avoids the overhead of DynamicInvoke by pattern matching to known delegate types
        return callback switch
        {
            // DelegateEffectDelegate is the generic wrapper - try common signatures first
            DelegateEffectDelegate ded => InvokeDelegateEffectDelegate(ded.Del, hasRelayVar, relayVar, target, source, sourceEffect),

            // Specific delegate types with known signatures
            OnFlinchEffectDelegate { OnFlinch: OnFlinchFunc off } => InvokeStandardDelegate(off.Func, hasRelayVar, relayVar, target, source, sourceEffect),
            OnCriticalHitEffectDelegate { OnCriticalHit: OnCriticalHitFunc ocf } => InvokeStandardDelegate(ocf.Function, hasRelayVar, relayVar, target, source, sourceEffect),
            OnFractionalPriorityEffectDelegate { OnFractionalPriority: OnFractionalPriorityFunc ofpf } => InvokeStandardDelegate(ofpf.Function, hasRelayVar, relayVar, target, source, sourceEffect),
            OnTakeItemEffectDelegate { OnTakeItem: OnTakeItemFunc otif } => InvokeStandardDelegate(otif.Func, hasRelayVar, relayVar, target, source, sourceEffect),
            OnTryHealEffectDelegate { OnTryHeal: OnTryHealFunc1 othf1 } => InvokeStandardDelegate(othf1.Func, hasRelayVar, relayVar, target, source, sourceEffect),
            OnTryHealEffectDelegate { OnTryHeal: OnTryHealFunc2 othf2 } => InvokeStandardDelegate(othf2.Func, hasRelayVar, relayVar, target, source, sourceEffect),
            OnTryEatItemEffectDelegate { OnTryEatItem: FuncOnTryEatItem fotei } => InvokeStandardDelegate(fotei.Func, hasRelayVar, relayVar, target, source, sourceEffect),
            OnNegateImmunityEffectDelegate { OnNegateImmunity: OnNegateImmunityFunc onif } => InvokeStandardDelegate(onif.Func, hasRelayVar, relayVar, target, source, sourceEffect),
            OnLockMoveEffectDelegate { OnLockMove: OnLockMoveFunc olmf } => InvokeStandardDelegate(olmf.Func, hasRelayVar, relayVar, target, source, sourceEffect),

            _ => throw new InvalidOperationException($"Unknown EffectDelegate type: {callback.GetType().Name}"),
        };
    }

    /// <summary>
    /// Helper method for invoking standard delegates with common signatures.
    /// This provides a single path for most delegate invocations, reducing code duplication.
    /// </summary>
    private RelayVar? InvokeStandardDelegate(Delegate del, bool hasRelayVar, RelayVar relayVar,
        SingleEventTarget? target, SingleEventSource? source, IEffect? sourceEffect)
    {
        // Reuse the optimized invocation logic
        return InvokeDelegateEffectDelegate(del, hasRelayVar, relayVar, target, source, sourceEffect);
    }

    #endregion
}