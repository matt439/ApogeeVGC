#pragma once

#include "EffectData.h"

struct ModdedEffectData : public EffectData
{
	ModdedEffectData(
		std::unique_ptr<std::string> name = nullptr,
		std::unique_ptr<std::string> desc = nullptr,
		std::unique_ptr<int> duration = nullptr,
		std::unique_ptr<std::function<int(Battle*, Pokemon*, Pokemon*, Effect*)>> duration_callback = nullptr,
		std::unique_ptr<EffectType> effect_type = nullptr,
		std::unique_ptr<bool> infiltrates = nullptr,
		std::unique_ptr<NonStandard> is_nonstandard = nullptr,
		std::unique_ptr<std::string> short_desc = nullptr);
};