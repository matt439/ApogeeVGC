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
	Condition(const std::string& name,
		const std::string& real_move = "",
		const std::string& full_name = "",
		bool exists = true,
		int num = 0,
		int gen = 0,
		const std::string& short_desc = "",
		const std::string& desc = "",
		NonStandard is_nonstandard = NonStandard::NONE,
		bool no_copy = false,
		bool affects_fainted = false,
		const std::string& source_effect = "");
};