#pragma once

#include <optional>

struct AbilityFlags
{
    // Can be suppressed by Mold Breaker and related effects
    std::optional<bool> breakable = std::nullopt;

    // Ability can't be suppressed by e.g. Gastro Acid or Neutralizing Gas
    std::optional<bool> cantsuppress = std::nullopt;

    // Role Play fails if target has this Ability
    std::optional<bool> failroleplay = std::nullopt;

    // Skill Swap fails if either the user or target has this Ability
    std::optional<bool> failskillswap = std::nullopt;

    // Entrainment fails if user has this Ability
    std::optional<bool> noentrain = std::nullopt;

    // Receiver and Power of Alchemy will not activate if an ally faints with this Ability
    std::optional<bool> noreceiver = std::nullopt;

    // Trace cannot copy this Ability
    std::optional<bool> notrace = std::nullopt;

    // Disables the Ability if the user is Transformed
    std::optional<bool> notransform = std::nullopt;
};