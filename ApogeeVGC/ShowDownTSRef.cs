///** The entire event system revolves around this function and runEvent. */
//singleEvent(
//	eventid: string, effect: Effect, state: EffectState | Record<string, never> | null,
//	target: string | Pokemon | Side | Field | Battle | null, source ?: string | Pokemon | Effect | false | null,
//	sourceEffect ?: Effect | string | null, relayVar ?: any, customCallback ?: unknown
//) {
//	if (this.eventDepth >= 8)
//	{
//		// oh fuck
//		this.add('message', 'STACK LIMIT EXCEEDED');
//		this.add('message', 'PLEASE REPORT IN BUG THREAD');
//		this.add('message', 'Event: ' + eventid);
//		this.add('message', 'Parent event: ' + this.event.id);
//		throw new Error("Stack overflow");
//	}
//	if (this.log.length - this.sentLogPos > 1000)
//	{
//		this.add('message', 'LINE LIMIT EXCEEDED');
//		this.add('message', 'PLEASE REPORT IN BUG THREAD');
//		this.add('message', 'Event: ' + eventid);
//		this.add('message', 'Parent event: ' + this.event.id);
//		throw new Error("Infinite loop");
//	}
//	// this.add('Event: ' + eventid + ' (depth ' + this.eventDepth + ')');
//	let hasRelayVar = true;
//	if (relayVar === undefined)
//	{
//		relayVar = true;
//		hasRelayVar = false;
//	}

//	if (effect.effectType === 'Status' && (target instanceof Pokemon) && target.status !== effect.id) {
//		// it's changed; call it off
//		return relayVar;
//	}
//	if (eventid === 'SwitchIn' && effect.effectType === 'Ability' && effect.flags['breakable'] &&
//		this.suppressingAbility(target as Pokemon))
//	{
//		this.debug(eventid + ' handler suppressed by Mold Breaker');
//		return relayVar;
//	}
//	if (eventid !== 'Start' && eventid !== 'TakeItem' && effect.effectType === 'Item' &&
//		(target instanceof Pokemon) && target.ignoringItem()) {
//		this.debug(eventid + ' handler suppressed by Embargo, Klutz or Magic Room');
//		return relayVar;
//	}
//	if (eventid !== 'End' && effect.effectType === 'Ability' && (target instanceof Pokemon) && target.ignoringAbility()) {
//		this.debug(eventid + ' handler suppressed by Gastro Acid or Neutralizing Gas');
//		return relayVar;
//	}
//	if (
//		effect.effectType === 'Weather' && eventid !== 'FieldStart' && eventid !== 'FieldResidual' &&
//		eventid !== 'FieldEnd' && this.field.suppressingWeather()
//	)
//	{
//		this.debug(eventid + ' handler suppressed by Air Lock');
//		return relayVar;
//	}

//	const callback = customCallback || (effect as any)[`on${ eventid}`];
//	if (callback === undefined) return relayVar;

//	const parentEffect = this.effect;
//	const parentEffectState = this.effectState;
//	const parentEvent = this.event;

//	this.effect = effect;
//	this.effectState = state as EffectState || this.initEffectState({ });
//	this.event = { id: eventid, target, source, effect: sourceEffect }
//	;
//	this.eventDepth++;

//	const args = [target, source, sourceEffect];
//	if (hasRelayVar) args.unshift(relayVar);

//	let returnVal;
//	if (typeof callback === 'function')
//	{
//		returnVal = callback.apply(this, args);
//	}
//	else
//	{
//		returnVal = callback;
//	}

//	this.eventDepth--;
//	this.effect = parentEffect;
//	this.effectState = parentEffectState;
//	this.event = parentEvent;

//	return returnVal === undefined ? relayVar : returnVal;
//}