#include "dex-data.h"

BasicEffectData::BasicEffectData(const std::string& name, bool exists)
	: name(name), exists(exists)
{
}

explicit DexNatures::DexNatures(IModdedDex* dex_ptr)
    : dex(dex_ptr), all_cache(std::nullopt)
{
}

NatureData::NatureData(const std::string& name, bool exists)
	: BasicEffectData(name, exists),
	plus(std::nullopt), minus(std::nullopt)
{
}

const Nature& DexNatures::get(const Nature& nature) const
{
    return nature;
}

const Nature& DexNatures::get(const std::string& name)
{
    std::string id = to_id(name);
    return get_by_id(id);
}

static const Nature EMPTY_NATURE(NatureData( "", false ));

const Nature& DexNatures::get_by_id(const std::string& id)
{
    if (id.empty()) return EMPTY_NATURE;

    auto it = nature_cache.find(id);
    if (it != nature_cache.end()) return it->second;

    // Alias resolution
    auto alias_opt = dex->get_alias(id);
    if (alias_opt)
    {
        const Nature& nature = get(alias_opt.value());
        if (nature.exists)
        {
            nature_cache[id] = nature;
        }
        return nature;
    }

    // Lookup in data
    auto& natures_map = dex->get_data_cache() ? dex->get_data_cache()->natures : dex->data().natures;
    auto data_it = natures_map.find(id);
    Nature nature = (data_it != natures_map.end())
        ? Nature(data_it->second)
        : Nature(NatureData(id, false));

    // If the nature's gen is greater than the dex's gen, mark as nonstandard (pseudo-code)
    if (nature.gen > dex->get_gen())
    {
        nature.is_nonstandard = NonStandard::FUTURE;
    }

    if (nature.exists)
    {
        nature_cache[id] = nature;
        return nature_cache[id];
    }
    // Return the temporary/dummy nature (not cached)
    static Nature dummy_nature(NatureData("", false));
    dummy_nature = nature;
    return dummy_nature;
}

const std::vector<Nature>& DexNatures::all()
{
    if (all_cache) return *all_cache;
    std::vector<Nature> natures;
    auto& natures_map = dex->get_data_cache() ? dex->get_data_cache()->natures : dex->data().natures;
    for (const auto& kv : natures_map)
    {
        natures.push_back(get_by_id(kv.first));
    }
    all_cache = std::move(natures);
    return *all_cache;
}