#pragma once

#include "DexTable.h"

// Forward declarations
class AbilityText;
class ItemText;
class MoveText;
class PokedexText;
class DefaultText;

struct TextTableData
{
	DexTable<AbilityText> abilities;
	DexTable<ItemText> items;
	DexTable<MoveText> moves;
	DexTable<PokedexText> pokedex;
	DexTable<DefaultText> default_text; // cant use default as a variable name in C++
};