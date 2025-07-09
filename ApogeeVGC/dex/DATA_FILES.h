#pragma once

#include "DataType.h"
#include <unordered_map>
#include <string_view>

const std::unordered_map<DataType, std::string_view> DATA_FILES =
{
    {DataType::ABILITIES, "abilities"},
    {DataType::RULESETS, "rulesets"},
    {DataType::FORMATS_DATA, "formats-data"},
    {DataType::ITEMS, "items"},
    {DataType::LEARNSETS, "learnsets"},
    {DataType::MOVES, "moves"},
    {DataType::NATURES, "natures"},
    {DataType::POKEDEX, "pokedex"},
    {DataType::POKEMON_GO_DATA, "pokemongo"},
    {DataType::SCRIPTS, "scripts"},
    {DataType::CONDITIONS, "conditions"},
    {DataType::TYPE_CHART, "typechart"}
};
