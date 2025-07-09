#pragma once

#include "../global-types/StatIDExceptHP.h"
#include "BasicEffectData.h"
#include <string>
#include <optional>

struct NatureData
{
    std::string name = ""; // also in BasicEffect
	const StatIDExceptHP* plus = nullptr; // optional
    const StatIDExceptHP* minus = nullptr; // optional

    NatureData() = default;
    NatureData(const NatureData&) = default;
    NatureData(const std::string& name, const StatIDExceptHP* plus = nullptr,
        const StatIDExceptHP* minus = nullptr);

    NatureData(const BasicEffectData& basic_effect_data,
        const StatIDExceptHP* plus = nullptr,
        const StatIDExceptHP* minus = nullptr);
};