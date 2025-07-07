#pragma once

#include "dex-conditions.h"
#include "dex-data.h"
#include <optional>

struct AbilityFlags
{
    std::optional<bool> breakable;      // Can be suppressed by Mold Breaker and related effects
    std::optional<bool> cantsuppress;   // Ability can't be suppressed by e.g. Gastro Acid or Neutralizing Gas
    std::optional<bool> failroleplay;   // Role Play fails if target has this Ability
    std::optional<bool> failskillswap;  // Skill Swap fails if either the user or target has this Ability
    std::optional<bool> noentrain;      // Entrainment fails if user has this Ability
    std::optional<bool> noreceiver;     // Receiver and Power of Alchemy will not activate if an ally faints with this Ability
    std::optional<bool> notrace;        // Trace cannot copy this Ability
    std::optional<bool> notransform;    // Disables the Ability if the user is Transformed
};

struct AbilityData : public BasicEffectData
{
    std::optional<bool> suppress_weather;
    std::optional<AbilityFlags> flags;
    std::optional<int> rating;
    std::optional<ConditionData> condition;
};

struct Ability : public BasicEffect
{
    static constexpr const char* effect_type = "Ability";
    int rating = 0;
    bool suppress_weather = false;
    AbilityFlags flags;
    std::optional<ConditionData> condition;

    Ability(const AbilityData& data);
};
