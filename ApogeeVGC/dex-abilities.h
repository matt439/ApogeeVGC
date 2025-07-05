#pragma once

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

// conditiondata
