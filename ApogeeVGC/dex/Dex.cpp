#include "Dex.h"

#include "BASE_MOD.h"


Dex::Dex()
{
	// Initialize the base modded dex
	dexes["base"] = ModdedDex();
	dexes[BASE_MOD] = dexes["base"];
}

std::unordered_map<std::string, ModdedDex>& Dex::get_dexes()
{
	return dexes;
}