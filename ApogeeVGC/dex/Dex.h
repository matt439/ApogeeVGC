#pragma once

#include "ModdedDex.h"
#include "IDex.h"
#include <unordered_map>
#include <string>

class Dex : IDex
{
public:
	std::unordered_map<std::string, ModdedDex> dexes;

	Dex();

	std::unordered_map<std::string, ModdedDex>& get_dexes() override;
};