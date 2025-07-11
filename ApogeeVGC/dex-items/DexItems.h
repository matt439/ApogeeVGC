#pragma once

#include "../dex/IModdedDex.h"
#include "../global-types/ID.h"
#include "Item.h"

struct DexItems
{
	IModdedDex* dex = nullptr;
	std::unordered_map<std::string, Item> item_cache = {};
	std::unique_ptr<std::vector<Item>> all_cache = nullptr;

	DexItems() = default;
    DexItems(IModdedDex* dex_ptr);

    // Get by Item (returns reference)
    const Item& get_item(const Item& item) const;

    // Get by name (string)
    const Item& get_item(const std::string& name);

    // Get by ID
    const Item& get_item_by_id(const ID& id);

    // Get all items
    const std::vector<Item>& get_all_items();
};