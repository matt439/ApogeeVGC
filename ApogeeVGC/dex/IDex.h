#pragma once

#include "ModdedDex.h"
#include <unordered_map>
#include <string>

struct IDex
{
	virtual ~IDex() = default;
	virtual std::unordered_map<std::string, ModdedDex>& get_dexes() = 0;
};