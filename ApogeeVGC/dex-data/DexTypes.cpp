#include "DexTypes.h"

#include "EMPTY_TYPE_INFO.h"

//TypeInfo EMPTY_TYPE_INFO(
//	ID(""),
//	"",
//	TypeInfoEffectType::EFFECT_TYPE,
//	false
//);

DexTypes::DexTypes(IModdedDex* dex_ptr) : dex(dex_ptr)
{
}

TypeInfo* DexTypes::get_type_info(const ID& id)
{
    if (id.empty())
        return EMPTY_TYPE_INFO.get();

    auto it = type_cache.find(id);
    if (it != type_cache.end())
		return it->second.get();

    // Capitalize first letter for display name
    std::string type_name = id;
    if (!type_name.empty())
        type_name[0] = std::toupper(type_name[0]);

    // Check if type exists in TypeChart
	auto type_chart = dex->get_data()->type_chart;
    auto chart_it = type_chart.find(id);
    TypeInfo type_info;
    if (chart_it != type_chart.end())
    {
        // Construct TypeInfo from chart data
        type_info = TypeInfo(
            id,
            type_name,
            TypeInfoEffectType::TYPE,
            true // exists
        );
    }
    else
    {
        // Type does not exist
        type_info = TypeInfo(
            id,
            type_name,
            TypeInfoEffectType::EFFECT_TYPE,
            false // exists
        );
    }

    // Cache if exists
    if (type_info.exists)
    {
        type_cache[id] = std::make_unique<TypeInfo>(type_info);
		return type_cache[id].get();
    }

	return EMPTY_TYPE_INFO.get();
}


TypeInfo* DexTypes::get_type_info(const TypeInfo& type_info)
{
	if (type_info.id.empty())
		return EMPTY_TYPE_INFO.get();

	// Check cache first
	auto it = type_cache.find(type_info.id);
	if (it != type_cache.end())
		return it->second.get();

	// If not in cache, create a new TypeInfo
	TypeInfo new_type_info(
		type_info.id,
		type_info.name,
		type_info.effect_type,
		type_info.exists,
		type_info.gen,
		type_info.damage_taken,
		std::make_unique<SparseStatsTable>(*type_info.hp_dvs),
		std::make_unique<SparseStatsTable>(*type_info.hp_ivs),
		std::make_unique<NonStandard>(*type_info.is_nonstandard)
	);

	// Store in cache
	type_cache[type_info.id] = std::make_unique<TypeInfo>(new_type_info);
	return type_cache[type_info.id].get();
}

std::vector<std::string>* DexTypes::get_names()
{
	if (names_cache)
		return names_cache.get();

	names_cache = std::make_unique<std::vector<std::string>>();
	for (const auto& kv : type_cache)
	{
		auto type_info = kv.second.get();
		if (type_info->exists)
			names_cache->push_back(type_info->name);
	}
	return names_cache.get();
}

#include "to_lower.h"

bool DexTypes::is_name(const std::string& name) const
{
	if (name.empty())
        return false;
	
	std::string id = to_lower(name); // convert to lowercase
	id[0] = std::toupper(id[0]); // Capitalize first letter

	return type_cache.find(ID(id)) != type_cache.end() &&
		type_cache.at(ID(id))->exists &&
		type_cache.at(ID(id))->name == id;
}

std::vector<std::unique_ptr<TypeInfo>>* DexTypes::get_all_type_infos()
{
	if (all_cache)
		return all_cache.get();

	all_cache = std::make_unique<std::vector<std::unique_ptr<TypeInfo>>>();
	for (const auto& kv : type_cache)
	{
		if (kv.second->exists)
		{
			all_cache->emplace_back(std::make_unique<TypeInfo>(*kv.second));
		}
	}
	return all_cache.get();
}