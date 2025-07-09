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
	ConditionData* condition = nullptr; // optional

	Ability() = default;
	Ability(const Ability&) = default;
    Ability(const AbilityData& data);
};