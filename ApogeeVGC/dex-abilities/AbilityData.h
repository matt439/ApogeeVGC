#pragma once

#include "../dex-data/BasicEffectData.h"
#include "AbilityFlags.h"

struct AbilityData : public BasicEffectData
{
    std::optional<bool> suppress_weather;
    std::optional<AbilityFlags> flags;
    std::optional<int> rating;
	// TODO: link to ConditionData
    std::optional<ConditionData> condition;
};