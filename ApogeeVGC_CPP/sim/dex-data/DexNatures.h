#pragma once

#include "../global-types/ID.h"
#include "../dex/IModdedDex.h"
#include "../dex/IDexDataManager.h"
#include "Nature.h"
#include <memory>

class DexNatures : public IDexDataManager
{
public:
    IModdedDex* dex = nullptr;
    std::unordered_map<ID, std::unique_ptr<Nature>> nature_cache = {};
    std::unique_ptr<std::vector<std::unique_ptr<Nature>>> all_cache = nullptr; // nullable

    DexNatures() = default;
    DexNatures(const DexNatures&) = default;
    DexNatures(IModdedDex* dex_ptr);

	Nature* get_nature(const std::string& name);
	Nature* get_nature(const Nature& nature);
	Nature* get_nature_by_id(const ID& id);
	std::vector<std::unique_ptr<Nature>>* get_all_natures();

    DataType get_data_type() const override;
};