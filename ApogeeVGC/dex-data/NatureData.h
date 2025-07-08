#pragma once

#include "../global-types/StatIDExceptHP.h"
#include "BasicEffectData.h"
#include <string>
#include <optional>

struct NatureData
{
    std::string name = ""; // also in BasicEffect
	std::optional<StatIDExceptHP> plus = std::nullopt;
    std::optional<StatIDExceptHP> minus = std::nullopt;

    NatureData() = default;
    NatureData(const NatureData&) = default;
    NatureData(const std::string& name, const std::optional<StatIDExceptHP>& plus = std::nullopt,
        const std::optional<StatIDExceptHP>& minus = std::nullopt);

    NatureData(const BasicEffectData& basic_effect_data,
        const std::optional<StatIDExceptHP>& plus = std::nullopt,
        const std::optional<StatIDExceptHP>& minus = std::nullopt);
};