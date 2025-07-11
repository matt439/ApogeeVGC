#include "DexItems.h"

#include "../dex-data/to_id.h"  
#include "EMPTY_ITEM.h"  
// #include "ItemData.h"

DexItems::DexItems(IModdedDex* dex_ptr)  
   : dex(dex_ptr)
{
}  

const Item& DexItems::get_item(const Item& item) const
{  
   return item;
}  

const Item& DexItems::get_item(const std::string& name)
{  
   std::string id = to_id(name); // Assume to_id trims and normalizes  
   return get_item_by_id(id);
}  

const Item& DexItems::get_item_by_id(const std::string& id)
{
    if (id.empty()) return EMPTY_ITEM;

    // 1. Check cache
    auto it = item_cache.find(id);
    if (it != item_cache.end()) return it->second;

    // 2. Check for alias
    auto alias = dex->get_alias(id);
    if (alias_ptr && !alias_ptr->empty()) {
        const Item& aliased = get_item_by_id(*alias_ptr);
        if (aliased.exists) {
            item_cache[id] = aliased;
        }
        return aliased;
    }

    // 3. Check for berry fallback
    auto& items_data = dex->get_data().items;
    if (!items_data.count(id) && items_data.count(id + "berry")) {
        const Item& berry = get_item_by_id(id + "berry");
        item_cache[id] = berry;
        return berry;
    }

    // 4. Check for existence in data
    if (items_data.count(id)) {
        const auto& item_data = items_data.at(id);
        // Construct Item from item_data (you may need to adapt this)
        Item item(id /* name */, /* ... fill in other fields from item_data ... */);

        // Optionally handle parent mod logic here if needed

        item_cache[id] = item;
        return item_cache[id];
    }

    // 5. Fallback: return dummy item
    Item dummy(id, false); // exists = false
    item_cache[id] = dummy;
    return item_cache[id];
}


const std::vector<Item>& DexItems::get_all_items()
{  
   if (all_cache) return *all_cache;  
   std::vector<Item> items;  
   for (const auto& kv : dex->data().items)  
   {  
       items.push_back(get_by_id(kv.first));  
   }  
   all_cache = std::move(items);  
   return *all_cache;  
}