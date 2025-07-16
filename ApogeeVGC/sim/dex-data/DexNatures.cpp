#include "DexNatures.h"

#include "to_id.h"
#include "EMPTY_NATURE.h"

DexNatures::DexNatures(IModdedDex* dex_ptr)
    : dex(dex_ptr)
{
}

DataType DexNatures::get_data_type() const
{
	return DataType::NATURES;
}

//const Nature& DexNatures::get(const Nature& nature) const
//{
//    return nature;
//}
//
//const Nature& DexNatures::get(const std::string& name)
//{
//    std::string id = to_id(name);
//    return get_by_id(id);
//}

//const Nature& DexNatures::get_by_id(const std::string& id)
//{
//    if (id.empty()) return EMPTY_NATURE;
//
//    auto it = nature_cache.find(id);
//    if (it != nature_cache.end()) return it->second;
//
//    // Alias resolution
//    auto alias_opt = dex->get_alias(id);
//    if (alias_opt)
//    {
//        const Nature& nature = get(alias_opt.value());
//        if (nature.exists)
//        {
//            nature_cache[id] = nature;
//        }
//        return nature;
//    }
//
//    // Lookup in data
//    auto& natures_map = dex->get_data_cache() ? dex->get_data_cache()->natures : dex->data().natures;
//    auto data_it = natures_map.find(id);
//    Nature nature = (data_it != natures_map.end())
//        ? Nature(BasicEffect(BasicEffectData(data_it->second.name, true)), data_it->second)
//        : Nature(BasicEffect(BasicEffectData(id, false)), NatureData{});
//
//
//
//    // If the nature's gen is greater than the dex's gen, mark as nonstandard (pseudo-code)
//    if (nature.gen > dex->get_gen())
//    {
//        nature.is_nonstandard = NonStandard::FUTURE;
//    }
//
//    if (nature.exists)
//    {
//        nature_cache[id] = nature;
//        return nature_cache[id];
//    }
//
//    return Nature(); // return an empty nature if not found
//}
//
//const std::vector<Nature>& DexNatures::all()
//{
//    if (all_cache) return *all_cache;
//    std::vector<Nature> natures;
//    auto& natures_map = dex->get_data_cache() ? dex->get_data_cache()->natures : dex->data().natures;
//    for (const auto& kv : natures_map)
//    {
//        natures.push_back(get_by_id(kv.first));
//    }
//    all_cache = std::move(natures);
//    return *all_cache;
//}