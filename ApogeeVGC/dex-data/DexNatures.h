#pragma once

#include "Nature.h"
#include "../dex/IModdedDex.h"

class DexNatures
{
public:
    IModdedDex* dex = nullptr;
    std::unordered_map<std::string, Nature> nature_cache = {};
    std::optional<std::vector<Nature>> all_cache = std::nullopt;

    DexNatures() = default;
    DexNatures(const DexNatures&) = default;
    DexNatures(IModdedDex* dex_ptr);

    // Get by Nature (returns reference)
    const Nature& get(const Nature& nature) const;

    // Get by name (string)
    const Nature& get(const std::string& name);

    // Get by ID
    const Nature& get_by_id(const std::string& id);

    // Get all natures
    const std::vector<Nature>& all();
};