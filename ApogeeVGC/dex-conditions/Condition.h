#pragma once

#include "../dex-data/BasicEffect.h"

class Condition : public BasicEffect
{
public:
	// Effect type (Condition, Weather, Status, Terrain)
	EffectType effect_type = EffectType::CONDITION;

	int* counter_max = nullptr; // optional
	int* effect_order = nullptr; // optional

	std::function<int(Battle*, Pokemon*, Pokemon*, Effect*)>* duration_callback = nullptr; // optional
	std::function<void(Battle*, Pokemon*)>* on_copy = nullptr; // optional
	std::function<void(Battle*, Pokemon*)>* on_end = nullptr; // optional
	std::function<bool(Battle*, Pokemon*, Pokemon*, Effect*)>* on_restart = nullptr; // optional
	std::function<bool(Battle*, Pokemon*, Pokemon*, Effect*)>* on_start = nullptr; // optional

	Condition() = default;
	// TODO: Implement a constructor that initializes the fields based on input data
};