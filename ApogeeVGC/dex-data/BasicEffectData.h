#pragma once

#include "../global-types/NonStandard.h"
#include "../global-types/EffectType.h"
#include <optional>
#include <string>

// Custom struct to hold the data for BasicEffect
struct BasicEffectData
{
    std::string name = "";
    std::optional<std::string> real_move = std::nullopt;
    std::optional<std::string> fullname = std::nullopt;
    std::optional<EffectType> effect_type = std::nullopt;
    std::optional<bool> exists = std::nullopt;
    std::optional<int> num = std::nullopt;
    std::optional<int> gen = std::nullopt;
    std::optional<std::string> short_desc = std::nullopt;
    std::optional<std::string> desc = std::nullopt;
    std::optional<NonStandard> is_nonstandard = std::nullopt;
    std::optional<int> duration = std::nullopt;
    std::optional<bool> no_copy = std::nullopt;
    std::optional<bool> affects_fainted = std::nullopt;
    std::optional<std::string> status = std::nullopt;
    std::optional<std::string> weather = std::nullopt;
    std::optional<std::string> source_effect = std::nullopt;

    BasicEffectData() = default;
    BasicEffectData(const BasicEffectData&) = default;
    BasicEffectData(const std::string& name, bool exists);
};