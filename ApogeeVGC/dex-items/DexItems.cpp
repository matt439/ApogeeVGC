#include "DexItems.h"

#include "../dex-data/to_id.h"  
#include "EMPTY_ITEM.h"  
// #include "ItemData.h"

DexItems::DexItems(IModdedDex* dex_ptr)  
   : dex(dex_ptr)
{
}  

//Item* DexItems::get_item(const Item& item) const
//{  
//   return item;
//}  

Item* DexItems::get_item(const std::string& name)
{  
   std::string id = to_id(name); // Assume to_id trims and normalizes  
   return get_item(id);
}  

Item* DexItems::get_item(const std::string& id)
{
	if (id.empty())
        return EMPTY_ITEM.get();

    // 1. Check cache
    auto it = item_cache.find(id);
	if (it != item_cache.end())
        return it->second.get();

    // 2. Check for alias
    auto alias = dex->get_alias(id);
	if (alias) {
		// If alias exists, recursively get the item by the alias
		return get_item(*alias);
	}

    // 3. Check for berry fallback
    auto& items_data = dex->get_data().items;
    if (!items_data.count(id) && items_data.count(id + "berry"))
    {
        auto berry = get_item(id + "berry");
		item_cache[id] = std::make_unique<Item>(berry);
        return berry;
    }

    // 4. Check for existence in data
    if (items_data.count(id))
    {
        const auto& item_data = items_data.at(id);
        // Get text descriptions for the item
        auto item_descs = dex->get_descriptions("items", id);

        // Construct the Item (you may need to adapt the constructor)
        auto item = std::make_unique<Item>(item_data.name);
        item->desc = std::make_unique<std::string>(item_descs.desc);
		item->short_desc = std::make_unique<std::string>(item_descs.short_desc);

        // If item's generation is greater than dex's generation, mark as nonstandard
        if (item->gen > dex->get_gen())
        {
            item->is_nonstandard = std::make_unique<NonStandard>(NonStandard::FUTURE);
        }

        // If parent mod exists, check for identical item and reuse
        if (!dex->get_parent_mod().empty())
        {
            auto parent = dex->get_modded_dex(dex->get_parent_mod());
            const auto& parent_items_data = parent->get_data().items;
            if (item_data == parent_items_data.at(id))
            {
                Item* parent_item = parent->items->get_item(id);
                if (item->is_nonstandard == parent_item->is_nonstandard &&
                    item->desc == parent_item->desc &&
                    item->short_desc == parent_item->short_desc)
                {
                    item = std::make_unique<Item>(*parent_item);
                }
            }
        }
        item_cache[id] = std::move(item);
        return item_cache[id].get();
    }

    // 5. Fallback: return dummy item
    Item dummy(id, false); // exists = false
	item_cache[id] = std::make_unique<Item>(dummy);
	return item_cache[id].get();
}


std::vector<std::unique_ptr<Item>>* DexItems::get_all_items()
{  
   if (all_cache)
	   return all_cache.get();

   std::vector<Item> items;  
   for (auto kv : dex->get_data().items)
   {  
       items.push_back(get_by_id(kv.first));  
   }  
   all_cache = std::move(items);  
   return *all_cache;  
}