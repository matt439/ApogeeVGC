#pragma once

// #include "ModdedDex.h"
#include <unordered_map>
#include <string>
#include <memory>

//class ModdedDex;
class IModdedDex;

class IDex
{
public:
	virtual ~IDex() = default;
	//virtual std::unordered_map<std::string, std::unique_ptr<IModdedDex>>* get_dexes() = 0;
	virtual IModdedDex* get_imodded_dex(const std::string& mod = "base") = 0;
};