#pragma once

#include "DataType.h"
// #include "DexTableData.h"
#include "Descriptions.h"
// #include "IDex.h"
#include <memory>
#include <string>

struct DexTableData;
class IDex;
class IDexDataManager;
// class ModdedDex;

class IModdedDex
{
public:
	virtual ~IModdedDex() = default;
	virtual IDex* get_idex_parent() const = 0;
	//virtual std::string* get_alias(const std::string& id) = 0;
    //// virtual DexTableData* get_data_cache() = 0;
	//// virtual std::unordered_map<std::string, ModdedDex>* get_dexes() = 0;
	virtual DexTableData* get_data() = 0;
	virtual int get_gen() const = 0;
	virtual Descriptions get_descriptions(const std::string& table, const std::string& id) = 0;
	//virtual const std::string& get_parent_mod() const = 0;
	//virtual IDexDataManager* get_data_manager(DataType data_type) const = 0;
	virtual bool get_is_base() const = 0;
	virtual IModdedDex* include_mods() = 0;


	//virtual ModdedDex* cast_to_modded_dex(IModdedDex* modded_dex) const = 0;
	//virtual ModdedDex* cast_to_modded_dex() = 0;
	
	//virtual ModdedDex* get_modded_dex(const std::string& mod = "base") = 0;

	// virtual bool get_mods_loaded() const = 0;
};
