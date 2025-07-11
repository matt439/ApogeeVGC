#pragma once

#include "../global-types/AbilityText.h"
#include "../global-types/ItemText.h"
#include "../global-types/MoveText.h"
#include "../global-types/PokedexText.h"
#include "../global-types/DefaultText.h"
#include "DexTable.h"
#include "Descriptions.h"

//// forward declarations
//struct AbilityText;
//struct ItemText;
//struct MoveText;
//struct PokedexText;
//struct DefaultText;

struct TextTableData
{
	DexTable<AbilityText> abilities = {};
	DexTable<ItemText> items = {};
	DexTable<MoveText> moves = {};
	DexTable<PokedexText> pokedex = {};
	DexTable<DefaultText> default_text = {}; // can't use default as a variable name in C++

    // Returns a pointer to the requested table, or nullptr if not found
	void* get_table(const std::string& name);

	Descriptions get_description_from_table(const std::string& table, const std::string& id);
};