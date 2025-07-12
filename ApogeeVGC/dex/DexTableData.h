#pragma once

//#include "../dex-abilities/AbilityData.h"
//#include "../dex-format/FormatData.h"
//#include "../dex-items/ItemData.h"
//#include "../dex-species/LearnsetData.h"
//#include "../dex-moves/MoveData.h"
//#include "../dex-data/NatureData.h"
//#include "../dex-species/SpeciesData.h"
//#include "../dex-species/SpeciesFormatsData.h"
//#include "../dex-species/PokemonGoData.h"
#include "../dex-conditions/ConditionData.h"
#include "../dex-data/TypeData.h"

#include "../global-types/AnyObject.h"
#include "DexTable.h"
// #include "TypeData.h"

// forward declarations
class AbilityData;
class FormatData;
class ItemData;
class LearnsetData;
class MoveData;
class NatureData;
class SpeciesData;
class SpeciesFormatsData;
class PokemonGoData;
//struct TypeData;

struct DexTableData
{
	DexTable<std::unique_ptr<AbilityData>> abilities = {};
	DexTable<std::unique_ptr<FormatData>> rulesets = {};
	DexTable<std::unique_ptr<ItemData>> items = {};
	DexTable<std::unique_ptr<LearnsetData>> learnsets = {};
	DexTable<std::unique_ptr<MoveData>> moves = {};
	DexTable<std::unique_ptr<NatureData>> natures = {};
	DexTable<std::unique_ptr<SpeciesData>> pokedex = {};
	DexTable<std::unique_ptr<SpeciesFormatsData>> formats_data = {};
	DexTable<std::unique_ptr<PokemonGoData>> pokemon_go_data = {};
	DexTable<std::unique_ptr<AnyObject>> scripts = {}; // generic AnyObject for scripts, can be any type of script data
	DexTable<std::unique_ptr<ConditionData>> conditions = {};
	DexTable<std::unique_ptr<TypeData>> type_chart = {};
};
