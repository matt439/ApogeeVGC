#pragma once

#include "../dex/IDexData.h"
#include "PokemonConditionData.h"
#include "SideConditionData.h"
#include "FieldConditionData.h"
#include <variant>

//using ConditionData = std::variant<PokemonConditionData, SideConditionData, FieldConditionData>;

struct ConditionData : public IDexData 
{
    std::variant<PokemonConditionData, SideConditionData, FieldConditionData> data;

    ConditionData(const PokemonConditionData& v);
    ConditionData(const SideConditionData& v);
    ConditionData(const FieldConditionData& v);

    DataType get_data_type() const override;
};
