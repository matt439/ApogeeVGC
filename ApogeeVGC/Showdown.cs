//interface EventListenerWithoutPriority
//{
//    effect: Effect;
//    target?: Pokemon;
//    index?: number;
//    callback?: Function;
//    state: EffectState | null;
//    end: Function | null;
//    endCallArgs?: any[];
//    effectHolder: Pokemon | Side | Field | Battle;
//}
//interface EventListener extends EventListenerWithoutPriority
//{
//    order: number | false;
//    priority: number;
//    subOrder: number;
//    effectOrder?: number;
//    speed?: number;
//}

///** The entire event system revolves around this function and runEvent. */
//singleEvent(
//    eventid: string, effect: Effect, state: EffectState | Record<string, never> | null,
//    target: string | Pokemon | Side | Field | Battle | null, source ?: string | Pokemon | Effect | false | null,
//    sourceEffect ?: Effect | string | null, relayVar ?: any, customCallback ?: unknown
//) {
//    if (this.eventDepth >= 8)
//    {
//        // oh fuck
//        this.add('message', 'STACK LIMIT EXCEEDED');
//        this.add('message', 'PLEASE REPORT IN BUG THREAD');
//        this.add('message', 'Event: ' + eventid);
//        this.add('message', 'Parent event: ' + this.event.id);
//        throw new Error("Stack overflow");
//    }
//    if (this.log.length - this.sentLogPos > 1000)
//    {
//        this.add('message', 'LINE LIMIT EXCEEDED');
//        this.add('message', 'PLEASE REPORT IN BUG THREAD');
//        this.add('message', 'Event: ' + eventid);
//        this.add('message', 'Parent event: ' + this.event.id);
//        throw new Error("Infinite loop");
//    }
//    // this.add('Event: ' + eventid + ' (depth ' + this.eventDepth + ')');
//    let hasRelayVar = true;
//    if (relayVar === undefined)
//    {
//        relayVar = true;
//        hasRelayVar = false;
//    }

//    if (effect.effectType === 'Status' && (target instanceof Pokemon) && target.status !== effect.id) {
//        // it's changed; call it off
//        return relayVar;
//    }
//    if (eventid === 'SwitchIn' && effect.effectType === 'Ability' && effect.flags['breakable'] &&
//        this.suppressingAbility(target as Pokemon))
//    {
//        this.debug(eventid + ' handler suppressed by Mold Breaker');
//        return relayVar;
//    }
//    if (eventid !== 'Start' && eventid !== 'TakeItem' && effect.effectType === 'Item' &&
//        (target instanceof Pokemon) && target.ignoringItem()) {
//        this.debug(eventid + ' handler suppressed by Embargo, Klutz or Magic Room');
//        return relayVar;
//    }
//    if (eventid !== 'End' && effect.effectType === 'Ability' && (target instanceof Pokemon) && target.ignoringAbility()) {
//        this.debug(eventid + ' handler suppressed by Gastro Acid or Neutralizing Gas');
//        return relayVar;
//    }
//    if (
//        effect.effectType === 'Weather' && eventid !== 'FieldStart' && eventid !== 'FieldResidual' &&
//        eventid !== 'FieldEnd' && this.field.suppressingWeather()
//    )
//    {
//        this.debug(eventid + ' handler suppressed by Air Lock');
//        return relayVar;
//    }

//    const callback = customCallback || (effect as any)[`on${ eventid}`];
//    if (callback === undefined) return relayVar;

//    const parentEffect = this.effect;
//    const parentEffectState = this.effectState;
//    const parentEvent = this.event;

//    this.effect = effect;
//    this.effectState = state as EffectState || this.initEffectState({ });
//    this.event = { id: eventid, target, source, effect: sourceEffect }
//    ;
//    this.eventDepth++;

//    const args = [target, source, sourceEffect];
//    if (hasRelayVar) args.unshift(relayVar);

//    let returnVal;
//    if (typeof callback === 'function')
//    {
//        returnVal = callback.apply(this, args);
//    }
//    else
//    {
//        returnVal = callback;
//    }

//    this.eventDepth--;
//    this.effect = parentEffect;
//    this.effectState = parentEffectState;
//    this.event = parentEvent;

//    return returnVal === undefined ? relayVar : returnVal;
//}

///**
// * runEvent is the core of Pokemon Showdown's event system.
// *
// * Basic usage
// * ===========
// *
// *   this.runEvent('Blah')
// * will trigger any onBlah global event handlers.
// *
// *   this.runEvent('Blah', target)
// * will additionally trigger any onBlah handlers on the target, onAllyBlah
// * handlers on any active pokemon on the target's team, and onFoeBlah
// * handlers on any active pokemon on the target's foe's team
// *
// *   this.runEvent('Blah', target, source)
// * will additionally trigger any onSourceBlah handlers on the source
// *
// *   this.runEvent('Blah', target, source, effect)
// * will additionally pass the effect onto all event handlers triggered
// *
// *   this.runEvent('Blah', target, source, effect, relayVar)
// * will additionally pass the relayVar as the first argument along all event
// * handlers
// *
// * You may leave any of these null. For instance, if you have a relayVar but
// * no source or effect:
// *   this.runEvent('Damage', target, null, null, 50)
// *
// * Event handlers
// * ==============
// *
// * Items, abilities, statuses, and other effects like SR, confusion, weather,
// * or Trick Room can have event handlers. Event handlers are functions that
// * can modify what happens during an event.
// *
// * event handlers are passed:
// *   function (target, source, effect)
// * although some of these can be blank.
// *
// * certain events have a relay variable, in which case they're passed:
// *   function (relayVar, target, source, effect)
// *
// * Relay variables are variables that give additional information about the
// * event. For instance, the damage event has a relayVar which is the amount
// * of damage dealt.
// *
// * If a relay variable isn't passed to runEvent, there will still be a secret
// * relayVar defaulting to `true`, but it won't get passed to any event
// * handlers.
// *
// * After an event handler is run, its return value helps determine what
// * happens next:
// * 1. If the return value isn't `undefined`, relayVar is set to the return
// *    value
// * 2. If relayVar is falsy, no more event handlers are run
// * 3. Otherwise, if there are more event handlers, the next one is run and
// *    we go back to step 1.
// * 4. Once all event handlers are run (or one of them results in a falsy
// *    relayVar), relayVar is returned by runEvent
// *
// * As a shortcut, an event handler that isn't a function will be interpreted
// * as a function that returns that value.
// *
// * You can have return values mean whatever you like, but in general, we
// * follow the convention that returning `false` or `null` means
// * stopping or interrupting the event.
// *
// * For instance, returning `false` from a TrySetStatus handler means that
// * the pokemon doesn't get statused.
// *
// * If a failed event usually results in a message like "But it failed!"
// * or "It had no effect!", returning `null` will suppress that message and
// * returning `false` will display it. Returning `null` is useful if your
// * event handler already gave its own custom failure message.
// *
// * Returning `undefined` means "don't change anything" or "keep going".
// * A function that does nothing but return `undefined` is the equivalent
// * of not having an event handler at all.
// *
// * Returning a value means that that value is the new `relayVar`. For
// * instance, if a Damage event handler returns 50, the damage event
// * will deal 50 damage instead of whatever it was going to deal before.
// *
// * Useful values
// * =============
// *
// * In addition to all the methods and attributes of Dex, Battle, and
// * Scripts, event handlers have some additional values they can access:
// *
// * this.effect:
// *   the Effect having the event handler
// * this.effectState:
// *   the data store associated with the above Effect. This is a plain Object
// *   and you can use it to store data for later event handlers.
// * this.effectState.target:
// *   the Pokemon, Side, or Battle that the event handler's effect was
// *   attached to.
// * this.event.id:
// *   the event ID
// * this.event.target, this.event.source, this.event.effect:
// *   the target, source, and effect of the event. These are the same
// *   variables that are passed as arguments to the event handler, but
// *   they're useful for functions called by the event handler.
// */
//runEvent(
//    eventid: string, target ?: Pokemon | Pokemon[] | Side | Battle | null, source ?: string | Pokemon | false | null,
//    sourceEffect ?: Effect | null, relayVar ?: any, onEffect ?: boolean, fastExit ?: boolean
//) {
//    // if (Battle.eventCounter) {
//    // 	if (!Battle.eventCounter[eventid]) Battle.eventCounter[eventid] = 0;
//    // 	Battle.eventCounter[eventid]++;
//    // }
//    if (this.eventDepth >= 8)
//    {
//        // oh fuck
//        this.add('message', 'STACK LIMIT EXCEEDED');
//        this.add('message', 'PLEASE REPORT IN BUG THREAD');
//        this.add('message', 'Event: ' + eventid);
//        this.add('message', 'Parent event: ' + this.event.id);
//        throw new Error("Stack overflow");
//    }
//    if (!target) target = this;
//    let effectSource = null;
//    if (source instanceof Pokemon) effectSource = source;
//    const handlers = this.findEventHandlers(target, eventid, effectSource);
//    if (onEffect)
//    {
//        if (!sourceEffect) throw new Error("onEffect passed without an effect");
//        const callback = (sourceEffect as any)[`on${ eventid}`];
//        if (callback !== undefined)
//        {
//            if (Array.isArray(target)) throw new Error("");
//            handlers.unshift(this.resolvePriority({
//            effect: sourceEffect, callback, state: this.initEffectState({ }), end: null, effectHolder: target,
//				}, `on${ eventid}`));
//        }
//    }

//    if (['Invulnerability', 'TryHit', 'DamagingHit', 'EntryHazard'].includes(eventid))
//    {
//        handlers.sort(Battle.compareLeftToRightOrder);
//    }
//    else if (fastExit)
//    {
//        handlers.sort(Battle.compareRedirectOrder);
//    }
//    else
//    {
//        this.speedSort(handlers);
//    }
//    let hasRelayVar = 1;
//    const args = [target, source, sourceEffect];
//    // console.log('Event: ' + eventid + ' (depth ' + this.eventDepth + ') t:' + target.id + ' s:' + (!source || source.id) + ' e:' + effect.id);
//    if (relayVar === undefined || relayVar === null)
//    {
//        relayVar = true;
//        hasRelayVar = 0;
//    }
//    else
//    {
//        args.unshift(relayVar);
//    }

//    const parentEvent = this.event;
//    this.event = { id: eventid, target, source, effect: sourceEffect, modifier: 1 }
//    ;
//    this.eventDepth++;

//    let targetRelayVars = [];
//    if (Array.isArray(target))
//    {
//        if (Array.isArray(relayVar))
//        {
//            targetRelayVars = relayVar;
//        }
//        else
//        {
//            for (let i = 0; i < target.length; i++) targetRelayVars[i] = true;
//        }
//    }
//    for (const handler of handlers) {
//        if (handler.index !== undefined)
//        {
//            // TODO: find a better way to do this
//            if (!targetRelayVars[handler.index] && !(targetRelayVars[handler.index] === 0 &&
//                eventid === 'DamagingHit')) continue;
//            if (handler.target)
//            {
//                args[hasRelayVar] = handler.target;
//                this.event.target = handler.target;
//            }
//            if (hasRelayVar) args[0] = targetRelayVars[handler.index];
//        }
//        const effect = handler.effect;
//        const effectHolder = handler.effectHolder;
//        // this.debug('match ' + eventid + ': ' + status.id + ' ' + status.effectType);
//        if (effect.effectType === 'Status' && (effectHolder as Pokemon).status !== effect.id)
//        {
//            // it's changed; call it off
//            continue;
//        }
//        if (effect.effectType === 'Ability' && effect.flags['breakable'] &&
//            this.suppressingAbility(effectHolder as Pokemon))
//        {
//            if (effect.flags['breakable'])
//            {
//                this.debug(eventid + ' handler suppressed by Mold Breaker');
//                continue;
//            }
//            if (!effect.num)
//            {
//                // ignore attacking events for custom abilities
//                const AttackingEvents = {
//                        BeforeMove: 1,
//						BasePower: 1,
//						Immunity: 1,
//						RedirectTarget: 1,
//						Heal: 1,
//						SetStatus: 1,
//						CriticalHit: 1,
//						ModifyAtk: 1, ModifyDef: 1, ModifySpA: 1, ModifySpD: 1, ModifySpe: 1, ModifyAccuracy: 1,
//						ModifyBoost: 1,
//						ModifyDamage: 1,
//						ModifySecondaries: 1,
//						ModifyWeight: 1,
//						TryAddVolatile: 1,
//						TryHit: 1,
//						TryHitSide: 1,
//						TryMove: 1,
//						Boost: 1,
//						DragOut: 1,
//						Effectiveness: 1,
//					}
//            ;
//            if (eventid in AttackingEvents) {
//                this.debug(eventid + ' handler suppressed by Mold Breaker');
//                continue;
//            } else if (eventid === 'Damage' && sourceEffect && sourceEffect.effectType === 'Move')
//            {
//                this.debug(eventid + ' handler suppressed by Mold Breaker');
//                continue;
//            }
//        }
//    }
//    if (eventid !== 'Start' && eventid !== 'SwitchIn' && eventid !== 'TakeItem' &&
//        effect.effectType === 'Item' && (effectHolder instanceof Pokemon) && effectHolder.ignoringItem()) {
//        if (eventid !== 'Update')
//        {
//            this.debug(eventid + ' handler suppressed by Embargo, Klutz or Magic Room');
//        }
//        continue;
//    } else if (
//        eventid !== 'End' && effect.effectType === 'Ability' &&
//        (effectHolder instanceof Pokemon) && effectHolder.ignoringAbility()
//			) {
//        if (eventid !== 'Update')
//        {
//            this.debug(eventid + ' handler suppressed by Gastro Acid or Neutralizing Gas');
//        }
//        continue;
//    }
//    if (
//        (effect.effectType === 'Weather' || eventid === 'Weather') &&
//        eventid !== 'Residual' && eventid !== 'End' && this.field.suppressingWeather()
//    )
//    {
//        this.debug(eventid + ' handler suppressed by Air Lock');
//        continue;
//    }
//    let returnVal;
//    if (typeof handler.callback === 'function')
//    {
//        const parentEffect = this.effect;
//        const parentEffectState = this.effectState;
//        this.effect = handler.effect;
//        this.effectState = handler.state || this.initEffectState({ });
//        this.effectState.target = effectHolder;

//        returnVal = handler.callback.apply(this, args);

//        this.effect = parentEffect;
//        this.effectState = parentEffectState;
//    }
//    else
//    {
//        returnVal = handler.callback;
//    }

//    if (returnVal !== undefined)
//    {
//        relayVar = returnVal;
//        if (!relayVar || fastExit)
//        {
//            if (handler.index !== undefined)
//            {
//                targetRelayVars[handler.index] = relayVar;
//                if (targetRelayVars.every(val => !val)) break;
//            }
//            else
//            {
//                break;
//            }
//        }
//        if (hasRelayVar)
//        {
//            args[0] = relayVar;
//        }
//    }
//}

//this.eventDepth--;
//if (typeof relayVar === 'number' && relayVar === Math.abs(Math.floor(relayVar)))
//{
//    // this.debug(eventid + ' modifier: 0x' +
//    // 	('0000' + (this.event.modifier * 4096).toString(16)).slice(-4).toUpperCase());
//    relayVar = this.modify(relayVar, this.event.modifier);
//}
//this.event = parentEvent;

//		return Array.isArray(target) ? targetRelayVars : relayVar;
//}


//findEventHandlers(target: Pokemon | Pokemon[] | Side | Battle, eventName: string, source ?: Pokemon | null) {
//    let handlers: EventListener[] = [];
//    if (Array.isArray(target))
//    {
//        for (const [i, pokemon] of target.entries()) {
//            // console.log(`Event: ${eventName}, Target: ${pokemon}, ${i}`);
//            const curHandlers = this.findEventHandlers(pokemon, eventName, source);
//            for (const handler of curHandlers) {
//                handler.target = pokemon; // Original "effectHolder"
//                handler.index = i;
//            }
//            handlers = handlers.concat(curHandlers);
//        }
//        return handlers;
//    }
//    // events that target a Pokemon normally bubble up to the Side
//    const shouldBubbleDown = target instanceof Side;
//    // events usually run through EachEvent should never have any handlers besides `on${eventName}` so don't check for them
//    const prefixedHandlers = ! ['BeforeTurn', 'Update', 'Weather', 'WeatherChange', 'TerrainChange'].includes(eventName);
//    if (target instanceof Pokemon && (target.isActive || source?.isActive)) {
//        handlers = this.findPokemonEventHandlers(target, `on${ eventName}`);
//        if (prefixedHandlers)
//        {
//            for (const allyActive of target.alliesAndSelf()) {
//                handlers.push(...this.findPokemonEventHandlers(allyActive, `onAlly${ eventName}`));
//                handlers.push(...this.findPokemonEventHandlers(allyActive, `onAny${ eventName}`));
//            }
//            for (const foeActive of target.foes()) {
//                handlers.push(...this.findPokemonEventHandlers(foeActive, `onFoe${ eventName}`));
//                handlers.push(...this.findPokemonEventHandlers(foeActive, `onAny${ eventName}`));
//            }
//        }
//        target = target.side;
//    }
//    if (source && prefixedHandlers)
//    {
//        handlers.push(...this.findPokemonEventHandlers(source, `onSource${ eventName}`));
//    }
//    if (target instanceof Side) {
//        for (const side of this.sides) {
//            if (shouldBubbleDown)
//            {
//                for (const active of side.active) {
//                    if (side === target || side === target.allySide)
//                    {
//                        handlers = handlers.concat(this.findPokemonEventHandlers(active, `on${ eventName}`));
//                    }
//                    else if (prefixedHandlers)
//                    {
//                        handlers = handlers.concat(this.findPokemonEventHandlers(active, `onFoe${ eventName}`));
//                    }
//                    if (prefixedHandlers) handlers = handlers.concat(this.findPokemonEventHandlers(active, `onAny${ eventName}`));
//                }
//            }
//            if (side.n < 2 || !side.allySide)
//            {
//                if (side === target || side === target.allySide)
//                {
//                    handlers.push(...this.findSideEventHandlers(side, `on${ eventName}`));
//                }
//                else if (prefixedHandlers)
//                {
//                    handlers.push(...this.findSideEventHandlers(side, `onFoe${ eventName}`));
//                }
//                if (prefixedHandlers) handlers.push(...this.findSideEventHandlers(side, `onAny${ eventName}`));
//            }
//        }
//    }
//    handlers.push(...this.findFieldEventHandlers(this.field, `on${ eventName}`));
//    handlers.push(...this.findBattleEventHandlers(`on${ eventName}`));
//    return handlers;
//}


//findPokemonEventHandlers(pokemon: Pokemon, callbackName: string, getKey ?: 'duration') {
//    const handlers: EventListener[] = [];

//    const status = pokemon.getStatus();
//    let callback = this.getCallback(pokemon, status, callbackName);
//    if (callback !== undefined || (getKey && pokemon.statusState[getKey]))
//    {
//        handlers.push(this.resolvePriority({
//        effect: status, callback, state: pokemon.statusState, end: pokemon.clearStatus, effectHolder: pokemon,
//			}, callbackName));
//    }
//    for (const id in pokemon.volatiles) {
//        const volatileState = pokemon.volatiles[id];
//        const volatile = this.dex.conditions.getByID(id as ID);
//        callback = this.getCallback(pokemon, volatile, callbackName);
//        if (callback !== undefined || (getKey && volatileState[getKey]))
//        {
//            handlers.push(this.resolvePriority({
//            effect: volatile, callback, state: volatileState, end: pokemon.removeVolatile, effectHolder: pokemon,
//				}, callbackName));
//        }
//    }
//    const ability = pokemon.getAbility();
//    callback = this.getCallback(pokemon, ability, callbackName);
//    if (callback !== undefined || (getKey && pokemon.abilityState[getKey]))
//    {
//        handlers.push(this.resolvePriority({
//        effect: ability, callback, state: pokemon.abilityState, end: pokemon.clearAbility, effectHolder: pokemon,
//			}, callbackName));
//    }
//    const item = pokemon.getItem();
//    callback = this.getCallback(pokemon, item, callbackName);
//    if (callback !== undefined || (getKey && pokemon.itemState[getKey]))
//    {
//        handlers.push(this.resolvePriority({
//        effect: item, callback, state: pokemon.itemState, end: pokemon.clearItem, effectHolder: pokemon,
//			}, callbackName));
//    }
//    const species = pokemon.baseSpecies;
//    callback = this.getCallback(pokemon, species, callbackName);
//    if (callback !== undefined)
//    {
//        handlers.push(this.resolvePriority({
//        effect: species, callback, state: pokemon.speciesState, end() { }, effectHolder: pokemon,
//			}, callbackName));
//    }
//    const side = pokemon.side;
//    for (const conditionid in side.slotConditions[pokemon.position]) {
//        const slotConditionState = side.slotConditions[pokemon.position][conditionid];
//        const slotCondition = this.dex.conditions.getByID(conditionid as ID);
//        callback = this.getCallback(pokemon, slotCondition, callbackName);
//        if (callback !== undefined || (getKey && slotConditionState[getKey]))
//        {
//            handlers.push(this.resolvePriority({
//            effect: slotCondition,
//					callback,
//					state: slotConditionState,
//					end: side.removeSlotCondition,
//					endCallArgs: [side, pokemon, slotCondition.id],
//					effectHolder: pokemon,
//				}, callbackName));
//        }
//    }

//    return handlers;
//}

//findBattleEventHandlers(callbackName: string, getKey ?: 'duration', customHolder ?: Pokemon) {
//    const handlers: EventListener[] = [];

//    let callback;
//    const format = this.format;
//    callback = this.getCallback(this, format, callbackName);
//    if (callback !== undefined || (getKey && this.formatData[getKey]))
//    {
//        handlers.push(this.resolvePriority({
//        effect: format, callback, state: this.formatData, end: null, effectHolder: customHolder || this,
//			}, callbackName));
//    }
//    if (this.events && (callback = this.events[callbackName]) !== undefined)
//    {
//        for (const handler of callback) {
//            const state = (handler.target.effectType === 'Format') ? this.formatData : null;
//            handlers.push({
//            effect: handler.target, callback: handler.callback, state, end: null, effectHolder: customHolder || this,
//					priority: handler.priority, order: handler.order, subOrder: handler.subOrder,
//				});
//        }
//    }
//    return handlers;
//}

//findFieldEventHandlers(field: Field, callbackName: string, getKey ?: 'duration', customHolder ?: Pokemon) {
//    const handlers: EventListener[] = [];

//    let callback;
//    for (const id in field.pseudoWeather) {
//        const pseudoWeatherState = field.pseudoWeather[id];
//        const pseudoWeather = this.dex.conditions.getByID(id as ID);
//        callback = this.getCallback(field, pseudoWeather, callbackName);
//        if (callback !== undefined || (getKey && pseudoWeatherState[getKey]))
//        {
//            handlers.push(this.resolvePriority({
//            effect: pseudoWeather, callback, state: pseudoWeatherState,
//					end: customHolder ? null : field.removePseudoWeather, effectHolder: customHolder || field,
//				}, callbackName));
//        }
//    }
//    const weather = field.getWeather();
//    callback = this.getCallback(field, weather, callbackName);
//    if (callback !== undefined || (getKey && this.field.weatherState[getKey]))
//    {
//        handlers.push(this.resolvePriority({
//        effect: weather, callback, state: this.field.weatherState,
//				end: customHolder ? null : field.clearWeather, effectHolder: customHolder || field,
//			}, callbackName));
//    }
//    const terrain = field.getTerrain();
//    callback = this.getCallback(field, terrain, callbackName);
//    if (callback !== undefined || (getKey && field.terrainState[getKey]))
//    {
//        handlers.push(this.resolvePriority({
//        effect: terrain, callback, state: field.terrainState,
//				end: customHolder ? null : field.clearTerrain, effectHolder: customHolder || field,
//			}, callbackName));
//    }

//    return handlers;
//}

//findSideEventHandlers(side: Side, callbackName: string, getKey ?: 'duration', customHolder ?: Pokemon) {
//    const handlers: EventListener[] = [];

//    for (const id in side.sideConditions) {
//        const sideConditionData = side.sideConditions[id];
//        const sideCondition = this.dex.conditions.getByID(id as ID);
//        const callback = this.getCallback(side, sideCondition, callbackName);
//        if (callback !== undefined || (getKey && sideConditionData[getKey]))
//        {
//            handlers.push(this.resolvePriority({
//            effect: sideCondition, callback, state: sideConditionData,
//					end: customHolder ? null : side.removeSideCondition, effectHolder: customHolder || side,
//				}, callbackName));
//        }
//    }
//    return handlers;
//}

//interface CommonHandlers
//{
//    ModifierEffect: (this: Battle, relayVar: number, target: Pokemon, source: Pokemon, effect: Effect) => number | void;
//    ModifierMove: (this: Battle, relayVar: number, target: Pokemon, source: Pokemon, move: ActiveMove) => number | void;
//    ResultMove: boolean | (
//    (this: Battle, target: Pokemon, source: Pokemon, move: ActiveMove) => boolean | null | "" | void
//    );
//    ExtResultMove: boolean | (
//    (this: Battle, target: Pokemon, source: Pokemon, move: ActiveMove) => boolean | null | number | "" | void
//    );
//    VoidEffect: (this: Battle, target: Pokemon, source: Pokemon, effect: Effect) => void;
//    VoidMove: (this: Battle, target: Pokemon, source: Pokemon, move: ActiveMove) => void;
//    ModifierSourceEffect: (

//    this: Battle, relayVar: number, source: Pokemon, target: Pokemon, effect: Effect
//    ) => number | void;
//    ModifierSourceMove: (

//    this: Battle, relayVar: number, source: Pokemon, target: Pokemon, move: ActiveMove
//    ) => number | void;
//    ResultSourceMove: boolean | (
//    (this: Battle, source: Pokemon, target: Pokemon, move: ActiveMove) => boolean | null | "" | void
//    );
//    ExtResultSourceMove: boolean | (
//    (this: Battle, source: Pokemon, target: Pokemon, move: ActiveMove) => boolean | null | number | "" | void
//    );
//    VoidSourceEffect: (this: Battle, source: Pokemon, target: Pokemon, effect: Effect) => void;
//    VoidSourceMove: (this: Battle, source: Pokemon, target: Pokemon, move: ActiveMove) => void;
//}