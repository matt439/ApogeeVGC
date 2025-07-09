#pragma once

#include <optional>

struct AbilityFlags
{
    // Can be suppressed by Mold Breaker and related effects
    bool* breakable = nullptr; // optional

    // Ability can't be suppressed by e.g. Gastro Acid or Neutralizing Gas
    bool* cantsuppress = nullptr; // optional

    // Role Play fails if target has this Ability
    bool* failroleplay = nullptr; // optional

    // Skill Swap fails if either the user or target has this Ability
    bool* failskillswap = nullptr; // optional

    // Entrainment fails if user has this Ability
    bool* noentrain = nullptr; // optional

    // Receiver and Power of Alchemy will not activate if an ally faints with this Ability
    bool* noreceiver = nullptr; // optional

    // Trace cannot copy this Ability
    bool* notrace = nullptr; // optional

    // Disables the Ability if the user is Transformed
    bool* notransform = nullptr; // optional
};