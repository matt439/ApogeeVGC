#pragma once

#include "../dex/IDexData.h"
#include "../global-types/StatIDExceptHP.h"
// #include "BasicEffectData.h"
#include <string>
#include <memory>

struct NatureData : public IDexData
{
    std::string name = "";
	std::unique_ptr<StatIDExceptHP> plus = nullptr; // optional
    std::unique_ptr<StatIDExceptHP> minus = nullptr; // optional

    NatureData() = default;
    NatureData(const std::string& name,
        std::unique_ptr<StatIDExceptHP> plus = nullptr,
        std::unique_ptr<StatIDExceptHP> minus = nullptr);
    NatureData(const NatureData& other);

	DataType get_data_type() const override;

    //NatureData(const BasicEffectData& basic_effect_data,
    //    const StatIDExceptHP* plus = nullptr,
    //    const StatIDExceptHP* minus = nullptr);
};