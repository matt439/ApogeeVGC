#pragma once

#include "../dex/IModdedDex.h"
#include "../dex/IDexDataManager.h"
#include "../global-types/ID.h"
#include "Item.h"

struct DexItems : public IDexDataManager
{
	IModdedDex* dex = nullptr;
	std::unordered_map<ID, std::unique_ptr<Item>> item_cache = {};
	std::unique_ptr<std::vector<std::unique_ptr<Item>>> all_cache = nullptr;

    // const std::unique_ptr<Item> EMPTY_ITEM = std::make_unique<Item>("empty");

	DexItems() = default;
    DexItems(IModdedDex* dex_ptr);

     Item* get_item(const Item& item) const;
     Item* get_item(const std::string& name);
	 Item* get_item_by_id(const ID& id);

    // Get all items
    std::vector<std::unique_ptr<Item>>* get_all_items();

    DataType get_data_type() const override;
};