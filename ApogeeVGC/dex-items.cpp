#include "dex-items.h"

ItemInitData::ItemInitData(const std::string& name, bool exists)
    : BasicEffectData(name, exists),
    plus(std::nullopt), minus(std::nullopt)
{
}

explicit DexItems::DexItems(IModdedDex* dex_ptr)
    : dex(dex_ptr), all_cache(std::nullopt)
{
}

const Item& DexItems::get(const Item& item) const
{
    return item;
}

const Item& DexItems::get(const std::string& name)
{
    std::string id = to_id(name); // Assume to_id trims and normalizes
    return get_by_id(id);
}

const Item& DexItems::get_by_id(const std::string& id)
{
    static const Item EMPTY_ITEM = Item(ItemData{}); // Provide a default empty item

    if (id.empty()) return EMPTY_ITEM;

    auto it = item_cache.find(id);
    if (it != item_cache.end()) return it->second;

    // Alias resolution (pseudo-code, depends on ModdedDex API)
    std::optional<std::string> alias = dex->get_alias(id);
    if (alias)
    {
        const Item& item = get(*alias);
        if (item.exists) item_cache[id] = item;
        return item;
    }

    // Berry fallback
    if (!dex->data().items.count(id) && dex->data().items.count(id + "berry"))
    {
        const Item& item = get_by_id(id + "berry");
        item_cache[id] = item;
        return item;
    }

    // Normal lookup
    if (dex->data().items.count(id))
    {
        ItemData itemData = dex->data().items.at(id);
        // Merge with text data if needed (pseudo-code)
        // ItemData itemTextData = dex->getDescs("Items", id, itemData);
        // Merge itemData and itemTextData as needed

        Item item(itemData);
        if (item.gen > dex->get_gen())
        {
            item.is_nonstandard = NonStandard::FUTURE;
        }
        // Parent mod logic omitted for brevity

        item_cache[id] = item;
        return item_cache[id];
    }
    else
    {
        Item item;
        item.name = id;
        item.exists = false;
        item_cache[id] = item;
        return item_cache[id];
    }
}

const std::vector<Item>& DexItems::all()
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