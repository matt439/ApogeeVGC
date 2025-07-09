#pragma once

#include "../dex-data/BasicEffect.h"

class Condition : public BasicEffect
{
public:
	// Effect type (Condition, Weather, Status, Terrain)
	EffectType effect_type = EffectType::CONDITION;

	std::optional<int> counter_max = std::nullopt;
	std::optional<int> effect_order = std::nullopt;

	std::optional<std::function<int(Battle*, Pokemon*, Pokemon*, std::optional<Effect*>)>> duration_callback = std::nullopt;
	std::optional<std::function<void(Battle*, Pokemon*)>> on_copy = std::nullopt;
	std::optional<std::function<void(Battle*, Pokemon*)>> on_end = std::nullopt;
	std::optional<std::function<std::optional<bool>(Battle*, Pokemon*, Pokemon*, Effect*)>> on_restart = std::nullopt;
	std::optional<std::function<std::optional<bool>(Battle*, Pokemon*, Pokemon*, Effect*)>> on_start = std::nullopt;

	Condition() = default;
	// TODO: Implement a constructor that initializes the fields based on input data
};