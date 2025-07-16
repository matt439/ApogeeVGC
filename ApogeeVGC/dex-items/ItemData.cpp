#include "ItemData.h"

//bool ItemData::operator==(const ItemData& other) const
//{
//	return name == other.name;
//}

ItemData::ItemData(
    const std::string& name,
    bool is_berry,
    bool ignore_klutz,
    bool is_gem,
    bool is_pokeball,
    bool is_primal_orb,

    const std::string& real_move,
    const std::string& full_name,
    bool* exists,
    int num,
    int gen,
    const std::string& short_desc,
    const std::string& desc,
    NonStandard is_nonstandard,
    bool no_copy,
    bool affects_fainted,
    const std::string& source_effect) :
    Item(name, is_berry, ignore_klutz, is_gem, is_pokeball, is_primal_orb,
        real_move, full_name, exists, num, gen, short_desc, desc,
        is_nonstandard, no_copy, affects_fainted, source_effect),
    PokemonEventMethods()
{
}

ItemData::ItemData(const Item& item,
    const PokemonEventMethods& pokemon_events) :
    Item(item),
    PokemonEventMethods(pokemon_events)
{
}

ItemData::ItemData(const ItemData& other) :
	Item(other),
	PokemonEventMethods(other)
{
}

DataType ItemData::get_data_type() const
{
	return DataType::ITEMS;
}