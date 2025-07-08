#pragma once

#include "../"

#include <optional>
#include <string>

struct HitEffect
{
    // Optional function, type depends on MoveEventMethods::on_hit signature
    std::optional<OnHitFunc> on_hit = std::nullopt;

    // set pokemon conditions
    std::optional<SparseBoostsTable> boosts = std::nullopt;
    std::optional<std::string> status = std::nullopt;
    std::optional<std::string> volatile_status = std::nullopt;

    // set side/slot conditions
    std::optional<std::string> side_condition = std::nullopt;
    std::optional<std::string> slot_condition = std::nullopt;

    // set field conditions
    std::optional<std::string> pseudo_weather = std::nullopt;
    std::optional<std::string> terrain = std::nullopt;
    std::optional<std::string> weather = std::nullopt;

    HitEffect() = default;
    HitEffect(const HitEffect&) = default;
};