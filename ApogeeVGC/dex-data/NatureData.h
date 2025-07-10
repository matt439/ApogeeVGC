#pragma once

#include "../global-types/StatIDExceptHP.h"
// #include "BasicEffectData.h"
#include <string>
#include <memory>

struct NatureData
{
    std::string name = "";
	std::unique_ptr<StatIDExceptHP> plus = nullptr; // optional
    std::unique_ptr<StatIDExceptHP> minus = nullptr; // optional

    NatureData() = default;
    NatureData(const std::string& name,
        std::unique_ptr<StatIDExceptHP> plus = nullptr,
        std::unique_ptr<StatIDExceptHP> minus = nullptr);

    //NatureData(const BasicEffectData& basic_effect_data,
    //    const StatIDExceptHP* plus = nullptr,
    //    const StatIDExceptHP* minus = nullptr);
};