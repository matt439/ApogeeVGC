#include "Dex.h"

#include "BASE_MOD.h"
#include "ModdedDex.h"

Dex::Dex()
{
	// Initialize the base modded dex
	dexes["base"] = std::make_unique<ModdedDex>(this, BASE_MOD);
	dexes[BASE_MOD] = std::make_unique<ModdedDex>(this, BASE_MOD);
}

std::unordered_map<std::string, std::unique_ptr<ModdedDex>>* Dex::get_dexes()
{
	return &dexes;
}

IModdedDex* Dex::get_modded_dex(const std::string& mod)
{
	if (mod.empty() || mod == "base")
		return dexes["base"].get();
	auto it = dexes.find(mod);
	if (it != dexes.end())
		return it->second.get();
	// If mod not found, return base modded dex
	return dexes["base"].get();
}