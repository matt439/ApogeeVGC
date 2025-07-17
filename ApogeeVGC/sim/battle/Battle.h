#pragma once

#include "../global-types/Effect.h"
#include "../pokemon/EffectState.h"
#include "Target.h"
#include "Source.h"
#include <any>

class Battle
{
public:
	Battle() = default;
	Battle(const Battle&) = default;

	void* single_event(
		const std::string& event_id,
		const Effect& effect,
		Target& target,
		EffectState* state = nullptr, // optional, nullable
		Source* source = nullptr, // optional
		std::variant<Effect*, std::string, std::monostate>* source_effect = nullptr, // optional
		std::any* relay_var = nullptr, // optional
		std::any* custom_callback = nullptr); // optional
private:
	unsigned int _id = 0;
	unsigned int _seed = 0;
	//Field _field;
	//Format _format;
	//Side _side_a;
	//Side _side_b;

	int _event_depth = 0;

	// std::unordered_map<std::string, std::string> _event = { "id", "" };

	//add
	//suppressing_ability
	//debug
	//init_effect_state
	//args.unshift
	//callback.apply

	//target.ignoringItem
	//target.ignoringAbility
	//field.suppressingWeather
};
