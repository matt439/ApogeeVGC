#pragma once

#include <memory>

struct AbilityFlags
{
    // Can be suppressed by Mold Breaker and related effects
    std::unique_ptr<bool> breakable = nullptr; // optional

    // Ability can't be suppressed by e.g. Gastro Acid or Neutralizing Gas
    std::unique_ptr<bool> cantsuppress = nullptr; // optional

    // Role Play fails if target has this Ability
    std::unique_ptr<bool> failroleplay = nullptr; // optional

    // Skill Swap fails if either the user or target has this Ability
    std::unique_ptr<bool> failskillswap = nullptr; // optional

    // Entrainment fails if user has this Ability
    std::unique_ptr<bool> noentrain = nullptr; // optional

    // Receiver and Power of Alchemy will not activate if an ally faints with this Ability
    std::unique_ptr<bool> noreceiver = nullptr; // optional

    // Trace cannot copy this Ability
    std::unique_ptr<bool> notrace = nullptr; // optional

    // Disables the Ability if the user is Transformed
    std::unique_ptr<bool> notransform = nullptr; // optional

	AbilityFlags() = default;
};