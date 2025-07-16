#pragma once

#include "../dex/IDexData.h"
#include "../dex-conditions/PokemonEventMethods.h"
#include "Item.h"
#include <string>

struct ItemData : public Item, public PokemonEventMethods, public IDexData
{
	//std::string name = "";

	ItemData() = default;
    ItemData(
        const std::string& name,
        bool is_berry = false,
        bool ignore_klutz = false,
        bool is_gem = false,
        bool is_pokeball = false,
        bool is_primal_orb = false,

        const std::string& real_move = "",
        const std::string& full_name = "",
        bool* exists = nullptr,
        int num = 0,
        int gen = 0,
        const std::string& short_desc = "",
        const std::string& desc = "",
        NonStandard is_nonstandard = NonStandard::NONE,
        bool no_copy = false,
        bool affects_fainted = false,
        const std::string& source_effect = "");

	ItemData(const Item& item,
		const PokemonEventMethods& pokemon_events = PokemonEventMethods());

	ItemData(const ItemData& other);

	//bool operator==(const ItemData& other) const;

	DataType get_data_type() const;

    std::unique_ptr<IDexData> clone() const override;
};