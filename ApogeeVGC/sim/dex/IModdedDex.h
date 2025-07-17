#pragma once

#include "../global-types/TextEntryType.h"
//#include "../global-types/ITextEntry.h"
#include "../global-types/ID.h"
#include "DataType.h"
// #include "DexTableData.h"
#include "Descriptions.h"
// #include "IDex.h"
#include <memory>
#include <string>

struct DexTableData;
class IDex;
class IDexDataManager;
class IDexData;
class ITextEntry;
// class ModdedDex;

class IModdedDex
{
public:
	virtual ~IModdedDex() = default;
	virtual IDex* get_idex_parent() const = 0;
	virtual std::string* get_alias(const ID& id) = 0;
    //// virtual DexTableData* get_data_cache() = 0;
	//// virtual std::unordered_map<std::string, ModdedDex>* get_dexes() = 0;
	virtual DexTableData* get_data() = 0;
	virtual int get_gen() const = 0;
	virtual Descriptions get_descriptions(const std::string& table, const std::string& id) = 0;
	//virtual const std::string& get_parent_mod() const = 0;
	//virtual IDexDataManager* get_data_manager(DataType data_type) const = 0;
	virtual bool get_is_base() const = 0;
	virtual IModdedDex* include_mods() = 0;
	virtual bool has_mod(const std::string& mod) = 0;

	virtual IDexData* get_from_dex_table_data(DataType data_type, const std::string& key) = 0;
	virtual void set_into_dex_table_data(const std::string& key, std::unique_ptr<IDexData> data) = 0;
	virtual bool exists_in_dex_table_data(DataType data_type, const std::string& key) = 0;


	virtual ITextEntry* get_from_text_table_data(TextEntryType type, const std::string& key) = 0;
	virtual void set_into_text_table_data(const std::string& key, std::unique_ptr<ITextEntry> entry) = 0;
	virtual bool exists_in_text_table_data(TextEntryType type, const std::string& key) = 0;

	//virtual ModdedDex* cast_to_modded_dex(IModdedDex* modded_dex) const = 0;
	//virtual ModdedDex* cast_to_modded_dex() = 0;
	
	//virtual ModdedDex* get_modded_dex(const std::string& mod = "base") = 0;

	// virtual bool get_mods_loaded() const = 0;
};
