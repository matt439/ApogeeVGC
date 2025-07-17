#pragma once

#include "../global-types/AbilityText.h"
#include "../global-types/ItemText.h"
#include "../global-types/MoveText.h"
#include "../global-types/PokedexText.h"
#include "../global-types/DefaultText.h"
#include "DexTable.h"
#include "Descriptions.h"
#include <memory>

//// forward declarations
//struct AbilityText;
//struct ItemText;
//struct MoveText;
//struct PokedexText;
//struct DefaultText;

struct TextTableData
{
	DexTable<std::unique_ptr<AbilityText>> abilities = {};
	DexTable<std::unique_ptr<ItemText>> items = {};
	DexTable<std::unique_ptr<MoveText>> moves = {};
	DexTable<std::unique_ptr<PokedexText>> pokedex = {};
	DexTable<std::unique_ptr<DefaultText>> default_text = {}; // can't use default as a variable name in C++

    // Returns a pointer to the requested table, or nullptr if not found
	ITextEntry* get_entry(TextEntryType type, const std::string& key);

	void set_entry(const std::string& key, std::unique_ptr<ITextEntry> entry);
	void set_entry(const std::string& key, ITextEntry* entry);

	// Returns a description from the specified table and ID

	Descriptions get_description(TextEntryType type, const std::string& key);

	DexTable<std::unique_ptr<AbilityText>>* get_abilities();
	DexTable<std::unique_ptr<ItemText>>* get_items();
	DexTable<std::unique_ptr<MoveText>>* get_moves();
	DexTable<std::unique_ptr<PokedexText>>* get_pokedex();
	DexTable<std::unique_ptr<DefaultText>>* get_default_text();
};