#pragma once

#include "HitEffect.h"

class Ability;

struct SecondaryEffect : public HitEffect
{
    int* chance = nullptr; // optional

    // Used to flag a secondary effect as added by Poison Touch
    Ability* ability = nullptr; // optional

    /**
     * Gen 2 specific mechanics: Bypasses Substitute only on Twineedle,
     * and allows it to flinch sleeping/frozen targets
     */
    bool* kingsrock = nullptr; // optional
    HitEffect* self = nullptr; // optional

    SecondaryEffect() = default;
    SecondaryEffect(const SecondaryEffect&) = default;
};