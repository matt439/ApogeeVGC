#include "TextTableData.h"

void* TextTableData::get_table(const std::string& name)
{
    if (name == "abilities") return &abilities;
    if (name == "items") return &items;
    if (name == "moves") return &moves;
    if (name == "pokedex") return &pokedex;
    if (name == "default_text") return &default_text;
    return nullptr;
}


Descriptions TextTableData::get_description_from_table(
	const std::string& table, const std::string& id) const
{
	void* table_ptr = get_table(table);

	if (!table_ptr)
		return { "", "" }; // Table not found

	if (table == "abilities")
		return static_cast<DexTable<AbilityText>*>(table_ptr)->at(id);
	else if (table == "items")
		return static_cast<DexTable<ItemText>*>(table_ptr)->get_description(id);
	else if (table == "moves")
		return static_cast<DexTable<MoveText>*>(table_ptr)->get_description(id);
	else if (table == "pokedex")
		return static_cast<DexTable<PokedexText>*>(table_ptr)->get_description(id);
	else if (table == "default_text")
		return static_cast<DexTable<DefaultText>*>(table_ptr)->get_description(id);
	return { "", "" }; // Not found
}