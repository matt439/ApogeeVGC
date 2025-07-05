#pragma once

#include "Field.h"
#include "Format.h"
#include "Side.h"
#include "Move.h"
// #include "rapidjson/document.h"
#include <variant>
#include <string>
#include <optional>

using Part = std::variant<
	std::monostate,           // for null/undefined
	std::string,
	int,                   // for number
	bool,
	Pokemon*,
	Side*,
	Effect*,
	Move*>;

class Battle; // Forward declaration

// For target
using Target = std::variant<
	std::monostate,           // for null
	std::string,
	Pokemon*,
	Side*,
	Field*,
	Battle*>;

// For source
using Source = std::variant<
	std::monostate,           // for null
	std::string,
	Pokemon*,
	Effect*,
	bool>;                     // only 'false' is valid, document this

class Battle
{
public:
	Battle();
	void* single_event(
		const std::string& event_id,
		const Effect& effect,
		std::optional<EffectState*> state, // nullable
		Target& target,
		std::optional<Source> source = std::nullopt,
		std::optional<std::variant<Effect*, std::string, std::monostate>> source_effect = std::nullopt,
		std::optional<std::any> relay_var = std::nullopt,
		std::optional<std::any> custom_callback = std::nullopt);
private:
	unsigned int _id = 0;
	unsigned int _seed = 0;
	Field _field;
	Format _format;
	Side _side_a;
	Side _side_b;

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
