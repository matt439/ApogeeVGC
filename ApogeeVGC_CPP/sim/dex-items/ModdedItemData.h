#pragma once

#include "../dex-conditions/PokemonEventMethods.h"

struct ModdedItemData : public PokemonEventMethods
{
	bool inherit = true;
	std::function<void(Battle*, Pokemon*)>* on_custap = nullptr; // optional
};