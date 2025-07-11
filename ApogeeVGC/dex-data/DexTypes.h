#pragma once

#include "../global-types/ID.h"
#include "../dex/IModdedDex.h"
#include "TypeInfo.h"

class DexTypes
{
public:
	IModdedDex* dex = nullptr;
	std::unordered_map<ID, std::unique_ptr<TypeInfo>> type_cache = {};
	std::unique_ptr<std::vector<std::unique_ptr<TypeInfo>>> all_cache = nullptr;
	std::unique_ptr<std::vector<std::string>> names_cache = nullptr;

	//TypeInfo EMPTY_TYPE_INFO;

	DexTypes() = default;
	DexTypes(IModdedDex* dex_ptr);

	TypeInfo* get_type_info(const ID& id);
	TypeInfo* get_type_info(const TypeInfo& type_info);
	std::vector<std::string>* get_names();
	bool is_name(const std::string& name) const;
	std::vector<std::unique_ptr<TypeInfo>>* get_all_type_infos();
};