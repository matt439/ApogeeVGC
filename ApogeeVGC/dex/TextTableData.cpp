#include "TextTableData.h"

#include <stdexcept>

ITextEntry* TextTableData::get_entry(TextEntryType type, const std::string& key)
{
	if (type == TextEntryType::ABILITY)
	{
		auto it = abilities.find(key);
		if (it != abilities.end())
			return it->second.get();
	}
	else if (type == TextEntryType::ITEM)
	{
		auto it = items.find(key);
		if (it != items.end())
			return it->second.get();
	}
	else if (type == TextEntryType::MOVE)
	{
		auto it = moves.find(key);
		if (it != moves.end())
			return it->second.get();
	}
	else if (type == TextEntryType::POKEDEX)
	{
		auto it = pokedex.find(key);
		if (it != pokedex.end())
			return it->second.get();
	}
	else if (type == TextEntryType::DEFAULT)
	{
		auto it = default_text.find(key);
		if (it != default_text.end())
			return it->second.get();
	}
	return nullptr; // Entry not found
}

void TextTableData::set_entry(const std::string& key, std::unique_ptr<ITextEntry> entry)
{
	TextEntryType type = entry->get_type();

	switch (type)
	{
	case TextEntryType::ABILITY:
	{
		AbilityText* ability_entry = dynamic_cast<AbilityText*>(entry.get());
		if (ability_entry)
			abilities[key] = std::unique_ptr<AbilityText>(ability_entry);
		break;
	}
	case TextEntryType::ITEM:
	{
		ItemText* item_entry = dynamic_cast<ItemText*>(entry.get());
		if (item_entry)
			items[key] = std::unique_ptr<ItemText>(item_entry);
		break;
	}
	case TextEntryType::MOVE:
	{
		MoveText* move_entry = dynamic_cast<MoveText*>(entry.get());
		if (move_entry)
			moves[key] = std::unique_ptr<MoveText>(move_entry);
		break;
	}
	case TextEntryType::POKEDEX:
	{
		PokedexText* pokedex_entry = dynamic_cast<PokedexText*>(entry.get());
		if (pokedex_entry)
			pokedex[key] = std::unique_ptr<PokedexText>(pokedex_entry);
		break;
	}
	case TextEntryType::DEFAULT:
	{
		DefaultText* default_entry = dynamic_cast<DefaultText*>(entry.get());
		if (default_entry)
			default_text[key] = std::unique_ptr<DefaultText>(default_entry);
		break;
	}
	default:
		throw std::runtime_error("Unknown text entry type: " + std::to_string(static_cast<int>(type)));
	};
}

Descriptions TextTableData::get_description(TextEntryType type, const std::string& key)
{
	switch (type)
	{
	case TextEntryType::ABILITY:
		return Descriptions(abilities[key]->get_descriptions());
	case TextEntryType::ITEM:
		return Descriptions(items[key]->get_descriptions());
	case TextEntryType::MOVE:
		return Descriptions(moves[key]->get_descriptions());
	case TextEntryType::POKEDEX:
		return Descriptions(pokedex[key]->get_descriptions());
	case TextEntryType::DEFAULT:
		return Descriptions(default_text[key]->get_descriptions());
	default:
		throw std::runtime_error("Unknown text entry type: " + std::to_string(static_cast<int>(type)));
	}
}

void TextTableData::set_entry(const std::string& key, ITextEntry* entry)
{
	if (!entry)
		throw std::invalid_argument("Entry cannot be null");
	set_entry(key, std::unique_ptr<ITextEntry>(entry->clone()));
}

DexTable<std::unique_ptr<AbilityText>>* TextTableData::get_abilities()
{
	return &abilities; 
}

DexTable<std::unique_ptr<ItemText>>* TextTableData::get_items() 
{ 
	return &items; 
}

DexTable<std::unique_ptr<MoveText>>* TextTableData::get_moves()
{ 
	return &moves; 
}

DexTable<std::unique_ptr<PokedexText>>* TextTableData::get_pokedex() 
{ 
	return &pokedex; 
}

DexTable<std::unique_ptr<DefaultText>>* TextTableData::get_default_text() 
{ 
	return &default_text; 
}