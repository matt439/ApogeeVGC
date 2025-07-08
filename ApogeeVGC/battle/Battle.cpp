#include "Battle.h"

// using namespace rapidjson;

//add(...parts: (Part | (() = > { side: SideID, secret : string, shared : string }))[]) {
//	if (!parts.some(part = > typeof part == = 'function')) {
//		this.log.push(`|${ parts.join('|') }`);
//		return;
//	}
//
//	let side : SideID | null = null;
//	const secret = [];
//	const shared = [];
//	for (const part of parts) {
//		if (typeof part == = 'function') {
//			const split = part();
//			if (side&& side != = split.side) throw new Error("Multiple sides passed to add");
//			side = split.side;
//			secret.push(split.secret);
//			shared.push(split.shared);
//		}
//		else {
//			secret.push(part);
//			shared.push(part);
//		}
//	}
//	this.addSplit(side!, secret, shared);
//}


void* Battle::single_event(
	const std::string& event_id,
	const Effect& effect,
	std::optional<EffectState*> state, // nullable
	Target& target,
	std::optional<Source> source,
	std::optional<std::variant<Effect*, std::string, std::monostate>> source_effect,
	std::optional<std::any> relay_var,
	std::optional<std::any> custom_callback)
{
	//if (this.eventDepth >= 8) {
	//	// oh fuck
	//	this.add('message', 'STACK LIMIT EXCEEDED');
	//	this.add('message', 'PLEASE REPORT IN BUG THREAD');
	//	this.add('message', 'Event: ' + eventid);
	//	this.add('message', 'Parent event: ' + this.event.id);
	//	throw new Error("Stack overflow");
	//}
	//if (this.log.length - this.sentLogPos > 1000) {
	//	this.add('message', 'LINE LIMIT EXCEEDED');
	//	this.add('message', 'PLEASE REPORT IN BUG THREAD');
	//	this.add('message', 'Event: ' + eventid);
	//	this.add('message', 'Parent event: ' + this.event.id);
	//	throw new Error("Infinite loop");
	//}
	//// this.add('Event: ' + eventid + ' (depth ' + this.eventDepth + ')');
	//let hasRelayVar = true;
	//if (relayVar == = undefined) {
	//	relayVar = true;
	//	hasRelayVar = false;
	//}

	//if (effect.effectType == = 'Status' && (target instanceof Pokemon) && target.status != = effect.id) {
	//	// it's changed; call it off
	//	return relayVar;
	//}
	//if (eventid == = 'SwitchIn' && effect.effectType == = 'Ability' && effect.flags['breakable'] &&
	//	this.suppressingAbility(target as Pokemon)) {
	//	this.debug(eventid + ' handler suppressed by Mold Breaker');
	//	return relayVar;
	//}
	//if (eventid != = 'Start' && eventid != = 'TakeItem' && effect.effectType == = 'Item' &&
	//	(target instanceof Pokemon) && target.ignoringItem()) {
	//	this.debug(eventid + ' handler suppressed by Embargo, Klutz or Magic Room');
	//	return relayVar;
	//}
	//if (eventid != = 'End' && effect.effectType == = 'Ability' && (target instanceof Pokemon) && target.ignoringAbility()) {
	//	this.debug(eventid + ' handler suppressed by Gastro Acid or Neutralizing Gas');
	//	return relayVar;
	//}
	//if (
	//	effect.effectType == = 'Weather' && eventid != = 'FieldStart' && eventid != = 'FieldResidual' &&
	//	eventid != = 'FieldEnd' && this.field.suppressingWeather()
	//	) {
	//	this.debug(eventid + ' handler suppressed by Air Lock');
	//	return relayVar;
	//}

	//const callback = customCallback || (effect as any)[`on${eventid}`];
	//if (callback == = undefined) return relayVar;

	//const parentEffect = this.effect;
	//const parentEffectState = this.effectState;
	//const parentEvent = this.event;

	//this.effect = effect;
	//this.effectState = state as EffectState || this.initEffectState({});
	//this.event = { id: eventid, target, source, effect : sourceEffect };
	//this.eventDepth++;

	//const args = [target, source, sourceEffect];
	//if (hasRelayVar) args.unshift(relayVar);

	//let returnVal;
	//if (typeof callback == = 'function') {
	//	returnVal = callback.apply(this, args);
	//}
	//else {
	//	returnVal = callback;
	//}

	//this.eventDepth--;
	//this.effect = parentEffect;
	//this.effectState = parentEffectState;
	//this.event = parentEvent;

	//return returnVal == = undefined ? relayVar : returnVal;

	return nullptr; // Placeholder return value, replace with actual logic
}