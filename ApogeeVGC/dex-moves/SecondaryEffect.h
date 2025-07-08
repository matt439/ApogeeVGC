#pragma once

#include "HitEffect.h"

class Ability;

struct SecondaryEffect : public HitEffect
{
    std::optional<int> chance = std::nullopt;

    // Used to flag a secondary effect as added by Poison Touch
    std::optional<Ability*> ability = std::nullopt;

    /**
     * Gen 2 specific mechanics: Bypasses Substitute only on Twineedle,
     * and allows it to flinch sleeping/frozen targets
     */
    std::optional<bool> kingsrock = std::nullopt;
    std::optional<HitEffect> self = std::nullopt;

    SecondaryEffect() = default;
    SecondaryEffect(const SecondaryEffect&) = default;
};