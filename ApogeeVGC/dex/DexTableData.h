#pragma once

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

#include "../global-types/AnyObject.h"
#include "DexTable.h"
#include "TypeData.h"

//// forward declarations
//class AbilityData;
//class FormatData;
//class ItemData;
//class LearnsetData;
//class MoveData;
//class NatureData;
//class SpeciesData;
//class SpeciesFormatsData;
//class PokemonGoData;

struct DexTableData
{
	DexTable<AbilityData> abilities = {};
	DexTable<FormatData> rulesets = {};
	DexTable<ItemData> items = {};
	DexTable<LearnsetData> learnsets = {};
	DexTable<MoveData> moves = {};
	DexTable<NatureData> natures = {};
	DexTable<SpeciesData> pokedex = {};
	DexTable<SpeciesFormatsData> formats_data = {};
	DexTable<PokemonGoData> pokemon_go_data = {};
	DexTable<AnyObject> scripts = {}; // generic AnyObject for scripts, can be any type of script data
	DexTable<ConditionData> conditions = {};
	DexTable<TypeData> type_chart = {};
};
