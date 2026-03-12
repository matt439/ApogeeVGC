using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class Battle
{
    public RelayVar? SingleEvent(EventId eventId, IEffect effect, EffectState? state = null,
        SingleEventTarget? target = null, SingleEventSource? source = null,
        IEffect? sourceEffect = null,
        RelayVar? relayVar = null, EffectDelegate? customCallback = null)
    {
        // Match Showdown: when relayVar is not provided, default to true
        // (Showdown line 593-594: if (relayVar === undefined) { relayVar = true; })
        relayVar ??= BoolRelayVar.True;
        // Debug logging for event entry
        if (DisplayUi && eventId == EventId.SwitchIn)
        {
            Debug(
                $"[SingleEvent] ENTRY: eventId={eventId}, effect={effect.Name}, effectType={effect.EffectType}");
        }

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
        if (LogMessageCount - SentLogPos > 5000)
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
        relayVar ??= BoolRelayVar.True;

        // Check if status effect has changed
        if (effect.EffectType == EffectType.Status &&
            target is { Kind: SingleEventTargetKind.Pokemon } pokemonTarget)
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
            target is { Kind: SingleEventTargetKind.Pokemon } moldbreakerTarget &&
            SuppressingAbility(moldbreakerTarget.Pokemon))
        {
            Debug($"{eventId} handler suppressed by Mold Breaker");
            return relayVar;
        }

        // Check if item is suppressed
        if (eventId != EventId.Start &&
            eventId != EventId.TakeItem &&
            effect.EffectType == EffectType.Item &&
            target is { Kind: SingleEventTargetKind.Pokemon } itemTarget &&
            itemTarget.Pokemon.IgnoringItem())
        {
            Debug($"{eventId} handler suppressed by Embargo, Klutz or Magic Room");
            return relayVar;
        }

        // Check if ability is suppressed by Gastro Acid/Neutralizing Gas
        if (eventId != EventId.End &&
            effect.EffectType == EffectType.Ability &&
            target is { Kind: SingleEventTargetKind.Pokemon } abilityTarget &&
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

        // Get the handler - either custom callback or from the effect's EventHandlerInfo
        EventHandlerInfo? handlerInfo = effect.GetEventHandlerInfo(eventId);

        if (DisplayUi && eventId == EventId.SwitchIn)
        {
            Debug(
                $"[SingleEvent] handlerInfo for {effect.Name}: {(handlerInfo != null ? $"FOUND (Id={handlerInfo.Id})" : "NULL")}");
        }

        if (handlerInfo == null)
        {
            return relayVar;
        }

        // Save parent context
        IEffect parentEffect = Effect;
        EffectState parentEffectState = EffectState;
        Event parentEvent = Event;

        // Set up new event context
        Effect = effect;
        bool rentedEffectState = state == null;
        EffectState = state ?? RentEffectState();
        Event rentedEvent = _eventPool.Count > 0 ? _eventPool.Pop() : new Event();
        rentedEvent.Id = eventId;
        rentedEvent.Target = target;
        rentedEvent.Source = source;
        rentedEvent.Effect = sourceEffect;
        Event = rentedEvent;
        EventDepth++;

        // Invoke the handler with appropriate parameters
        RelayVar? returnVal;
        try
        {
            if (DisplayUi && eventId == EventId.SwitchIn)
            {
                Debug(
                    $"[SingleEvent] About to invoke handler for {effect.Name}, handlerInfo.Id={handlerInfo.Id}");
            }

            returnVal = InvokeEventHandlerInfo(handlerInfo, hasRelayVar, relayVar, target,
                source, sourceEffect);

            if (DisplayUi && eventId == EventId.SwitchIn)
            {
                Debug(
                    $"[SingleEvent] Handler invoked for {effect.Name}, returnVal={(returnVal != null ? returnVal.GetType().Name : "null")}");
            }
        }
        finally
        {
            // Restore parent context and return event/state to pools
            EventDepth--;
            Effect = parentEffect;
            if (rentedEffectState) ReturnEffectState(EffectState);
            EffectState = parentEffectState;
            rentedEvent.Reset();
            _eventPool.Push(rentedEvent);
            Event = parentEvent;
        }

        // If handler returned null (TS undefined), return relayVar (defaults to true).
        // This matches pokemon-showdown's singleEvent (line 651): returnVal === undefined ? relayVar : returnVal
        // where relayVar defaults to true (line 594) when not explicitly provided.
        return returnVal ?? relayVar;
    }

    public RelayVar? RunEvent(EventId eventId, RunEventTarget? target = null,
        RunEventSource? source = null,
        IEffect? sourceEffect = null, RelayVar? relayVar = null, bool? onEffect = null,
        bool? fastExit = null)
    {
        // Debug logging for event entry - commented out for reduced verbosity

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
        target ??= RunEventTarget.FromBattle(this);

        // Extract the source Pokemon for handler lookup
        Pokemon? effectSource = source switch
        {
            { IsPokemon: true } pokemonSource => pokemonSource.Pokemon,
            _ => null,
        };

        // Find all handlers for this event
        List<EventListener> handlers;
        if (_handlerListPool.Count > 0)
        {
            handlers = _handlerListPool.Pop();
            handlers.Clear();
        }
        else
        {
            handlers = [];
        }
        FindEventHandlers(handlers, target.Value, eventId, effectSource);

        // Debug logging for handler count - commented out for reduced verbosity

        // If onEffect is true, add the sourceEffect's handler at the front
        if (onEffect == true)
        {
            if (sourceEffect == null)
            {
                throw new ArgumentNullException(nameof(sourceEffect),
                    "onEffect passed without an effect");
            }

            EventHandlerInfo? handlerInfo = sourceEffect.GetEventHandlerInfo(eventId);
            if (handlerInfo != null)
            {
                if (target is { Kind: RunEventTargetKind.PokemonArray })
                {
                    throw new InvalidOperationException("Cannot use onEffect with array target");
                }

                // Add the effect's handler as the first handler
                handlers.Insert(0, ResolvePriority(new EventListener
                {
                    Effect = sourceEffect,
                    HandlerInfo = handlerInfo,
                    State = InitEffectState(),
                    End = null,
                    EffectHolder = target switch
                    {
                        { Kind: RunEventTargetKind.Pokemon } t => t.Pokemon,
                        { Kind: RunEventTargetKind.Side } t => t.Side,
                        { Kind: RunEventTargetKind.Field } t => t.Field,
                        { Kind: RunEventTargetKind.Battle } => EffectHolder.FromBattle(this),
                        _ => throw new InvalidOperationException(
                            $"Unknown target type: {target?.Kind}"),
                    },
                }, eventId));
            }
        }

        // Sort handlers based on event type
        if (eventId is EventId.Invulnerability or EventId.TryHit or EventId.DamagingHit
            or EventId.EntryHazard)
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
        relayVar ??= BoolRelayVar.True;

        // Save parent context
        Event parentEvent = Event;
        Event rentedEvent = _eventPool.Count > 0 ? _eventPool.Pop() : new Event();
        rentedEvent.Id = eventId;
        rentedEvent.Target = target?.ToSingleEventTarget();
        rentedEvent.Source = source switch
        {
            { IsPokemon: true } pokemonSource => SingleEventSource.FromPokemon(pokemonSource
                .Pokemon),
            { IsType: true } typeSource => SingleEventSource.FromPokemonType(typeSource.Type),
            _ => null as SingleEventSource?,
        };
        rentedEvent.Effect = sourceEffect;
        rentedEvent.Modifier = 1.0;
        Event = rentedEvent;
        EventDepth++;

        try
        {
        // Handle array targets
        List<RelayVar>? targetRelayVars = null;
        if (target is { Kind: RunEventTargetKind.PokemonArray } arrayTarget)
        {
            if (relayVar is ArrayRelayVar arrayRelayVar)
            {
                // Copy values without LINQ .ToList() allocation
                List<RelayVar> srcValues = arrayRelayVar.Values;
                targetRelayVars = new List<RelayVar>(srcValues.Count);
                for (int i = 0; i < srcValues.Count; i++)
                {
                    targetRelayVars.Add(srcValues[i]);
                }
            }
            else
            {
                // Initialize all to true
                targetRelayVars = new List<RelayVar>(arrayTarget.PokemonList.Length);
                for (int i = 0; i < arrayTarget.PokemonList.Length; i++)
                {
                    targetRelayVars.Add(BoolRelayVar.True);
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
                    !(eventId == EventId.DamagingHit &&
                      currentRelayVar is IntRelayVar { Value: 0 }))
                {
                    continue;
                }

                // Update event target for this handler
                if (handler.Target != null)
                {
                    Event.Target = SingleEventTarget.FromNullablePokemon(handler.Target);
                }

                // Use this target's relay var
                relayVar = currentRelayVar;
            }

            IEffect effect = handler.Effect;
            EffectHolder effectHolder = handler.EffectHolder;

            // Skip fainted Pokemon unless this is a slot condition (Showdown: battle.ts line 512)
            if (effectHolder is PokemonEffectHolder { Pokemon.Fainted: true })
            {
                if (handler.State?.IsSlotCondition != true)
                {
                    continue;
                }
            }

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
                effectHolder is PokemonEffectHolder pokemonHolder2 &&
                effect is Ability ability)
            {
                if ((ability.Flags.Breakable ?? false) &&
                    SuppressingAbility(pokemonHolder2.Pokemon))
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
            bool rentedHandlerState = handler.State == null;

            try
            {
                // Set up handler's effect context
                Effect = handler.Effect;
                EffectState = handler.State ?? RentEffectState();
                EffectState.Target = effectHolder switch
                {
                    PokemonEffectHolder pokemonEh => new PokemonEffectStateTarget(pokemonEh.Pokemon),
                    SideEffectHolder sideEh => new SideEffectStateTarget(sideEh.Side),
                    FieldEffectHolder fieldEh => new FieldEffectStateTarget(fieldEh.Field),
                    BattleEffectHolder battleEh => EffectStateTarget.FromBattle(battleEh.Battle),
                    _ => null,
                };

                // Invoke the handler if present
                if (handler.HandlerInfo != null)
                {
                    returnVal = InvokeEventHandlerInfo(
                        handler.HandlerInfo,
                        hasRelayVar,
                        relayVar,
                        Event.Target,
                        Event.Source,
                        sourceEffect
                    );

                }
            }
            finally
            {
                // Restore parent effect context (always, even on exception)
                Effect = parentEffect;
                if (rentedHandlerState) ReturnEffectState(EffectState);
                EffectState = parentEffectState;
            }

            // Process return value
            if (returnVal != null)
            {
                // Don't replace relayVar if handler returned VoidReturn (meaning "no change")
                if (returnVal is not VoidReturnRelayVar)
                {
                    relayVar = returnVal;
                }

                // Check for early exit
                if (!IsRelayVarTruthy(relayVar) || fastExit == true)
                {
                    if (handler.Index.HasValue && targetRelayVars != null)
                    {
                        // Update this target's relay var
                        targetRelayVars[handler.Index.Value] = relayVar;

                        // Check if all targets are falsy
                        bool allFalsy = true;
                        for (int ti = 0; ti < targetRelayVars.Count; ti++)
                        {
                            if (IsRelayVarTruthy(targetRelayVars[ti]))
                            {
                                allFalsy = false;
                                break;
                            }
                        }
                        if (allFalsy)
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

        // Apply event modifier to numeric relay vars
        if (relayVar is IntRelayVar intRelay && intRelay.Value == Math.Abs(intRelay.Value))
        {
            relayVar = IntRelayVar.Get(FinalModify(intRelay.Value));
        }

        // Return appropriate result
        return target is { Kind: RunEventTargetKind.PokemonArray }
            ? new ArrayRelayVar(targetRelayVars ?? [])
            : relayVar;
        }
        finally
        {
            // Return handler list to pool for reuse
            handlers.Clear();
            _handlerListPool.Push(handlers);

            // Return event to pool and restore parent event (always, even on exception)
            EventDepth--;
            rentedEvent.Reset();
            _eventPool.Push(rentedEvent);
            Event = parentEvent;
        }
    }

    /// <summary>
    /// Runs an event with no source on each Pok�mon on the field, in Speed order.
    /// Speed ties are resolved randomly using the battle's PRNG.
    /// </summary>
    public void EachEvent(EventId eventId, IEffect? effect = null, bool? relayVar = null)
    {
        var actives = RentPokemonList();
        try
        {
        while (true)
        {
            // Get all active Pok�mon on the field
            FillAllActive(actives);

            // Use current battle effect if none provided
            effect ??= Effect;

            // Sort by speed (highest to lowest) with proper speed tie resolution
            if (Prng.TraceEnabled)
            {
                Console.Error.WriteLine($"[EachEvent] {eventId} speeds: {string.Join(", ", actives.Select(p => $"{p.Name}={p.Speed}"))}");
            }
            SpeedSort(actives, (a, b) => b.Speed.CompareTo(a.Speed));

            // Convert bool? to RelayVar? for RunEvent
            RelayVar? relayVarConverted =
                relayVar.HasValue ? (relayVar.Value ? BoolRelayVar.True : BoolRelayVar.False) : null;

            // Run the event on each Pok�mon
            foreach (Pokemon pokemon in actives)
            {
                RunEvent(eventId, (RunEventTarget)pokemon, null, effect,
                    relayVarConverted);
            }

            // Special handling for Weather events in Gen 7+
            if (eventId == EventId.Weather && Gen >= 7)
            {
                eventId = EventId.Update;
                effect = null;
                relayVar = null;
                continue;
            }

            break;
        }
        }
        finally
        {
            ReturnPokemonList(actives);
        }
    }

    public void FieldEvent(EventId eventId, List<Pokemon>? targets = null)
    {
        // Debug logging for event entry
        if (DisplayUi && eventId == EventId.SwitchIn)
        {
            string targetsStr = targets != null
                ? $"[{string.Join(", ", targets.Select(p => p.Name))}]"
                : "all";
            Debug($"[FieldEvent] ENTRY: {eventId} | Targets: {targetsStr}");
        }

        // Debug: uncomment to trace residual handlers
        // if (eventId == EventId.Residual) { ... }

        // Determine if we should track duration for this event
        EffectStateKey? getKey = eventId == EventId.Residual ? EffectStateKey.Duration : null;

        // Build the field and side event IDs based on the base event
        // e.g., "Residual" becomes "FieldResidual" and "SideResidual"
        EventId fieldEventId = GetFieldEventId(eventId);
        EventId sideEventId = GetSideEventId(eventId);

        // Collect all handlers from field-level effects
        List<EventListener> handlers;
        if (_handlerListPool.Count > 0)
        {
            handlers = _handlerListPool.Pop();
            handlers.Clear();
        }
        else
        {
            handlers = [];
        }
        FindFieldEventHandlers(handlers, Field, fieldEventId, EventPrefix.None, getKey);

        // Collect handlers from sides and active Pokemon
        foreach (Side side in Sides)
        {
            // Add side condition handlers (but not for ally sides in multi battles)
            // In single battles, side.N is always < 2, so this always executes
            if (side.N < 2)
            {
                FindSideEventHandlers(handlers, side, sideEventId, EventPrefix.None, getKey);
            }

            // Process each active Pokemon on this side
            foreach (Pokemon? activeSlot in side.Active)
            {
                if (activeSlot is not Pokemon active) continue;
                // For SwitchIn events, also trigger AnySwitchIn handlers
                if (eventId == EventId.SwitchIn)
                {
                    FindPokemonEventHandlers(handlers, active, eventId, EventPrefix.Any);
                }

                // If targets were specified, only process those Pokemon
                if (targets != null && !targets.Contains(active)) continue;

                // Collect handlers from this Pokemon and related effects
                FindPokemonEventHandlers(handlers, active, eventId, EventPrefix.None,
                    getKey);
                FindSideEventHandlers(handlers, side, eventId, customHolder: active);
                FindFieldEventHandlers(handlers, Field, eventId, customHolder: active);
                FindBattleEventHandlers(handlers, eventId, getKey, active);
            }
        }

        // Sort handlers by speed order
        SpeedSort(handlers);

        // Execute each handler in order
        int handlerIndex = 0;
        try
        {
        while (handlers.Count > 0)
        {
            EventListener handler = handlers[0];
            handlers.RemoveAt(0);
            handlerIndex++;

            IEffect effect = handler.Effect;

            if (DisplayUi && eventId == EventId.SwitchIn)
            {
                Debug(
                    $"[FieldEvent] Processing handler {handlerIndex}: {effect.Name} ({effect.EffectType})");
            }

            // Skip fainted Pokemon unless this is a slot condition
            if (handler.EffectHolder is PokemonEffectHolder { Pokemon.Fainted: true })
            {
                if (handler.State?.IsSlotCondition != true)
                {
                    continue;
                }
            }

            if (DisplayUi && eventId == EventId.SwitchIn)
            {
                Debug($"[FieldEvent] {effect.Name}: Passed fainted check");
            }

            // Handle duration tracking for Residual events
            if (eventId == EventId.Residual &&
                handler is { End: not null, State.Duration: not null })
            {
                handler.State.Duration--;

                if (handler.State.Duration <= 0)
                {
                    // Effect has expired, trigger its end callback
                    // Use provided EndCallArgs or empty array for no args
                    object?[] endCallArgs = handler.EndCallArgs?.ToArray() ?? [];

                    // Invoke the end callback
                    Delegate? endDelegate = handler.End.GetDelegate();
                    endDelegate?.DynamicInvoke(endCallArgs);

                    if (Ended) return;
                    continue;
                }
            }

            if (DisplayUi && eventId == EventId.SwitchIn)
            {
                Debug($"[FieldEvent] {effect.Name}: Passed duration check");
                Debug(
                    $"[FieldEvent] {effect.Name}: handler.State is {(handler.State != null ? "NOT NULL" : "NULL")}");
                if (handler.State != null)
                {
                    Debug(
                        $"[FieldEvent] {effect.Name}: handler.State.Target is {(handler.State.Target != null ? "NOT NULL" : "NULL")}");
                }
            }

            // Debug($"[FieldEvent] {effect.Name}: Passed duration check");

            // Verify the effect hasn't been removed by a prior handler
            // (e.g., Toxic Spikes being absorbed during a double switch)
            //
            // Skip verification for abilities and items on SwitchIn events, since these are
            // triggered via their OnStart handlers and the state references may not match yet
            bool skipVerification = eventId == EventId.SwitchIn &&
                                    effect.EffectType is EffectType.Ability or EffectType.Item;

            if (!skipVerification && handler.State?.Target != null)
            {
                EffectState? expectedStateLocation = null;

                if (handler.State.Target is PokemonEffectStateTarget pokemonTarget)
                {
                    Pokemon pokemon = pokemonTarget.Pokemon;

                    // Determine where this effect's state should be stored
                    // Check if this is the main ability state (AbilityEffectStateId = main ability)
                    if (effect is
                        { EffectType: EffectType.Ability, EffectStateId: AbilityEffectStateId })
                    {
                        expectedStateLocation = pokemon.AbilityState;

                        if (DisplayUi && eventId == EventId.SwitchIn)
                        {
                            Debug(
                                $"[FieldEvent] {effect.Name}: Ability state check - expectedStateLocation == handler.State? {ReferenceEquals(expectedStateLocation, handler.State)}");
                        }
                    }
                    // Check if this is an item state (not starting with "item:" prefix means it's the main item)
                    else if (effect.EffectType == EffectType.Item)
                    {
                        expectedStateLocation = pokemon.ItemState;
                    }
                    else if (effect.EffectType == EffectType.Status)
                    {
                        expectedStateLocation = pokemon.StatusState;
                    }
                    else if (effect.EffectStateId is ConditionEffectStateId conditionId)
                    {
                        pokemon.Volatiles.TryGetValue(conditionId.ConditionId,
                            out expectedStateLocation);
                    }

                    // If the state doesn't match, the effect was removed
                    if (expectedStateLocation != handler.State)
                    {
                        // Debug(
                        // $"FieldEvent {eventId}: Skipping {effect.Name} (effect was removed)");

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
                            // Debug(
                            //  $"FieldEvent {eventId}: Skipping {effect.Name} (side condition was removed)");

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
                        // Debug(
                        //     $"FieldEvent {eventId}: Skipping {effect.Name} (field effect was removed)");

                        continue;
                    }
                }
            }

            if (DisplayUi && eventId == EventId.SwitchIn)
            {
                Debug($"[FieldEvent] {effect.Name}: Passed effect verification check");
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

            if (DisplayUi && eventId == EventId.SwitchIn)
            {
                Debug($"[FieldEvent] {effect.Name}: Determined handlerEventId={handlerEventId}");
            }

            // Execute the handler's callback
            if (DisplayUi && eventId == EventId.SwitchIn)
            {
                Debug(
                    $"[FieldEvent] handler.HandlerInfo is {(handler.HandlerInfo != null ? "NOT NULL" : "NULL")} for {effect.Name}");
            }

            if (handler.HandlerInfo != null)
            {
                SingleEventTarget? singleEventTarget = handler.EffectHolder switch
                {
                    PokemonEffectHolder pokemonEh =>
                        (SingleEventTarget?)pokemonEh.Pokemon,
                    SideEffectHolder sideEh => (SingleEventTarget?)sideEh.Side,
                    FieldEffectHolder fieldEh => (SingleEventTarget?)fieldEh.Field,
                    BattleEffectHolder battleEh => (SingleEventTarget?)SingleEventTarget.FromBattle(battleEh.Battle),
                    _ => null,
                };

                if (DisplayUi)
                {
                    Debug(
                        $"[FieldEvent] About to invoke handler for {effect.Name}, handlerEventId={handlerEventId}, handler.HandlerInfo.Id={handler.HandlerInfo.Id}");
                }

                // Save parent context
                IEffect parentEffect = Effect;
                EffectState parentEffectState = EffectState;
                Event parentEvent = Event;

                // Set up new event context
                Effect = effect;
                EffectState = handler.State ?? InitEffectState();
                Event = new Event
                {
                    Id = handlerEventId,
                    Target = singleEventTarget,
                    Source = null,
                    Effect = null,
                };
                EventDepth++;

                // Invoke the handler directly
                RelayVar? returnVal;
                try
                {
                    returnVal = InvokeEventHandlerInfo(handler.HandlerInfo, false,
                        BoolRelayVar.True, singleEventTarget, null, null);

                    if (DisplayUi)
                    {
                        Debug(
                            $"[FieldEvent] Handler invoked for {effect.Name}, returnVal={(returnVal != null ? returnVal.GetType().Name : "null")}");
                    }
                }
                finally
                {
                    // Restore parent context
                    EventDepth--;
                    Effect = parentEffect;
                    EffectState = parentEffectState;
                    Event = parentEvent;
                }
            }
            else if (DisplayUi)
            {
                Debug($"[FieldEvent] Handler.HandlerInfo is NULL for {effect.Name}");
            }
            // else Debug($"[FieldEvent] Handler.HandlerInfo is NULL for {effect.Name}");

            // Process any faint messages and check if battle has ended
            FaintMessages();
            if (Ended) return;
        }
        }
        finally
        {
            handlers.Clear();
            _handlerListPool.Push(handlers);
        }

        // Debug($"FieldEvent {eventId}: Completed processing all handlers");
    }

    public RelayVar? PriorityEvent(EventId eventId, PokemonSideBattleUnion target,
        Pokemon? source = null,
        IEffect? effect = null, RelayVar? relayVar = null, bool onEffect = false)
    {
        return RunEvent(eventId, RunEventTarget.FromPokemonSideBattleUnion(target),
            RunEventSource.FromNullablePokemon(source), effect, relayVar, onEffect, true);
    }

    public void OnEvent(EventId eventId, Format target, object[] rest)
    {
        throw new NotImplementedException(
            "This method is for attaching custom event handlers to a Battle." +
            "It shouldn't be used in this implementation.");
    }

    #region Helpers

    /// <summary>
    /// Invokes an event handler using EventHandlerInfo.
    /// This provides type-safe invocation with compile-time validation and eliminates
    /// the need for complex pattern matching on EffectDelegate union types.
    /// Uses a pooled EventContext to avoid per-invocation heap allocations.
    /// </summary>
    private RelayVar? InvokeEventHandlerInfo(EventHandlerInfo? handlerInfo, bool hasRelayVar,
        RelayVar relayVar,
        SingleEventTarget? target, SingleEventSource? source, IEffect? sourceEffect)
    {
        if (handlerInfo == null) return relayVar;

        // Check for constant values in union types (fast path)
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (handlerInfo is IUnionEventHandler unionHandler && unionHandler.IsConstant())
        {
            object? constantValue = unionHandler.GetConstantValue();
            return ConvertConstantToRelayVar(constantValue, handlerInfo.Id);
        }

        // Use context-based handler if available
        if (handlerInfo.UsesContextHandler)
        {
            // Rent a pooled EventContext and populate it directly
            // (eliminates the EventInvocationContext intermediary allocation)
            EventContext eventContext = RentEventContext();
            eventContext.Battle = this;
            eventContext.EventId = handlerInfo.Id;
            eventContext.Effect = Effect;
            eventContext.TargetPokemon = target is { Kind: SingleEventTargetKind.Pokemon } tp ? tp.Pokemon : null;
            eventContext.TargetSide = target switch
            {
                { Kind: SingleEventTargetKind.Side } s => s.Side,
                { Kind: SingleEventTargetKind.Pokemon } p => p.Pokemon.Side,
                _ => null
            };
            eventContext.TargetField = target is { Kind: SingleEventTargetKind.Field } tf ? tf.Field : null;
            eventContext.SourcePokemon = source switch
            {
                { IsPokemon: true } s => s.Pokemon,
                _ => null
            };
            eventContext.SourceType = source switch
            {
                { IsPokemonType: true } s => s.Type,
                _ => null
            };
            eventContext.SourceEffect = sourceEffect;
            eventContext.Move = sourceEffect as Moves.ActiveMove;
            eventContext.RelayVar = relayVar;

            // Capture effect name before execution since nested events may change Effect
            string effectName = Effect?.Name ?? "unknown";
            EffectType? effectType = Effect?.EffectType;

            try
            {
                return handlerInfo.ContextHandler!(eventContext);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Event {handlerInfo.Id} context handler failed on effect {effectName} ({effectType})",
                    ex);
            }
            finally
            {
                ReturnEventContext(eventContext);
            }
        }

        // No handler available
        return relayVar;
    }

    /// <summary>
    /// Converts a constant value from a union handler to a RelayVar.
    /// </summary>
    private static RelayVar? ConvertConstantToRelayVar(object? constantValue, EventId eventId)
    {
        return constantValue switch
        {
            bool boolValue => boolValue ? BoolRelayVar.True : BoolRelayVar.False,
            int intValue => IntRelayVar.Get(intValue),
            decimal decimalValue => new DecimalRelayVar(decimalValue),
            string stringValue => new StringRelayVar(stringValue),
            MoveId moveId => new MoveIdRelayVar(moveId),
            null => null,
            _ => throw new InvalidOperationException(
                $"Event {eventId}: Unsupported constant value type: {constantValue.GetType().Name}"),
        };
    }

    /// <summary>
    /// Invokes an EventHandlerInfo callback and extracts the return value.
    /// This is a helper for invoking callback properties like DurationCallback, BasePowerCallback, etc.
    /// Supports both context-based handlers (Create pattern) and legacy delegate handlers.
    /// </summary>
    /// <typeparam name="TResult">The expected return type</typeparam>
    /// <param name="handlerInfo">The handler info to invoke</param>
    /// <param name="args">Arguments to pass to the handler.
    /// Convention: args[0]=Battle, then Pokemon/ActiveMove/IEffect by position.</param>
    /// <returns>The result of the invocation, or default(TResult) if handler is null</returns>
    public TResult? InvokeCallback<TResult>(EventHandlerInfo? handlerInfo, params object?[] args)
    {
        if (handlerInfo == null) return default;

        // Use context-based handler (set by Create factory)
        if (handlerInfo.ContextHandler is not null)
        {
            var context = RentCallbackContext(handlerInfo.Id, args);
            try
            {
                RelayVar? relayResult = handlerInfo.ContextHandler(context);
                return ConvertRelayVarToCallbackResult<TResult>(relayResult);
            }
            finally
            {
                ReturnEventContext(context);
            }
        }

        return default;
    }

    /// <summary>
    /// Rents a pooled EventContext and populates it from positional callback arguments.
    /// Maps args by type: Battle, then first Pokemon → Target, second Pokemon → Source,
    /// ActiveMove → Move, IEffect → SourceEffect.
    /// </summary>
    private EventContext RentCallbackContext(EventId eventId, object?[] args)
    {
        Battle? battle = null;
        Pokemon? targetPokemon = null;
        Pokemon? sourcePokemon = null;
        ActiveMove? move = null;
        IEffect? sourceEffect = null;

        foreach (object? arg in args)
        {
            switch (arg)
            {
                case Battle b:
                    battle = b;
                    break;
                case Pokemon p when targetPokemon is null:
                    targetPokemon = p;
                    break;
                case Pokemon p:
                    sourcePokemon = p;
                    break;
                case ActiveMove m:
                    move = m;
                    break;
                case IEffect e:
                    sourceEffect = e;
                    break;
            }
        }

        EventContext context = RentEventContext();
        context.Battle = battle ?? throw new InvalidOperationException(
            $"Callback {eventId}: No Battle argument found in args");
        context.EventId = eventId;
        context.TargetPokemon = targetPokemon;
        context.SourcePokemon = sourcePokemon;
        context.Move = move;
        context.SourceEffect = sourceEffect;
        return context;
    }

    /// <summary>
    /// Converts a RelayVar? from a context handler back to the expected callback result type.
    /// </summary>
    private static TResult? ConvertRelayVarToCallbackResult<TResult>(RelayVar? relayVar)
    {
        if (relayVar is null) return default;

        // Direct type match
        if (relayVar is TResult directMatch) return directMatch;

        // Unwrap common RelayVar types to primitives
        object? unwrapped = relayVar switch
        {
            IntRelayVar irv => irv.Value,
            BoolRelayVar brv => brv.Value,
            DecimalRelayVar drv => drv.Value,
            _ => null
        };

        if (unwrapped is TResult typedResult) return typedResult;

        // For union return types, reconstruct from RelayVar
        if (typeof(TResult) == typeof(IntFalseUnion))
        {
            object? union = relayVar switch
            {
                IntRelayVar i => (object)IntFalseUnion.FromInt(i.Value),
                BoolRelayVar { Value: false } => IntFalseUnion.FromFalse(),
                _ => null
            };
            if (union is TResult u) return u;
        }

        return default;
    }

    #endregion
}