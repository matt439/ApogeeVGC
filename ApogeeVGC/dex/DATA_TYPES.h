#pragma once

#include "DataType.h"
#include <array>

constexpr std::array<DataType, 12> DATA_TYPES =
{
    DataType::ABILITIES,
    DataType::RULESETS,
    DataType::FORMATS_DATA,
    DataType::ITEMS,
    DataType::LEARNSETS,
    DataType::MOVES,
    DataType::NATURES,
    DataType::POKEDEX,
    DataType::SCRIPTS,
    DataType::CONDITIONS,
    DataType::TYPE_CHART,
    DataType::POKEMON_GO_DATA
};