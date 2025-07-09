#pragma once

#include "function_type_aliases.h"
// #include "../global-types/type_aliases.h"
#include <optional>
#include <string>

struct HitEffect
{
    // Optional function, type depends on MoveEventMethods::on_hit signature
    OnHitFunc* on_hit = nullptr; // optional

    // set pokemon conditions
    SparseBoostsTable* boosts = nullptr; // optional
    std::string* status = nullptr; // optional
    std::string* volatile_status = nullptr; // optional

    // set side/slot conditions
    std::string* side_condition = nullptr; // optional
    std::string* slot_condition = nullptr; // optional

    // set field conditions
    std::string* pseudo_weather = nullptr; // optional
    std::string* terrain = nullptr; // optional
    std::string* weather = nullptr; // optional

    HitEffect() = default;
    HitEffect(const HitEffect&) = default;
};