#pragma once

#include "../dex-conditions/PokemonEventMethods.h"
#include "Item.h"
#include <string>

struct ItemData : public Item, public PokemonEventMethods
{
	std::string name = "";

	ItemData() = default;

	bool operator==(const ItemData& other) const;
};