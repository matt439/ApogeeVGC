#pragma once

#include "ModdedDex.h"
#include "IDex.h"
#include <unordered_map>
#include <string>

class Dex : public IDex
{
public:
	std::unordered_map<std::string, std::unique_ptr<ModdedDex>> dexes = {};

	Dex();

	std::unordered_map<std::string, std::unique_ptr<ModdedDex>>* get_dexes();
	IModdedDex* get_modded_dex(const std::string& mod = "base") override;
};