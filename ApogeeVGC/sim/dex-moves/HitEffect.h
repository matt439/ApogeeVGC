#pragma once

#include "function_type_aliases.h"
#include "../global-types/SparseBoostsTable.h"
#include <string>

struct HitEffect
{
    // Optional function, type depends on MoveEventMethods::on_hit signature
    std::unique_ptr<OnHitFunc> on_hit = nullptr; // optional

    // set pokemon conditions
    std::unique_ptr<SparseBoostsTable> boosts = nullptr; // optional
    std::unique_ptr<std::string> status = nullptr; // optional
    std::unique_ptr<std::string> volatile_status = nullptr; // optional

    // set side/slot conditions
    std::unique_ptr<std::string> side_condition = nullptr; // optional
    std::unique_ptr<std::string> slot_condition = nullptr; // optional

    // set field conditions
    std::unique_ptr<std::string> pseudo_weather = nullptr; // optional
    std::unique_ptr<std::string> terrain = nullptr; // optional
    std::unique_ptr<std::string> weather = nullptr; // optional

    HitEffect(std::unique_ptr<OnHitFunc> on_hit = nullptr,
        std::unique_ptr<SparseBoostsTable> boosts = nullptr,
        std::unique_ptr<std::string> status = nullptr,
        std::unique_ptr<std::string> volatile_status = nullptr,
        std::unique_ptr<std::string> side_condition = nullptr,
        std::unique_ptr<std::string> slot_condition = nullptr,
        std::unique_ptr<std::string> pseudo_weather = nullptr,
        std::unique_ptr<std::string> terrain = nullptr,
        std::unique_ptr<std::string> weather = nullptr);

    HitEffect(const HitEffect& other);
};