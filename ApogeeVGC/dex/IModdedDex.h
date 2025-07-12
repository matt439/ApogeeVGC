#pragma once

#include "DataType.h"
// #include "DexTableData.h"
#include "Descriptions.h"
// #include "IDex.h"
#include <memory>
#include <string>

class DexTableData;
class IDex;

struct IModdedDex
{
	virtual ~IModdedDex() = default;
	virtual IDex* get_dex_parent() const = 0;
	virtual std::string* get_alias(const std::string& id) = 0;
    virtual DexTableData* get_data_cache() = 0;
	// virtual std::unordered_map<std::string, ModdedDex>* get_dexes() = 0;
	virtual DexTableData* get_data() = 0;
	virtual int get_gen() const = 0;
	virtual Descriptions get_descriptions(const std::string& table, const std::string& id) = 0;
	virtual const std::string& get_parent_mod() const = 0;
	
	//virtual ModdedDex* get_modded_dex(const std::string& mod = "base") = 0;
};
