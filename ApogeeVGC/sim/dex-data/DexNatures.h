#pragma once

#include "Nature.h"
#include "../dex/IModdedDex.h"
#include "../dex/IDexDataManager.h"
#include <memory>

class DexNatures : public IDexDataManager
{
public:
    IModdedDex* dex = nullptr;
    std::unordered_map<std::string, Nature> nature_cache = {};
    std::unique_ptr<Nature> all_cache = nullptr; // optional

    DexNatures() = default;
    DexNatures(const DexNatures&) = default;
    DexNatures(IModdedDex* dex_ptr);

    //// Get by Nature (returns reference)
    //const Nature& get(const Nature& nature) const;

    //// Get by name (string)
    //const Nature& get(const std::string& name);

    //// Get by ID
    //const Nature& get_by_id(const std::string& id);

    //// Get all natures
    //const std::vector<Nature>& all();

    DataType get_data_type() const override;
};