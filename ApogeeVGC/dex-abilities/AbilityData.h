#pragma once

#include "../dex-data/BasicEffectData.h"
#include "AbilityFlags.h"

struct AbilityData : public BasicEffectData
{
    std::optional<bool> suppress_weather = std::nullopt;
	std::optional<AbilityFlags> flags = std::nullopt;
	std::optional<int> rating = std::nullopt;
	// TODO: link to ConditionData
	std::optional<ConditionData> condition = std::nullopt;
};