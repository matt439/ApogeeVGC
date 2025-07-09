#pragma once

#include "../dex/IModdedDex.h"
#include "Item.h"

struct DexItems
{
	IModdedDex* dex = nullptr;
	std::unordered_map<std::string, Item> item_cache = {};
	std::optional<std::vector<Item>> all_cache = std::nullopt;

    explicit DexItems(IModdedDex* dex_ptr);

    // Get by Item (returns reference)
    const Item& get(const Item& item) const;

    // Get by name (string)
    const Item& get(const std::string& name);

    // Get by ID
    const Item& get_by_id(const std::string& id);

    // Get all items
    const std::vector<Item>& all();
};