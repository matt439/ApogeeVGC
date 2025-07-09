#pragma once

#include "../global-types/AbilityText.h"
#include "../global-types/ItemText.h"
#include "../global-types/MoveText.h"
#include "../global-types/PokedexText.h"
#include "../global-types/DefaultText.h"
#include "DexTable.h"

struct TextTableData
{
	DexTable<AbilityText> abilities;
	DexTable<ItemText> items;
	DexTable<MoveText> moves;
	DexTable<PokedexText> pokedex;
	DexTable<DefaultText> default_text; // cant use default as a variable name in C++
};