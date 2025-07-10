#pragma once

#include "DexTableData.h"
#include <memory>
#include <string>

struct IModdedDex
{
	virtual ~IModdedDex() = default;
	virtual const std::string& get_alias(const std::string& id) = 0;
	// virtual DexTableData& get_data_cache() = 0;
    virtual DexTableData& get_data() = 0;
	virtual std::unordered_map<std::string, ModdedDex>& get_dexes() = 0;
	virtual int get_gen() const = 0;
};
