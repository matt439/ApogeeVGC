#pragma once

// #include "ModdedDex.h"
#include "IDex.h"
#include <unordered_map>
#include <string>

class Dex : IDex
{
public:
	std::unordered_map<std::string, std::unique_ptr<IModdedDex>> dexes = {};

	Dex();

	std::unordered_map<std::string, std::unique_ptr<IModdedDex>>* get_dexes() override;
	IModdedDex* get_modded_dex(const std::string& mod = "base") override;
};