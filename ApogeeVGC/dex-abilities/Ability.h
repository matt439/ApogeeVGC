#pragma once

#include "../dex-data/BasicEffect.h"
#include "../dex-conditions/ConditionData.h"
#include "AbilityData.h"

struct Ability : public BasicEffect
{
    static constexpr std::string_view effect_type = "Ability";
    int rating = 0;
    bool suppress_weather = false;
	AbilityFlags flags = AbilityFlags{};
	std::optional<ConditionData> condition = std::nullopt;

	Ability() = default;
	Ability(const Ability&) = default;
    Ability(const AbilityData& data);
};