#include "Battle.h"

using namespace rapidjson;

Battle::Battle()
{
	//this->_event.insert({ "id", "" });
}

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


void* Battle::single_event(const std::string& eventid,
	const Effect& effect,
	EffectState* state, // can be null
	Target& target, // can be null
	Source& source, // can be null
	Effect* source_effect, // can be null
	void* relay_var, // was type 'any'
	void* custom_callback) // was type 'unknown'
{
	
	Value json;


	//if (this->_event_depth >= 8) {
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

	return nullptr; // Placeholder for actual event handling logic
}