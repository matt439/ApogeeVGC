#pragma once

#include "../global-types/type_aliases.h"
#include "../dex-conditions/ConditionData.h"
#include "../dex-abilities/AbilityData.h"
#include "../dex-format/FormatData.h"
#include "../dex-items/ItemData.h"
#include "../dex-species/LearnsetData.h"
#include "../dex-moves/MoveData.h"
#include "../dex-data/NatureData.h"
#include "../dex-species/SpeciesData.h"
#include "../dex-species/SpeciesFormatsData.h"
#include "../dex-species/PokemonGoData.h"
#include "DexTable.h"
#include "TypeData.h"

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
