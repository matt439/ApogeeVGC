#pragma once

#include "../global-types/ba"

class Condition : public BasicEffect
{
	// Effect type (Condition, Weather, Status, Terrain)
	EffectType effect_type = EffectType::CONDITION;

	std::optional<int> counter_max;
	std::optional<int> effect_order;

	std::optional<std::function<int(Battle*, Pokemon*, Pokemon*, std::optional<Effect*>)>> duration_callback;
	std::optional<std::function<void(Battle*, Pokemon*)>> on_copy;
	std::optional<std::function<void(Battle*, Pokemon*)>> on_end;
	std::optional<std::function<std::optional<bool>(Battle*, Pokemon*, Pokemon*, Effect*)>> on_restart;
	std::optional<std::function<std::optional<bool>(Battle*, Pokemon*, Pokemon*, Effect*)>> on_start;

	Condition() = default;
	// TODO: Implement a constructor that initializes the fields based on input data
};