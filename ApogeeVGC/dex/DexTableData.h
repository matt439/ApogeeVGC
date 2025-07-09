#pragma once

#include "../global-types/type_aliases.h"
#include "../dex-conditions/ConditionData.h"
#include "DexTable.h"

// forward declarations
struct AbilityData;
struct FormatData;
struct ItemData;
struct LearnsetData;
struct MoveData;
struct NatureData;
struct SpeciesData;
struct SpeciesFormatsData;
struct PokemonGoData;

struct TypeData;

struct DexTableData
{
    DexTable<AbilityData> abilities;
    DexTable<FormatData> rulesets;
    DexTable<ItemData> items;
    DexTable<LearnsetData> learnsets;
    DexTable<MoveData> moves;
    DexTable<NatureData> natures;
    DexTable<SpeciesData> pokedex;
    DexTable<SpeciesFormatsData> formats_data;
    DexTable<PokemonGoData> pokemon_go_data;
    DexTable<AnyObject> scripts;
    DexTable<ConditionData> conditions;
    DexTable<TypeData> type_chart;
};
